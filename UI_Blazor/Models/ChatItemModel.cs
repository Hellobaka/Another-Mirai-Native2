namespace Another_Mirai_Native.BlazorUI.Models
{
    public enum DetailItemType
    {
        Notice,

        Receive,

        Send
    }

    public enum AvatarTypes
    {
        QQGroup,

        QQPrivate,

        Fallback
    }

    public class ChatItemModel
    {
        /// <summary>
        /// 消息位置
        /// </summary>
        public DetailItemType DetailItemType { get; set; }

        /// <summary>
        /// 显示的名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 消息块的GUID
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// 消息所属的QQ
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 未转换的消息内容
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// 消息ID
        /// </summary>
        public int MsgId { get; set; }

        /// <summary>
        /// 消息所属的组ID 群号或QQ
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 消息来源
        /// </summary>
        public AvatarTypes ParentType { get; set; } = AvatarTypes.Fallback;

        /// <summary>
        /// 是否已撤回
        /// </summary>
        public bool Recalled { get; set; }

        /// <summary>
        /// 显示的时间
        /// </summary>
        public DateTime Time { get; set; }

        public bool Sending { get; set; }

        public bool Failed => MsgId == 0;

        public int HistoryId { get; set; }

        public AvatarModel Avatar { get; set; } = new AvatarModel();
    }
}
