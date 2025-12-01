using Another_Mirai_Native.UI.Controls;
using Another_Mirai_Native.UI.ViewModel;
using System.ComponentModel;
using Xunit;

namespace Another_Mirai_Native.UI.Tests.ViewModels
{
    /// <summary>
    /// ToolbarViewModel 单元测试
    /// </summary>
    public class ToolbarViewModelTests
    {
        #region 构造函数测试

        [Fact]
        public void Constructor_ShouldInitializeWithAllButtonsDisabled()
        {
            // Arrange & Act
            var viewModel = new ToolbarViewModel();

            // Assert
            Assert.False(viewModel.IsFaceEnabled);
            Assert.False(viewModel.IsAtEnabled);
            Assert.False(viewModel.IsPictureEnabled);
            Assert.False(viewModel.IsAudioEnabled);
            Assert.False(viewModel.IsSendEnabled);
            Assert.False(viewModel.IsClearMessageEnabled);
            Assert.False(viewModel.IsClearSendEnabled);
        }

        #endregion

        #region UpdateButtonStates 测试

        [Fact]
        public void UpdateButtonStates_WithNull_ShouldDisableAllButtons()
        {
            // Arrange
            var viewModel = new ToolbarViewModel();
            // 先启用所有按钮
            viewModel.UpdateButtonStates(ChatAvatar.AvatarTypes.QQGroup);
            
            // Act
            viewModel.UpdateButtonStates(null);

            // Assert
            Assert.False(viewModel.IsFaceEnabled);
            Assert.False(viewModel.IsAtEnabled);
            Assert.False(viewModel.IsPictureEnabled);
            Assert.False(viewModel.IsAudioEnabled);
            Assert.False(viewModel.IsSendEnabled);
            Assert.False(viewModel.IsClearMessageEnabled);
            Assert.False(viewModel.IsClearSendEnabled);
        }

        [Fact]
        public void UpdateButtonStates_WithQQGroup_ShouldEnableAllButtons()
        {
            // Arrange
            var viewModel = new ToolbarViewModel();

            // Act
            viewModel.UpdateButtonStates(ChatAvatar.AvatarTypes.QQGroup);

            // Assert
            Assert.True(viewModel.IsFaceEnabled);
            Assert.True(viewModel.IsAtEnabled);
            Assert.True(viewModel.IsPictureEnabled);
            Assert.True(viewModel.IsAudioEnabled);
            Assert.True(viewModel.IsSendEnabled);
            Assert.True(viewModel.IsClearMessageEnabled);
            Assert.True(viewModel.IsClearSendEnabled);
        }

        [Fact]
        public void UpdateButtonStates_WithQQPrivate_ShouldEnableAllButtonsExceptAt()
        {
            // Arrange
            var viewModel = new ToolbarViewModel();

            // Act
            viewModel.UpdateButtonStates(ChatAvatar.AvatarTypes.QQPrivate);

            // Assert
            Assert.True(viewModel.IsFaceEnabled);
            Assert.False(viewModel.IsAtEnabled); // At按钮在私聊时禁用
            Assert.True(viewModel.IsPictureEnabled);
            Assert.True(viewModel.IsAudioEnabled);
            Assert.True(viewModel.IsSendEnabled);
            Assert.True(viewModel.IsClearMessageEnabled);
            Assert.True(viewModel.IsClearSendEnabled);
        }

        #endregion

        #region PropertyChanged 事件测试

        [Fact]
        public void IsFaceEnabled_WhenChanged_ShouldRaisePropertyChanged()
        {
            // Arrange
            var viewModel = new ToolbarViewModel();
            var propertyChangedRaised = false;
            string? changedPropertyName = null;
            
            viewModel.PropertyChanged += (sender, e) =>
            {
                propertyChangedRaised = true;
                changedPropertyName = e.PropertyName;
            };

            // Act
            viewModel.IsFaceEnabled = true;

            // Assert
            Assert.True(propertyChangedRaised);
            Assert.Equal(nameof(ToolbarViewModel.IsFaceEnabled), changedPropertyName);
        }

        [Fact]
        public void IsAtEnabled_WhenChanged_ShouldRaisePropertyChanged()
        {
            // Arrange
            var viewModel = new ToolbarViewModel();
            var propertyChangedRaised = false;
            string? changedPropertyName = null;
            
            viewModel.PropertyChanged += (sender, e) =>
            {
                propertyChangedRaised = true;
                changedPropertyName = e.PropertyName;
            };

            // Act
            viewModel.IsAtEnabled = true;

            // Assert
            Assert.True(propertyChangedRaised);
            Assert.Equal(nameof(ToolbarViewModel.IsAtEnabled), changedPropertyName);
        }

        [Fact]
        public void IsPictureEnabled_WhenChanged_ShouldRaisePropertyChanged()
        {
            // Arrange
            var viewModel = new ToolbarViewModel();
            var propertyChangedRaised = false;
            string? changedPropertyName = null;
            
            viewModel.PropertyChanged += (sender, e) =>
            {
                propertyChangedRaised = true;
                changedPropertyName = e.PropertyName;
            };

            // Act
            viewModel.IsPictureEnabled = true;

            // Assert
            Assert.True(propertyChangedRaised);
            Assert.Equal(nameof(ToolbarViewModel.IsPictureEnabled), changedPropertyName);
        }

        [Fact]
        public void IsAudioEnabled_WhenChanged_ShouldRaisePropertyChanged()
        {
            // Arrange
            var viewModel = new ToolbarViewModel();
            var propertyChangedRaised = false;
            string? changedPropertyName = null;
            
            viewModel.PropertyChanged += (sender, e) =>
            {
                propertyChangedRaised = true;
                changedPropertyName = e.PropertyName;
            };

            // Act
            viewModel.IsAudioEnabled = true;

            // Assert
            Assert.True(propertyChangedRaised);
            Assert.Equal(nameof(ToolbarViewModel.IsAudioEnabled), changedPropertyName);
        }

        [Fact]
        public void IsSendEnabled_WhenChanged_ShouldRaisePropertyChanged()
        {
            // Arrange
            var viewModel = new ToolbarViewModel();
            var propertyChangedRaised = false;
            string? changedPropertyName = null;
            
            viewModel.PropertyChanged += (sender, e) =>
            {
                propertyChangedRaised = true;
                changedPropertyName = e.PropertyName;
            };

            // Act
            viewModel.IsSendEnabled = true;

            // Assert
            Assert.True(propertyChangedRaised);
            Assert.Equal(nameof(ToolbarViewModel.IsSendEnabled), changedPropertyName);
        }

        [Fact]
        public void IsClearMessageEnabled_WhenChanged_ShouldRaisePropertyChanged()
        {
            // Arrange
            var viewModel = new ToolbarViewModel();
            var propertyChangedRaised = false;
            string? changedPropertyName = null;
            
            viewModel.PropertyChanged += (sender, e) =>
            {
                propertyChangedRaised = true;
                changedPropertyName = e.PropertyName;
            };

            // Act
            viewModel.IsClearMessageEnabled = true;

            // Assert
            Assert.True(propertyChangedRaised);
            Assert.Equal(nameof(ToolbarViewModel.IsClearMessageEnabled), changedPropertyName);
        }

        [Fact]
        public void IsClearSendEnabled_WhenChanged_ShouldRaisePropertyChanged()
        {
            // Arrange
            var viewModel = new ToolbarViewModel();
            var propertyChangedRaised = false;
            string? changedPropertyName = null;
            
            viewModel.PropertyChanged += (sender, e) =>
            {
                propertyChangedRaised = true;
                changedPropertyName = e.PropertyName;
            };

            // Act
            viewModel.IsClearSendEnabled = true;

            // Assert
            Assert.True(propertyChangedRaised);
            Assert.Equal(nameof(ToolbarViewModel.IsClearSendEnabled), changedPropertyName);
        }

        #endregion

        #region 值不变时不触发事件测试

        [Fact]
        public void IsFaceEnabled_WhenSetToSameValue_ShouldNotRaisePropertyChanged()
        {
            // Arrange
            var viewModel = new ToolbarViewModel();
            viewModel.IsFaceEnabled = false;
            var propertyChangedRaised = false;
            
            viewModel.PropertyChanged += (sender, e) =>
            {
                propertyChangedRaised = true;
            };

            // Act
            viewModel.IsFaceEnabled = false;

            // Assert
            Assert.False(propertyChangedRaised);
        }

        [Fact]
        public void IsAtEnabled_WhenSetToSameValue_ShouldNotRaisePropertyChanged()
        {
            // Arrange
            var viewModel = new ToolbarViewModel();
            viewModel.IsAtEnabled = false;
            var propertyChangedRaised = false;
            
            viewModel.PropertyChanged += (sender, e) =>
            {
                propertyChangedRaised = true;
            };

            // Act
            viewModel.IsAtEnabled = false;

            // Assert
            Assert.False(propertyChangedRaised);
        }

        #endregion

        #region 状态切换测试

        [Fact]
        public void UpdateButtonStates_SwitchingFromGroupToPrivate_ShouldDisableAtButton()
        {
            // Arrange
            var viewModel = new ToolbarViewModel();
            viewModel.UpdateButtonStates(ChatAvatar.AvatarTypes.QQGroup);
            Assert.True(viewModel.IsAtEnabled);

            // Act
            viewModel.UpdateButtonStates(ChatAvatar.AvatarTypes.QQPrivate);

            // Assert
            Assert.False(viewModel.IsAtEnabled);
            Assert.True(viewModel.IsFaceEnabled); // 其他按钮仍然启用
        }

        [Fact]
        public void UpdateButtonStates_SwitchingFromPrivateToGroup_ShouldEnableAtButton()
        {
            // Arrange
            var viewModel = new ToolbarViewModel();
            viewModel.UpdateButtonStates(ChatAvatar.AvatarTypes.QQPrivate);
            Assert.False(viewModel.IsAtEnabled);

            // Act
            viewModel.UpdateButtonStates(ChatAvatar.AvatarTypes.QQGroup);

            // Assert
            Assert.True(viewModel.IsAtEnabled);
        }

        [Fact]
        public void UpdateButtonStates_SwitchingFromGroupToNull_ShouldDisableAllButtons()
        {
            // Arrange
            var viewModel = new ToolbarViewModel();
            viewModel.UpdateButtonStates(ChatAvatar.AvatarTypes.QQGroup);
            Assert.True(viewModel.IsFaceEnabled);

            // Act
            viewModel.UpdateButtonStates(null);

            // Assert
            Assert.False(viewModel.IsFaceEnabled);
            Assert.False(viewModel.IsAtEnabled);
            Assert.False(viewModel.IsPictureEnabled);
            Assert.False(viewModel.IsAudioEnabled);
            Assert.False(viewModel.IsSendEnabled);
        }

        #endregion
    }
}
