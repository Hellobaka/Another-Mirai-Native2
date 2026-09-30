# Another-Mirai-Native.Loader.Cpp

> Developed by GPT-6-Sol

## 源码目录

| 目录 | 用途 |
| --- | --- |
| `loader/` | 进程入口、插件加载与事件分发、命名管道、菜单 UI 线程 |
| `bridge/` | CQP API 桥接入口 |
| `xlz/` | 小栗子 API 实现与消息格式转换 |
| `common/` | 字符编码、JSON 编解码、运行日志 |
| `generated/` | 自动生成的 API 导出、名称表、结构体和 DLL 导出定义 |
| `tools/` | 导出代码生成器 |
| `tests/` | 原生及 C# SDK 测试插件、测试程序 |
| `third_party/` | 第三方头文件 |

解决方案和项目文件保留在根目录。运行输出目录及部署方式不变。

## 加载与兼容

用于酷 Q 和小栗子（Xlz）V3/V4 插件的加载器。插件进程通过命名管道连接主程序，配套的 `CQP.dll` 将 CQP API 转发为 RPC 请求。Xlz 支持以原 `Natives/CQP/XiaoLiZI_API.cs` 为基准：导出全部 400 个`Function_N` 签名，移植其中已实现的 36 个接口，其余保留原来的默认返回。

修改 `Natives/CQP/DllEntry.cs`、`Natives/CQP/XiaoLiZI_API.cs`、Xlz 的 API 名称表或结构体定义后，执行 `python Natives/Another-Mirai-Native.Loader.Cpp/tools/generate_exports.py`更新导出列表和 ABI 定义。生成器会在发现原项目新增已实现接口而未移植时失败。

`third_party/picojson.h` 的 BSD 2-Clause 许可声明保留在头文件中。
