using Another_Mirai_Native.UI.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Another_Mirai_Native.UI.ViewModel
{
    public class ChatListItemViewModel : INotifyPropertyChanged
    {
        public void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }

        public ChatListItemViewModel()
        {
            CreateRelayCommands();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ChatType AvatarType { get; set; } = ChatType.Fallback;

        public string Detail { get; set; } = "";

        public string GroupName { get; set; } = "";

        public long Id { get; set; }

        public DateTime Time { get; set; }

        public int UnreadCount { get; set; }

        public bool UnreadBadgeVisible => UnreadCount > 0;

        #region Commands

        public RelayCommand Command_ChatList_CopyNick { get; set; }

        public RelayCommand Command_ChatList_CopyId { get; set; }

        #endregion

        #region ContextMenu
        public void ChatList_CopyNick(object? parameter)
        {
            Clipboard.SetText(GroupName);
        }

        public void ChatList_CopyId(object? parameter)
        {
            Clipboard.SetText(Id.ToString());
        }

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
    }
}