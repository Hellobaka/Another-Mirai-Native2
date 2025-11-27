# ChatPage 重构任务清单

## 项目概述
重构 `UI_WPF\Pages\ChatPage.xaml` 和 `ChatPage.xaml.cs`，减少重复代码，修复逻辑错误，拆分组件，提高可维护性。

**当前状态**：
- 代码行数：XAML 232行，C# 1422行
- 主要问题：职责不清、重复代码多、缺少服务层抽象、UI组件耦合度高

---

## 阶段1：代码后台重构（C# Backend）

### 1.1 创建服务层抽象 ⭐ 优先级：高
**目标**：将业务逻辑从UI层分离

#### [x] 任务1.1.1：创建 ICacheService 接口和实现
**文件**：`UI_WPF\Services\ICacheService.cs`, `UI_WPF\Services\CacheService.cs`
**描述**：统一管理好友、群、群成员信息缓存
**状态**：✅ 已完成
**当前问题**：
- 三个独立的Dictionary：FriendInfoCache, GroupInfoCache, GroupMemberCache
- 重复的缓存获取逻辑：GetFriendNick, GetGroupMemberNick, GetGroupName

**重构内容**：
```csharp
// 接口方法：
- Task<string> GetFriendNickAsync(long qq)
- Task<string> GetGroupNameAsync(long groupId)
- Task<string> GetGroupMemberNickAsync(long groupId, long qq)
- void ClearCache()
```

**预期改进**：
- 减少重复代码约100行
- 统一缓存管理逻辑
- 便于单元测试

---

#### [x] 任务1.1.2：创建 IMessageService 接口和实现
**文件**：`UI_WPF\Services\IMessageService.cs`, `UI_WPF\Services\MessageService.cs`
**描述**：处理消息发送、持久化、历史记录
**状态**：✅ 已完成
**当前问题**：
- AddGroupChatItem 和 AddPrivateChatItem 重复代码约80%
- CallGroupMsgSend 和 CallPrivateMsgSend 逻辑相似
- ExecuteSendMessage 包含过多职责

**重构内容**：
```csharp
// 接口方法：
- Task<int> SendMessageAsync(long targetId, ChatType chatType, string message)
- Task<int> AddChatItemAsync(ChatItemParameters parameters)
- Task<ChatDetailItemViewModel> ParseHistoryAsync(ChatHistory history, ChatAvatar.AvatarTypes avatarType)
```

**预期改进**：
- 减少重复代码约200行
- 职责分离更清晰
- 便于消息发送流程的维护

---

#### [x] 任务1.1.3：创建 IChatListService 接口和实现
**文件**：`UI_WPF\Services\IChatListService.cs`, `UI_WPF\Services\ChatListService.cs`
**描述**：管理左侧聊天列表的添加、更新、排序
**状态**：✅ 已完成
**当前问题**：
- AddOrUpdateGroupChatList 和 AddOrUpdatePrivateChatList 几乎完全相同
- ReorderChatList 逻辑分散

**重构内容**：
```csharp
// 接口方法：
- Task AddOrUpdateChatListAsync(long id, ChatType type, long senderId, string message)
- Task ReorderChatListAsync()
- Task LoadChatHistoryAsync()
- void UpdateUnreadCount(long id, ChatType type, int count)
```

**预期改进**：
- 减少重复代码约80行
- 统一聊天列表管理
- 便于扩展新的聊天类型

---

### 1.2 提取辅助类 ⭐ 优先级：中

#### [ ] 任务1.2.1：创建 LazyLoadManager
**文件**：`UI_WPF\Pages\Helpers\LazyLoadManager.cs`
**描述**：管理消息列表懒加载逻辑
**当前问题**：
- LazyLoad 方法过于复杂（约50行）
- 防抖逻辑与UI代码混合
- MessageScrollViewer_ScrollChanged 职责过多

**重构内容**：
```csharp
// 类方法：
- Task LoadMoreMessagesAsync(int count, int targetMsgId = -1)
- void EnableLazyLoad()
- void DisableLazyLoad()
- void HandleScroll(ScrollChangedEventArgs e)
```

**预期改进**：
- 减少ChatPage.xaml.cs约80行
- 懒加载逻辑更清晰
- 便于调试和测试

---

#### [ ] 任务1.2.2：创建 MessageContainerManager
**文件**：`UI_WPF\Pages\Helpers\MessageContainerManager.cs`
**描述**：管理消息容器的添加、删除、滚动
**当前问题**：
- AddItemToMessageContainer 与清理逻辑混合
- ScrollToBottom 逻辑分散
- 消息数量管理硬编码

**重构内容**：
```csharp
// 类方法：
- void AddMessage(ChatDetailItemViewModel item, bool autoClean = true)
- void ClearMessages()
- void ScrollToBottom(bool forced = false)
- bool HasMessage(string guid)
- void RemoveOldMessages(int maxCount)
```

**预期改进**：
- 减少ChatPage.xaml.cs约60行
- 消息容器管理更清晰
- 便于调整清理策略

---

#### [x] 任务1.2.3：创建 RichTextBoxHelper
**文件**：`UI_WPF\Pages\Helpers\RichTextBoxHelper.cs`
**描述**：处理RichTextBox相关操作
**状态**：✅ 已完成
**当前问题**：
- BuildTextFromRichTextBox 逻辑复杂
- RichTextboxPasteOverrideAction 职责过多
- AddTextToSendBox 分散

**重构内容**：
```csharp
// 静态方法：
- static string ConvertToCQCode(RichTextBox richTextBox)
- static void HandlePaste(DataObjectPastingEventArgs e, RichTextBox target)
- static void InsertText(RichTextBox richTextBox, string text)
- static void InsertImage(RichTextBox richTextBox, BitmapSource image)
```

**预期改进**：
- 减少ChatPage.xaml.cs约100行
- RichTextBox操作复用性更高
- 便于单元测试

---

### 1.3 优化ViewModel ⭐ 优先级：高

#### [ ] 任务1.3.1：创建 ChatPageViewModel
**文件**：`UI_WPF\Pages\ViewModels\ChatPageViewModel.cs`
**描述**：将数据绑定逻辑从Page移到ViewModel
**当前问题**：
- ChatPage直接继承INotifyPropertyChanged
- 属性和UI逻辑混合
- 缺少Command模式

**重构内容**：
```csharp
// ViewModel属性：
- ObservableCollection<ChatListItemViewModel> ChatList
- ChatListItemViewModel SelectedChatItem
- string GroupName
- bool IsChatEnabled

// ViewModel命令：
- ICommand SendMessageCommand
- ICommand ClearMessageCommand
- ICommand SelectFaceCommand
- ICommand SelectImageCommand
- ICommand SelectAudioCommand
- ICommand AtMemberCommand
```

**预期改进**：
- 符合MVVM模式
- UI与业务逻辑分离
- 便于单元测试

---

#### [ ] 任务1.3.2：创建 ToolbarViewModel
**文件**：`UI_WPF\Pages\ViewModels\ToolbarViewModel.cs`
**描述**：管理工具栏按钮状态
**当前问题**：
- 按钮启用状态硬编码在SelectionChanged中
- 每个按钮单独设置IsEnabled

**重构内容**：
```csharp
// ViewModel属性：
- bool IsFaceEnabled
- bool IsAtEnabled
- bool IsPictureEnabled
- bool IsAudioEnabled
- bool IsSendEnabled

// 逻辑方法：
- void UpdateButtonStates(ChatType? chatType)
```

**预期改进**：
- 减少重复代码约20行
- 工具栏状态管理集中化
- 支持数据绑定

---

## 阶段2：XAML重构

### 2.1 创建可复用控件 ⭐ 优先级：中

#### [ ] 任务2.1.1：创建 ChatToolbar 用户控件
**文件**：`UI_WPF\Controls\ChatToolbar.xaml`, `ChatToolbar.xaml.cs`
**描述**：将工具栏按钮提取为独立控件
**当前问题**：
- 工具栏按钮代码重复（约60行）
- 按钮样式不统一
- 难以维护和扩展

**重构内容**：
```xml
<!-- 包含按钮：-->
- FaceButton (带Flyout)
- AtButton
- PictureButton
- AudioButton
- ClearMessageButton

<!-- 依赖属性：-->
- IsAtEnabled
- SelectedChatType
```

**预期改进**：
- 减少XAML约50行
- 工具栏复用性更高
- 便于添加新按钮

---

#### [ ] 任务2.1.2：创建 MessageInputPanel 用户控件
**文件**：`UI_WPF\Controls\MessageInputPanel.xaml`, `MessageInputPanel.xaml.cs`
**描述**：将消息输入区域提取为独立控件
**当前问题**：
- 输入框、工具栏、发送按钮混在一起
- Grid嵌套过深（3-4层）

**重构内容**：
```xml
<!-- 包含组件：-->
- ChatToolbar
- RichTextBox (SendText)
- ClearButton
- SendButton

<!-- 事件：-->
- MessageSending
- MessageCleared
```

**预期改进**：
- 减少XAML约80行
- 输入面板组件化
- 便于在其他页面复用

---

#### [ ] 任务2.1.3：创建 ChatListPanel 用户控件
**文件**：`UI_WPF\Controls\ChatListPanel.xaml`, `ChatListPanel.xaml.cs`
**描述**：将左侧聊天列表提取为独立控件
**当前问题**：
- 列表和空状态提示混在一起
- ScrollViewer包装不必要

**重构内容**：
```xml
<!-- 包含组件：-->
- EmptyHint TextBlock
- ListView (ChatListDisplay)

<!-- 依赖属性：-->
- ItemsSource
- SelectedItem

<!-- 事件：-->
- SelectionChanged
```

**预期改进**：
- 减少XAML约30行
- 列表组件独立
- 便于样式定制

---

#### [ ] 任务2.1.4：创建 MessageDisplayPanel 用户控件
**文件**：`UI_WPF\Controls\MessageDisplayPanel.xaml`, `MessageDisplayPanel.xaml.cs`
**描述**：将消息显示区域提取为独立控件
**当前问题**：
- 消息容器、滚动按钮、标题混在一起
- Border和Grid嵌套复杂

**重构内容**：
```xml
<!-- 包含组件：-->
- GroupName TextBlock
- MessageScrollViewer
- MessageContainer StackPanel
- ScrollToBottomButton

<!-- 依赖属性：-->
- GroupName
- Messages (ObservableCollection)

<!-- 事件：-->
- ScrollChanged
- ScrollToBottomClicked
```

**预期改进**：
- 减少XAML约60行
- 消息显示逻辑独立
- 便于扩展功能

---

### 2.2 简化布局结构 ⭐ 优先级：低

#### [ ] 任务2.2.1：减少Grid嵌套层级
**文件**：`UI_WPF\Pages\ChatPage.xaml`
**描述**：简化Grid嵌套，使用更扁平的结构
**当前问题**：
- DetailContainer下Grid嵌套3-4层
- Row定义和实际使用不匹配

**重构内容**：
- 使用StackPanel替代部分Grid
- 合并不必要的Border
- 使用DockPanel简化布局

**预期改进**：
- 提高XAML可读性
- 减少布局计算
- 便于理解结构

---

#### [ ] 任务2.2.2：统一命名规范
**文件**：`UI_WPF\Pages\ChatPage.xaml`
**描述**：统一控件命名风格
**当前问题**：
- 命名不一致：EmptyHint, ChatListDisplay, DetailContainer
- 部分控件缺少x:Name

**重构内容**：
- 使用统一的命名前缀（如：chatList, messageContainer）
- 或使用统一的命名后缀（如：EmptyHintText, ChatListView）

**预期改进**：
- 代码可读性提升
- 便于查找和修改

---

## 阶段3：数据绑定优化

### 3.1 使用Command替代事件 ⭐ 优先级：中

#### [ ] 任务3.1.1：将Click事件改为Command
**文件**：`UI_WPF\Pages\ChatPage.xaml`, `ChatPageViewModel.cs`
**描述**：使用ICommand替代直接的Click事件处理
**当前问题**：
- 所有按钮使用Click事件
- 代码后台耦合度高

**改造按钮列表**：
- SendBtn_Click → SendMessageCommand
- CleanSendBtn_Click → ClearSendCommand
- AtBtn_Click → ShowAtSelectorCommand
- PictureBtn_Click → SelectPictureCommand
- AudioBtn_Click → SelectAudioCommand
- CleanMessageBtn_Click → ClearMessageCommand
- ScrollToBottomBtn_Click → ScrollToBottomCommand

**预期改进**：
- 符合MVVM最佳实践
- 便于单元测试
- 支持CommandParameter

---

#### [ ] 任务3.1.2：使用Binding替代硬编码
**文件**：`UI_WPF\Pages\ChatPage.xaml`
**描述**：将按钮IsEnabled状态改为数据绑定
**当前问题**：
- IsEnabled在代码后台硬编码
- 状态变化需要手动更新每个按钮

**改造内容**：
```xml
<!-- 原代码： -->
<Button x:Name="FaceBtn" IsEnabled="False" />

<!-- 改为： -->
<Button Command="{Binding ToolbarViewModel.SelectFaceCommand}" 
        IsEnabled="{Binding ToolbarViewModel.IsFaceEnabled}" />
```

**预期改进**：
- 状态自动同步
- 减少代码后台逻辑
- 更容易扩展

---

## 阶段4：代码质量改进

### 4.1 修复逻辑错误 ⭐ 优先级：高

#### [x] 任务4.1.1：修复AddOrUpdatePrivateChatList的Id错误
**文件**：`UI_WPF\Pages\ChatPage.xaml.cs` Line 588
**描述**：私聊列表的Id应该是qq而不是sender
**状态**：✅ 已修复
**当前代码**：
```csharp
Id = sender,  // 错误：应该是qq
```

**修复为**：
```csharp
Id = qq,
```

**影响**：私聊列表可能显示错误的聊天对象

---

#### [ ] 任务4.1.2：修复缓存竞态条件
**文件**：`UI_WPF\Pages\ChatPage.xaml.cs`
**描述**：多个缓存获取方法可能存在竞态条件
**当前问题**：
- APILock的使用不一致
- 异步方法中可能同时更新缓存

**改进方案**：
- 使用ConcurrentDictionary替代Dictionary
- 或改进锁机制确保线程安全

**预期改进**：
- 避免缓存数据损坏
- 提高并发性能

---

#### [ ] 任务4.1.3：修复内存泄漏风险
**文件**：`UI_WPF\Pages\ChatPage.xaml.cs`
**描述**：事件订阅可能导致内存泄漏
**当前问题**：
- 静态事件 MsgRecalled, WindowSizeChanged
- 多个PluginManagerProxy事件订阅未取消

**改进方案**：
- 实现IDisposable
- 在Unloaded事件中取消订阅
- 使用WeakEventManager

**预期改进**：
- 避免内存泄漏
- 提高应用稳定性

---

### 4.2 性能优化 ⭐ 优先级：低

#### [ ] 任务4.2.1：优化消息容器渲染
**文件**：`UI_WPF\Pages\ChatPage.xaml`
**描述**：使用虚拟化优化大量消息渲染
**当前问题**：
- StackPanel不支持虚拟化
- 消息过多时性能下降

**改进方案**：
- 使用VirtualizingStackPanel
- 或使用ItemsControl + VirtualizingPanel

**预期改进**：
- 提高滚动流畅度
- 降低内存占用

---

#### [ ] 任务4.2.2：优化缓存查询
**文件**：CacheService.cs
**描述**：批量获取缓存信息
**当前问题**：
- 每次获取昵称都需要查询
- 没有预加载机制

**改进方案**：
- 提供批量查询接口
- 实现缓存预热
- 添加缓存过期策略

**预期改进**：
- 减少API调用次数
- 提高响应速度

---

## 阶段5：测试和文档

### 5.1 单元测试 ⭐ 优先级：中

#### [ ] 任务5.1.1：为服务层添加单元测试
**文件**：`Tests\Services\*Tests.cs`
**描述**：测试CacheService, MessageService, ChatListService
**测试内容**：
- 缓存命中和缺失场景
- 消息发送成功和失败
- 列表添加、更新、排序

---

#### [ ] 任务5.1.2：为ViewModel添加单元测试
**文件**：`Tests\ViewModels\*Tests.cs`
**描述**：测试ChatPageViewModel, ToolbarViewModel
**测试内容**：
- 属性变化通知
- Command执行
- 状态更新逻辑

---

### 5.2 文档更新 ⭐ 优先级：低

#### [ ] 任务5.2.1：更新架构文档
**文件**：`Docs\ChatPage_Architecture.md`
**描述**：记录新的架构设计和组件关系
**内容包括**：
- 组件层次图
- 服务依赖关系
- 数据流向图

---

#### [ ] 任务5.2.2：添加代码注释
**文件**：所有新创建的文件
**描述**：为新类和方法添加XML注释
**注释内容**：
- 类的职责说明
- 方法的参数和返回值
- 使用示例

---

## 进度追踪

**总任务数**：38
**已完成**：5
**进行中**：0
**未开始**：33

**阶段进度**：
- [ ] 阶段1：代码后台重构（4/11）
- [ ] 阶段2：XAML重构（0/6）
- [ ] 阶段3：数据绑定优化（0/2）
- [ ] 阶段4：代码质量改进（1/5）
- [ ] 阶段2：XAML重构（0/6）
- [ ] 阶段3：数据绑定优化（0/2）
- [ ] 阶段4：代码质量改进（0/5）
- [ ] 阶段5：测试和文档（0/4）

---

## 预期收益

### 代码行数减少：
- XAML：从232行减少到约150行（-35%）
- C#：从1422行减少到约800行（-44%）
- 新增服务层代码：约600行
- 净减少：约200行，模块化更好

### 可维护性提升：
- 职责分离明确
- 代码复用性提高
- 单元测试覆盖率提升
- 便于功能扩展

### 性能改进：
- 消息渲染性能提升
- 缓存查询优化
- 减少内存泄漏风险

---

## 执行建议

1. **推荐执行顺序**：
   - 先完成阶段1.1（服务层抽象）- 基础设施
   - 再完成阶段1.3（ViewModel优化）- 架构调整
   - 然后完成阶段4.1（修复逻辑错误）- 确保正确性
   - 最后完成阶段2（XAML重构）- UI优化

2. **每个任务的时间估算**：
   - 高优先级任务：2-4小时
   - 中优先级任务：1-2小时
   - 低优先级任务：0.5-1小时
   - 总预估时间：40-60小时

3. **风险点**：
   - 重构过程中可能引入新bug，需要充分测试
   - 依赖注入可能需要调整程序启动逻辑
   - XAML重构可能影响现有样式

4. **回退策略**：
   - 每个阶段完成后提交Git
   - 保留原代码备份
   - 逐步集成，避免大规模改动

---

## 更新日志

- 2025-11-27 06:15：创建TODO文档，完成初步分析和计划
- 2025-11-27 06:17：✅ 完成任务1.1.1、1.1.2、1.1.3 - 创建服务层抽象（ICacheService, IMessageService, IChatListService）
- 2025-11-27 06:22：✅ 完成任务1.2.3 - 创建RichTextBoxHelper辅助类
- 2025-11-27 06:23：✅ 完成任务4.1.1 - 修复AddOrUpdatePrivateChatList的Id错误
- 2025-11-27 06:25：🔄 ChatPage.xaml.cs重构 - 集成服务层，移除重复代码约400行
- 2025-11-27 06:27：🐛 修复编译错误 - 添加缺失的using引用
- 2025-11-27 06:28：✅ **编译通过验证** - 所有修改已通过编译测试
