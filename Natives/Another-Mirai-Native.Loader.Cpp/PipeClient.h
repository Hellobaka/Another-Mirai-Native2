#pragma once

#include "JsonCodec.h"

#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <windows.h>

#include <atomic>
#include <condition_variable>
#include <functional>
#include <memory>
#include <mutex>
#include <string>
#include <thread>
#include <unordered_map>

namespace amn {

class PipeClient {
public:
    using RequestHandler = std::function<void(const Json&)>;

    static std::unique_ptr<PipeClient> Connect(int core_pid);

    explicit PipeClient(HANDLE handle);
    ~PipeClient();

    PipeClient(const PipeClient&) = delete;
    PipeClient& operator=(const PipeClient&) = delete;

    void Start(RequestHandler handler);
    void Join();
    void Close();
    bool Send(const Json& packet);
    Json CallCore(const std::string& function, const JsonArray& args);

private:
    bool ReadExact(void* buffer, DWORD length);
    bool Receive(Json& packet);
    void ReadLoop();

    HANDLE handle_ = INVALID_HANDLE_VALUE;
    std::atomic<bool> closed_{false};
    std::atomic<unsigned long> next_id_{1};
    std::mutex send_mutex_;
    std::mutex response_mutex_;
    std::condition_variable response_ready_;
    std::unordered_map<std::string, Json> responses_;
    std::thread reader_;
    std::atomic<unsigned int> active_workers_{0};
    std::mutex workers_mutex_;
    std::condition_variable workers_done_;
    RequestHandler handler_;
};

} // namespace amn
