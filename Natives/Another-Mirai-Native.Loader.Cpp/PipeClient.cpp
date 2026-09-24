#include "PipeClient.h"

#include "Diagnostics.h"

#include <chrono>
#include <cstdint>

namespace amn {
namespace {

constexpr uint32_t kMaxMessageBytes = 16 * 1024 * 1024;
constexpr auto kApiTimeout = std::chrono::seconds(120);

class EventHandle {
public:
    EventHandle() : handle_(CreateEventW(nullptr, TRUE, FALSE, nullptr)) {}
    ~EventHandle() { if (handle_) CloseHandle(handle_); }
    HANDLE Get() const { return handle_; }

private:
    HANDLE handle_;
};

} // namespace

std::unique_ptr<PipeClient> PipeClient::Connect(int core_pid) {
    const std::wstring name = L"\\\\.\\pipe\\Another_Mirai_Native2_NamedPipe_" +
                              std::to_wstring(core_pid);
    if (!WaitNamedPipeW(name.c_str(), 5000)) {
        ConsoleLog(ConsoleLevel::Error, "waiting for named pipe",
                   "Windows error=" + std::to_string(GetLastError()));
        return {};
    }

    HANDLE pipe = CreateFileW(name.c_str(), GENERIC_READ | GENERIC_WRITE, 0,
                              nullptr, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, nullptr);
    if (pipe == INVALID_HANDLE_VALUE) {
        ConsoleLog(ConsoleLevel::Error, "opening named pipe",
                   "Windows error=" + std::to_string(GetLastError()));
        return {};
    }

    DWORD mode = PIPE_READMODE_BYTE;
    if (!SetNamedPipeHandleState(pipe, &mode, nullptr, nullptr)) {
        const DWORD error = GetLastError();
        CloseHandle(pipe);
        ConsoleLog(ConsoleLevel::Error, "setting named pipe mode",
                   "Windows error=" + std::to_string(error));
        return {};
    }
    return std::make_unique<PipeClient>(pipe);
}

PipeClient::PipeClient(HANDLE handle) : handle_(handle) {}

PipeClient::~PipeClient() {
    Close();
    Join();
    if (handle_ != INVALID_HANDLE_VALUE) CloseHandle(handle_);
}

void PipeClient::Start(RequestHandler handler) {
    handler_ = std::move(handler);
    reader_ = std::thread([this] { ReadLoop(); });
}

void PipeClient::Join() {
    if (reader_.joinable()) reader_.join();
}

void PipeClient::Close() {
    if (!closed_.exchange(true) && handle_ != INVALID_HANDLE_VALUE) {
        CancelIoEx(handle_, nullptr);
        response_ready_.notify_all();
    }
}

bool PipeClient::ReadExact(void* buffer, DWORD length) {
    auto* cursor = static_cast<char*>(buffer);
    while (length > 0 && !closed_) {
        EventHandle event;
        if (!event.Get()) return false;

        OVERLAPPED operation{};
        operation.hEvent = event.Get();
        DWORD received = 0;
        BOOL ok = ReadFile(handle_, cursor, length, &received, &operation);
        if (!ok && GetLastError() == ERROR_IO_PENDING) {
            ok = GetOverlappedResult(handle_, &operation, &received, TRUE);
        }
        if (!ok || received == 0) {
            if (!closed_) {
                ConsoleLog(ConsoleLevel::Error, "reading named pipe",
                           "Windows error=" + std::to_string(ok ? ERROR_BROKEN_PIPE : GetLastError()));
            }
            return false;
        }

        cursor += received;
        length -= received;
    }
    return length == 0;
}

bool PipeClient::Receive(Json& packet) {
    uint32_t length = 0;
    if (!ReadExact(&length, sizeof(length)) || length == 0 || length > kMaxMessageBytes) {
        return false;
    }

    std::string payload(length, '\0');
    return ReadExact(payload.data(), length) && ParseJson(payload, packet);
}

bool PipeClient::Send(const Json& packet) {
    if (closed_) return false;

    const std::string payload = packet.serialize();
    const uint32_t length = static_cast<uint32_t>(payload.size());
    std::string frame(reinterpret_cast<const char*>(&length), sizeof(length));
    frame += payload;

    std::lock_guard<std::mutex> lock(send_mutex_);
    EventHandle event;
    if (!event.Get()) return false;

    OVERLAPPED operation{};
    operation.hEvent = event.Get();
    DWORD written = 0;
    BOOL ok = WriteFile(handle_, frame.data(), static_cast<DWORD>(frame.size()),
                        &written, &operation);
    if (!ok && GetLastError() == ERROR_IO_PENDING) {
        ok = GetOverlappedResult(handle_, &operation, &written, TRUE);
    }
    if (!ok || written != frame.size()) {
        ConsoleLog(ConsoleLevel::Error, "writing named pipe",
                   "Windows error=" + std::to_string(ok ? ERROR_WRITE_FAULT : GetLastError()));
        return false;
    }
    return true;
}

Json PipeClient::CallCore(const std::string& function, const JsonArray& args) {
    const std::string id = std::to_string(GetCurrentProcessId()) + "-" +
                           std::to_string(next_id_++);
    const Json request(JsonObject{{"GUID", Json(id)},
                                  {"Function", Json("InvokeCQP_" + function)},
                                  {"Args", Json(args)}});
    ConsoleLog(ConsoleLevel::Info, "CQP API request", function + " GUID=" + id);
    if (!Send(request)) return {};

    std::unique_lock<std::mutex> lock(response_mutex_);
    const bool received = response_ready_.wait_for(lock, kApiTimeout, [&] {
        return closed_ || responses_.count(id) != 0;
    });
    if (!received || closed_) {
        ConsoleLog(ConsoleLevel::Error, "CQP API response missing",
                   function + " GUID=" + id);
        return {};
    }

    Json result = Member(responses_.at(id), "Result");
    responses_.erase(id);
    ConsoleLog(ConsoleLevel::Info, "CQP API response received", function + " GUID=" + id);
    return result;
}

void PipeClient::ReadLoop() {
    Json packet;
    while (!closed_ && Receive(packet)) {
        if (HasField(packet, "Args")) {
            ++active_workers_;
            std::thread([this, packet] {
                try {
                    handler_(packet);
                } catch (...) {
                    ConsoleLog(ConsoleLevel::Error, "plugin callback threw an exception",
                               Field(packet, "Function"));
                }
                --active_workers_;
                workers_done_.notify_all();
            }).detach();
        } else {
            const std::string id = Field(packet, "GUID");
            if (!id.empty()) {
                std::lock_guard<std::mutex> lock(response_mutex_);
                responses_[id] = packet;
                response_ready_.notify_all();
            }
        }
    }

    ConsoleLog(ConsoleLevel::Info, "named pipe reader stopped");
    Close();
    std::unique_lock<std::mutex> lock(workers_mutex_);
    workers_done_.wait(lock, [this] { return active_workers_ == 0; });
}

} // namespace amn
