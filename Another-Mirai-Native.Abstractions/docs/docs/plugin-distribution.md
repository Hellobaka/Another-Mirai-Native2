# 插件构建

插件在构建时支持自动将依赖合并为单一文件，并自动生成插件元数据。

## ⚙️ 构建选项

可以在项目文件的 `PropertyGroup` 中添加以下选项，控制插件构建行为：

| 选项 | 作用 |
| -- | -- |
| `GenerateAmnManifest` | 是否执行依赖合并和自动元数据生成，默认启用 |
| `CleanAmnOutputDir` | 是否清理输出目录，并在合并依赖后删除多余文件，默认启用 |
| `IgnoreDependencyVersion` | 是否忽略依赖版本匹配，仅按程序集名称匹配，默认禁用 |

> 💡 **提示**：这些选项都可以直接写在项目文件的 `PropertyGroup` 中。

## 📦 依赖合并

构建完成后，工具会扫描插件输出目录中的所有托管 DLL，并与框架加载器自带的依赖进行去重。是否要求同名依赖的版本号和 PublicKeyToken 同时一致，取决于 `IgnoreDependencyVersion` 的配置。

完成去重后，工具会使用 ILRepack 将需要保留的托管依赖合并到插件主 DLL 中。随后，输出目录中多余的托管 DLL 会被删除，最终通常只保留插件本体。

### 📚 框架已内置的依赖

可以参考 [DependencyManifest-dotnet9.json](https://github.com/Hellobaka/Another-Mirai-Native2/blob/csharp-plugin/Another-Mirai-Native.Abstractions/tools/DependencyManifest-dotnet9.json)。

> 💡 **提示**：选择依赖时，优先使用框架已经提供的同版本依赖，这样可以减少最终插件包的体积。

### ⚠️ 非托管依赖

某些库在运行时还依赖非托管 DLL，例如 `e_sqlite3.dll`、`libSkiaSharp.dll`。这些文件不会被自动打包进插件，因此需要手动放置到以下位置之一：

- 框架根目录
- `libraries` 文件夹

## 🧾 插件元数据

插件元数据指的是 `Native_*.json` 文件，用于帮助框架识别插件名称、描述、版本、使用的事件以及声明的窗口。

构建完成后，工具会通过 .NET 元数据扫描插件中声明的处理器、插件描述信息以及窗口处理器名称，并自动生成对应的元数据文件。

## 插件分发

构建结果中的`Native_*.dll`和`Native_*.json`即是最终打包结果，分发时选择这两个文件。
