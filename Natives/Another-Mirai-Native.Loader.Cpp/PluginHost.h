#pragma once

#include "JsonCodec.h"
#include "MenuUiThread.h"
#include "PipeClient.h"

#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <windows.h>

#include <string>
#include <unordered_map>

namespace amn {

class PluginHost {
public:
    PluginHost(PipeClient& pipe, std::wstring plugin_path, int auth_code);
    ~PluginHost();

    bool Load();
    bool InitializeAndNotify();
    void HandleRequest(const Json& request);
    void ReportError(const std::string& message) const;

private:
    int InvokeEvent(const std::string& name, const JsonArray& args) const;
    int InvokeMenu(const JsonArray& args) const;

    PipeClient& pipe_;
    std::wstring plugin_path_;
    int auth_code_;
    HMODULE plugin_ = nullptr;
    MenuUiThread menu_thread_;
    Json metadata_;
    std::unordered_map<std::string, FARPROC> events_;
    std::unordered_map<std::string, FARPROC> menus_;
};

} // namespace amn
