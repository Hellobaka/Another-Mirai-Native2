# 发送消息

## 📄 消息格式

插件使用 `CQ码` 作为消息格式，同时提供了 `MessageBuilder` 构造器，避免手写 CQ码。

### CQ码简介

CQ码是指`酷Q`所定义的一种用纯文本表示非文本元素格式，以一对中括号包裹，使用逗号分隔各部分。例如 @某人：

```plain
[CQ:at,qq=114514]
```

其中 `CQ:at` 为消息类别，后面的部分为消息内容（键值对形式）。各元素的具体格式请参考 [OneBot11 文档](https://github.com/botuniverse/onebot-11/blob/master/message/segment.md)。

> 💡 **提示**：可以直接发送 CQ码字符串，框架会自动解析。在构造器中对应使用 `Text()` 方法：
>
> ```csharp
> await e.SendMessageAsync("");
> // 等价于
> await e.SendMessageAsync(new MessageBuilder().Text("[CQ:at,qq=114514] 你好").Build());
> ```

> ⚠️ **注意**：虽然 CQ码支持的元素类别很多，但本框架不保证所有格式都支持。

## 🔧 消息构造器

通过链式调用构造器方法拼接消息元素，构造器不会自动添加空格或制表符。

```csharp
public async Task<EventHandleResult> OnReceiveGroupMessageAsync(GroupMessageContext e, CancellationToken ct)
{
    if (e.Message.Text.Trim().ToLower() == "r")
    {
        var random = new Random();
        var dice = random.Next(1, 7);

        var message = new MessageBuilder()
            .At(e.FromQQ.Id)
            .Text($" 🎲 你掷出了 {dice} 点！")
            .Build();

        await e.SendMessageAsync(message);
        return EventHandleResult.Block;
    }

    return EventHandleResult.Pass;
}
```

上述代码会发送类似 `[CQ:at,qq=114514] 🎲 你掷出了 3 点！` 的消息。

### 📋 支持的元素类型

| 📌 元素 | 📝 含义 | 📎 备注 |
| -- | -- | -- |
| Text | 纯文本消息 | |
| Image | 本地图片 | 绝对路径或相对于 `data\image` 路径 |
| ImageHash | 网络图片 | 收到消息中的图片 `file` 参数，通常为 32 位哈希字符串 |
| Record | 本地音频 | 绝对路径或相对于 `data\record` 路径 |
| RecordHash | 网络音频 | 收到消息中的音频 `file` 参数，通常为 32 位哈希字符串 |
| At | @某人 | 参数为 QQ 号 |
| AtAll | @全体成员 | 仅限群聊 |
| Face | QQ 表情 | 参数为表情 ID |

### 消息链组合示例

多个元素可以链式拼接，构建富文本消息：

```csharp
var message = new MessageBuilder()
    .At(userId)                    // @用户
    .Text(" 恭喜你获得了奖励！")     // 文本
    .Face(CQFace.doge)             // 表情
    .Image("gift.png")             // 图片
    .Build();
```

## 📤 发送消息方式

### 通过上下文对象发送

上下文对象提供了快捷发送方法，直接向事件来源发送消息：

```csharp
public async Task<EventHandleResult> OnReceiveGroupMessageAsync(GroupMessageContext e, CancellationToken ct)
{
    if (e.Message.Text.Trim().ToLower() == "/ping")
    {
        await e.FromGroup.SendGroupMessageAsync("pong!");
        return EventHandleResult.Block;
    }

    return EventHandleResult.Pass;
}
```

### 通过 API 对象发送

适用于主动发送消息（无需收到事件触发）。API 对象有两种获取方式：

1. 通过 PluginBase 基类

```csharp
public class Entry : PluginBase
{
    public override async Task OnEnableAsync(CancellationToken ct)
    {
        // 插件启用时主动发送消息
        await API.MessageApi.SendGroupMessageAsync(114514, "骰子插件已启用！");
    }
}
```

2. 通过上下文对象

```csharp
public async Task<EventHandleResult> OnReceiveGroupMessageAsync(GroupMessageContext e, CancellationToken ct)
{
    if (e.Message.Text.Trim().ToLower() == "/broadcast")
    {
        // 向其他群发送消息
        await e.API.MessageApi.SendGroupMessageAsync(1919810, "广播消息");
        return EventHandleResult.Block;
    }

    return EventHandleResult.Pass;
}
```

## 💬 回复消息

上下文对象提供了 `Reply()` 方法，可以回复原消息（带消息引用）：

```csharp
public async Task<EventHandleResult> OnReceiveGroupMessageAsync(GroupMessageContext e, CancellationToken ct)
{
    if (e.Message.Text.Trim().ToLower() == "/help")
    {
        // 回复并引用原消息
        await e.Reply("这是帮助信息...");
        return EventHandleResult.Block;
    }

    return EventHandleResult.Pass;
}
```

## 🔄 撤回消息

通过 `IMessageApi` 可以撤回已发送的消息，需要传入消息 ID：

```csharp
// 发送消息并获取消息 ID
var message = await API.MessageApi.SendGroupMessageAsync(groupId, "这条消息将被撤回");

// 延时 3 秒
await Task.Delay(3000);

// 撤回消息 (对象内封装)
await message.DeleteMessageAsync();

// 撤回消息 (使用 API 对象)
await e.API.MessageApi.DeleteMessageAsync(message.Id);
```

## ⚠️ 发送结果判断

消息发送不会抛出异常，而是通过返回的 `Message` 对象的 `IsSuccess` 属性判断是否成功：

```csharp
var message = await e.SendMessageAsync("测试消息");

if (message.IsSuccess)
{
    API.Logger.Info($"消息发送成功，ID: {message.Id}");
}
else
{
    API.Logger.Warn("消息发送失败");
}
```

## 📦 Message 对象

`Message` 对象不仅用于发送结果判断，还提供了消息解析功能。创建时会自动将原始消息拆解为消息元素链，可遍历以获取结构化的消息元素列表。

> 💡 **提示**：消息元素链在处理复杂消息（如提取图片、解析 @对象）时非常实用，无需手动解析 CQ码。
