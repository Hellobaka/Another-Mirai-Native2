#include "Diagnostics.h"

#define WIN32_LEAN_AND_MEAN
#include <windows.h>

#include <cstdio>
#include <mutex>
#include <string>

namespace amn {
namespace {

std::mutex console_mutex;

} // namespace

void ConsoleLog(ConsoleLevel level, std::string_view stage, std::string_view detail) {
    HANDLE output = GetStdHandle(STD_ERROR_HANDLE);
    if (!output || output == INVALID_HANDLE_VALUE) return;

    SYSTEMTIME time{};
    GetLocalTime(&time);
    char prefix[96]{};
    std::snprintf(prefix, sizeof(prefix), "[%02u:%02u:%02u.%03u][%lu][%s] ",
                  time.wHour, time.wMinute, time.wSecond, time.wMilliseconds,
                  GetCurrentProcessId(), level == ConsoleLevel::Error ? "ERROR" : "INFO");

    std::string line(prefix);
    line.append(stage.data(), stage.size());
    if (!detail.empty()) {
        line.append(": ");
        line.append(detail.data(), detail.size());
    }
    line.push_back('\n');

    std::lock_guard<std::mutex> lock(console_mutex);
    DWORD mode = 0;
    DWORD written = 0;
    if (GetConsoleMode(output, &mode)) {
        const int count = MultiByteToWideChar(CP_UTF8, 0, line.data(),
                                               static_cast<int>(line.size()), nullptr, 0);
        if (count <= 0) return;
        std::wstring wide(count, L'\0');
        MultiByteToWideChar(CP_UTF8, 0, line.data(), static_cast<int>(line.size()),
                            wide.data(), count);
        WriteConsoleW(output, wide.data(), static_cast<DWORD>(wide.size()), &written, nullptr);
    } else {
        WriteFile(output, line.data(), static_cast<DWORD>(line.size()), &written, nullptr);
    }
}

} // namespace amn
