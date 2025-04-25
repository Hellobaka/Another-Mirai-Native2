using Another_Mirai_Native.Model;

namespace Another_Mirai_Native
{
    /// <summary>
    /// 协议需实现的接口
    /// </summary>
    public interface IProtocol
    {
        /// <summary>
        /// 协议名称，通常指代协议连接端的名称 例如:
        /// <example>MiraiAPIHTTP</example>
        /// <example>NoneBot</example>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 与协议连接端进行连接动作
        /// </summary>
        /// <returns>是否成功</returns>
        public bool Connect();

        /// <summary>
        /// 连接是否依旧正常
        /// </summary>
        public bool IsConnected { get; set; }

        /// <summary>
        /// 主动与协议连接端断开连接, 通常指示着要切换协议, 此时应当中断所有线程以及定时器
        /// </summary>
        /// <returns>是否断开成功</returns>
        public bool Disconnect();

        /// <summary>
        /// 获取连接参数, <see cref="KeyValuePair{TKey, TValue}">Item</see> 表示每个连接参数, Key为参数名称, Value为参数的值.
        /// 参数的值可以将保存的值填入
        /// 支持 Bool 参数，将 Key 设置为 "bool_" 开头即可，Value 推荐使用小写的 "true" 或 "false"
        /// </summary>
        /// <returns>连接参数</returns>
        public Dictionary<string, string> GetConnectionConfig();

        /// <summary>
        /// 设置连接参数, 检查传入的 <see cref="Dictionary{TKey, TValue}"><paramref name="config"/></see> 的每一项是否符合要求
        /// 若符合要求便保存并应用参数
        /// </summary>
        /// <param name="config">欲设置的连接参数</param>
        /// <returns>是否检查通过</returns>
        public bool SetConnectionConfig(Dictionary<string, string> config);

        /// <summary>
        /// 协议是否支持发送图片
        /// </summary>
        /// <returns>1为 <see cref="bool">True</see>; 0为 <see cref="bool">False</see></returns>
        public int CanSendImage();

        /// <summary>
        /// 协议是否支持发送语音
        /// </summary>
        /// <returns>1为 <see cref="bool">True</see>; 0为 <see cref="bool">False</see></returns>
        public int CanSendRecord();

        /// <summary>
        /// 撤回消息
        /// </summary>
        /// <param name="msgId">消息 ID</param>
        /// <returns>1为 <see cref="bool">成功</see>; 0为 <see cref="bool">失败</see></returns>
        public int DeleteMsg(long msgId);

        /// <summary>
        /// 获取指定域名的 Cookie
        /// </summary>
        /// <param name="domain">指定域名</param>
        /// <returns>Cookie</returns>
        public string GetCookies(string domain);

        /// <summary>
        /// 获取 CsrfToken
        /// </summary>
        /// <returns>CsrfToken</returns>
        public string GetCsrfToken();

        /// <summary>
        /// 获取序列化后的好友列表
        /// </summary>
        /// <param name="reserved">是否倒序</param>
        /// <returns>序列化后的好友列表</returns>
        public string GetFriendList(bool reserved);

        /// <summary>
        /// 获取序列化后的群组信息
        /// </summary>
        /// <param name="groupId">群 ID</param>
        /// <param name="notCache">不使用缓存</param>
        /// <returns>序列化后的群组信息</returns>
        public string GetGroupInfo(long groupId, bool notCache);

        /// <summary>
        /// 获取序列化后的群组列表
        /// </summary>
        /// <returns>序列化后的群组列表</returns>
        public string GetGroupList();

        /// <summary>
        /// 获取序列化后的群成员信息
        /// </summary>
        /// <param name="groupId">群 ID</param>
        /// <param name="qqId">成员 ID</param>
        /// <param name="isCache">是否使用缓存</param>
        /// <returns>序列化后的群成员信息</returns>
        public string GetGroupMemberInfo(long groupId, long qqId, bool isCache);

        /// <summary>
        /// 获取序列化后的群成员列表
        /// </summary>
        /// <param name="groupId">群 ID</param>
        /// <returns>序列化后的群成员列表</returns>
        public string GetGroupMemberList(long groupId);

        /// <summary>
        /// 获取原始好友列表
        /// </summary>
        /// <param name="reserved">是否倒序</param>
        /// <returns>好友列表</returns>
        public List<FriendInfo> GetRawFriendList(bool reserved);

        /// <summary>
        /// 获取原始群组信息
        /// </summary>
        /// <param name="groupId">群 ID</param>
        /// <param name="notCache">不使用缓存</param>
        /// <returns>群组信息</returns>
        public GroupInfo GetRawGroupInfo(long groupId, bool notCache);

        /// <summary>
        /// 获取原始群组列表
        /// </summary>
        /// <returns>群组列表</returns>
        public List<GroupInfo> GetRawGroupList();

        /// <summary>
        /// 获取原始群成员信息
        /// </summary>
        /// <param name="groupId">群 ID</param>
        /// <param name="qqId">成员 ID</param>
        /// <param name="isCache">是否使用缓存</param>
        /// <returns>群成员信息</returns>
        public GroupMemberInfo GetRawGroupMemberInfo(long groupId, long qqId, bool isCache);

        /// <summary>
        /// 获取群成员列表
        /// </summary>
        /// <param name="groupId">群 ID</param>
        /// <returns>原始群成员列表</returns>
        public List<GroupMemberInfo> GetRawGroupMemberList(long groupId);

        /// <summary>
        /// 获取登录实例的账号昵称
        /// </summary>
        /// <returns>账号昵称</returns>
        public string GetLoginNick();

        /// <summary>
        /// 获取登录实例的账号 ID
        /// </summary>
        /// <returns>账号 ID</returns>
        public long GetLoginQQ();

        /// <summary>
        /// 获取序列后的陌生人信息
        /// </summary>
        /// <param name="qqId">陌生人 ID</param>
        /// <param name="notCache">强制缓存</param>
        /// <returns>序列后的陌生人信息</returns>
        public string GetStrangerInfo(long qqId, bool notCache);

        /// <summary>
        /// 发送群组信息
        /// </summary>
        /// <param name="groupId">群 ID</param>
        /// <param name="msg">欲发送的消息</param>
        /// <param name="msgId">引用消息 ID; 当 ID = 0 时表示不引用发送</param>
        /// <returns>消息ID 失败时返回0</returns>
        public int SendGroupMessage(long groupId, string msg, int msgId = 0);

        /// <summary>
        /// 发送名片赞
        /// </summary>
        /// <param name="qqId">欲发送的 ID</param>
        /// <param name="count">发送数量</param>
        /// <returns>0为 <see cref="bool">成功</see>; 1为 <see cref="bool">失败</see></returns>
        public int SendLike(long qqId, int count);

        /// <summary>
        /// 发送单聊信息
        /// </summary>
        /// <param name="qqId">群 ID</param>
        /// <param name="msg">欲发送的消息</param>
        /// <returns>消息ID 失败时返回0</returns>
        public int SendPrivateMessage(long qqId, string msg);

        /// <summary>
        /// 发送讨论组信息
        /// </summary>
        /// <param name="discussId">讨论组 ID</param>
        /// <param name="msg">欲发送的消息</param>
        /// <returns>消息ID 失败时返回0</returns>
        public int SendDiscussMsg(long discussId, string msg);

        /// <summary>
        /// 主动离开讨论组
        /// </summary>
        /// <param name="discussId">讨论组 ID</param>
        /// <returns>0为 <see cref="bool">成功</see>; 1为 <see cref="bool">失败</see></returns>
        public int SetDiscussLeave(long discussId);

        /// <summary>
        /// 处理好友添加请求
        /// </summary>
        /// <param name="identifying">请求标识</param>
        /// <param name="responseType">处理结果: 1 为通过; 2 为拒绝</param>
        /// <param name="appendMsg">备注消息</param>
        /// <returns>0为 <see cref="bool">操作成功</see>; 1为 <see cref="bool">操作失败</see></returns>
        public int SetFriendAddRequest(string identifying, int responseType, string appendMsg);

        /// <summary>
        /// 处理群组添加请求
        /// </summary>
        /// <param name="identifying">请求标识</param>
        /// <param name="requestType">添加类型: 1 为主动进群; 2 为邀请进群</param>
        /// <param name="responseType">处理结果: 1 为通过; 2 为拒绝</param>
        /// <param name="appendMsg">备注消息</param>
        /// <returns>0为 <see cref="bool">操作成功</see>; 1为 <see cref="bool">操作失败</see></returns>
        public int SetGroupAddRequest(string identifying, int requestType, int responseType, string appendMsg);

        /// <summary>
        /// 设置群管理
        /// </summary>
        /// <param name="groupId">群组 ID</param>
        /// <param name="qqId">被操作者 ID</param>
        /// <param name="isSet"><see cref="bool">True</see> 为设置; <see cref="bool">False</see> 为罢免</param>
        /// <returns>0为 <see cref="bool">操作成功</see>; 1为 <see cref="bool">操作失败</see></returns>
        public int SetGroupAdmin(long groupId, long qqId, bool isSet);

        /// <summary>
        /// 设置群组是否开启匿名
        /// </summary>
        /// <param name="groupId">群组 ID</param>
        /// <param name="isOpen">是否开启匿名</param>
        /// <returns>0为 <see cref="bool">操作成功</see>; 1为 <see cref="bool">操作失败</see></returns>
        public int SetGroupAnonymous(long groupId, bool isOpen);

        /// <summary>
        /// 禁言群匿名成员
        /// </summary>
        /// <param name="groupId">群组 ID</param>
        /// <param name="anonymous">匿名 ID</param>
        /// <param name="banTime">禁言时长 (单位: 秒)</param>
        /// <returns>0为 <see cref="bool">操作成功</see>; 1为 <see cref="bool">操作失败</see></returns>
        public int SetGroupAnonymousBan(long groupId, string anonymous, long banTime);

        /// <summary>
        /// 禁言群成员
        /// </summary>
        /// <param name="groupId">群组 ID</param>
        /// <param name="qqId">成员 ID</param>
        /// <param name="banTime">禁言时长 (单位: 秒)</param>
        /// <returns>0为 <see cref="bool">操作成功</see>; 1为 <see cref="bool">操作失败</see></returns>
        public int SetGroupBan(long groupId, long qqId, long banTime);

        /// <summary>
        /// 设置群组成员名片
        /// </summary>
        /// <param name="groupId">群组 ID</param>
        /// <param name="qqId">成员 ID</param>
        /// <param name="newCard">新名片</param>
        /// <returns>0为 <see cref="bool">操作成功</see>; 1为 <see cref="bool">操作失败</see></returns>
        public int SetGroupCard(long groupId, long qqId, string newCard);

        /// <summary>
        /// 移除群组成员
        /// </summary>
        /// <param name="groupId">群组 ID</param>
        /// <param name="qqId">成员 ID</param>
        /// <param name="refuses">是否拒绝后续入群</param>
        /// <returns>0为 <see cref="bool">操作成功</see>; 1为 <see cref="bool">操作失败</see></returns>
        public int SetGroupKick(long groupId, long qqId, bool refuses);

        /// <summary>
        /// 主动离开群组
        /// </summary>
        /// <param name="groupId">群组 ID</param>
        /// <param name="isDisband"><see cref="bool">True</see> 为解散; <see cref="bool">False</see> 为离开</param>
        /// <returns>0为 <see cref="bool">操作成功</see>; 1为 <see cref="bool">操作失败</see></returns>
        public int SetGroupLeave(long groupId, bool isDisband);

        /// <summary>
        /// 设置群组成员头衔
        /// </summary>
        /// <param name="groupId">群组 ID</param>
        /// <param name="qqId">成员 ID</param>
        /// <param name="title">头衔</param>
        /// <param name="durationTime">过期时间</param>
        /// <returns>0为 <see cref="bool">操作成功</see>; 1为 <see cref="bool">操作失败</see></returns>
        public int SetGroupSpecialTitle(long groupId, long qqId, string title, long durationTime);

        /// <summary>
        /// 设置群组全员禁言
        /// </summary>
        /// <param name="groupId">群组 ID</param>
        /// <param name="isDisband"><see cref="bool">True</see> 为开启; <see cref="bool">False</see> 为关闭</param>
        /// <returns>0为 <see cref="bool">操作成功</see>; 1为 <see cref="bool">操作失败</see></returns>
        public int SetGroupWholeBan(long groupId, bool isOpen);

        /// <summary>
        /// 二维码显示，禁止阻塞
        /// Url, 二维码图片buffer
        /// </summary>
        public event Action<string, byte[]> QRCodeDisplayAction;

        /// <summary>
        /// 二维码完成，关闭显示，禁止阻塞
        /// </summary>
        public event Action QRCodeFinishedAction;

        public event Action OnProtocolOnline;

        public event Action OnProtocolOffline;
    }
}