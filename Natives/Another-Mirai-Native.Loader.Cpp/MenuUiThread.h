#pragma once

#define WIN32_LEAN_AND_MEAN
#include <windows.h>

#include <thread>

namespace amn {

// Owns an STA thread with a Windows message loop for CQP menu callbacks.
class MenuUiThread {
public:
    MenuUiThread() = default;
    ~MenuUiThread();

    MenuUiThread(const MenuUiThread&) = delete;
    MenuUiThread& operator=(const MenuUiThread&) = delete;

    bool Start();
    bool Post(FARPROC callback) const;
    // Returns false when a plugin callback is still running after the timeout.
    bool Stop();

private:
    HWND window_ = nullptr;
    std::thread thread_;
};

} // namespace amn
