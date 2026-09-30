#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <mscoree.h>
#include <filesystem>
#include <string>

// Only hosts CLR and exposes apprun. APIs and message layouts are exercised
// by the unmodified SDK in the managed fixture, not by native copies.
static ICLRRuntimeHost* runtime = nullptr;
static std::wstring assembly;

extern "C" const char* __stdcall apprun(const char* addresses, const char* auth) {
    HMODULE module = nullptr;
    GetModuleHandleExW(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS | GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT,
                      reinterpret_cast<LPCWSTR>(apprun), &module);
    wchar_t path[32768]{};
    GetModuleFileNameW(module, path, 32768);
    assembly = (std::filesystem::path(path).parent_path() / L"XlzSdkFixture.Managed.dll").wstring();
    if (!runtime) {
        HRESULT result = CorBindToRuntimeEx(L"v4.0.30319", L"wks", 0, CLSID_CLRRuntimeHost,
                                           IID_ICLRRuntimeHost, reinterpret_cast<void**>(&runtime));
        if (FAILED(result) || FAILED(runtime->Start())) return "{}";
    }
    const char* output = nullptr;
    const std::wstring context = std::to_wstring(reinterpret_cast<uintptr_t>(addresses)) + L"|" +
        std::to_wstring(reinterpret_cast<uintptr_t>(auth)) + L"|" + std::to_wstring(reinterpret_cast<uintptr_t>(&output));
    DWORD result = 0;
    HRESULT status = runtime->ExecuteInDefaultAppDomain(assembly.c_str(), L"Amn.XlzSdkTests.Fixture",
                                                       L"Initialize", context.c_str(), &result);
    return SUCCEEDED(status) && result == 1 && output ? output : "{}";
}

extern "C" int __stdcall GetMenuCalls() {
    DWORD result = 0;
    if (!runtime || FAILED(runtime->ExecuteInDefaultAppDomain(assembly.c_str(), L"Amn.XlzSdkTests.Fixture",
                                                            L"MenuCalls", L"", &result))) return -1;
    return static_cast<int>(result);
}
