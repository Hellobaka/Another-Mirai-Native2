#pragma once

#include <filesystem>
#include <regex>
#include <string>
#include <unordered_map>

namespace amn::xlz {

// MessageParser.cs uses the same token-by-token conversion. Unknown bracketed
// tokens are ignored, while ordinary text is preserved.
inline std::string ConvertMessage(const std::string& input, bool to_cq) {
    static const std::regex xlz_token("\\[.*?\\]");
    static const std::regex cq_token("\\[CQ:.*?\\]");
    const auto& token = to_cq ? xlz_token : cq_token;
    std::string output;
    size_t cursor = 0;
    for (std::sregex_iterator it(input.begin(), input.end(), token), end; it != end; ++it) {
        const auto& match = *it;
        output.append(input, cursor, match.position() - cursor);
        cursor = match.position() + match.length();
        std::string text = match.str();
        if (to_cq && text.rfind("[@", 0) == 0) {
            output += "[CQ:at,qq=" + text.substr(2);
            continue;
        }
        if (to_cq && text.rfind("[bq", 0) == 0) {
            output += "[CQ:face,id=" + text.substr(3);
            continue;
        }
        text = text.substr(to_cq ? 1 : 4, text.size() - (to_cq ? 2 : 5));
        const auto comma = text.find(',');
        const std::string type = text.substr(0, comma);
        std::unordered_map<std::string, std::string> fields;
        size_t offset = comma;
        while (offset != std::string::npos) {
            const size_t next = text.find(',', offset + 1);
            const std::string item = text.substr(offset + 1, next == std::string::npos ? next : next - offset - 1);
            const auto equals = item.find('=');
            if (equals != std::string::npos) fields[item.substr(0, equals)] = item.substr(equals + 1);
            offset = next;
        }
        auto value = [&](const char* key) { return fields.count(key) ? fields[key] : std::string{}; };
        if (to_cq) {
            if (type == "Shake") output += "[CQ:shake]";
            else if (type == "Reply" && fields.count("Req")) output += "[CQ:reply,id=" + value("Req") + "]";
            else if ((type == "bigFace" || type == "smallFace") && fields.count("Id"))
                output += std::string(type == "bigFace" ? "[CQ:bFace,id=" : "[CQ:sface,id=") + value("Id") + "]";
            else if (type == "picFile" || type == "pic" || type == "AudioFile" || type == "flashPicFile") {
                const std::string cq_type = type == "AudioFile" ? "record" : "image";
                if (fields.count("path")) {
                    std::error_code error;
                    auto path = std::filesystem::u8path(value("path"));
                    auto relative = std::filesystem::relative(path, std::filesystem::current_path(), error);
                    const std::string file = error ? value("path") : relative.u8string();
                    output += "[CQ:" + cq_type + ",file=" + file + (type == "flashPicFile" ? ",flash=true]" : "]");
                } else if (fields.count("hash")) output += "[CQ:" + cq_type + ",id=" + value("hash") + "]";
            }
        } else {
            if (type == "face") output += "[bq" + value("id") + "]";
            else if (type == "at") output += "[@" + value("qq") + "]";
            else if (type == "reply") output += "[Reply,Req=" + value("id") + "]";
            else if (type == "shake") output += "[Shake]";
            else if (type == "image" || type == "record") {
                if (fields.count("id")) output += std::string(type == "image" ? "[pic,hash=" : "[audio,hash=") + value("id") + "]";
                else if (fields.count("file")) output += std::string(type == "image" ? "[picFile,path=data\\image\\" : "[AudioFile,path=data\\record\\") + value("file") + "]";
            }
        }
    }
    output.append(input, cursor, std::string::npos);
    return output;
}

} // namespace amn::xlz
