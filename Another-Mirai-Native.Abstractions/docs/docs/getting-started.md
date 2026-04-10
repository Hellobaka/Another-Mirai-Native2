## CSharp Start!
通过本文，你将在 15 分钟内创建一个能响应群消息的骰子插件，体验 AMN2 插件开发的完整流程。

## 先决条件
- **Windows** 操作系统
- **.NET Framework 4.8** 或 **.NET 9.0**（任选其一）
- **Visual Studio 2022 或更新版本**或其他 C# 开发环境

## 创建项目
示例均以 Visual Studio 2026 做示例。
1. 打开 Visual Studio，并选择`创建新项目`
![创建项目](/images/CreateProject.png)
2. 根据要使用的目标框架，选择对应的 `类库`模板
    1. 假如选择`.net framework4.8`，则搜索`类库(.Net Framework)`
    2. 假如选择`.net9.0`，则搜索`类库`
    ![选择模板](/images/CreateClassLibrary.png)
3. 下一步选择项目位置，随后选择模板框架。
4. 看见代码页面后即为创建完成。
![创建完成](/images/CreateFinish.png)

5. **重要**：启用本地依赖输出，以便生成完整的插件文件。
   - 在解决方案资源管理器中双击项目文件
   - 在 `.csproj` 文件的 `<PropertyGroup>` 部分添加：
     ```xml
     <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
     ```
![启用本地依赖输出](/images/CopyLocalAssemblies.png)


## Nuget 安装
1. 打开`工具 - Nuget 包管理器 - 管理解决方案的 Nuget 程序包`
![打开Nuget管理器](/images/OpenNugetManager.png)
2. 搜索`Another-Mirai-Native.Abstractions`，并安装到项目
![安装Nuget](/images/InstallNuget.png)

## 编写插件
用以下完整代码替换 `Class1.cs` 文件的内容：

```csharp
using Another_Mirai_Native.Abstractions;
using Another_Mirai_Native.Abstractions.Models;
using Another_Mirai_Native.Abstractions.Context;
using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Abstractions.Handlers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DemoPlugin
{
    [PluginInfo(
        appId: "com.demo.dice",           // 插件唯一标识，建议使用反向域名格式
        name: "骰子插件",                 // 在插件管理器中显示的名称
        version: "1.0.0",                 // 版本号，建议遵循语义化版本
        description: "一个简单的骰子插件，输入 'r' 掷骰子",  // 可选：插件描述
        author: "示例作者"                 // 可选：作者信息
    )]
    public class Entry : PluginBase, IGroupMessageHandler
    {
        /// <summary>
        /// 当收到群消息时触发
        /// </summary>
        public async Task<EventHandleResult> OnReceiveGroupMessageAsync(
            GroupMessageContext context, 
            CancellationToken cancellationToken)
        {
            // 检查消息内容是否为 'r'（不区分大小写，忽略前后空格）
            if (context.Message.Text.Trim().ToLower() == "r")
            {
                // 生成 1-6 的随机数
                var random = new Random();
                var dice = random.Next(1, 7);
                
                // 异步回复骰子结果
                await context.FromGroup.SendGroupMessageAsync($"🎲 你掷出了 {dice} 点！");
                
                // 返回 Block 阻止其他插件处理此消息
                return EventHandleResult.Block;
            }
            
            // 返回 Pass 让其他插件可以继续处理此消息
            return EventHandleResult.Pass;
        }
    }
}
```

### 加载插件并测试功能
1. 按`Ctrl + B`生成项目，或者手动在解决方案的项目处，点击右键，随后点击`生成`。右键项目，选择`在文件资源管理器中打开`
![打开生成目录](/images/OpenOutputDir.png)
2. 复制其中`Native_`开头的 dll 与 json 文件，至框架的`data\plugins`文件夹
![复制dll](/images/CopyPlugin.png)
3. 重启框架或者点击框架的重载插件 <br>
![重载插件](/images/ReloadPlugins.png)

4. 查看框架日志是否显示了我们的插件
![检查插件加载](/images/CheckLoadOK.png)
5. 在协议测试框或 Bot 所在群内发送消息`r`
![发送r](/images/SendR.png)
6. 查看是否插件发送了骰子的结果
![检查插件结果](/images/CheckDice.png)

这样我们就实现了一个骰子功能！

### 代码做了什么

#### 1. 插件元数据 (`[PluginInfo]`)
- `appId`: 插件唯一标识，**必须使用反向域名格式**确保全局唯一性
- `name`: 在 AMN2 插件管理器中显示的友好名称
- `version`: 版本号，建议使用语义化版本（如 `1.0.0`、`2.1.0-beta.1`）
- `description`（可选）: 插件功能描述，显示在 UI 中
- `author`（可选）: 作者信息

#### 2. 基类和接口
- 继承 `PluginBase`: 获取框架提供的 `API` 属性，可访问日志、消息、群组等 API
- 实现 `IGroupMessageHandler`: 告诉框架"我要处理群消息"

#### 3. 消息处理逻辑
- `context.Message.Text`: 获取消息文本内容
- `Trim().ToLower()`: 规范化输入（忽略空格和大小写）
- `context.FromGroup`: 获取消息来源的群组对象，已绑定群号
- `SendGroupMessageAsync()`: **异步发送群消息**，避免阻塞主线程

#### 4. 返回值含义
- `EventHandleResult.Block`: 阻止其他插件处理此消息
- `EventHandleResult.Pass`: 允许其他插件继续处理此消息

## 🎉 恭喜！你的第一个插件已完成！

你已经成功创建了一个功能完整的 AMN2 插件。这个骰子插件展示了：
- ✅ 插件创建和元数据定义
- ✅ 群消息事件处理
- ✅ 异步消息发送
- ✅ 插件部署和测试流程

## 遇到问题？

### ❌ 插件没有加载？
- 检查 `data\plugins` 目录是否正确
- 确认文件名为 `Native_` 开头
- 查看框架日志中的错误信息

### ❌ 消息没有回复？
- 确认机器人已登录且在线
- 检查插件是否成功加载（查看日志）
- 确认消息内容匹配（'r' 前后不要有空格）

### ❌ 构建失败？
- 确保已安装 `Another-Mirai-Native.Abstractions` NuGet 包
- 检查 `.csproj` 中是否有 `<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>`

---

现在你已经掌握了 AMN2 插件开发的基础！探索 `Another_Mirai_Native.Abstractions` 命名空间中的其他接口，创建更强大的插件吧！

> **提示**：完整 API 文档可在 [API 文档](/api/Another_Mirai_Native.Abstractions.html) 页面查看，更多示例请参考 [GitHub 仓库](https://github.com/Hellobaka/Another-Mirai-Native2)。

