#include "Encoding.h"

#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <windows.h>

namespace amn {
namespace {

constexpr UINT kGb18030 = 54936;

std::wstring ToWide(const std::string& input, UINT code_page) {
    if (input.empty()) return {};

    const int length = MultiByteToWideChar(code_page, 0, input.data(),
                                           static_cast<int>(input.size()), nullptr, 0);
    if (length == 0) return {};

    std::wstring result(length, L'\0');
    MultiByteToWideChar(code_page, 0, input.data(), static_cast<int>(input.size()),
                        result.data(), length);
    return result;
}

std::string FromWide(const std::wstring& input, UINT code_page) {
    if (input.empty()) return {};

    const int length = WideCharToMultiByte(code_page, 0, input.data(),
                                           static_cast<int>(input.size()), nullptr, 0,
                                           nullptr, nullptr);
    if (length == 0) return {};

    std::string result(length, '\0');
    WideCharToMultiByte(code_page, 0, input.data(), static_cast<int>(input.size()),
                        result.data(), length, nullptr, nullptr);
    return result;
}

} // namespace

std::string Gb18030ToUtf8(const char* text) {
    return text ? FromWide(ToWide(text, kGb18030), CP_UTF8) : std::string{};
}

std::string Utf8ToGb18030(const std::string& text) {
    return FromWide(ToWide(text, CP_UTF8), kGb18030);
}

std::wstring Utf8ToWide(const std::string& text) {
    return ToWide(text, CP_UTF8);
}

} // namespace amn
