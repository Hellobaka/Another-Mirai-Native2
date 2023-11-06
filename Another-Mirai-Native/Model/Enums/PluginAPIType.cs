namespace Another_Mirai_Native.Model.Enums
{
    public enum PluginAPIType
    {
        /// <summary>
        /// [敏感]取Cookies
        /// </summary>
        getCookies = 20,

        getCsrfToken = 20,

        getCookiesV2 = 20,

        /// <summary>
        /// 接收语音
        /// </summary>
        getRecord = 30,

        /// <summary>
        /// 发送群消息
        /// </summary>
        sendGroupMsg = 101,

        /// <summary>
        /// 发送讨论组消息
        /// </summary>
        sendDiscussMsg = 103,

        /// <summary>
        /// 发送私聊消息
        /// </summary>
        sendPrivateMsg = 106,

        /// <summary>
        /// [敏感]发送赞
        /// </summary>
        sendLike = 110,

        /// <summary>
        /// 置群员移除
        /// </summary>
        setGroupKick = 120,

        /// <summary>
        /// 置群员禁言
        /// </summary>
        setGroupBan = 121,

        /// <summary>
        /// 置群管理员
        /// </summary>
        setGroupAdmin = 122,

        /// <summary>
        /// 置全群禁言
        /// </summary>
        setGroupWholeBan = 123,

        /// <summary>
        /// 置匿名群员禁言
        /// </summary>
        setGroupAnonymousBan = 124,

        /// <summary>
        /// 置群匿名设置
        /// </summary>
        setGroupAnonymous = 125,

        /// <summary>
        /// 置群成员名片
        /// </summary>
        setGroupCard = 126,

        /// <summary>
        /// [敏感]置群退出
        /// </summary>
        setGroupLeave = 127,

        /// <summary>
        /// 置群成员专属头衔
        /// </summary>
        setGroupSpecialTitle = 128,

        /// <summary>
        /// 取群成员信息
        /// </summary>
        getGroupMemberInfoV2 = 130,

        /// <summary>
        /// 取陌生人信息
        /// </summary>
        getStrangerInfo = 131,

        /// <summary>
        /// 取群信息
        /// </summary>
        getGroupInfo = 132,

        /// <summary>
        /// 置讨论组退出
        /// </summary>
        setDiscussLeave = 140,

        /// <summary>
        /// 置好友添加请求
        /// </summary>
        setFriendAddRequest = 150,

        /// <summary>
        /// 置群添加请求
        /// </summary>
        setGroupAddRequestV2 = 151,

        /// <summary>
        /// 取群成员列表
        /// </summary>
        getGroupMemberList = 160,

        /// <summary>
        /// 取群列表
        /// </summary>
        getGroupList = 161,

        /// <summary>
        /// 取好友列表
        /// </summary>
        getFriendList = 162,

        /// <summary>
        /// 撤回消息
        /// </summary>
        deleteMsg = 180
    }
}