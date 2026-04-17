# API 参考

API 对象是框架提供给插件的核心接口，用于实现消息发送、群组管理、好友管理等功能。

## 🔌 获取 API 对象

API 对象有两种获取方式：

### 1. 通过 PluginBase 基类

继承 `PluginBase` 的插件可以直接使用 `API` 属性：

```csharp
public class MyPlugin : PluginBase
{
    public override async Task OnEnableAsync(CancellationToken ct)
    {
        // 使用 API 对象
        var groups = await API.GroupApi.GetGroupListAsync();
        API.Logger.Info("MyPlugin", $"已加入 {groups.Count} 个群");
    }
}
```

### 2. 通过上下文对象

在事件处理器中，上下文对象也提供了 `API` 属性：

```csharp
public async Task<EventHandleResult> OnReceiveGroupMessageAsync(GroupMessageContext e, CancellationToken ct)
{
    // 通过上下文获取 API
    var info = await e.API.GroupApi.GetGroupInfoAsync(e.FromGroup.Id);
    return EventHandleResult.Pass;
}
```

## 📦 IPluginApi 接口

`IPluginApi` 是所有 API 的聚合接口，包含以下属性：

| 📌 属性 | 📝 类型 | 📎 说明 |
| -- | -- | -- |
| Logger | ILogger | 日志记录器 |
| MessageApi | IMessageApi | 消息相关操作 |
| FriendApi | IFriendApi | 好友相关操作 |
| GroupApi | IGroupApi | 群组相关操作 |
| AppApi | IAppApi | 应用相关操作 |

---

## 📨 IMessageApi 消息接口

提供消息发送、撤回、获取聊天记录等功能。

### 发送消息

| 📌 方法 | 📝 说明 |
| -- | -- |
| `SendPrivateMessage(userId, message)` | 发送私聊消息 |
| `SendGroupMessage(groupId, message)` | 发送群聊消息 |
| `SendPrivateMessageAsync(userId, message)` | 异步发送私聊消息 |
| `SendGroupMessageAsync(groupId, message)` | 异步发送群聊消息 |

> 💡 **提示**：返回值为消息 ID，发送失败返回 0。

### 撤回消息

| 📌 方法 | 📝 说明 |
| -- | -- |
| `DeleteMessage(messageId)` | 撤回消息 |
| `DeleteMessageAsync(messageId)` | 异步撤回消息 |

### 获取聊天记录

| 📌 方法 | 📝 说明 |
| -- | -- |
| `GetChatHistories(groupId, qq, count)` | 获取聊天记录（私聊时 groupId 设为 0） |
| `GetChatHistoryById(parentId, isGroup, messageId)` | 通过消息 ID 获取聊天记录 |

---

## 👥 IGroupApi 群组接口

提供群组管理、成员管理、禁言等功能。

### 获取群信息

| 📌 方法 | 📝 说明 |
| -- | -- |
| `GetGroupInfo(groupId)` | 获取群信息 |
| `GetGroupList()` | 获取已加入的群列表 |
| `GetGroupMemberInfo(groupId, qq)` | 获取群成员信息 |
| `GetGroupMembers(groupId)` | 获取群成员列表（成员多时耗时较长） |

### 禁言操作

| 📌 方法 | 📝 说明 |
| -- | -- |
| `BanGroup(groupId, enable)` | 全群禁言 / 解除全群禁言 |
| `BanMember(groupId, qq, duration)` | 禁言群成员（duration=0 解除禁言） |

> ⚠️ **注意**：禁言操作要求当前账号是群主或管理员，且不能对群主或自己操作。

### 成员管理

| 📌 方法 | 📝 说明 |
| -- | -- |
| `Kick(groupId, qq, rejectAddRequest)` | 移除群成员（可选拒绝后续入群） |
| `SetAdmin(groupId, qq, isAdmin)` | 设置 / 取消管理员 |
| `SetMemberCard(groupId, qq, card)` | 设置群名片 |
| `SetMemberTitle(groupId, qq, title)` | 设置群头衔（仅群主可用） |

### 其他操作

| 📌 方法 | 📝 说明 |
| -- | -- |
| `Leave(groupId)` | 退出群（群主时会解散群） |
| `SetGroupAddRequest(flag, accept, reason)` | 处理入群请求 |
| `SetGroupInviteRequest(flag, accept, reason)` | 处理被邀请入群请求 |

---

## 👤 IFriendApi 好友接口

提供好友列表获取、名片赞、处理好友请求等功能。

| 📌 方法 | 📝 说明 |
| -- | -- |
| `GetFriendInfos()` | 获取好友列表 |
| `SendPraise(qq, count)` | 发送名片赞 |
| `SetFriendAddRequest(flag, accept, card)` | 处理好友添加请求 |

> 💡 **提示**：名片赞可能有每日次数限制，具体取决于协议实现。

---

## 🖥️ IAppApi 应用接口

提供应用信息获取、插件控制等功能。

### 获取信息

| 📌 方法 | 📝 说明 |
| -- | -- |
| `GetAppDirectory()` | 获取插件数据目录（绝对路径） |
| `GetLoginQQ()` | 获取当前登录的 QQ 号（未登录返回 0） |
| `GetLoginQQNick()` | 获取当前登录的昵称（未登录返回空字符串） |

### 插件控制

| 📌 方法 | 📝 说明 |
| -- | -- |
| `ReloadPlugin()` | 重载当前插件（会导致插件进程终止） |
| `DisablePlugin()` | 禁用当前插件（会导致插件进程终止） |

> ⚠️ **注意**：`ReloadPlugin` 和 `DisablePlugin` 会导致插件进程终止，调用后代码不会继续执行。

---

## 📝 ILogger 日志接口

提供分级日志记录功能。

| 📌 方法 | 📝 说明 |
| -- | -- |
| `Debug(type, message)` | Debug 级别日志 |
| `Info(type, message)` | Info 级别日志 |
| `Warn(type, message)` | Warn 级别日志（可能有桌面通知） |
| `Error(type, message)` | Error 级别日志（可能有桌面通知） |
| `Fatal(message)` | Fatal 级别日志（可能有桌面通知） |

### 使用示例

```csharp
public class MyPlugin : PluginBase, IGroupMessageHandler
{
    public async Task<EventHandleResult> OnReceiveGroupMessageAsync(GroupMessageContext e, CancellationToken ct)
    {
        // 记录调试信息
        API.Logger.Debug("收到消息", e.Message.Text);

        try
        {
            // 业务逻辑...
            API.Logger.Info("收到消息", "成功响应消息");
        }
        catch (Exception ex)
        {
            API.Logger.Error("收到消息", ex.Message);
        }

        return EventHandleResult.Pass;
    }
}
```

> 💡 **提示**：`type` 参数用于描述日志对应的动作（如"初始化"、"处理消息"、"绘制时长图"），方便开发者定位日志产生位置。框架会自动区分不同插件的日志来源。

---

## 📋 异步方法说明

所有 API 方法都提供了同步和异步两个版本（如 `SendGroupMessage` 和 `SendGroupMessageAsync`）。

```csharp
// 异步
await API.MessageApi.SendGroupMessageAsync(groupId, "消息");

// 同步
API.MessageApi.SendGroupMessage(groupId, "消息");
```
