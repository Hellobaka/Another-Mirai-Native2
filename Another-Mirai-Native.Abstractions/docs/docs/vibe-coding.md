# Vibe Coding

氛围编程是通过自然语言描述需求，让 AI 自动完成插件开发的工作方式。框架提供了一份 `llms.txt` 文档，包含了开发插件所需的全部信息，AI 仅凭这一份文件就能独立生成插件代码。

## 📄 什么是 llms.txt

`llms.txt` 是一份专为 AI 设计的单文件文档，它整合了项目搭建、事件处理、指令模式、API 参考、消息构建、窗口创建、构建分发及常见错误模式等全部内容。AI 读入这份文件后，即可独立完成插件的编写，无需再查阅其他资料。

点击此处[下载](/images/llms.txt)

> 💡 **提示**：你也可以将 `llms.txt` 复制到插件项目的根目录或 `.cursor/` 目录下，方便 AI 工具自动读取。

## 🚀 编写需求

将 `llms.txt` 提供给 AI 后，用自然语言描述你的插件需求。以下是有效的提示词示例：

**简单指令**

```
按照 llms.txt，帮我创建一个群聊复读插件，appId 为 com.example.repeater
```

**带业务逻辑**

```
帮我做一个签到插件。用户发送 /sign 时记录签到，同一用户每天只能签到一次。
每次签到随机获得 1-100 积分，发送 /rank 查看积分排行。使用 sqlite 存储数据。
appId 用 com.example.checkin
```

**带菜单窗口**

```
做一个图片管理插件。从 data\image\submit\ 目录下读取新图片作为待审核，
通过菜单打开审核窗口，窗口显示图片预览，支持"通过"和"拒绝"两个按钮。
通过的图片移动到 data\image\approved\。appId 用 com.example.imagereview
```

### 提示词要写什么

| 要说明的 | 示例 |
| -- | -- |
| 功能描述 | "用户发送 /roll 掷骰子，支持 /roll 2d6 格式" |
| appId | "appId 用 com.example.dice" |
| 数据存储 | "使用 sqlite 存储" 或 "使用 json 文件保存配置" |
| 特殊行为 | "管理员才能使用此功能"、"每天早上 8 点自动推送" |

### 不需要说明的内容

以下内容 AI 会从 `llms.txt` 中自行获取，无需在提示词中重复：

- 如何创建项目、配置 .csproj
- 如何发送消息、构建 CQ 码
- 如何注册事件处理器
- 如何使用指令模式
- 插件文件应放在什么路径
- 返回值 Block/Pass 的含义
- 如何启用 MCP 服务器、有哪些 MCP 工具可用

## 🏗️ 构建与部署

AI 生成代码后的操作步骤：

1. 使用 `dotnet build` 编译项目
2. 从输出目录中找到 `Native_*.dll` 和 `Native_*.json`
3. 将这两个文件复制到 AMN2 框架的 `data\plugins\` 目录
4. 重启框架或点击"重载插件"

## 🔄 迭代完善

测试插件功能后，发现的问题或新需求可以直接反馈给 AI，让它修改代码并重新构建。常见迭代方向：

- "收到消息后响应太慢了，加上超时处理"
- "签到成功时加一个表情回复，积分榜前三名显示头衔"
- "图片审核增加一个理由输入框，拒绝时必须填写理由"
- "把数据库从 sqlite 换成 json 文件存储"

## 🤖 MCP 服务器：让 AI 直接操作框架

前面的迭代循环（构建 → 复制文件 → 重载 → 观察 → 再改）需要不少人工操作。框架提供了一个捷径：NoConnection 测试协议内置了 MCP（Model Context Protocol）服务器。启用后，AI 客户端可以通过标准 MCP 协议直接操作运行中的框架——上传插件、发送模拟消息、查询日志——从而自主完成"测试 → 修复"的完整循环，全程无需打开框架 UI，也无需真实的 QQ 连接。

### 启用

1. 让框架连接到 **NoConnection 协议**（连接后会自动弹出 Tester 测试窗口）
2. 在 Tester 页面切换到 **"MCP服务"** 选项卡
3. 勾选 **"启用 MCP 服务器"**
4. 点击 **"应用并重启"**

启动成功后，页面状态标签会显示 `状态: 运行中 (http://127.0.0.1:46000/)`，框架日志也会输出 `MCP 服务已启动，监听 http://127.0.0.1:46000/`。

监听 IP 与端口（默认 `127.0.0.1:46000`）可以在同一选项卡中修改，改完后再次点击"应用并重启"即可生效；也可以通过页面上的"停止"按钮随时停止服务。

> ⚠️ **注意**：MCP 服务器仅在 .NET 10（`net10.0-windows`）运行时可用，.NET Framework 4.8 构建下"MCP服务"选项卡会被禁用。

### 连接 MCP 客户端

MCP 端点位于**根路径** `http://127.0.0.1:46000/`（注意不是 `/mcp`），使用 Streamable HTTP 传输（stateless 无状态模式）。支持远程 HTTP 服务器的 MCP 客户端，配置示例：

```json
{
  "mcpServers": {
    "amn2": {
      "url": "http://127.0.0.1:46000/"
    }
  }
}
```

> ⚠️ **注意**：MCP 服务器没有任何认证机制，请勿将端口暴露到公网。

### 工具列表

服务器共公开 7 个工具：

| 工具名 | 用途 | 参数 |
| -- | -- | -- |
| `send_message` | 模拟接收一条群聊/私聊消息（支持 CQ 码），可选延时自动撤回 | `isPrivateChat`、`groupId`（私聊填 0）、`senderId`、`message` 必填；`autoRevoke`（默认 false）、`autoRevokeSeconds`（默认 10）、`messageId` 可选 |
| `add_plugin` | 上传插件：指定 DLL 与 JSON 文件的绝对路径，复制到插件目录并注册 | `dllPath`、`jsonPath` |
| `enable_plugin` | 按插件的**中文名称**（JSON 清单 `name` 字段，区分大小写）启用插件 | `pluginName` |
| `reload_plugin` | 按 AuthCode 重新加载插件（需处于启用状态） | `authCode` |
| `disable_plugin` | 按 AuthCode 禁用插件 | `authCode` |
| `list_plugins` | 获取所有已加载插件的详细信息（名称、AuthCode、版本、启用状态等） | 无 |
| `get_latest_logs` | 获取框架最近的运行日志条目，用于诊断 | `logEntryCount`（默认 10，最大 100） |

### 典型工作流

1. 让 AI 构建插件，生成 `Native_*.dll` 和 `Native_*.json`
2. `add_plugin` 上传到运行中的框架
3. 首次用 `enable_plugin` 启用；之后迭代用 `reload_plugin` 重载
4. `send_message` 发送模拟消息触发插件逻辑（返回值会告知哪个插件处理了该消息；配合 `autoRevoke` 可测试撤回处理）
5. `get_latest_logs` 查看日志，把结果反馈给 AI 继续修复

如此，AI 即可自主完成"写代码 → 部署 → 测试 → 修复"的闭环，人只需要在旁边看着。

## ⚠️ 常见踩坑点

即使有 `llms.txt`，AI 偶尔也可能忽略以下细节。如果构建失败或插件无法加载，优先检查：

1. `.csproj` 中是否有 `<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>`
2. `[PluginInfo]` 的 appId 是否为反向域名格式
3. 图片/音频路径是否正确（必须在 `data\image\` 或 `data\record\` 下）
4. 插件目标框架是否与 AMN2 运行时的 .NET 版本一致

> ⚠️ **注意**：AI 无法运行和测试代码，首次生成后通常需要少量调试。
