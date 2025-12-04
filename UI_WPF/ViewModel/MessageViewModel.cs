using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.UI.Controls;
using Another_Mirai_Native.UI.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Another_Mirai_Native.UI.ViewModel
{
    public class MessageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MessageViewModel()
        {
            CreateRelayCommands();
        }

        public ChatType AvatarType { get; set; } = ChatType.Fallback;

        // TODO: 留意如何赋值
        public ChatType ParentAvatarType { get; set; } = ChatType.Fallback;

        public string Content { get; set; } = "";

        public DetailItemType DetailItemType { get; set; } = DetailItemType.Send;

        public QQGroupMemberType GroupMemberType { get; set; } = QQGroupMemberType.Member;

        public long Id { get; set; }

        // TODO: 留意如何赋值
        public long ParentId { get; set; }

        public string Nick { get; set; } = "";

        public DateTime Time { get; set; } = DateTime.Now;

        public string GUID { get; set; } = Guid.NewGuid().ToString();

        public bool Recalled { get; set; }

        public int MsgId { get; set; }

        public int SqlId { get; set; }

        public MessageStatus MessageStatus { get; set; } = MessageStatus.Sent;

        #region Commands
        public RelayCommand Command_Message_Copy { get; set; }

        public RelayCommand Command_Message_Repeat { get; set; }

        public RelayCommand Command_Message_At { get; set; }

        public RelayCommand Command_Message_Reply { get; set; }

        public RelayCommand Command_Message_Recall { get; set; }

        public RelayCommand Command_Avatar_CopyNick { get; set; }

        public RelayCommand Command_Avatar_CopyId { get; set; }

        public RelayCommand Command_Avatar_At { get; set; }
        #endregion

        #region ContextMenu
        public void Message_Copy(object? parameter)
        {
            Clipboard.SetText(Content);
        }

        public async void Message_Repeat(object? parameter)
        {
            await ChatViewModel.Instance.ExecuteSendMessageAsync(ParentId, AvatarType, Content);
        }

        public void Message_At(object? parameter)
        {
            ChatViewModel.Instance.AddTextToSendBox(CQCode.CQCode_At(Id).ToSendString());
        }

        public void Message_Reply(object? parameter)
        {
            ChatViewModel.Instance.AddTextToSendBox($"[CQ:reply,id={MsgId}]");
        }

        public async void Message_Recall(object? parameter)
        {
            await Task.Run(() => ProtocolManager.Instance.CurrentProtocol.DeleteMsg(MsgId));
        }

        public void Avatar_CopyNick(object? parameter)
        {
            Clipboard.SetText(Nick);
        }

        public void Avatar_CopyId(object? parameter)
        {
            Clipboard.SetText(Id.ToString());
        }

        public void Avatar_At(object? parameter)
        {
            ChatViewModel.Instance.AddTextToSendBox(CQCode.CQCode_At(Id).ToSendString());
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