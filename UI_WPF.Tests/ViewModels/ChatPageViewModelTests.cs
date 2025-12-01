using Another_Mirai_Native.UI.Controls;
using Another_Mirai_Native.UI.Services;
using Another_Mirai_Native.UI.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xunit;

namespace Another_Mirai_Native.UI.Tests.ViewModels
{
    /// <summary>
    /// ChatPageViewModel 单元测试
    /// </summary>
    public class ChatPageViewModelTests
    {
        #region 构造函数测试

        [Fact]
        public void Constructor_ShouldInitializeChatList()
        {
            // Arrange & Act
            var viewModel = new ChatPageViewModel();

            // Assert
            Assert.NotNull(viewModel.ChatList);
            Assert.Empty(viewModel.ChatList);
            Assert.IsType<ObservableCollection<ChatListItemViewModel>>(viewModel.ChatList);
        }

        [Fact]
        public void Constructor_ShouldInitializeToolbarViewModel()
        {
            // Arrange & Act
            var viewModel = new ChatPageViewModel();

            // Assert
            Assert.NotNull(viewModel.ToolbarViewModel);
        }

        [Fact]
        public void Constructor_ShouldInitializeCommands()
        {
            // Arrange & Act
            var viewModel = new ChatPageViewModel();

            // Assert
            Assert.NotNull(viewModel.SendMessageCommand);
            Assert.NotNull(viewModel.ClearMessageCommand);
            Assert.NotNull(viewModel.ClearSendBoxCommand);
            Assert.NotNull(viewModel.ScrollToBottomCommand);
            Assert.NotNull(viewModel.ShowAtSelectorCommand);
            Assert.NotNull(viewModel.SelectPictureCommand);
            Assert.NotNull(viewModel.SelectAudioCommand);
        }

        [Fact]
        public void Constructor_ShouldInitializeWithEmptyGroupName()
        {
            // Arrange & Act
            var viewModel = new ChatPageViewModel();

            // Assert
            Assert.Equal("", viewModel.GroupName);
        }

        [Fact]
        public void Constructor_ShouldInitializeWithNullSelectedChatItem()
        {
            // Arrange & Act
            var viewModel = new ChatPageViewModel();

            // Assert
            Assert.Null(viewModel.SelectedChatItem);
        }

        #endregion

        #region GroupName 属性测试

        [Fact]
        public void GroupName_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            var propertyChangedRaised = false;
            string? changedPropertyName = null;
            
            viewModel.PropertyChanged += (sender, e) =>
            {
                propertyChangedRaised = true;
                changedPropertyName = e.PropertyName;
            };

            // Act
            viewModel.GroupName = "Test Group";

            // Assert
            Assert.True(propertyChangedRaised);
            Assert.Equal(nameof(ChatPageViewModel.GroupName), changedPropertyName);
            Assert.Equal("Test Group", viewModel.GroupName);
        }

        [Fact]
        public void GroupName_WhenSetToSameValue_ShouldNotRaisePropertyChanged()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            viewModel.GroupName = "Test";
            var propertyChangedRaised = false;
            
            viewModel.PropertyChanged += (sender, e) =>
            {
                propertyChangedRaised = true;
            };

            // Act
            viewModel.GroupName = "Test";

            // Assert
            Assert.False(propertyChangedRaised);
        }

        #endregion

        #region IsChatEnabled 属性测试

        [Fact]
        public void IsChatEnabled_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            var propertyChangedRaised = false;
            string? changedPropertyName = null;
            
            viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ChatPageViewModel.IsChatEnabled))
                {
                    propertyChangedRaised = true;
                    changedPropertyName = e.PropertyName;
                }
            };

            // Act - 切换状态
            var originalValue = viewModel.IsChatEnabled;
            viewModel.IsChatEnabled = !originalValue;

            // Assert
            Assert.True(propertyChangedRaised);
            Assert.Equal(nameof(ChatPageViewModel.IsChatEnabled), changedPropertyName);
        }

        #endregion

        #region SelectedChatItem 属性测试

        [Fact]
        public void SelectedChatItem_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            var propertyChangedRaised = false;
            string? changedPropertyName = null;
            
            viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ChatPageViewModel.SelectedChatItem))
                {
                    propertyChangedRaised = true;
                    changedPropertyName = e.PropertyName;
                }
            };

            var chatItem = new ChatListItemViewModel
            {
                Id = 12345,
                AvatarType = ChatAvatar.AvatarTypes.QQGroup,
                GroupName = "Test Group"
            };

            // Act
            viewModel.SelectedChatItem = chatItem;

            // Assert
            Assert.True(propertyChangedRaised);
            Assert.Equal(nameof(ChatPageViewModel.SelectedChatItem), changedPropertyName);
            Assert.Equal(chatItem, viewModel.SelectedChatItem);
        }

        [Fact]
        public void SelectedChatItem_WhenSetToNull_ShouldRaisePropertyChanged()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            viewModel.SelectedChatItem = new ChatListItemViewModel { Id = 12345 };
            
            var propertyChangedRaised = false;
            
            viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ChatPageViewModel.SelectedChatItem))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            viewModel.SelectedChatItem = null;

            // Assert
            Assert.True(propertyChangedRaised);
            Assert.Null(viewModel.SelectedChatItem);
        }

        [Fact]
        public void SelectedChatItem_WhenSetToSameItem_ShouldNotRaisePropertyChanged()
        {
            // Arrange
            var chatItem = new ChatListItemViewModel { Id = 12345 };
            var viewModel = new ChatPageViewModel();
            viewModel.SelectedChatItem = chatItem;
            
            var propertyChangedRaised = false;
            
            viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ChatPageViewModel.SelectedChatItem))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            viewModel.SelectedChatItem = chatItem;

            // Assert
            Assert.False(propertyChangedRaised);
        }

        #endregion

        #region SelectedChatItemChanged 事件测试

        [Fact]
        public void SelectedChatItem_WhenChanged_ShouldRaiseSelectedChatItemChangedEvent()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            var eventRaised = false;
            ChatListItemViewModel? eventItem = null;
            
            viewModel.SelectedChatItemChanged += (sender, item) =>
            {
                eventRaised = true;
                eventItem = item;
            };

            var chatItem = new ChatListItemViewModel
            {
                Id = 12345,
                AvatarType = ChatAvatar.AvatarTypes.QQGroup
            };

            // Act
            viewModel.SelectedChatItem = chatItem;

            // Assert
            Assert.True(eventRaised);
            Assert.Equal(chatItem, eventItem);
        }

        [Fact]
        public void SelectedChatItem_WhenChanged_ShouldUpdateToolbarState()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            
            var chatItem = new ChatListItemViewModel
            {
                Id = 12345,
                AvatarType = ChatAvatar.AvatarTypes.QQGroup
            };

            // Act
            viewModel.SelectedChatItem = chatItem;

            // Assert - 选中群聊时，所有按钮应该启用
            Assert.True(viewModel.ToolbarViewModel.IsFaceEnabled);
            Assert.True(viewModel.ToolbarViewModel.IsAtEnabled);
            Assert.True(viewModel.ToolbarViewModel.IsPictureEnabled);
            Assert.True(viewModel.ToolbarViewModel.IsAudioEnabled);
        }

        [Fact]
        public void SelectedChatItem_WhenSetToPrivateChat_ShouldDisableAtButton()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            
            var chatItem = new ChatListItemViewModel
            {
                Id = 12345,
                AvatarType = ChatAvatar.AvatarTypes.QQPrivate
            };

            // Act
            viewModel.SelectedChatItem = chatItem;

            // Assert - 私聊时At按钮应该禁用
            Assert.True(viewModel.ToolbarViewModel.IsFaceEnabled);
            Assert.False(viewModel.ToolbarViewModel.IsAtEnabled);
            Assert.True(viewModel.ToolbarViewModel.IsPictureEnabled);
            Assert.True(viewModel.ToolbarViewModel.IsAudioEnabled);
        }

        [Fact]
        public void SelectedChatItem_WhenSetToNull_ShouldDisableAllToolbarButtons()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            viewModel.SelectedChatItem = new ChatListItemViewModel
            {
                Id = 12345,
                AvatarType = ChatAvatar.AvatarTypes.QQGroup
            };

            // 确认按钮已启用
            Assert.True(viewModel.ToolbarViewModel.IsFaceEnabled);

            // Act
            viewModel.SelectedChatItem = null;

            // Assert - 未选中时所有按钮应该禁用
            Assert.False(viewModel.ToolbarViewModel.IsFaceEnabled);
            Assert.False(viewModel.ToolbarViewModel.IsAtEnabled);
            Assert.False(viewModel.ToolbarViewModel.IsPictureEnabled);
            Assert.False(viewModel.ToolbarViewModel.IsAudioEnabled);
        }

        #endregion

        #region UnreadCount 清零测试

        [Fact]
        public void SelectedChatItem_WhenChanged_ShouldClearUnreadCount()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            
            var chatItem = new ChatListItemViewModel
            {
                Id = 12345,
                AvatarType = ChatAvatar.AvatarTypes.QQGroup,
                UnreadCount = 5
            };

            // Act
            viewModel.SelectedChatItem = chatItem;

            // Assert
            Assert.Equal(0, chatItem.UnreadCount);
        }

        #endregion

        #region ChatList 操作测试

        [Fact]
        public void ChatList_CanAddItems()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            var chatItem = new ChatListItemViewModel
            {
                Id = 12345,
                GroupName = "Test Group"
            };

            // Act
            viewModel.ChatList.Add(chatItem);

            // Assert
            Assert.Single(viewModel.ChatList);
            Assert.Equal(chatItem, viewModel.ChatList[0]);
        }

        [Fact]
        public void ChatList_CanRemoveItems()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            var chatItem = new ChatListItemViewModel { Id = 12345 };
            viewModel.ChatList.Add(chatItem);

            // Act
            viewModel.ChatList.Remove(chatItem);

            // Assert
            Assert.Empty(viewModel.ChatList);
        }

        [Fact]
        public void ChatList_CanClearItems()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            viewModel.ChatList.Add(new ChatListItemViewModel { Id = 1 });
            viewModel.ChatList.Add(new ChatListItemViewModel { Id = 2 });
            viewModel.ChatList.Add(new ChatListItemViewModel { Id = 3 });

            // Act
            viewModel.ChatList.Clear();

            // Assert
            Assert.Empty(viewModel.ChatList);
        }

        #endregion

        #region SendMessageRequested 事件测试

        [Fact]
        public void SendMessageCommand_WhenExecuted_ShouldRaiseSendMessageRequestedEvent()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            viewModel.IsChatEnabled = true;
            
            var chatItem = new ChatListItemViewModel
            {
                Id = 12345,
                AvatarType = ChatAvatar.AvatarTypes.QQGroup
            };
            viewModel.SelectedChatItem = chatItem;

            var eventRaised = false;
            SendMessageEventArgs? eventArgs = null;
            
            viewModel.SendMessageRequested += (sender, args) =>
            {
                eventRaised = true;
                eventArgs = args;
            };

            // Act
            viewModel.SendMessageCommand.Execute(null);

            // Assert
            Assert.True(eventRaised);
            Assert.NotNull(eventArgs);
            Assert.Equal(12345, eventArgs.TargetId);
            Assert.Equal(ChatType.Group, eventArgs.ChatType);
        }

        [Fact]
        public void SendMessageCommand_WithPrivateChat_ShouldSetCorrectChatType()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            viewModel.IsChatEnabled = true;
            
            var chatItem = new ChatListItemViewModel
            {
                Id = 12345,
                AvatarType = ChatAvatar.AvatarTypes.QQPrivate
            };
            viewModel.SelectedChatItem = chatItem;

            SendMessageEventArgs? eventArgs = null;
            
            viewModel.SendMessageRequested += (sender, args) =>
            {
                eventArgs = args;
            };

            // Act
            viewModel.SendMessageCommand.Execute(null);

            // Assert
            Assert.NotNull(eventArgs);
            Assert.Equal(ChatType.Private, eventArgs.ChatType);
        }

        [Fact]
        public void SendMessageCommand_WithNoSelection_ShouldNotRaiseEvent()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            viewModel.IsChatEnabled = true;
            viewModel.SelectedChatItem = null;

            var eventRaised = false;
            
            viewModel.SendMessageRequested += (sender, args) =>
            {
                eventRaised = true;
            };

            // Act
            viewModel.SendMessageCommand.Execute(null);

            // Assert
            Assert.False(eventRaised);
        }

        #endregion

        #region ClearMessageRequested 事件测试

        [Fact]
        public void ClearMessageCommand_WhenExecuted_ShouldRaiseClearMessageRequestedEvent()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            viewModel.IsChatEnabled = true;
            viewModel.SelectedChatItem = new ChatListItemViewModel { Id = 12345 };

            var eventRaised = false;
            
            viewModel.ClearMessageRequested += (sender, args) =>
            {
                eventRaised = true;
            };

            // Act
            viewModel.ClearMessageCommand.Execute(null);

            // Assert
            Assert.True(eventRaised);
        }

        #endregion

        #region ClearSendBoxRequested 事件测试

        [Fact]
        public void ClearSendBoxCommand_WhenExecuted_ShouldRaiseClearSendBoxRequestedEvent()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            viewModel.IsChatEnabled = true;
            viewModel.SelectedChatItem = new ChatListItemViewModel { Id = 12345 };

            var eventRaised = false;
            
            viewModel.ClearSendBoxRequested += (sender, args) =>
            {
                eventRaised = true;
            };

            // Act
            viewModel.ClearSendBoxCommand.Execute(null);

            // Assert
            Assert.True(eventRaised);
        }

        #endregion

        #region ScrollToBottomRequested 事件测试

        [Fact]
        public void ScrollToBottomCommand_WhenExecuted_ShouldRaiseScrollToBottomRequestedEvent()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();

            var eventRaised = false;
            
            viewModel.ScrollToBottomRequested += (sender, args) =>
            {
                eventRaised = true;
            };

            // Act
            viewModel.ScrollToBottomCommand.Execute(null);

            // Assert
            Assert.True(eventRaised);
        }

        #endregion

        #region ShowAtSelectorRequested 事件测试

        [Fact]
        public void ShowAtSelectorCommand_WithGroupChat_ShouldRaiseShowAtSelectorRequestedEvent()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            viewModel.IsChatEnabled = true;
            viewModel.SelectedChatItem = new ChatListItemViewModel
            {
                Id = 12345,
                AvatarType = ChatAvatar.AvatarTypes.QQGroup
            };

            var eventRaised = false;
            
            viewModel.ShowAtSelectorRequested += (sender, args) =>
            {
                eventRaised = true;
            };

            // Act
            if (viewModel.ShowAtSelectorCommand.CanExecute(null))
            {
                viewModel.ShowAtSelectorCommand.Execute(null);
            }

            // Assert
            Assert.True(eventRaised);
        }

        #endregion

        #region SelectPictureRequested 事件测试

        [Fact]
        public void SelectPictureCommand_WhenExecuted_ShouldRaiseSelectPictureRequestedEvent()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            viewModel.IsChatEnabled = true;
            viewModel.SelectedChatItem = new ChatListItemViewModel { Id = 12345 };

            var eventRaised = false;
            
            viewModel.SelectPictureRequested += (sender, args) =>
            {
                eventRaised = true;
            };

            // Act
            if (viewModel.SelectPictureCommand.CanExecute(null))
            {
                viewModel.SelectPictureCommand.Execute(null);
            }

            // Assert
            Assert.True(eventRaised);
        }

        #endregion

        #region SelectAudioRequested 事件测试

        [Fact]
        public void SelectAudioCommand_WhenExecuted_ShouldRaiseSelectAudioRequestedEvent()
        {
            // Arrange
            var viewModel = new ChatPageViewModel();
            viewModel.IsChatEnabled = true;
            viewModel.SelectedChatItem = new ChatListItemViewModel { Id = 12345 };

            var eventRaised = false;
            
            viewModel.SelectAudioRequested += (sender, args) =>
            {
                eventRaised = true;
            };

            // Act
            if (viewModel.SelectAudioCommand.CanExecute(null))
            {
                viewModel.SelectAudioCommand.Execute(null);
            }

            // Assert
            Assert.True(eventRaised);
        }

        #endregion
    }
}
