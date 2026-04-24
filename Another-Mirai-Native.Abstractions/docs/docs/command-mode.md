# 指令模式

指令模式是框架在消息分发前提供的一层预处理机制，支持五种匹配模式：首匹配、尾匹配、全匹配、模糊匹配和正则匹配。其中，正则匹配支持具名分组，并可自动将匹配结果转换为方法参数。

> ⚠️ **注意**：指令模式自动实现了 `IGroupMessageHandler` 与 `IPrivateMessageHandler` 接口。在使用指令模式基类的情况下，插件不要再实现这两个接口。
>
> ⚠️ **注意**：如果重复实现了接口，事件分发行为会变得不可预测，可能导致其中一个处理路径收不到消息。
>
> 💡 **提示**：你可以通过重写基类方法添加自定义逻辑，但在自定义逻辑执行完毕后，仍需调用基类方法，才能继续完成指令解析和分发。

## 🚀 最小示例

1. 新建一个类，并继承 `CommandHandlerBase` 基类。
2. 创建一个方法，并在方法上添加 `[Command]` 特性，指定匹配模式和用于匹配消息内容的模板字符串。

```csharp
using Another_Mirai_Native.Abstractions;
using Another_Mirai_Native.Abstractions.Attributes;
using Another_Mirai_Native.Abstractions.Context;
using Another_Mirai_Native.Abstractions.Enums;

public class Command : CommandHandlerBase
{
    [Command(MatchMode.FullMatch, "/ping")]
    public void Ping(GroupMessageContext e)
    {
        e.SendMessage("pong");
    }
}
```

通过上述代码，当群组收到 `/ping` 消息时，插件会向该群发送 `pong`。

## 📌 特性参数

`Command` 特性支持三个参数：

- 匹配模式：上文提到的五种匹配模式之一。
- 匹配模板：用于参与匹配的模板字符串。不要填写空字符串，否则会匹配所有消息。
- 指令范围：指定该指令可在哪些来源中生效，例如群聊、私聊或二者都可。

```csharp
[Command(MatchMode.FullMatch, "/ping", MessageScope.All)]
```

## ↩️ 方法返回值

指令方法既支持同步写法，也支持异步写法。

| 返回值 | 含义 |
| -- | -- |
| `EventHandleResult` | 显式控制当前事件是 `Pass` 还是 `Block` |
| `void` | 同步方法；默认按阻塞事件处理 |
| `Task<EventHandleResult>` | 异步方法；可显式控制当前事件是 `Pass` 还是 `Block` |
| `Task` | 异步方法；默认按阻塞事件处理 |

> 💡 **提示**：如果你需要让消息继续传递给后续插件，优先使用 `EventHandleResult` 或 `Task<EventHandleResult>` 作为返回值。

## 🔍 匹配模式

| 模式 | 作用 |
| -- | -- |
| `MatchMode.StartWith` | 模板处于消息开头时会被匹配 |
| `MatchMode.EndWith` | 模板处于消息结尾时会被匹配 |
| `MatchMode.Contain` | 模板包含在消息中时会被匹配 |
| `MatchMode.FullMatch` | 模板与消息完全一致时会被匹配 |
| `MatchMode.Regex` | 模板作为正则表达式能够与消息匹配时会被匹配 |

### 🧩 正则模式说明

正则模式是指令模式中最灵活的匹配方式，适合将一条指令拆解为多个方法参数。通过具名分组，框架会尝试将匹配结果转换为方法中同名的参数。

例如，下面这个方法有三个参数：

```csharp
void Give(string userId, int itemId, int count)
```

那么可以编写如下正则表达式：

```regex
^\/give (?<userId>\S+) (?<itemId>\d+) (?<count>\d+)$
```

它可以匹配如下文本，并将结果映射到方法参数：

```plain
/give momo 114 514

userId = momo
itemId = 114
count = 514
```

对应的代码如下：

```csharp
[Command(MatchMode.Regex, "^\\/give (?<userId>\\S+) (?<itemId>\\d+) (?<count>\\d+)$")]
public void Give(GroupMessageContext e, string userId, int itemId, int count)
{
    e.SendMessage($"Give {itemId}[{count}] To {userId}");
}
```

## 🧱 方法参数

框架支持动态参数分发，共有四类参数，参数顺序不固定，也允许按需省略。

- GroupMessageContext/PrivateMessageContext：群聊或私聊的[消息上下文](/api/Another_Mirai_Native.Abstractions.Context.GroupMessageContext.html)。
- CancellationToken：取消令牌，详见[处理事件](/docs/event-handling#-取消令牌-cancellationtoken.html)。
- `raw`：原始消息文本。
- 正则具名分组对应的参数：参数名需要与分组名称一致，顺序不作要求。

> ⚠️ **注意**：如果方法参数中使用了 `GroupMessageContext`，但 Scope 设置为 `All`，那么当收到可匹配的私聊消息时，该方法不会被调用；`PrivateMessageContext` 同理。
>
> 💡 **提示**：`GroupMessageContext` 与 `PrivateMessageContext` 可以同时出现在同一个方法参数列表中。收到群组消息时，`PrivateMessageContext` 为 `null`；收到私聊消息时，`GroupMessageContext` 为 `null`。

## 🔄 重写基类方法

基类提供了以下可重写方法：

| 方法 | 作用 |
| -- | -- |
| `OnReceiveGroupMessageAsync(GroupMessageContext e, CancellationToken ct)` | 原始的群聊消息事件 |
| `OnReceivePrivateMessageAsync(PrivateMessageContext e, CancellationToken ct)` | 原始的私聊消息事件 |
| `OnNoMatchAsync(GroupMessageContext e, CancellationToken ct)` | 当没有任何指令匹配群聊消息时调用 |
| `OnNoMatchAsync(PrivateMessageContext e, CancellationToken ct)` | 当没有任何指令匹配私聊消息时调用 |

> ⚠️ **注意**：如果重写 `OnReceiveGroupMessageAsync` 或 `OnReceivePrivateMessageAsync`，在处理完自定义逻辑后，需要调用基类方法，以继续分发指令模式。
>
> ```csharp
> public override async Task<EventHandleResult> OnReceiveGroupMessageAsync(GroupMessageContext e, CancellationToken ct)
> {
>     if (e.Message.Text == "r")
>     {
>         await e.SendMessageAsync(Random.Shared.Next(1, 7).ToString());
>         return EventHandleResult.Block;
>     }
>     return await base.OnReceiveGroupMessageAsync(e, ct);
> }
> ```
