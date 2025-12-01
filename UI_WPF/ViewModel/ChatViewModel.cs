using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.UI.Controls.Chat;
using Another_Mirai_Native.UI.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;

namespace Another_Mirai_Native.UI.ViewModel
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<string>? OnTextAddRequested;
        public event Action<int>? OnMessageJumpRequested;
        public event Action? OnScrollToBottomRequested;

        public void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ChatViewModel()
        {
            CreateRelayCommands();
            Instance = this;
        }

        public static ChatViewModel Instance { get; set; }

        /// <summary>
        /// 侧边栏列表
        /// </summary>
        public ObservableCollection<ChatListItemViewModel> ChatList { get; set; } = [];

        /// <summary>
        /// 消息容器列表
        /// </summary>
        public ObservableCollection<MessageViewModel> Messages { get; set; } = [];

        public ChatListItemViewModel? SelectedChat { get; set; }

        public FlowDocument SendText { get; set; } = new();

        public bool IsGroupChat => SelectedChat != null && SelectedChat.AvatarType == AvatarTypes.QQGroup;

        public bool EmptyHintVisible => ChatList.Count == 0 || SelectedChat == null;

        public bool Avatar_IsAtEnabled { get; set; }

        public bool Message_IsAtEnabled { get; set; }

        public bool LazyLoading { get; set; }

        #region Commands

        public RelayCommand Command_ToolPicture { get; set; }

        public RelayCommand Command_ToolAudio { get; set; }

        public RelayCommand Command_ToolClear { get; set; }

        public RelayCommand Command_ToolSendText { get; set; }

        #endregion

        private void CreateRelayCommands()
        {
            var fields = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(RelayCommand) && p.Name.StartsWith("Command_"));

            foreach (var prop in fields)
            {
                string methodName = prop.Name.Replace("Command_", "");
                var methodInfo = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (methodInfo != null)
                {
                    // 创建委托
                    Action<object> action = (obj) => methodInfo.Invoke(this, [obj]);
                    var cmd = new RelayCommand(action);
                    prop.SetValue(this, cmd);
                }
                else
                {
                    // 没有找到方法时
                    throw new Exception($"方法 {methodName} 未定义!");
                }
            }
        }

        /// <summary>
        /// 向发送框中添加文本
        /// </summary>
        /// <param name="text">添加的文本</param>
        public void AddTextToSendBox(string text)
        {
            OnTextAddRequested?.Invoke(text);
        }

        public void ToolPicture(object? parameter)
        {
            OpenFileDialog openFileDialog = new()
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.webp|所有文件|*.*",
                Title = "请选择要发送的图片"
            };
            if (openFileDialog.ShowDialog() is false)
            {
                return;
            }
            foreach (var file in openFileDialog.FileNames)
            {
                string filePath = file;
                string picPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image\cached");
                Directory.CreateDirectory(picPath);
                if (filePath.StartsWith(picPath))
                {
                    // 选中图片已经存在于缓存文件夹
                    filePath = filePath.Replace(picPath, "");
                }
                else
                {
                    // 复制至缓存文件夹
                    string fileName = Path.GetFileName(filePath);
                    File.Copy(filePath, Path.Combine(picPath, fileName), true);
                    filePath = @$"cached\\{fileName}";
                }
                AddTextToSendBox(CQCode.CQCode_Image(filePath).ToSendString());
            }
        }

        public void ToolClear(object? parameter)
        {
            Messages.Clear();
        }

        public async Task ToolSendText(object? parameter)
        {
            if (SelectedChat == null)
            {
                return;
            }
            string sendText = BuildTextFromRichTextBox();
            AvatarTypes avatar = SelectedChat.AvatarType;
            long id = SelectedChat.Id;
            await ExecuteSendMessageAsync(id, avatar, sendText);
            // 清空发送框
            SendText.Blocks.Clear();
        }

        public async Task ExecuteSendMessageAsync(long id, AvatarTypes avatar, string sendText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 将历史转换为消息模型
        /// </summary>
        /// <param name="avatarType">消息来源</param>
        /// <param name="history">聊天历史</param>
        /// <returns>消息模型</returns>
        public async static Task<MessageViewModel> ParseChatHistoryToViewModel(AvatarTypes avatarType, ChatHistory history)
        {
            return new MessageViewModel
            {
                AvatarType = avatarType,
                Content = history.Message,
                DetailItemType = history.Type == ChatHistoryType.Notice ? DetailItemType.Notice : (history.SenderID == AppConfig.Instance.CurrentQQ ? DetailItemType.Send : DetailItemType.Receive),
                Id = history.SenderID,
                ParentId = history.ParentID,
                MsgId = history.MsgId,
                Nick = (avatarType == AvatarTypes.QQPrivate ? await Caches.GetFriendNick(history.SenderID) : await Caches.GetGroupMemberNick(history.ParentID, history.SenderID))
                     + (string.IsNullOrEmpty(history.PluginName) ? "" : $" [{history.PluginName}]"),
                Recalled = history.Recalled,
                Time = history.Time,
                SqlId = history.ID
            };
        }

        public void ToolAudio(object? parameter)
        {
            OpenFileDialog openFileDialog = new()
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                Filter = "音频文件|*.wav;*.mp3;*.flac;*.amr;*.m4a|所有文件|*.*",
                Title = "请选择要发送的音频"
            };
            if (openFileDialog.ShowDialog() is false)
            {
                return;
            }
            string filePath = openFileDialog.FileName;
            string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\record\cached");
            Directory.CreateDirectory(audioPath);
            // 选择的文件在缓存文件夹中
            if (filePath.StartsWith(audioPath))
            {
                filePath = filePath.Replace(audioPath, "");
            }
            else
            {
                string fileName = Path.GetFileName(filePath);
                // 复制文件到缓存文件夹中
                File.Copy(filePath, Path.Combine(audioPath, fileName), true);
                filePath = @$"cached\\{fileName}";
            }
            AddTextToSendBox(CQCode.CQCode_Record(filePath).ToSendString());
        }

        public void ScrollToBottom()
        {
            OnScrollToBottomRequested?.Invoke();
        }

        public void JumpToMessage(int msgId)
        {
            OnMessageJumpRequested?.Invoke(msgId);
        }

        /// <summary>
        /// 发送消息转CQ码
        /// </summary>
        /// <returns>处理后的CQ码消息</returns>
        private string BuildTextFromRichTextBox()
        {
            StringBuilder stringBuilder = new();
            foreach (Block item in SendText.Blocks)
            {
                // 粘贴的图片
                if (item is BlockUIContainer blockImgContainer && blockImgContainer.Child is Image blockImg)
                {
                    stringBuilder.Append(blockImg.Tag?.ToString());
                    continue;
                }
                if (item is not Paragraph paragraph)
                {
                    continue;
                }
                foreach (Inline inline in paragraph.Inlines)
                {
                    if (inline is InlineUIContainer uiContainer && uiContainer.Child is Image inlineImage)
                    {
                        stringBuilder.Append(inlineImage.Tag?.ToString());
                    }
                    else
                    {
                        stringBuilder.Append(new TextRange(inline.ContentStart, inline.ContentEnd).Text);
                    }
                }
            }
            return stringBuilder.ToString();
        }
    }
}
