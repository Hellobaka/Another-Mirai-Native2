"""Generate native CQP/Xlz exports and ABI definitions from the original bridge."""
from pathlib import Path
import re

root = Path(__file__).resolve().parent.parent
source = (root.parent / "CQP" / "DllEntry.cs").read_text(encoding="utf-8-sig")
pattern = re.compile(r'\[DllExport\(ExportName = "(CQ_[^"]+|cq_start)"[^\n]*\]\s*public static (int|long|IntPtr|bool) \w+\(([^)]*)\)')
out = []
names = []
x86_aliases = []
for name, result_type, signature in pattern.findall(source):
    names.append(name)
    parameters = []
    arguments = []
    stack_bytes = 0
    for param in signature.split(", ") if signature else []:
        kind, ident = param.split()
        stack_bytes += 8 if kind == "long" else 4
        cpp_param = {"int":"int", "long":"int64_t", "IntPtr":"const char*", "bool":"bool"}[kind]
        parameters.append(f"{cpp_param} {ident}")
        arguments.append(ident)
    cpp_type = {"int":"int", "long":"int64_t", "IntPtr":"const char*", "bool":"bool"}[result_type]
    if name == "cq_start":
        expression = "return true;"
    else:
        invocation = f'call("{name}"{", " if arguments else ""}{", ".join(arguments)})'
        expression = (f"return result_text({invocation});" if result_type == "IntPtr" else
                      f"return static_cast<{cpp_type}>(result_number({invocation}));")
    out.append(
        f'extern "C" {cpp_type} __stdcall {name}({", ".join(parameters)})\n'
        '{\n'
        f'    {expression}\n'
        '}'
    )
    x86_aliases.append(f"    {name}=_{name}@{stack_bytes}")
(root / "generated/cqp_exports.inc").write_text("// Generated from CQP/DllEntry.cs.\n" + "\n".join(out) + "\n", encoding="utf-8")
# Xlz exposes all signatures, including APIs which the original CQP project
# deliberately leaves as default-return stubs. Implemented APIs live in the
# native bridge; fail generation if the source adds an implementation we missed.
xlz_source = (root.parent / "CQP" / "XiaoLiZI_API.cs").read_text(encoding="utf-8-sig")
implemented = {5, 6, 7, 8, 14, 15, 24, 25, 26, 27, 31, 32, 33, 34, 35,
               36, 37, 41, 42, 43, 44, 46, 56, 57, 61, 62, 73, 74, 75,
               76, 77, 128, 136, 137, 223, 224}
xlz_out = []
xlz_pattern = re.compile(r'public static (\w+) (Function_(\d+))\(([^)]*)\)')
types = {"string": "const char*", "int": "int32_t", "long": "int64_t",
         "bool": "int32_t", "double": "double", "IntPtr": "void*",
         "object": "void*", "byte[]": "const void*", "void": "void"}
for match in xlz_pattern.finditer(xlz_source):
    result_type, name, number, signature = match.groups()
    number = int(number)
    end = xlz_source.find("public static", match.end())
    body = xlz_source[match.end():end if end >= 0 else len(xlz_source)]
    if "使用了未实现" not in body and number not in implemented:
        raise ValueError(f"Missing native implementation for {name}")
    params, arguments, writebacks = [], [], []
    stack_bytes = 0
    for index, param in enumerate(signature.split(", ")):
        words = param.split()
        byref = words[0] in ("ref", "out")
        kind, ident = words[-2:]
        cpp_type = types[kind]
        params.append(f'{cpp_type}{"*" if byref else ""} {ident}')
        stack_bytes += 8 if kind in ("long", "double") and not byref else 4
        if number in implemented:
            if byref:
                arguments.append(f'Json(static_cast<int64_t>({ident} ? reinterpret_cast<intptr_t>(*{ident}) : 0))' if kind == "IntPtr" else
                                 f'Argument({ident} ? *{ident} : 0)')
            else:
                arguments.append(f"Argument({ident})")
            if byref:
                expression = (f'reinterpret_cast<void*>(static_cast<intptr_t>(amn::Integer(args[{index}])))'
                              if kind == "IntPtr" else f'static_cast<{cpp_type}>(amn::Integer(args[{index}]))')
                writebacks.append(f'    if ({ident}) *{ident} = {expression};')
    default = {"string": 'return xlz::ReturnText("");', "void": "return;", "IntPtr": "return nullptr;"}.get(result_type, "return 0;")
    lines = [f'extern "C" {types[result_type]} __stdcall {name}({", ".join(params)})', "{"]
    if number in implemented:
        lines += [f'    JsonArray args{{{", ".join(arguments)}}};',
                  f'    const Json result = xlz::Invoke({number}, args);']
        lines += writebacks
        lines.append('    ' + ('return xlz::ReturnText(amn::Text(result));' if result_type == "string" else
                              'return;' if result_type == "void" else
                              f'return static_cast<{types[result_type]}>(amn::Integer(result));'))
    else:
        lines.append("    " + default)
    lines.append("}")
    xlz_out.append("\n".join(lines))
    names.append(name)
    x86_aliases.append(f"    {name}=_{name}@{stack_bytes}")
(root / "generated/xlz_exports.inc").write_text("// Generated from CQP/XiaoLiZI_API.cs.\n" + "\n".join(xlz_out) + "\n", encoding="utf-8")

api_source = (root.parent.parent / "Another-Mirai-Native" / "Native" / "Handler" / "XiaoLiZi" / "API.cs").read_text(encoding="utf-8-sig")
api_names = re.findall(r'\{ "([^"]*)", "(Function_\d+)" \}', api_source)
(root / "generated/XlzApiNames.h").write_text('#pragma once\n// Generated from XiaoLiZi/API.cs.\nnamespace amn {\ninline constexpr const char* XlzApiNames[][2] = {\n' +
    "\n".join(f'    {{"{name}", "{export}"}},' for name, export in api_names if name) + '\n};\n}\n', encoding="utf-8")

# Mirror the original packed event/output layouts, keeping pointer fields native.
struct_source = (root.parent.parent / "Another-Mirai-Native" / "Model" / "Other" / "XiaoLiZi" / "Structs.cs").read_text(encoding="utf-8-sig")
struct_source = re.sub(r'//[^\n]*', '', struct_source)
selected = ["ServiceInfo", "PrivateMessageEvent", "EventTypeBase", "GroupMessageEvent", "FriendInfo", "GroupInfo", "GroupMemberInfo"]
structs = ['#pragma once', '// Generated from XiaoLiZi/Structs.cs.', '#include <cstdint>', 'namespace amn::xlz {', '#pragma pack(push, 1)']
for name in selected:
    body = re.search(r'public struct ' + name + r'\s*\{([^}]+)\}', struct_source).group(1)
    structs.append(f'struct {name} {{')
    for kind, field in re.findall(r'public (\w+) (\w+)\s*;', body):
        # SessionToken is IntPtr in SDK 3.6.4. Native E plugins also copy
        # FileMD5 as a byte set (header + length + data), despite the SDK's
        # LPStr declaration. Neither field can contain an empty C string.
        if name == "PrivateMessageEvent" and field in ("SessionToken", "FileMD5"):
            kind = "nint"
        cpp_type = {"string": "const char*", "long": "int64_t", "uint": "uint32_t", "int": "int32_t", "nint": "void*", "ServiceInfo": "ServiceInfo"}.get(kind, "int32_t")
        initializer = ' = nullptr' if kind == "string" else '{}'
        structs.append(f'    {cpp_type} {field}{initializer};')
    structs.append('};')
structs += ['#pragma pack(pop)', '}']
(root / "generated/XlzStructs.h").write_text("\n".join(structs) + "\n", encoding="utf-8")
names += ["amn_xlz_cache_request"]
x86_aliases += ["    amn_xlz_cache_request=_amn_xlz_cache_request@4"]
(root / "generated/cqp.def").write_text("LIBRARY CQP\nEXPORTS\n" + "\n".join(names) + "\n", encoding="utf-8")
(root / "generated/cqp_x86.def").write_text("LIBRARY CQP\nEXPORTS\n" + "\n".join(x86_aliases) + "\n", encoding="utf-8")
print(f"Generated {len(names)} CQP/Xlz exports")
