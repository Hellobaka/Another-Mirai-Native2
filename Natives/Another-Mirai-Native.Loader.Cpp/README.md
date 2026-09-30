# Another-Mirai-Native.Loader.Cpp

> Developed by GPT-6-Sol

用于酷 Q 插件的加载器。插件进程通过命名管道连接主程序，配套的 `CQP.dll` 将 38 个 CQP API 转发为 RPC 请求。

插件元数据允许 JSON 注释。

旧版 .NET Framework 插件的 SQLite 兼容依赖在构建时通过
`tools/Prepare-CppLoaderSqlite.ps1` 从核心项目的 net48 NuGet 依赖复制，
包括加载器目录下的 `System.Data.SQLite.dll`、对应位数目录下的
`SQLite.Interop.dll` 和 `Another-Mirai-Native.Loader.Cpp.exe.config`。
配置按实际程序集版本生成绑定重定向，避免旧插件内嵌的 SQLite 程序集
与新版原生 DLL 混用。首次构建缺少 net48 还原结果时会自动执行还原。
部署时需完整复制加载器目录，不能仅复制 EXE 和 CQP.dll。

修改 `Natives/CQP/DllEntry.cs` 的导出签名后，执行`python Natives/Another-Mirai-Native.Loader.Cpp/generate_exports.py` 更新导出列表。
`third_party/picojson.h` 的 BSD 2-Clause 许可声明保留在头文件中。
