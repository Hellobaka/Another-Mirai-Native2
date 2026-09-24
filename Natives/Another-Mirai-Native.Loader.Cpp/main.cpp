#include "Diagnostics.h"
#include "JsonCodec.h"
#include "PipeClient.h"
#include "PluginHost.h"

#define WIN32_LEAN_AND_MEAN
#include <windows.h>

#include <cwchar>
#include <filesystem>
#include <string>

namespace {

amn::PipeClient* active_pipe = nullptr;

struct Options {
    std::wstring plugin_path;
    int core_pid = 0;
    int auth_code = 0;
};

Options ParseOptions(int argc, wchar_t** argv) {
    Options options;
    for (int index = 1; index + 1 < argc; ++index) {
        if (_wcsicmp(argv[index], L"-Path") == 0) {
            options.plugin_path = argv[++index];
        } else if (_wcsicmp(argv[index], L"-PID") == 0) {
            options.core_pid = _wtoi(argv[++index]);
        } else if (_wcsicmp(argv[index], L"-AuthCode") == 0) {
            options.auth_code = _wtoi(argv[++index]);
        }
    }
    return options;
}

bool SetFrameworkCurrentDirectory(int core_pid, DWORD& error) {
    HANDLE core = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, FALSE,
                              static_cast<DWORD>(core_pid));
    if (!core) {
        error = GetLastError();
        return false;
    }

    std::wstring executable(32768, L'\0');
    DWORD length = static_cast<DWORD>(executable.size());
    const BOOL found = QueryFullProcessImageNameW(core, 0, executable.data(), &length);
    error = found ? ERROR_SUCCESS : GetLastError();
    CloseHandle(core);
    if (!found) return false;

    executable.resize(length);
    const std::filesystem::path framework_root =
        std::filesystem::path(executable).parent_path();
    if (!SetCurrentDirectoryW(framework_root.c_str())) {
        error = GetLastError();
        return false;
    }
    return true;
}

} // namespace

// CQP.dll resolves this export from the loader process. The returned buffer
// remains valid until this thread makes another CQP call.
extern "C" __declspec(dllexport) const char* __cdecl amn_call(
    const char* function, const char* args_json) {
    if (!active_pipe || !function || !args_json) return nullptr;

    amn::Json args;
    if (!amn::ParseJson(args_json, args) || !args.is<amn::JsonArray>()) return nullptr;

    thread_local std::string response;
    response = active_pipe->CallCore(function, args.get<amn::JsonArray>()).serialize();
    return response.c_str();
}

int wmain(int argc, wchar_t** argv) {
    const Options options = ParseOptions(argc, argv);
    if (options.plugin_path.empty() || options.core_pid <= 0) {
        amn::ConsoleLog(amn::ConsoleLevel::Error, "arguments", "-Path and -PID are required");
        return 2;
    }

    amn::ConsoleLog(amn::ConsoleLevel::Info, "loader starting",
                    "core PID=" + std::to_string(options.core_pid) +
                    ", plugin=" + std::filesystem::path(options.plugin_path).u8string());

    amn::ConsoleLog(amn::ConsoleLevel::Info, "connecting named pipe");
    auto pipe = amn::PipeClient::Connect(options.core_pid);
    if (!pipe) {
        amn::ConsoleLog(amn::ConsoleLevel::Error, "named pipe connection failed");
        return 3;
    }
    amn::ConsoleLog(amn::ConsoleLevel::Info, "named pipe connected");
    active_pipe = pipe.get();

    amn::PluginHost plugin(*pipe, options.plugin_path, options.auth_code);
    DWORD directory_error = ERROR_SUCCESS;
    if (!SetFrameworkCurrentDirectory(options.core_pid, directory_error)) {
        plugin.ReportError("无法将工作目录设为主框架根目录，Windows 错误码 " +
                           std::to_string(directory_error));
        return 6;
    }
    amn::ConsoleLog(amn::ConsoleLevel::Info, "framework directory set",
                    std::filesystem::current_path().u8string());
    if (!plugin.Load()) return 4;

    // The reader must run before Initialize: plugins may call CQP APIs there.
    pipe->Start([&plugin](const amn::Json& request) { plugin.HandleRequest(request); });
    amn::ConsoleLog(amn::ConsoleLevel::Info, "pipe reader started");
    if (!plugin.InitializeAndNotify()) {
        pipe->Close();
        pipe->Join();
        return 5;
    }

    amn::ConsoleLog(amn::ConsoleLevel::Info, "waiting for framework requests");
    pipe->Join();
    active_pipe = nullptr;
    amn::ConsoleLog(amn::ConsoleLevel::Info, "loader stopped");
    return 0;
}
