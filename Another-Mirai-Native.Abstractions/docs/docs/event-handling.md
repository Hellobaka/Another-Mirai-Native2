# 处理事件

插件是**事件驱动**的，表现在开发中则是实现了各种处理器接口（`Handler`），通过这些接口框架也可以了解插件期望监听哪些事件，并实现对应的分发。

## 🔌 实现处理器接口

处理器是框架定义的接口，用于接收并处理各类事件。框架会将上游协议（如 OneBot、MiraiAPIHttp）的消息转换为统一格式，再分发给插件处理。

> 💡 **提示**：
>
> - 当不实现处理器时，框架将会使用默认行为，关于行为的具体动作可查看处理器的文档
> - 处理器可以不在继承 `PluginBase` 的类上实现，可以在其他类中实现，但必须使用 `public` 可访问性修饰
> - 处理器可以在一个类上实现多个
> - 处理器的默认实现返回的均为 `Task<>`，为方便开发，建议使用 `async/await`

### 📋 可用处理器接口

框架提供了如下的处理器接口：

| 📌 名称 | ⏰ 触发时机 |
| :--- | :--- |
| IAdminChangeHandler | 当群聊管理员成员变更时 |
| IFriendAddRequestHandler | 收到好友添加请求时 |
| IFriendAddedHandler | 当好友添加完成时 |
| IGroupAddRequestHandler | 当收到群添加请求时 |
| IGroupFileUploadHandler | 当收到群文件上传时 |
| IGroupInviteRequestHandler | 当收到 Bot 被邀请加入群请求时 |
| IGroupMemberBannedHandler | 当群成员被禁言时 |
| IGroupMemberDecreaseHandler | 当群成员退出群聊时 |
| IGroupMemberIncreaseHandler | 当群成员加入群聊时 |
| IGroupMemberUnbannedHandler | 当群成员被解除禁言时 |
| IGroupMessageHandler | 当收到群消息时 |
| IGroupWholeBannedHandler | 当群全员禁言时 |
| IGroupWholeUnbannedHandler | 当群解除全员禁言时 |
| IMenuHandler | 当点击插件菜单时 |
| IPrivateMessageHandler | 当收到私聊消息时 |

### 🚧 阻塞事件传递

所有处理器的返回值 `EventHandleResult` 表示本插件对此事件的处理状态，有两个枚举值：

| 值 | 含义 | 使用场景 |
| :--- | :--- | :--- |
| `Pass` | 允许事件继续传递给后续插件 | 不满足处理条件，或只是"旁观"事件 |
| `Block` | 阻止事件传递，后续插件不再收到此事件 | 已处理此事件并执行了相关动作 |

### ⚡ 事件优先级

使用 `[EventPriority]` 特性可以修改事件优先级，**数字越大优先级越高**，会被优先处理。

```csharp
[EventPriority(PluginEventType.GroupMsg, 100)]  // 高优先级，先处理
public class MyPlugin : PluginBase, IGroupMessageHandler { ... }
```

> ⚠️ **注意**：此特性可以写在 `PluginBase` 基类或事件处理器上。不建议对同一事件在多个位置声明多次，覆盖顺序可能不可预料。

### 📝 示例代码

以下示例实现了 `IGroupAddRequestHandler`，判断入群答案并处理请求：

```csharp
using Another_Mirai_Native.Abstractions.Context;
using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Abstractions.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPlugin
{
    [EventPriority(PluginEventType.GroupAddRequest, 10)]
    public class HandleGroupRequest : IGroupAddRequestHandler
    {
        public async Task<EventHandleResult> OnGroupAddRequestAsync(GroupAddRequestContext e, CancellationToken ct)
        {
            if (e.AppendMessage.Trim() == "2")
            {
                await e.SetRequestResultAsync(RequestHandleResult.Accept);
            }
            else
            {
                await e.SetRequestResultAsync(RequestHandleResult.Deny, "1+1=?");
            }
            return EventHandleResult.Block;
        }
    }
}
```

## 🔄 生命周期

### 📥 加载插件

1. 寻找继承了 `PluginBase` 的类，并实例化
2. 加载插件元数据：优先读取 `[PluginInfo]` 特性，若未定义则读取重写的 `PluginInfo` 属性
3. 实例化 API 对象，传递插件唯一 ID
4. 框架调用 `OnEnableAsync` 事件，此时插件可以进行配置加载、数据库初始化等逻辑
5. 若事件无异常结束，插件加载完成

### 📤 卸载插件

1. 框架调用 `OnDisableAsync` 事件，此时插件可以进行资源清理等逻辑
2. 若事件无异常结束，所有事件的 `CancellationTokenSource` 将被标记为取消
3. 框架关闭加载器进程，插件卸载完成

## 📦 上下文对象

每个处理器都会收到一个上下文对象 `e`，包含事件相关的所有数据和快捷操作方法。

以群消息事件为例：

```csharp
public async Task<EventHandleResult> OnReceiveGroupMessageAsync(GroupMessageContext e, CancellationToken ct)
{
    // 检查消息内容是否为 'r'（不区分大小写，忽略前后空格）
    if (e.Message.Text.Trim().ToLower() == "r")
    {
        // 生成 1-6 的随机数
        var random = new Random();
        var dice = random.Next(1, 7);

        // 异步回复骰子结果
        await e.SendMessageAsync($"🎲 你掷出了 {dice} 点！");

        // 返回 Block 阻止其他插件处理此消息
        return EventHandleResult.Block;
    }

    return EventHandleResult.Pass;
}
```

### 常用成员

| 成员 | 说明 |
| :--- | :--- |
| `e.Message` | 本次消息的内容对象 |
| `e.Message.Text` | 消息文本内容 |
| `e.SendMessageAsync()` | 向来源群发送消息（快捷方法） |
| `e.Reply()` | 回复本条消息（快捷方法） |
| `e.API` | 框架 API 对象 |

> 💡 **提示**：上下文对象提供了事件所有可参考的参数以及快捷动作，想对事件做点什么时不妨展开上下文对象的成员列表看看！

关于`GroupMessageContext`的详细文档请查看 [API 文档](/api/Another_Mirai_Native.Abstractions.Context.GroupMessageContext.html)

## ⏹️ 取消令牌 (CancellationToken)

每个处理器都会收到一个 `CancellationToken` 参数，用于响应插件的卸载请求。

### 用途

当插件被卸载时，框架会将所有正在执行的事件的取消令牌标记为已取消。你的代码应该检查这个令牌，以便：

- 及时终止长时间运行的操作，实现优雅退出
- 避免在卸载过程中执行不必要的计算或 I/O 操作

### 使用示例

```csharp
public async Task<EventHandleResult> OnReceiveGroupMessageAsync(GroupMessageContext e, CancellationToken ct)
{
    // 检查是否已被取消
    if (ct.IsCancellationRequested)
    {
        return EventHandleResult.Pass;
    }

    // 执行耗时操作时传入令牌
    var result = await SomeLongRunningOperationAsync(ct);

    // 或者在等待时检查取消状态
    await Task.Delay(5000, ct);  // 如果取消，会抛出 OperationCanceledException

    return EventHandleResult.Block;
}
```

> 💡 **提示**：许多异步 API（如 `Task.Delay`、`HttpClient` 等）都支持 `CancellationToken`，建议在耗时操作中始终传入。
