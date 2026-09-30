#include "Encoding.h"
#include "JsonCodec.h"
#include "XlzMessage.h"
#include "XlzStructs.h"

#define WIN32_LEAN_AND_MEAN
#include <windows.h>

#include <cstring>
#include <string>
#include <objbase.h>
#include <atomic>
#include <chrono>
#include <map>
#include <mutex>
#include <stdexcept>
#include <vector>
#include <algorithm>
#include <type_traits>

namespace {

using amn::Json;
using amn::JsonArray;

Json Argument(int value) { return Json(static_cast<int64_t>(value)); }
Json Argument(int64_t value) { return Json(value); }
Json Argument(bool value) { return Json(value); }
Json Argument(const char* value) { return Json(amn::Gb18030ToUtf8(value)); }

Json call_json(const char* name, const JsonArray& args) {
    using CallFunction = const char*(__cdecl*)(const char*, const char*);
    static auto callback = reinterpret_cast<CallFunction>(
        GetProcAddress(GetModuleHandleW(nullptr), "amn_call"));
    if (!callback) return {};

    const std::string payload = Json(args).serialize();
    const char* response = callback(name, payload.c_str());
    if (!response) return {};

    Json result;
    return amn::ParseJson(response, result) ? result : Json{};
}

template <typename... Args>
Json call(const char* name, Args... args) {
    return call_json(name, JsonArray{Argument(args)...});
}

int64_t result_number(const Json& result) {
    return amn::Integer(result);
}

const char* result_text(const Json& result) {
    thread_local std::string buffer;
    buffer = amn::Utf8ToGb18030(amn::Text(result));
    return buffer.c_str();
}

} // namespace

#include "cqp_exports.inc"
#include "XlzBridge.inc"
#include "xlz_exports.inc"
