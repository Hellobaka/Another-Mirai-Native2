# ChatPage 架构文档

## 概述

ChatPage 是 Another-Mirai-Native2 的聊天界面，采用 MVVM 模式进行架构设计，实现了服务层抽象、辅助类封装和可复用控件拆分。

## 架构图

```
┌─────────────────────────────────────────────────────────────────────────┐
│                              ChatPage.xaml                                │
│  ┌─────────────┐  ┌─────────────────────────────────────────────────┐   │
│  │ ChatListPanel│  │                  DetailPanel                     │   │
│  │             │  │  ┌─────────────────────────────────────────────┐│   │
│  │ EmptyHintText│  │  │           MessageDisplayPanel              ││   │
│  │ ChatListView │  │  │  ┌─────────────────────────────────────┐   ││   │
│  │             │  │  │  │        MessageContainer            │   ││   │
│  │             │  │  │  │  (ChatDetailListItem 列表)         │   ││   │
│  │             │  │  │  └─────────────────────────────────────┘   ││   │
│  │             │  │  │  ScrollToBottomButton                     ││   │
│  │             │  │  └─────────────────────────────────────────────┘│   │
│  │             │  │  ┌─────────────────────────────────────────────┐│   │
│  │             │  │  │           MessageInputPanel               ││   │
│  │             │  │  │  ChatToolbar | SendTextBox | SendButton  ││   │
│  │             │  │  └─────────────────────────────────────────────┘│   │
│  └─────────────┘  └─────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────────┘
```

## 层次结构

### 1. View 层 (UI)

```
UI_WPF/Pages/
├── ChatPage.xaml              # 主页面布局
└── ChatPage.xaml.cs           # 页面代码后台（协调器角色）

UI_WPF/Controls/
├── ChatToolbar.xaml/cs        # 工具栏控件（表情、@、图片、音频、清空）
├── MessageInputPanel.xaml/cs  # 消息输入面板
├── ChatListPanel.xaml/cs      # 聊天列表面板
├── MessageDisplayPanel.xaml/cs # 消息显示面板
├── ChatDetailListItem.xaml/cs # 单条消息块控件
├── ChatListItem.xaml/cs       # 左侧列表项控件
├── ChatAvatar.xaml/cs         # 头像控件
├── AtTargetSelector.xaml/cs   # @成员选择器
└── FaceImageSelector.xaml/cs  # 表情选择器
```

### 2. ViewModel 层

```
UI_WPF/ViewModel/
├── ChatPageViewModel.cs       # 聊天页面主ViewModel
├── ToolbarViewModel.cs        # 工具栏按钮状态ViewModel
├── ChatListItemViewModel.cs   # 聊天列表项ViewModel
├── ChatDetailItemViewModel.cs # 消息详情项ViewModel
└── RelayCommand.cs            # MVVM命令实现
```

### 3. Service 层 (业务逻辑)

```
UI_WPF/Services/
├── ICacheService.cs           # 缓存服务接口
├── CacheService.cs            # 好友/群/群成员信息缓存
├── IMessageService.cs         # 消息服务接口
├── MessageService.cs          # 消息发送和历史记录处理
├── IChatListService.cs        # 聊天列表服务接口
├── ChatListService.cs         # 左侧聊天列表管理
└── MessageSendingCoordinator.cs # 消息发送协调器
```

### 4. Helper 层 (辅助类)

```
UI_WPF/Pages/Helpers/
├── LazyLoadManager.cs         # 消息懒加载管理
├── MessageContainerManager.cs # 消息容器管理（添加/删除/滚动）
└── RichTextBoxHelper.cs       # RichTextBox操作辅助
```

## 数据流

```
┌─────────────┐     事件      ┌───────────────────┐    调用     ┌───────────────┐
│   View      │ ───────────> │    ViewModel       │ ─────────> │   Service     │
│ (ChatPage)  │              │ (ChatPageViewModel) │            │ (CacheService │
│             │ <─────────── │ (ToolbarViewModel)  │ <───────── │  MessageService│
└─────────────┘   绑定更新    └───────────────────┘   返回结果  │ ChatListService)│
                                    │                          └───────────────┘
                                    │ 管理
                                    ▼
                            ┌───────────────┐
                            │    Helper     │
                            │ (LazyLoadManager│
                            │ MessageContainerManager│
                            │ RichTextBoxHelper)│
                            └───────────────┘
```

## 核心类说明

### ChatPageViewModel

主ViewModel，负责管理聊天页面状态：

- **ChatList**: 左侧聊天列表 `ObservableCollection<ChatListItemViewModel>`
- **SelectedChatItem**: 当前选中的聊天项
- **ToolbarViewModel**: 工具栏状态子ViewModel
- **Commands**: SendMessageCommand, ClearMessageCommand, ScrollToBottomCommand 等

### ToolbarViewModel

管理工具栏按钮的启用/禁用状态：

- 根据聊天类型（群聊/私聊）自动更新按钮状态
- 私聊时禁用 @按钮
- 未选中聊天时禁用所有按钮

### CacheService

统一的缓存管理服务：

- 使用 `ConcurrentDictionary` 保证线程安全
- 支持单个和批量获取昵称/群名
- 支持缓存预热
- 智能预加载阈值（缺失成员>5个时预加载整个群成员列表）

### MessageSendingCoordinator

统一的消息发送流程协调器：

```csharp
// 发送流程
1. SaveToDatabase()      // 保存到数据库
2. AddToUI()            // 添加到UI并获取GUID
3. UpdateSendingStatus() // 显示发送中状态
4. SendMessage()        // 调用API发送
5. UpdateSuccess/Failed() // 更新结果状态
```

### LazyLoadManager

消息懒加载管理：

- 滚动到顶部时自动触发加载
- 300ms 防抖机制避免频繁加载
- 分页加载历史消息
- 保持滚动位置

### MessageContainerManager

消息容器管理：

- 添加/批量添加/删除消息
- 自动清理旧消息（保持数量在最大值内）
- 滚动控制（底部按钮智能显示/隐藏）
- 消息状态更新（发送中/成功/失败）

## 控件依赖

```
ChatPage.xaml
├── ChatListView (ListView)
│   └── ChatListItem (DataTemplate)
│       └── ChatAvatar
├── GroupNameText
├── MessageScrollViewer
│   └── MessageContainer (StackPanel)
│       └── ChatDetailListItem (动态添加)
│           └── ChatAvatar
├── ScrollToBottomButton
├── FaceImageSelector (Flyout内)
├── AtTargetSelector (Flyout内)
├── SendTextBox (RichTextBox)
└── SendButton
```

## 事件流

### 消息发送流程

```
1. 用户点击发送按钮
   ↓
2. SendBtn_Click / SendMessageCommand
   ↓
3. BuildTextFromRichTextBox() - 提取CQ码
   ↓
4. ExecuteSendMessage() - ChatPage
   ↓
5. MessageSendingCoordinator.SendMessageAsync()
   ├── SaveToDatabase()
   ├── AddToUI() → AddGroupChatItem/AddPrivateChatItem
   ├── UpdateSendingStatus()
   ├── MessageService.SendMessage()
   └── UpdateSuccess/UpdateFailed()
   ↓
6. UI更新：消息块状态变化
```

### 消息接收流程

```
1. PluginManagerProxy.OnGroupMsg/OnPrivateMsg 事件
   ↓
2. ChatPage.PluginManagerProxy_OnGroupMsg/OnPrivateMsg
   ↓
3. AddGroupChatItem/AddPrivateChatItem
   ├── GetGroupMemberNick/GetFriendNick (CacheService)
   ├── BuildChatDetailItem()
   ├── ChatHistoryHelper.GetHistoriesByMsgId()
   ├── AddOrUpdateGroupChatList/AddOrUpdatePrivateChatList
   └── MessageContainerManager.AddMessage()
   ↓
4. UI更新：消息显示，左侧列表更新
```

### 懒加载流程

```
1. 用户滚动到顶部（距离<50px）
   ↓
2. LazyLoadManager.OnScrollChanged()
   ↓
3. TriggerLazyLoadWithDebounce() - 300ms防抖
   ↓
4. LoadMoreMessagesAsync()
   ├── ChatHistoryHelper.GetHistoriesByPage()
   ├── ParseChatHistoryToViewModel()
   └── MessageContainerManager.AddMessages(insertAtBeginning: true)
   ↓
5. 恢复滚动位置
```

## 命名规范

控件命名采用 `{组件用途}{控件类型}` 格式：

| 名称 | 类型 | 说明 |
|------|------|------|
| EmptyHintText | TextBlock | 空状态提示 |
| ChatListView | ListView | 聊天列表 |
| DetailPanel | DockPanel | 详情面板 |
| GroupNameText | TextBlock | 群名显示 |
| FaceButton | Button | 表情按钮 |
| AtButton | Button | @按钮 |
| PictureButton | Button | 图片按钮 |
| AudioButton | Button | 音频按钮 |
| ClearMessageButton | Button | 清空消息按钮 |
| SendTextBox | RichTextBox | 发送输入框 |
| ClearSendButton | Button | 清空发送框按钮 |
| SendButton | Button | 发送按钮 |
| ScrollToBottomButton | Button | 滚动到底部按钮 |
| DisabledHintText | TextBlock | 禁用提示 |

## 线程安全

- **ConcurrentDictionary**: CacheService中的缓存字典
- **SemaphoreSlim**: API调用锁，避免并发请求
- **Dispatcher**: UI线程调度
- **TaskCompletionSource**: 异步等待回调完成

## 内存管理

- **IDisposable**: ChatPage实现IDisposable接口
- 页面卸载时取消所有事件订阅
- MessageContainerManager自动清理旧消息
- GC.Collect(): 清空消息容器后主动回收

## 扩展点

1. **添加新的聊天类型**: 
   - 在 ChatAvatar.AvatarTypes 添加新类型
   - 在 ToolbarViewModel.UpdateButtonStates() 添加对应逻辑

2. **添加新的工具栏按钮**:
   - 在 ToolbarViewModel 添加新的IsEnabled属性
   - 在 ChatPageViewModel 添加对应的Command
   - 在 ChatPage.xaml 添加按钮绑定

3. **添加新的消息类型**:
   - 在 ChatDetailListItem 支持新的消息渲染

4. **添加缓存策略**:
   - 扩展 ICacheService 接口
   - 在 CacheService 中实现缓存淘汰策略

## 更新历史

- 2025-11-27: 创建服务层抽象，完成MVVM模式实现
- 2025-12-01: 创建可复用UI控件，完成XAML布局优化
- 2025-12-01: 统一命名规范，完成缓存批量查询和预热
- 2025-12-01: 创建架构文档
