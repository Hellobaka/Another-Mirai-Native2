# Another-Mirai-Native.Loader.Cpp

> Developed by GPT-6-Sol

用于酷 Q 插件的加载器。插件进程通过命名管道连接主程序，配套的 `CQP.dll` 将 38 个 CQP API 转发为 RPC 请求。

插件元数据允许 JSON 注释。

修改 `Natives/CQP/DllEntry.cs` 的导出签名后，执行`python Natives/Another-Mirai-Native.Loader.Cpp/generate_exports.py` 更新导出列表。
`third_party/picojson.h` 的 BSD 2-Clause 许可声明保留在头文件中。
