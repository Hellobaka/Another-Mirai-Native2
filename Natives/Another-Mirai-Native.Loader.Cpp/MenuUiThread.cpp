#include "MenuUiThread.h"

#include "Diagnostics.h"

#include <objbase.h>

#include <future>
#include <string>
#include <utility>

namespace amn {
namespace {

constexpr UINT kInvokeMenu = WM_APP + 1;
constexpr wchar_t kWindowClass[] = L"AnotherMiraiNativeCppMenuThread";

LRESULT CALLBACK MenuWindowProc(HWND window, UINT message, WPARAM callback, LPARAM data) {
    if (message == kInvokeMenu) {
        ConsoleLog(ConsoleLevel::Info, "menu callback started");
        try {
            reinterpret_cast<int(__stdcall*)()>(callback)();
        } catch (...) {
            ConsoleLog(ConsoleLevel::Error, "menu callback threw an exception");
        }
        ConsoleLog(ConsoleLevel::Info, "menu callback returned");
        return 0;
    }
    if (message == WM_CLOSE) {
        DestroyWindow(window);
        return 0;
    }
    if (message == WM_DESTROY) {
        PostQuitMessage(0);
        return 0;
    }
    return DefWindowProcW(window, message, callback, data);
}

void RunMenuThread(std::promise<HWND> ready) {
    const HRESULT initialized = CoInitializeEx(nullptr, COINIT_APARTMENTTHREADED);
    if (FAILED(initialized)) {
        ConsoleLog(ConsoleLevel::Error, "STA initialization failed",
                   "HRESULT=" + std::to_string(static_cast<unsigned long>(initialized)));
        ready.set_value(nullptr);
        return;
    }

    WNDCLASSW window_class{};
    window_class.lpfnWndProc = MenuWindowProc;
    window_class.hInstance = GetModuleHandleW(nullptr);
    window_class.lpszClassName = kWindowClass;
    if (!RegisterClassW(&window_class) && GetLastError() != ERROR_CLASS_ALREADY_EXISTS) {
        ConsoleLog(ConsoleLevel::Error, "menu window registration failed",
                   "Windows error=" + std::to_string(GetLastError()));
        ready.set_value(nullptr);
        CoUninitialize();
        return;
    }

    HWND window = CreateWindowExW(0, kWindowClass, L"", 0, 0, 0, 0, 0,
                                  HWND_MESSAGE, nullptr, window_class.hInstance, nullptr);
    if (!window) {
        ConsoleLog(ConsoleLevel::Error, "menu window creation failed",
                   "Windows error=" + std::to_string(GetLastError()));
        ready.set_value(nullptr);
        CoUninitialize();
        return;
    }

    ConsoleLog(ConsoleLevel::Info, "STA menu thread ready",
               "thread ID=" + std::to_string(GetCurrentThreadId()));
    ready.set_value(window);

    MSG message{};
    while (GetMessageW(&message, nullptr, 0, 0) > 0) {
        TranslateMessage(&message);
        DispatchMessageW(&message);
    }
    ConsoleLog(ConsoleLevel::Info, "STA menu thread stopped");
    CoUninitialize();
}

} // namespace

MenuUiThread::~MenuUiThread() {
    Stop();
}

bool MenuUiThread::Start() {
    if (thread_.joinable()) return window_ != nullptr;

    std::promise<HWND> ready;
    auto result = ready.get_future();
    thread_ = std::thread(RunMenuThread, std::move(ready));
    window_ = result.get();
    if (!window_) thread_.join();
    return window_ != nullptr;
}

bool MenuUiThread::Post(FARPROC callback) const {
    return window_ && PostMessageW(window_, kInvokeMenu,
                                   reinterpret_cast<WPARAM>(callback), 0);
}

bool MenuUiThread::Stop() {
    if (!thread_.joinable()) return true;
    if (window_) PostMessageW(window_, WM_CLOSE, 0, 0);
    window_ = nullptr;

    if (WaitForSingleObject(thread_.native_handle(), 2000) == WAIT_OBJECT_0) {
        thread_.join();
        return true;
    }
    ConsoleLog(ConsoleLevel::Error, "STA menu thread did not stop within two seconds");
    thread_.detach();
    return false;
}

} // namespace amn
