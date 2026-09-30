#include "PluginHost.h"

#include "Diagnostics.h"
#include "Encoding.h"
#include "XlzApiNames.h"
#include "XlzMessage.h"
#include "XlzStructs.h"

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

FARPROC ResolveExport(HMODULE module, const std::string& name) {
    if (name.empty()) return nullptr;
    if (FARPROC function = GetProcAddress(module, name.c_str())) return function;
    return GetProcAddress(module, Utf8ToGb18030(name).c_str());
}

} // namespace

PluginHost::PluginHost(PipeClient& pipe, std::wstring plugin_path, int auth_code, int64_t current_qq)
    : pipe_(pipe), plugin_path_(std::move(plugin_path)), auth_code_(auth_code), current_qq_(current_qq) {}

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
    // CQP uses <plugin>.json. Xlz V4 stores its manifest in
    // <plugin>.dll.json; keep the original extension when looking for it.
    std::filesystem::path xlz_json_path(plugin_path_);
    xlz_json_path += L".json";
    std::ifstream xlz_file(xlz_json_path, std::ios::binary);
    std::ifstream file;
    if (xlz_file) {
        xlz_plugin_ = true;
        json_path = xlz_json_path;
        file.swap(xlz_file);
    } else {
        json_path.replace_extension(L".json");
        file.open(json_path, std::ios::binary);
    }
    if (file && !xlz_plugin_) {
        std::string json((std::istreambuf_iterator<char>(file)), {});
        if (json.rfind("\xef\xbb\xbf", 0) == 0) json.erase(0, 3);
        if (!ParseJson(RemoveJsonComments(json), metadata_)) {
            ReportError("插件元数据 JSON 解析失败: " + json_path.u8string());
            return false;
        }
        ConsoleLog(ConsoleLevel::Info, "manifest parsed", json_path.u8string());
    }

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
    cqp_ = LoadLibraryW(cqp_path.c_str());
    if (!cqp_) {
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

    xlz_plugin_ = xlz_plugin_ || ResolveExport(plugin_, "初始化") || ResolveExport(plugin_, "apprun");
    // Xlz initialization may call APIs. Defer it until the pipe reader runs.
    if (xlz_plugin_) return true;
    if (!metadata_.is<JsonObject>()) {
        ReportError("无法读取插件元数据: " + json_path.u8string());
        return false;
    }
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

bool PluginHost::InitializeXlz() {
    using Entry = const char*(__stdcall*)(const char*, const char*);
    FARPROC entry = ResolveExport(plugin_, "初始化");
    if (!entry) entry = ResolveExport(plugin_, "apprun");
    if (!entry) {
        ReportError("无法获取小栗子插件入口点 初始化/apprun");
        return false;
    }
    JsonObject addresses;
    for (const auto& api : XlzApiNames) {
        FARPROC function = GetProcAddress(cqp_, api[1]);
        if (!function) {
            ReportError(std::string("CQP 桥接 DLL 缺少小栗子 API: ") + api[1]);
            return false;
        }
        addresses[api[0]] = Json(static_cast<int64_t>(reinterpret_cast<intptr_t>(function)));
    }
    const std::string api_info = Utf8ToGb18030(Json(addresses).serialize());
    const std::string auth = std::to_string(auth_code_);
    const char* raw_info = reinterpret_cast<Entry>(entry)(api_info.c_str(), auth.c_str());
    Json native_info;
    if (!raw_info || !ParseJson(RemoveJsonComments(Gb18030ToUtf8(raw_info)), native_info) ||
        Field(native_info, "appname").empty()) {
        ReportError("小栗子插件初始化未返回有效插件信息");
        return false;
    }
    metadata_ = native_info;
    const auto sidecar = std::filesystem::path(plugin_path_).wstring() + L".json";
    std::ifstream file(sidecar, std::ios::binary);
    const bool v4 = static_cast<bool>(file);
    if (v4) {
        std::string text((std::istreambuf_iterator<char>(file)), {});
        if (text.rfind("\xef\xbb\xbf", 0) == 0) text.erase(0, 3);
        if (!ParseJson(RemoveJsonComments(text), metadata_) || !metadata_.is<JsonObject>()) {
            ReportError("小栗子 V4 元数据 JSON 解析失败");
            return false;
        }
    }
    struct Callback { const char* name; const char* v3; const char* v4; };
    const Callback callbacks[] = {
        {"PrivateMsg", "friendmsaddres", "私聊消息处理函数"},
        {"GroupMsg", "groupmsaddres", "群聊消息处理函数"},
        {"ReceiveEvent", "eventmsaddres", "事件消息处理函数"},
        {"Enable", "useproaddres", "被启用处理函数"},
        {"Disable", "banproaddres", "被禁用处理函数"},
        {"Exit", "unitproaddres", "将被卸载处理函数"},
        {"Menu", "setproaddres", "插件菜单处理函数"},
    };
    for (const auto& callback : callbacks) {
        const int64_t address = Integer(Member(metadata_, callback.v3));
        const std::string export_name = Field(metadata_, callback.v4);
        FARPROC function = v4 ? ResolveExport(plugin_, export_name) :
            reinterpret_cast<FARPROC>(static_cast<uintptr_t>(address));
        const bool configured = v4 ? !export_name.empty() : address != 0;
        if (configured && !function) {
            ReportError("无法获取小栗子事件回调: " + export_name);
            return false;
        }
        if (!function) continue;
        if (std::string(callback.name) == "Menu") menus_[export_name] = function;
        else events_[callback.name] = function;
    }
    ConsoleLog(ConsoleLevel::Info, v4 ? "Xlz V4 initialized" : "Xlz V3 initialized",
               Field(native_info, "appname") + ", events=" + std::to_string(events_.size()));
    return true;
}

bool PluginHost::InitializeAndNotify() {
    using InitializeFunction = int(__stdcall*)(int);
    using AppInfoFunction = const char*(__stdcall*)();

    if (xlz_plugin_ && !InitializeXlz()) return false;
    if (!xlz_plugin_) {
        if (auto initialize = reinterpret_cast<InitializeFunction>(GetProcAddress(plugin_, "Initialize"))) {
            ConsoleLog(ConsoleLevel::Info, "calling Initialize");
            initialize(auth_code_);
            ConsoleLog(ConsoleLevel::Info, "Initialize returned");
        }
    }

    std::string app_id = xlz_plugin_
        ? std::filesystem::path(plugin_path_).stem().u8string()
        : Field(metadata_, "AppId");
    if (!xlz_plugin_) {
        if (auto app_info = reinterpret_cast<AppInfoFunction>(
                GetProcAddress(plugin_, "AppInfo"))) {
            if (const char* text = app_info()) {
                const std::string response(text);
                const size_t separator = response.find(',');
                if (separator != std::string::npos) app_id = response.substr(separator + 1);
            }
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
    const auto found = xlz_plugin_ ? menus_.begin() : menus_.find(Text(args[0]));
    if (found == menus_.end()) return -1;
    // Match the managed loader: acknowledge scheduling without waiting for a
    // modal dialog to close on the UI thread.
    return menu_thread_.Post(found->second) ? 1 : -1;
}

int PluginHost::InvokeXlzEvent(const std::string& name, const JsonArray& args) const {
    const auto found = events_.find(name == "StartUp" ? "Enable" : name);
    if (name == "StartUp" || name == "Enable" || name == "Disable" || name == "Exit") {
        if (!args.empty()) return -1;
        if (found == events_.end()) return 0;
        if (name == "Exit" || name == "Disable") {
            reinterpret_cast<void(__stdcall*)()>(found->second)();
            return 0;
        }
        // Xlz plugins may return 1 on successful activation. AMN expects 0
        // for a completed CQP lifecycle event; only message results block.
        reinterpret_cast<int(__stdcall*)()>(found->second)();
        return 0;
    }
    const int now = static_cast<int>(std::chrono::duration_cast<std::chrono::seconds>(
        std::chrono::system_clock::now().time_since_epoch()).count());
    using MessageCallback = int(__stdcall*)(const void*);
    if (name == "PrivateMsg" || name == "GroupMsg") {
        if (args.size() != (name == "PrivateMsg" ? 5 : 7)) return -1;
        if (found == events_.end()) return 0;
        const std::string message = Utf8ToGb18030(xlz::ConvertMessage(Text(args[name == "PrivateMsg" ? 3 : 5]), false));
        if (name == "PrivateMsg") {
            xlz::PrivateMessageEvent event;
            event.SenderQQ = Number(args, 2); event.ThisQQ = current_qq_.load();
            event.MessageReq = Number32(args, 1); event.MessageSendTime = now;
            event.MessageReceiveTime = now; event.MessageType = 166;
            event.MessageContent = message.c_str();
            // SessionToken is an E-language byte set, not a C string.
            event.SessionToken = nullptr; event.SourceEventQQName = "";
            event.FileID = ""; event.FileMD5 = nullptr; event.FileName = "";
            return reinterpret_cast<MessageCallback>(found->second)(&event);
        }
        xlz::GroupMessageEvent event;
        const std::string anonymous = PluginString(args, 4);
        event.SenderQQ = Number(args, 3); event.ThisQQ = current_qq_.load();
        event.MessageGroupQQ = Number(args, 2); event.MessageReq = Number32(args, 1);
        event.MessageSendTime = now; event.MessageReceiveTime = now;
        event.MessageType = 134; event.MessageContent = message.c_str();
        event.SourceGroupName = ""; event.SenderNickname = ""; event.SenderTitle = "";
        event.ReplyMessageContent = ""; event.ReservedParameters = "";
        event.AnonymousNickname = anonymous.c_str(); event.FontId = Number32(args, 6);
        return reinterpret_cast<MessageCallback>(found->second)(&event);
    }
    const auto unified = events_.find("ReceiveEvent");
    if (unified == events_.end() || name == "Upload" || name == "DiscussMsg") return 0;
    xlz::EventTypeBase event;
    event.ThisQQ = current_qq_.load();
    std::string group, trigger, operate, message;
    if (name == "AdminChange") {
        if (args.size() != 4) return -1;
        event.EventType = Number32(args, 0) == 1 ? 9 : 10;
        event.MessageTimestamp = now; event.EventSubType = Number32(args, 0);
        event.SourceGroupQQ = Number(args, 2); event.TriggerQQ = Number(args, 3);
    } else if (name == "FriendAdded" || name == "FriendRequest") {
        if (args.size() != (name == "FriendAdded" ? 3 : 5)) return -1;
        event.EventType = name == "FriendAdded" ? 104 : 105;
        event.MessageTimestamp = name == "FriendAdded" ? now : Number32(args, 1);
        event.TriggerQQ = Number(args, 2);
        event.EventSubType = name == "FriendAdded" ? Number32(args, 0) : 2;
        if (name == "FriendRequest") message = PluginString(args, 3);
    } else if (name == "GroupAddRequest") {
        if (args.size() != 6) return -1;
        event.EventType = 3; event.MessageTimestamp = Number32(args, 1);
        event.SourceGroupQQ = Number(args, 2); event.TriggerQQ = Number(args, 3);
        message = PluginString(args, 4);
    } else if (name == "GroupBan" || name == "GroupMemberDecrease" || name == "GroupMemberIncrease") {
        if (args.size() != (name == "GroupBan" ? 6 : 5)) return -1;
        event.MessageTimestamp = Number32(args, 1); event.SourceGroupQQ = Number(args, 2);
        event.OperateQQ = Number(args, 3); event.TriggerQQ = Number(args, 4);
        if (name == "GroupBan") {
            event.MessageSeq = Number(args, 5); event.EventType = event.MessageSeq == 0 ? 28 : 7;
        } else if (name == "GroupMemberDecrease") event.EventType = Number32(args, 0) == 1 ? 5 : 6;
        else event.EventType = Number32(args, 0) == 1 ? 2 : 25;
    } else return 0;
    if (name == "FriendRequest" || name == "GroupAddRequest") {
        using CacheRequest = int64_t(__stdcall*)(const char*);
        auto cache = reinterpret_cast<CacheRequest>(GetProcAddress(cqp_, "amn_xlz_cache_request"));
        if (!cache) return -1;
        const std::string flag = PluginString(args, args.size() - 1);
        event.MessageSeq = cache(flag.c_str());
    }
    group = event.SourceGroupQQ ? std::to_string(event.SourceGroupQQ) : "";
    trigger = std::to_string(event.TriggerQQ); operate = std::to_string(event.OperateQQ);
    event.SourceGroupName = group.c_str(); event.TriggerQQName = trigger.c_str();
    event.OperateQQName = operate.c_str(); event.MessageContent = message.c_str();
    return reinterpret_cast<MessageCallback>(unified->second)(&event);
}

int PluginHost::InvokeEvent(const std::string& name, const JsonArray& args) const {
    if (xlz_plugin_) return InvokeXlzEvent(name, args);
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
    if (function == "HeartBeat") return;
    if (function == "CurrentQQChanged") {
        const Json& args = Member(request, "Args");
        if (args.is<JsonArray>() && !args.get<JsonArray>().empty()) current_qq_ = Integer(args.get<JsonArray>()[0]);
        return;
    }
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
