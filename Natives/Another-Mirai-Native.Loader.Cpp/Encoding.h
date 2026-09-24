#pragma once

#include <string>

namespace amn {

std::string Gb18030ToUtf8(const char* text);
std::string Utf8ToGb18030(const std::string& text);
std::wstring Utf8ToWide(const std::string& text);

} // namespace amn
