// Native equivalents of the implemented methods in CQP/XiaoLiZI_API.cs.
// All pointers are created in this process; only CQP values cross the pipe.
namespace amn::xlz {
namespace {

std::mutex cache_mutex;
std::map<std::pair<int64_t, int32_t>, int64_t> message_cache;
std::map<int64_t, std::string> request_cache;
std::atomic<int64_t> next_key{1};

int64_t Timestamp() {
    return std::chrono::duration_cast<std::chrono::seconds>(
        std::chrono::system_clock::now().time_since_epoch()).count();
}

void* Allocate(size_t size) {
    void* result = CoTaskMemAlloc(size);
    if (!result) throw std::bad_alloc();
    return result;
}

const char* NativeText(const std::string& value) {
    auto* buffer = static_cast<char*>(Allocate(value.size() + 1));
    memcpy(buffer, value.c_str(), value.size() + 1);
    return buffer;
}

template <typename T> void* NativeStruct(const T& value) {
    void* buffer = Allocate(sizeof(T));
    memcpy(buffer, &value, sizeof(T));
    return buffer;
}

// CQP collections contain a big-endian count followed by length-prefixed
// records, whose strings are GB18030. Bound every read before allocating.
class Reader {
public:
    explicit Reader(std::string bytes) : bytes_(std::move(bytes)) {}
    uint64_t Number(size_t size) {
        if (size > Remaining()) throw std::runtime_error("truncated CQP record");
        uint64_t value = 0;
        while (size--) value = (value << 8) | static_cast<unsigned char>(bytes_[offset_++]);
        return value;
    }
    std::string Token() {
        const size_t length = static_cast<size_t>(Number(2));
        if (length > Remaining()) throw std::runtime_error("truncated CQP string");
        auto result = bytes_.substr(offset_, length);
        offset_ += length;
        return result;
    }
    size_t Remaining() const { return bytes_.size() - offset_; }
private:
    std::string bytes_;
    size_t offset_ = 0;
};

std::string DecodeBase64(const std::string& text) {
    static const std::string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
    std::string result;
    uint32_t bits = 0;
    int available = 0;
    for (char c : text) {
        if (c == '=') break;
        if (c == '\r' || c == '\n' || c == ' ') continue;
        const auto value = alphabet.find(c);
        if (value == std::string::npos) throw std::runtime_error("invalid CQP base64");
        bits = (bits << 6) | static_cast<uint32_t>(value);
        available += 6;
        if (available >= 8) {
            available -= 8;
            result.push_back(static_cast<char>((bits >> available) & 255));
        }
    }
    return result;
}

struct MemberRecord {
    int64_t group{}, qq{};
    std::string nick, card, title;
    int32_t sex{}, age{}, joined{}, spoke{}, role{}, title_expires{};
};
struct FriendRecord { int64_t qq{}; std::string name, note; };
struct GroupRecord { int64_t qq{}, count{}; std::string name; };

MemberRecord ReadMember(Reader reader) {
    MemberRecord record;
    record.group = static_cast<int64_t>(reader.Number(8));
    record.qq = static_cast<int64_t>(reader.Number(8));
    record.nick = reader.Token(); record.card = reader.Token();
    record.sex = static_cast<int32_t>(reader.Number(4));
    record.age = static_cast<int32_t>(reader.Number(4));
    reader.Token(); // area
    record.joined = static_cast<int32_t>(reader.Number(4));
    record.spoke = static_cast<int32_t>(reader.Number(4));
    reader.Token(); // level
    record.role = static_cast<int32_t>(reader.Number(4));
    reader.Number(4); // bad record
    record.title = reader.Token();
    record.title_expires = static_cast<int32_t>(reader.Number(4));
    reader.Number(4); // editable card
    return record;
}

GroupMemberInfo ToNative(const MemberRecord& record) {
    GroupMemberInfo info;
    info.QQNumber = NativeText(std::to_string(record.qq));
    info.Name = NativeText(record.nick); info.Nickname = NativeText(record.card);
    info.Age = record.age; info.Gender = record.sex;
    info.JoinTime = record.joined; info.ChatTime = record.spoke;
    info.Title = NativeText(record.title); info.TitleTimeout = record.title_expires;
    return info;
}

template <typename T, typename Convert>
std::vector<T> ReadList(const Json& response, Convert convert) {
    const std::string text = amn::Text(response);
    if (text.empty()) return {};
    Reader reader(DecodeBase64(text));
    const auto count = reader.Number(4);
    if (count > reader.Remaining() / 2) throw std::runtime_error("invalid CQP list count");
    std::vector<T> result;
    for (uint64_t i = 0; i < count; ++i) result.push_back(convert(Reader(reader.Token())));
    return result;
}

std::vector<FriendRecord> Friends(int auth) {
    return ReadList<FriendRecord>(call("CQ_getFriendList", auth, false), [](Reader reader) {
        FriendRecord info;
        info.qq = static_cast<int64_t>(reader.Number(8));
        info.name = reader.Token(); info.note = reader.Token();
        return info;
    });
}

std::vector<GroupRecord> Groups(int auth) {
    return ReadList<GroupRecord>(call("CQ_getGroupList", auth), [](Reader reader) {
        GroupRecord info;
        info.qq = static_cast<int64_t>(reader.Number(8));
        info.name = reader.Token();
        // Group-list records omit counts; standalone group-info records include them.
        if (reader.Remaining() >= 8) {
            info.count = reader.Number(4); reader.Number(4);
        }
        return info;
    });
}

FriendInfo ToNative(const FriendRecord& record) {
    FriendInfo info;
    info.QQNumber = record.qq;
    info.Name = NativeText(record.name); info.Note = NativeText(record.note);
    return info;
}

GroupInfo ToNative(const GroupRecord& record) {
    GroupInfo info;
    info.GroupQQ = record.qq; info.GroupMemberCount = record.count;
    info.GroupName = NativeText(record.name);
    return info;
}

std::vector<MemberRecord> Members(int auth, int64_t group) {
    return ReadList<MemberRecord>(call("CQ_getGroupMemberList", auth, group), ReadMember);
}

template <typename T> Json SetList(Json& pointer, const std::vector<T>& values) {
    // E-language arrays inline the pointer slots after the two 32-bit headers.
    // SDK 3.6.4 marshals a two-element array of structs with fixed byte arrays:
    // 10 KiB for friends/groups and 100 KiB for members. Pad the allocation
    // so that marshaling those declarations cannot read beyond this buffer.
    constexpr size_t sdk_bytes = 2 * (8 + (std::is_same_v<T, GroupMemberInfo> ? 102400 : 10240));
    const size_t bytes = (std::max)(8 + sizeof(uintptr_t) * values.size(), sdk_bytes);
    auto* list = static_cast<unsigned char*>(Allocate(bytes));
    memset(list, 0, bytes);
    const int32_t headers[] = {1, static_cast<int32_t>(values.size())};
    memcpy(list, headers, 8);
    for (size_t i = 0; i < values.size(); ++i) {
        void* record = NativeStruct(values[i]);
        memcpy(list + 8 + sizeof(uintptr_t) * i, &record, sizeof(record));
    }
    pointer = Json(static_cast<int64_t>(reinterpret_cast<intptr_t>(list)));
    return Json(static_cast<int64_t>(values.size()));
}

template <typename T> void SetStruct(Json& pointer, const T& value) {
    pointer = Json(static_cast<int64_t>(reinterpret_cast<intptr_t>(NativeStruct(value))));
}

std::string RequestFlag(int64_t token) {
    std::lock_guard<std::mutex> lock(cache_mutex);
    const auto found = request_cache.find(token);
    return found == request_cache.end() ? std::to_string(token) : found->second;
}

} // namespace

const char* ReturnText(const std::string& utf8) {
    return NativeText(Utf8ToGb18030(utf8));
}

Json Invoke(int api, JsonArray& args) {
    try {
        const int auth = static_cast<int>(Integer(args[0]));
        auto n = [&](size_t i) { return Integer(args[i]); };
        auto s = [&](size_t i) { return Text(args[i]); };
        auto rpc = [&](const char* name, JsonArray values) {
            values.insert(values.begin(), Json(static_cast<int64_t>(auth)));
            return call_json(name, values);
        };
        if (api == 5) { rpc("CQ_addLog", {Json(int64_t(10)), Json("输出日志"), args[1]}); return Json(""); }
        if (api == 31) return Json(Text(call("CQ_getLoginQQ", auth)));
        if (api == 73) return call("CQ_getAppDirectory", auth);
        if (api == 223) return Json("https://q.qlogo.cn/g?b=qq&nk=" + std::to_string(n(1)) + "&s=160");
        if (api == 224) return Json("http://p.qlogo.cn/gh/" + std::to_string(n(1)) + "/" + std::to_string(n(1)) + "/0");

        const bool unchecked = api == 25 || api == 26 || api == 27 || api == 76 || api == 77 || api == 128;
        if (!unchecked && n(api == 75 ? 2 : 1) != Integer(call("CQ_getLoginQQ", auth))) return {};

        if (api == 6 || api == 7 || api == 8 || api == 14 || api == 15 || api == 136 || api == 137) {
            const bool group = api == 7 || api == 15 || api == 137;
            const bool temporary = api == 8;
            std::string message = s(temporary ? 4 : 3);
            if (api == 14 || api == 15) message = "[CQ:json,content=" + message + "]";
            else if (api == 136 || api == 137) message = "[CQ:xml,content=" + message + "]";
            else message = ConvertMessage(message, true);
            const int64_t id = Integer(rpc(group ? "CQ_sendGroupMsg" : "CQ_sendPrivateMsg",
                {args[temporary ? 3 : 2], Json(message)}));
            if (id > 0 && !group) {
                int64_t random = temporary ? n(5) : next_key++;
                int32_t req = temporary ? static_cast<int32_t>(n(6)) : static_cast<int32_t>(next_key++);
                if (api == 6 || api == 14) { args[4] = Json(random); args[5] = Json(int64_t(req)); }
                std::lock_guard<std::mutex> lock(cache_mutex);
                message_cache[{random, req}] = id;
            }
            return Json(std::to_string(Timestamp()));
        }
        if (api == 24) { rpc("CQ_setGroupCard", {args[2], args[3], args[4]}); return Json(""); }
        if (api == 25 || api == 26 || api == 32 || api == 76) {
            const auto list = Friends(auth);
            if (api == 32) {
                std::vector<FriendInfo> native;
                for (const auto& info : list) native.push_back(ToNative(info));
                return SetList(args[2], native);
            }
            for (const auto& info : list) if (std::to_string(info.qq) == s(api == 25 ? 1 : 2)) {
                if (api == 76) { SetStruct(args[3], ToNative(info)); return Json(int64_t(1)); }
                return Json(Gb18030ToUtf8(info.name.c_str()));
            }
            return api == 26 ? args[2] : Json{};
        }
        if (api == 27 || api == 33 || api == 77) {
            const auto list = Groups(auth);
            if (api == 33) {
                std::vector<GroupInfo> native;
                for (const auto& info : list) native.push_back(ToNative(info));
                return SetList(args[2], native);
            }
            for (const auto& info : list) if (std::to_string(info.qq) == s(api == 27 ? 1 : 2)) {
                if (api == 77) { SetStruct(args[3], ToNative(info)); return Json(int64_t(1)); }
                return Json(Gb18030ToUtf8(info.name.c_str()));
            }
            return {};
        }
        if (api == 34 || api == 36 || api == 128) {
            const auto list = Members(auth, n(2));
            std::vector<GroupMemberInfo> native;
            std::string administrators;
            for (const auto& member : list) {
                if (api == 128 && member.group == n(2) && member.qq == n(3)) {
                    SetStruct(args[4], ToNative(member)); return Json("");
                }
                if (api == 34) native.push_back(ToNative(member));
                if (api == 36 && (member.role == 2 || member.role == 3)) administrators += Gb18030ToUtf8(member.card.c_str()) + "\r\n";
            }
            if (api == 34) return SetList(args[3], native);
            return Json(administrators);
        }
        if (api == 37) {
            const auto response = call("CQ_getGroupMemberInfoV2", auth, n(2), n(3), false);
            if (Text(response).empty()) return {};
            return Json(Gb18030ToUtf8(ReadMember(Reader(DecodeBase64(Text(response)))).card.c_str()));
        }
        if (api == 35 || api == 41 || api == 42 || api == 43 || api == 44 || api == 46) {
            const char* name = api == 35 ? "CQ_setGroupAdmin" : api == 41 ? "CQ_setGroupKick" :
                api == 42 ? "CQ_setGroupBan" : api == 46 ? "CQ_setGroupWholeBan" : "CQ_setGroupLeave";
            JsonArray values{args[2]};
            if (api == 43 || api == 44) values.push_back(Json(api == 44));
            else if (api == 46) values.push_back(Json(n(3) != 0));
            else {
                values.push_back(args[3]);
                values.push_back(api == 42 ? args[4] : Json(n(4) != 0));
            }
            return Json(int64_t(Integer(rpc(name, values)) == 1));
        }
        if (api == 56 || api == 57) {
            int64_t id = -1;
            {
                std::lock_guard<std::mutex> lock(cache_mutex);
                const auto found = message_cache.find({n(3), static_cast<int32_t>(n(4))});
                if (found != message_cache.end()) id = found->second;
            }
            return Json(int64_t(id > 0 && Integer(call("CQ_deleteMsg", auth, id)) == 1));
        }
        if (api == 61) {
            rpc("CQ_setGroupAddRequestV2", {Json(RequestFlag(n(4))), Json(int64_t(n(6) == 3 ? 1 : 2)),
                Json(int64_t(n(5) == 11 ? 1 : 2)), args[7]}); return {};
        }
        if (api == 62) { rpc("CQ_setFriendAddRequest", {Json(RequestFlag(n(3))), args[4], Json("")}); return {}; }
        if (api == 74) return Json(Text(call("CQ_sendLikeV2", auth, n(2), 1)));
        if (api == 75) {
            const std::string message = s(1);
            const auto start = message.find("[pic,hash=");
            if (start == std::string::npos) return {};
            const auto end = message.find_first_of(",]", start + 10);
            // '[pic,hash=' contains ten bytes.
            return rpc("CQ_getImage", {Json(message.substr(start + 10, end - start - 10))});
        }
    } catch (...) {
        // Keep malformed protocol data from unwinding across a plugin ABI.
        return {};
    }
    return {};
}

} // namespace amn::xlz

namespace xlz = amn::xlz;

extern "C" int64_t __stdcall amn_xlz_cache_request(const char* flag) {
    const int64_t token = amn::xlz::next_key++;
    std::lock_guard<std::mutex> lock(amn::xlz::cache_mutex);
    amn::xlz::request_cache[token] = amn::Gb18030ToUtf8(flag);
    return token;
}
