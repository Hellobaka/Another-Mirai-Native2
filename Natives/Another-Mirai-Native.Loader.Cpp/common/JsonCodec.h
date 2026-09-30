#pragma once

#define PICOJSON_USE_INT64
#include "third_party/picojson.h"

#include <cstdint>
#include <cstdlib>
#include <string>

namespace amn {

using Json = picojson::value;
using JsonArray = picojson::array;
using JsonObject = picojson::object;

inline bool HasField(const Json& object, const char* name) {
    return object.is<JsonObject>() && object.get<JsonObject>().count(name) != 0;
}

inline const Json& Member(const Json& object, const char* name) {
    static const Json null_value;
    if (!object.is<JsonObject>()) return null_value;

    const auto& fields = object.get<JsonObject>();
    const auto found = fields.find(name);
    return found == fields.end() ? null_value : found->second;
}

inline std::string Text(const Json& value) {
    if (value.is<std::string>()) return value.get<std::string>();
    return value.is<picojson::null>() ? "" : value.serialize();
}

inline std::string Field(const Json& object, const char* name) {
    const Json& value = Member(object, name);
    return value.is<std::string>() ? value.get<std::string>() : "";
}

inline int64_t Integer(const Json& value) {
    if (value.is<int64_t>()) return value.get<int64_t>();
    if (value.is<double>()) return static_cast<int64_t>(value.get<double>());
    if (value.is<bool>()) return value.get<bool>() ? 1 : 0;
    if (value.is<std::string>()) return _strtoi64(value.get<std::string>().c_str(), nullptr, 10);
    return 0;
}

inline bool ParseJson(const std::string& input, Json& output) {
    std::string error;
    picojson::parse(output, input.begin(), input.end(), &error);
    return error.empty();
}

} // namespace amn
