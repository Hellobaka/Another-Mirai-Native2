"""Generate native CQP wrappers from the signatures in CQP/DllEntry.cs."""
from pathlib import Path
import re

root = Path(__file__).resolve().parent
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
(root / "cqp_exports.inc").write_text("// Generated from CQP/DllEntry.cs.\n" + "\n".join(out) + "\n", encoding="utf-8")
(root / "cqp.def").write_text("LIBRARY CQP\nEXPORTS\n" + "\n".join(names) + "\n", encoding="utf-8")
(root / "cqp_x86.def").write_text("LIBRARY CQP\nEXPORTS\n" + "\n".join(x86_aliases) + "\n", encoding="utf-8")
print(f"Generated {len(names)} exports")
