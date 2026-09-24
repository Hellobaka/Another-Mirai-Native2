#pragma once

#include <string_view>

namespace amn {

enum class ConsoleLevel { Info, Error };

// Writes one UTF-8 message to stderr, or Unicode text when stderr is a console.
void ConsoleLog(ConsoleLevel level, std::string_view stage,
                std::string_view detail = {});

} // namespace amn
