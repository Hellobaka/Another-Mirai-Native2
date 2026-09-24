#include "PluginHost.h"

#include "Diagnostics.h"
#include "Encoding.h"

#include <chrono>
#include <filesystem>
#include <fstream>
#include <iterator>
#include <utility>

namespace amn {
namespace {

// Json.NET accepts comments in CQP manifests. Remove them before passing the
// manifest to picojson, while leaving URLs and escaped quotes in strings alone.
std::string RemoveJsonComments(const std::string& input) {
    enum class State { Normal, String, LineComment, BlockComment };
    State state = State::Normal;
    bool escaped = false;
    std::string output = input;

    for (size_t index = 0; index < input.size(); ++index) {
        const char current = input[index];
        const char next = index + 1 < input.size() ? input[index + 1] : '\0';
        switch (state) {
        case State::Normal:
            if (current == '"') {
                state = State::String;
            } else if (current == '/' && (next == '/' || next == '*')) {
                state = next == '/' ? State::LineComment : State::BlockComment;
                output[index] = output[index + 1] = ' ';
                ++index;
            }
            break;
        case State::String:
            if (escaped) {
                escaped = false;
            } else if (current == '\\') {
                escaped = true;
            } else if (current == '"') {
                state = State::Normal;
            }
            break;
        case State::LineComment:
            if (current == '\n' || current == '\r') {
                state = State::Normal;
            } else {
                output[index] = ' ';
            }
            break;
        case State::BlockComment:
            if (current == '*' && next == '/') {
                output[index] = output[index + 1] = ' ';
                ++index;
                state = State::Normal;
            } else if (current != '\n' && current != '\r') {
                output[index] = ' ';
            }
            break;
        }
    }
    return output;
}

const char* EventName(int type) {
    switch (type) {
    case 21: return "PrivateMsg";
    case 2: return "GroupMsg";
    case 11: return "Upload";
    case 101: return "AdminChange";
    case 102: return "GroupMemberDecrease";
    case 103: return "GroupMemberIncrease";
    case 104: return "GroupBan";
    case 201: return "FriendAdded";
    case 301: return "FriendRequest";
    case 302: return "GroupAddRequest";
    case 1001: return "StartUp";
    case 1002: return "Exit";
    case 1003: return "Enable";
    case 1004: return "Disable";
    default: return nullptr;
    }
}

void LoadLibraries(const wchar_t* pattern) {
    WIN32_FIND_DATAW entry{};
    HANDLE search = FindFirstFileW(pattern, &entry);
    if (search == INVALID_HANDLE_VALUE) return;

    do {
        if (!(entry.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)) {
            const std::wstring path = std::wstring(L"libraries\\") + entry.cFileName;
            LoadLibraryW(path.c_str());
        }
    } while (FindNextFileW(search, &entry));
    FindClose(search);
}

std::string PluginString(const JsonArray& args, size_t index) {
    return Utf8ToGb18030(Text(args[index]));
}

int64_t Number(const JsonArray& args, size_t index) {
    return Integer(args[index]);
}

int Number32(const JsonArray& args, size_t index) {
    return static_cast<int>(Number(args, index));
}

} // namespace

PluginHost::PluginHost(PipeClient& pipe, std::wstring plugin_path, int auth_code)
    : pipe_(pipe), plugin_path_(std::move(plugin_path)), auth_code_(auth_code) {}

PluginHost::~PluginHost() {
    // A callback may still be displaying a modal window during shutdown.
    // Keep its DLL mapped until process exit if the UI thread cannot stop.
    const bool menu_thread_stopped = menu_thread_.Stop();
    if (plugin_ && menu_thread_stopped) FreeLibrary(plugin_);
}

void PluginHost::ReportError(const std::string& message) const {
    ConsoleLog(ConsoleLevel::Error, "plugin load", message);
    const auto now = std::chrono::system_clock::now();
    const auto timestamp = std::chrono::duration_cast<std::chrono::seconds>(
        now.time_since_epoch()).count();
    const Json log(JsonObject{
        {"time", Json(static_cast<int64_t>(timestamp))},
        {"priority", Json(static_cast<int64_t>(30))},
        {"source", Json("C++ 插件加载器")},
        {"status", Json("")},
        {"name", Json("加载插件")},
        {"detail", Json(message)}
    });
    pipe_.Send(Json(JsonObject{
        {"GUID", Json("")},
        {"Function", Json("InvokeCore_AddLog")},
        {"Args", Json(JsonArray{log})}
    }));
}

bool PluginHost::Load() {
    std::filesystem::path json_path(plugin_path_);
    json_path.replace_extension(L".json");
    std::ifstream file(json_path, std::ios::binary);
    if (!file) {
        ReportError("无法读取插件元数据: " + json_path.u8string());
        return false;
    }

    std::string json((std::istreambuf_iterator<char>(file)), {});
    if (json.rfind("\xef\xbb\xbf", 0) == 0) json.erase(0, 3);
    if (!ParseJson(RemoveJsonComments(json), metadata_)) {
        ReportError("插件元数据 JSON 解析失败: " + json_path.u8string());
        return false;
    }
    ConsoleLog(ConsoleLevel::Info, "manifest parsed", json_path.u8string());

    wchar_t executable[MAX_PATH]{};
    if (!GetModuleFileNameW(nullptr, executable, MAX_PATH)) {
        ReportError("无法获取加载器路径，Windows 错误码 " +
                    std::to_string(GetLastError()));
        return false;
    }
    const std::filesystem::path cqp_path =
        std::filesystem::path(executable).parent_path() / L"CQP.dll";

    CreateDirectoryW(L"libraries", nullptr);
    LoadLibraries(L"libraries\\*.dll");
    LoadLibraries(L"libraries\\*.exe");
    if (!LoadLibraryW(cqp_path.c_str())) {
        const DWORD error = GetLastError();
        ReportError("无法加载 CQP 桥接 DLL: " + cqp_path.u8string() +
                    "，Windows 错误码 " + std::to_string(error));
        return false;
    }
    ConsoleLog(ConsoleLevel::Info, "CQP bridge loaded", cqp_path.u8string());

    plugin_ = LoadLibraryW(plugin_path_.c_str());
    if (!plugin_) {
        const DWORD error = GetLastError();
        ReportError("无法加载插件 DLL: " + std::filesystem::path(plugin_path_).u8string() +
                    "，Windows 错误码 " + std::to_string(error) +
                    "（126 通常表示缺少依赖 DLL）");
        return false;
    }
    ConsoleLog(ConsoleLevel::Info, "plugin DLL loaded",
               std::filesystem::path(plugin_path_).u8string());

    const Json& configured_events = Member(metadata_, "event");
    if (configured_events.is<JsonArray>()) {
        for (const Json& event : configured_events.get<JsonArray>()) {
            const char* name = EventName(static_cast<int>(Integer(Member(event, "type"))));
            const std::string export_name = Field(event, "function");
            if (!name || export_name.empty()) continue;
            if (FARPROC function = GetProcAddress(plugin_, export_name.c_str())) {
                events_[name] = function;
            }
        }
    }

    const Json& configured_menus = Member(metadata_, "menu");
    if (configured_menus.is<JsonArray>()) {
        for (const Json& menu : configured_menus.get<JsonArray>()) {
            const std::string export_name = Field(menu, "function");
            if (FARPROC function = GetProcAddress(plugin_, export_name.c_str())) {
                menus_[export_name] = function;
            }
        }
    }
    ConsoleLog(ConsoleLevel::Info, "plugin callbacks bound",
               "events=" + std::to_string(events_.size()) +
               ", menus=" + std::to_string(menus_.size()));
    return true;
}

bool PluginHost::InitializeAndNotify() {
    using InitializeFunction = int(__stdcall*)(int);
    using AppInfoFunction = const char*(__stdcall*)();

    if (auto initialize = reinterpret_cast<InitializeFunction>(
            GetProcAddress(plugin_, "Initialize"))) {
        ConsoleLog(ConsoleLevel::Info, "calling Initialize");
        initialize(auth_code_);
        ConsoleLog(ConsoleLevel::Info, "Initialize returned");
    }

    std::string app_id = Field(metadata_, "AppId");
    if (auto app_info = reinterpret_cast<AppInfoFunction>(
            GetProcAddress(plugin_, "AppInfo"))) {
        if (const char* text = app_info()) {
            const std::string response(text);
            const size_t separator = response.find(',');
            if (separator != std::string::npos) app_id = response.substr(separator + 1);
        }
    }
    if (app_id.empty()) {
        ReportError("插件 AppInfo 未返回有效的 AppId: " +
                    std::filesystem::path(plugin_path_).u8string());
        return false;
    }
    ConsoleLog(ConsoleLevel::Info, "plugin AppId", app_id);

    if (!menus_.empty() && !menu_thread_.Start()) {
        ReportError("无法创建菜单专用 STA UI 线程");
        return false;
    }

    CreateDirectoryW(L"data", nullptr);
    CreateDirectoryW(L"data\\app", nullptr);
    const std::wstring data_path = L"data\\app\\" + Utf8ToWide(app_id);
    CreateDirectoryW(data_path.c_str(), nullptr);

    const Json startup(JsonObject{
        {"Type", Json("ClientStartUp_" + std::to_string(GetCurrentProcessId()))},
        {"GUID", Json("")},
        {"Result", Json(app_id)}
    });
    if (!pipe_.Send(startup)) {
        ReportError("插件初始化完成，但向主程序发送启动通知失败");
        return false;
    }
    ConsoleLog(ConsoleLevel::Info, "startup handshake sent");
    return true;
}

int PluginHost::InvokeMenu(const JsonArray& args) const {
    if (args.size() != 1) return -1;
    const auto found = menus_.find(Text(args[0]));
    if (found == menus_.end()) return -1;
    // Match the managed loader: acknowledge scheduling without waiting for a
    // modal dialog to close on the UI thread.
    return menu_thread_.Post(found->second) ? 1 : -1;
}

int PluginHost::InvokeEvent(const std::string& name, const JsonArray& args) const {
    const auto found = events_.find(name);
    if (found == events_.end()) return 0;
    FARPROC function = found->second;

    if (name == "PrivateMsg") {
        if (args.size() != 5) return -1;
        const std::string message = PluginString(args, 3);
        using Callback = int(__stdcall*)(int, int, int64_t, const char*, int);
        return reinterpret_cast<Callback>(function)(Number32(args, 0), Number32(args, 1),
                                                    Number(args, 2), message.c_str(),
                                                    Number32(args, 4));
    }
    if (name == "GroupMsg") {
        if (args.size() != 7) return -1;
        const std::string anonymous = PluginString(args, 4);
        const std::string message = PluginString(args, 5);
        using Callback = int(__stdcall*)(int, int, int64_t, int64_t,
                                         const char*, const char*, int);
        return reinterpret_cast<Callback>(function)(Number32(args, 0), Number32(args, 1),
                                                    Number(args, 2), Number(args, 3),
                                                    anonymous.c_str(), message.c_str(),
                                                    Number32(args, 6));
    }
    if (name == "Upload") {
        if (args.size() != 5) return -1;
        const std::string file = PluginString(args, 4);
        using Callback = int(__stdcall*)(int, int, int64_t, int64_t, const char*);
        return reinterpret_cast<Callback>(function)(Number32(args, 0), Number32(args, 1),
                                                    Number(args, 2), Number(args, 3),
                                                    file.c_str());
    }
    if (name == "AdminChange") {
        if (args.size() != 4) return -1;
        using Callback = int(__stdcall*)(int, int, int64_t, int64_t);
        return reinterpret_cast<Callback>(function)(Number32(args, 0), Number32(args, 1),
                                                    Number(args, 2), Number(args, 3));
    }
    if (name == "GroupMemberDecrease" || name == "GroupMemberIncrease") {
        if (args.size() != 5) return -1;
        using Callback = int(__stdcall*)(int, int, int64_t, int64_t, int64_t);
        return reinterpret_cast<Callback>(function)(Number32(args, 0), Number32(args, 1),
                                                    Number(args, 2), Number(args, 3),
                                                    Number(args, 4));
    }
    if (name == "GroupBan") {
        if (args.size() != 6) return -1;
        using Callback = int(__stdcall*)(int, int, int64_t, int64_t, int64_t, int64_t);
        return reinterpret_cast<Callback>(function)(Number32(args, 0), Number32(args, 1),
                                                    Number(args, 2), Number(args, 3),
                                                    Number(args, 4), Number(args, 5));
    }
    if (name == "FriendAdded") {
        if (args.size() != 3) return -1;
        using Callback = int(__stdcall*)(int, int, int64_t);
        return reinterpret_cast<Callback>(function)(Number32(args, 0), Number32(args, 1),
                                                    Number(args, 2));
    }
    if (name == "FriendRequest") {
        if (args.size() != 5) return -1;
        const std::string message = PluginString(args, 3);
        const std::string flag = PluginString(args, 4);
        using Callback = int(__stdcall*)(int, int, int64_t, const char*, const char*);
        return reinterpret_cast<Callback>(function)(Number32(args, 0), Number32(args, 1),
                                                    Number(args, 2), message.c_str(),
                                                    flag.c_str());
    }
    if (name == "GroupAddRequest") {
        if (args.size() != 6) return -1;
        const std::string message = PluginString(args, 4);
        const std::string flag = PluginString(args, 5);
        using Callback = int(__stdcall*)(int, int, int64_t, int64_t,
                                         const char*, const char*);
        return reinterpret_cast<Callback>(function)(Number32(args, 0), Number32(args, 1),
                                                    Number(args, 2), Number(args, 3),
                                                    message.c_str(), flag.c_str());
    }

    if (!args.empty()) return -1;
    return reinterpret_cast<int(__stdcall*)()>(function)();
}

void PluginHost::HandleRequest(const Json& request) {
    const std::string function = Field(request, "Function");
    if (function == "HeartBeat" || function == "CurrentQQChanged") return;
    ConsoleLog(ConsoleLevel::Info, "framework request", function);

    int result = 0;
    if (function.rfind("InvokeEvent_", 0) == 0) {
        const Json& raw_args = Member(request, "Args");
        const JsonArray args = raw_args.is<JsonArray>() ? raw_args.get<JsonArray>() : JsonArray{};
        const std::string event_name = function.substr(sizeof("InvokeEvent_") - 1);
        result = event_name == "Menu" ? InvokeMenu(args) : InvokeEvent(event_name, args);
    } else if (function == "KillProcess") {
        result = 1;
    }

    if (!pipe_.Send(Json(JsonObject{
        {"GUID", Json(Field(request, "GUID"))},
        {"Type", Json(function)},
        {"Result", Json(static_cast<int64_t>(result))}
    }))) {
        ConsoleLog(ConsoleLevel::Error, "event response failed", function);
    } else {
        ConsoleLog(ConsoleLevel::Info, "event response sent",
                   function + " result=" + std::to_string(result));
    }
    if (function == "KillProcess") ExitProcess(0);
}

} // namespace amn
