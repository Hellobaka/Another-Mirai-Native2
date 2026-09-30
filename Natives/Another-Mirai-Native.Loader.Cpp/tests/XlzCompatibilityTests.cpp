#include "loader/PluginHost.h"
#include "common/Encoding.h"
#include "xlz/XlzMessage.h"
#include <windows.h>
#include <cassert>
#include <filesystem>
#include <fstream>
#include <iostream>

using namespace amn;
static int64_t login_qq = 1111;
static int sent = 0, deleted = 0, approved = 0;

static void Number(std::string& output, uint64_t value, int bytes) {
    for (int i = bytes - 1; i >= 0; --i) output.push_back(char(value >> (i * 8)));
}
static void Token(std::string& output, const std::string& value) {
    Number(output, value.size(), 2); output += value;
}
static std::string Base64(const std::string& input) {
    const char* alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
    std::string result;
    uint32_t bits = 0; int available = 0;
    for (unsigned char c : input) {
        bits = (bits << 8) | c; available += 8;
        while (available >= 6) { available -= 6; result += alphabet[(bits >> available) & 63]; }
    }
    if (available) result += alphabet[(bits << (6 - available)) & 63];
    while (result.size() % 4) result += '=';
    return result;
}
static Json Collection(const std::string& record) {
    std::string bytes; Number(bytes, 1, 4); Token(bytes, record); return Json(Base64(bytes));
}

extern "C" __declspec(dllexport) const char* __cdecl amn_call(const char* function, const char* arguments) {
    Json json; assert(ParseJson(arguments, json));
    const auto args = json.get<JsonArray>();
    assert(Integer(args[0]) == 123);
    Json result;
    const std::string name(function);
    if (name == "CQ_getLoginQQ") result = Json(login_qq);
    else if (name == "CQ_addLog") { assert(Text(args[3]) == "fixture"); result = Json(int64_t(1)); }
    else if (name == "CQ_sendPrivateMsg") {
        assert(Integer(args[1]) == 3333 && Text(args[2]) == "你好[CQ:at,qq=3333][CQ:face,id=1]");
        ++sent; result = Json(int64_t(99));
    } else if (name == "CQ_deleteMsg") { assert(Integer(args[1]) == 99); ++deleted; result = Json(int64_t(1)); }
    else if (name == "CQ_setGroupAdmin") {
        assert(Integer(args[1]) == 4444 && Integer(args[2]) == 3333 && args[3].is<bool>() && args[3].get<bool>());
        result = Json(int64_t(1));
    } else if (name == "CQ_getFriendList") {
        std::string record; Number(record, 3333, 8); Token(record, Utf8ToGb18030("测试好友")); Token(record, "remark");
        result = Collection(record);
    } else if (name == "CQ_getGroupList") {
        std::string record; Number(record, 4444, 8); Token(record, Utf8ToGb18030("测试群")); result = Collection(record);
    } else if (name == "CQ_getGroupMemberList") {
        std::string record; Number(record, 4444, 8); Number(record, 3333, 8);
        Token(record, "nick"); Token(record, "card"); Number(record, 0, 4); Number(record, 20, 4);
        Token(record, "area"); Number(record, 100, 4); Number(record, 200, 4); Token(record, "level");
        Number(record, 2, 4); Number(record, 0, 4); Token(record, "title"); Number(record, 300, 4); Number(record, 1, 4);
        result = Collection(record);
    } else if (name == "CQ_setFriendAddRequest") {
        assert(Text(args[1]) == "original-request-flag" && Integer(args[2]) == 1); ++approved; result = Json(int64_t(1));
    } else { std::cerr << "Unexpected API: " << name << '\n'; std::abort(); }
    thread_local std::string response; response = result.serialize(); return response.c_str();
}

static Json ReadPacket(HANDLE pipe) {
    DWORD read = 0; uint32_t length = 0;
    assert(ReadFile(pipe, &length, sizeof(length), &read, nullptr) && read == sizeof(length));
    std::string bytes(length, '\0');
    assert(ReadFile(pipe, bytes.data(), length, &read, nullptr) && read == length);
    Json result; assert(ParseJson(bytes, result)); return result;
}

static void Run(const std::filesystem::path& plugin_path, bool v4) {
    login_qq = 1111;
    if (v4) {
        std::ofstream file(plugin_path.wstring() + L".json");
        file << Json(JsonObject{
            {"私聊消息处理函数", Json("OnPrivate")}, {"群聊消息处理函数", Json("OnGroup")},
            {"事件消息处理函数", Json("OnEvent")}, {"被启用处理函数", Json("OnEnable")},
            {"被禁用处理函数", Json("OnDisable")}, {"将被卸载处理函数", Json("OnExit")},
            {"插件菜单处理函数", Json("OnMenu")}
        }).serialize();
    }
    const std::wstring pipe_name = L"\\\\.\\pipe\\amn-xlz-test-" + std::to_wstring(GetCurrentProcessId()) + (v4 ? L"-v4" : L"-v3");
    HANDLE server = CreateNamedPipeW(pipe_name.c_str(), PIPE_ACCESS_DUPLEX, PIPE_TYPE_BYTE, 1, 65536, 65536, 0, nullptr);
    assert(server != INVALID_HANDLE_VALUE);
    HANDLE client = CreateFileW(pipe_name.c_str(), GENERIC_READ | GENERIC_WRITE, 0, nullptr, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, nullptr);
    assert(client != INVALID_HANDLE_VALUE);
    assert(ConnectNamedPipe(server, nullptr) || GetLastError() == ERROR_PIPE_CONNECTED);
    {
        PipeClient pipe(client);
        PluginHost host(pipe, plugin_path.wstring(), 123, 1111);
        assert(host.Load()); assert(host.InitializeAndNotify());
        assert(Field(ReadPacket(server), "Result") == plugin_path.stem().u8string());
        host.HandleRequest(Json(JsonObject{{"Function", Json("CurrentQQChanged")}, {"Args", Json(JsonArray{Json(int64_t(2222)), Json("nick")})}}));
        login_qq = 2222;
        auto event = [&](const char* name, JsonArray args, int expected = 0) {
            host.HandleRequest(Json(JsonObject{{"GUID", Json("test")}, {"Function", Json(std::string("InvokeEvent_") + name)}, {"Args", Json(args)}}));
            assert(Integer(Member(ReadPacket(server), "Result")) == expected);
        };
        event("StartUp", {}); event("Enable", {});
        event("PrivateMsg", {Json(int64_t(1)), Json(int64_t(42)), Json(int64_t(3333)), Json("你好[CQ:at,qq=3333][CQ:face,id=1]"), Json(int64_t(0))});
        event("GroupMsg", {Json(int64_t(1)), Json(int64_t(42)), Json(int64_t(4444)), Json(int64_t(3333)), Json(""), Json("[CQ:reply,id=9]群消息"), Json(int64_t(7))});
        event("FriendRequest", {Json(int64_t(1)), Json(int64_t(123456)), Json(int64_t(3333)), Json("申请"), Json("original-request-flag")});
        event("GroupBan", {Json(int64_t(1)), Json(int64_t(123456)), Json(int64_t(4444)), Json(int64_t(5555)), Json(int64_t(3333)), Json(int64_t(60))});
        event("PrivateMsg", {}, -1);
        event("Menu", {Json(v4 ? "OnMenu" : "")}, 1);
        HMODULE plugin = GetModuleHandleW(plugin_path.c_str());
        auto count = reinterpret_cast<int(__stdcall*)()>(GetProcAddress(plugin, "GetMenuCalls"));
        for (int i = 0; i < 100 && count() == 0; ++i) Sleep(10);
        assert(count() > 0);
        event("Disable", {}); event("Exit", {});
    }
    CloseHandle(server);
}

int wmain(int argc, wchar_t** argv) {
    assert(argc == 3 || argc == 4);
    Run(std::filesystem::absolute(argv[1]), false);
    Run(std::filesystem::absolute(argv[2]), true);
    assert(sent == 2 && deleted == 2 && approved == 2);
    if (argc == 4) {
        Run(std::filesystem::absolute(argv[3]), false);
        assert(sent == 3 && deleted == 2 && approved == 3);
        std::cout << "Original C# SDK API delegates, arrays and callback layouts passed\n";
    }
    // Both conversion directions must work in one process.
    assert(xlz::ConvertMessage("[bq1]", true) == "[CQ:face,id=1]");
    assert(xlz::ConvertMessage("[CQ:face,id=1]", false) == "[bq1]");
    std::cout << "Xlz V3/V4 ABI, APIs, arrays, requests, lifecycle and menus passed\n";
}
