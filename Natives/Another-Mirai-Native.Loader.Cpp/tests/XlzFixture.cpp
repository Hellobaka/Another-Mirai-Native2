#include "common/Encoding.h"
#include "common/JsonCodec.h"
#include "generated/XlzApiNames.h"
#include "generated/XlzStructs.h"
#include <windows.h>
#include <atomic>
#include <cassert>
#include <cstddef>
#include <cstring>

using namespace amn;
static Json api;
static std::atomic<int> menu_calls{0};

template <typename T> T API(const char* name) {
    return reinterpret_cast<T>(static_cast<uintptr_t>(Integer(Member(api, name))));
}

static_assert(sizeof(xlz::PrivateMessageEvent) == 128, "x86 private message ABI");
static_assert(offsetof(xlz::PrivateMessageEvent, MessageContent) == 68, "x86 private content offset");
static_assert(sizeof(xlz::GroupMessageEvent) == 120, "x86 group message ABI");
static_assert(offsetof(xlz::GroupMessageEvent, MessageContent) == 76, "x86 group content offset");
static_assert(sizeof(xlz::EventTypeBase) == 68, "x86 unified event ABI");

extern "C" int __stdcall OnPrivate(const xlz::PrivateMessageEvent* message) {
    assert(message->SessionToken == nullptr);
    assert(message->FileMD5 == nullptr);
    return message->ThisQQ == 2222 && message->SenderQQ == 3333 && message->MessageReq == 42 &&
        message->MessageType == 166 && Gb18030ToUtf8(message->MessageContent) == "你好[@3333][bq1]" ? 0 : -71;
}
extern "C" int __stdcall OnGroup(const xlz::GroupMessageEvent* message) {
    return message->ThisQQ == 2222 && message->SenderQQ == 3333 && message->MessageGroupQQ == 4444 &&
        message->MessageType == 134 && message->FontId == 7 &&
        Gb18030ToUtf8(message->MessageContent) == "[Reply,Req=9]群消息" ? 0 : -72;
}
extern "C" int __stdcall OnEvent(const xlz::EventTypeBase* event) {
    if (event->ThisQQ != 2222 || event->TriggerQQ != 3333) return -73;
    if (event->EventType == 105) {
        API<void(__stdcall*)(const char*, int64_t, int64_t, int64_t, int32_t)>("处理好友验证事件")(
            "123", event->ThisQQ, event->TriggerQQ, event->MessageSeq, 1);
    }
    return 0;
}
extern "C" int __stdcall OnEnable() { return 1; }
extern "C" void __stdcall OnDisable() { SetLastError(73); }
extern "C" void __stdcall OnExit() { SetLastError(74); }
extern "C" int __stdcall OnMenu() { ++menu_calls; return 0; }
extern "C" int __stdcall GetMenuCalls() { return menu_calls; }

extern "C" const char* __stdcall apprun(const char* addresses, const char* auth) {
    if (strcmp(auth, "123") || !ParseJson(Gb18030ToUtf8(addresses), api)) return "{}";
    for (const auto& name : XlzApiNames) if (!Integer(Member(api, name[0]))) return "{}";
    const char* empty = API<const char*(__stdcall*)(const char*, int64_t, int64_t, const char*, const char*)>("添加好友")(
        auth, 1111, 3333, "", "");
    if (!empty || *empty) return "{}";
    CoTaskMemFree(const_cast<char*>(empty));
    API<const char*(__stdcall*)(const char*, const char*, int32_t, int32_t)>("输出日志")(auth, "fixture", 0, 0);
    int64_t random = -1; int32_t req = -1;
    const std::string text = Utf8ToGb18030("你好[@3333][bq1]");
    const char* sent = API<const char*(__stdcall*)(const char*, int64_t, int64_t, const char*, int64_t*, int32_t*)>("发送好友消息")(
        auth, 1111, 3333, text.c_str(), &random, &req);
    if (!sent || !*sent || random <= 0 || req <= 0) return "{}";
    CoTaskMemFree(const_cast<char*>(sent));
    if (!API<int32_t(__stdcall*)(const char*, int64_t, int64_t, int64_t, int32_t)>("撤回消息_群聊")(
            auth, 1111, 4444, random, req)) return "{}";
    if (!API<int32_t(__stdcall*)(const char*, int64_t, int64_t, int64_t, int32_t)>("设置管理员")(
            auth, 1111, 4444, 3333, 1)) return "{}";
    void* list = nullptr;
    if (API<int32_t(__stdcall*)(const char*, int64_t, void**)>("取好友列表")(auth, 1111, &list) != 1 || !list) return "{}";
    int32_t headers[2]; memcpy(headers, list, 8);
    xlz::FriendInfo* friend_info = nullptr; memcpy(&friend_info, static_cast<char*>(list) + 8, sizeof(friend_info));
    if (headers[0] != 1 || headers[1] != 1 || friend_info->QQNumber != 3333 ||
        Gb18030ToUtf8(friend_info->Name) != "测试好友") return "{}";
    void* group_list = nullptr;
    if (API<int32_t(__stdcall*)(const char*, int64_t, void**)>("取群列表")(auth, 1111, &group_list) != 1) return "{}";
    void* members = nullptr;
    if (API<int32_t(__stdcall*)(const char*, int64_t, int64_t, void**)>("取群成员列表")(auth, 1111, 4444, &members) != 1) return "{}";
    // Default stubs must not dereference unimplemented byte-array/object inputs.
    const char* stub = API<const char*(__stdcall*)(const char*, int64_t, int64_t, int32_t, const void*, int32_t, int32_t, int32_t)>("上传好友图片")(
        auth, 1111, 3333, 0, reinterpret_cast<void*>(1), 0, 0, 0);
    if (!stub || *stub) return "{}";
    CoTaskMemFree(const_cast<char*>(stub));
    static std::string result;
    result = Utf8ToGb18030(Json(JsonObject{
        {"appname", Json("Xlz 测试插件")}, {"data", Json(JsonObject{})},
        {"friendmsaddres", Json(int64_t(reinterpret_cast<intptr_t>(OnPrivate)))},
        {"groupmsaddres", Json(int64_t(reinterpret_cast<intptr_t>(OnGroup)))},
        {"eventmsaddres", Json(int64_t(reinterpret_cast<intptr_t>(OnEvent)))},
        {"useproaddres", Json(int64_t(reinterpret_cast<intptr_t>(OnEnable)))},
        {"banproaddres", Json(int64_t(reinterpret_cast<intptr_t>(OnDisable)))},
        {"unitproaddres", Json(int64_t(reinterpret_cast<intptr_t>(OnExit)))},
        {"setproaddres", Json(int64_t(reinterpret_cast<intptr_t>(OnMenu)))}
    }).serialize());
    return result.c_str();
}
