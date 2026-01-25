namespace Another_Mirai_Native.Protocol.OneBot.Enums
{
    public enum APIType
    {
        ///<summary>
        ///发送私聊消息
        ///</summary>
        send_private_msg,

        ///<summary>
        ///发送群消息
        ///</summary>
        send_group_msg,

        ///<summary>
        ///发送消息
        ///</summary>
        send_msg,

        ///<summary>
        ///撤回消息
        ///</summary>
        delete_msg,

        ///<summary>
        ///获取消息
        ///</summary>
        get_msg,

        ///<summary>
        ///获取合并转发消息
        ///</summary>
        get_forward_msg,

        ///<summary>
        ///发送好友赞
        ///</summary>
        send_like,

        ///<summary>
        ///群组踢人
        ///</summary>
        set_group_kick,

        ///<summary>
        ///群组单人禁言
        ///</summary>
        set_group_ban,

        ///<summary>
        ///群组匿名用户禁言
        ///</summary>
        set_group_anonymous_ban,

        ///<summary>
        ///群组全员禁言
        ///</summary>
        set_group_whole_ban,

        ///<summary>
        ///群组设置管理员
        ///</summary>
        set_group_admin,

        ///<summary>
        ///群组匿名
        ///</summary>
        set_group_anonymous,

        ///<summary>
        ///设置群名片（群备注）
        ///</summary>
        set_group_card,

        ///<summary>
        ///设置群名
        ///</summary>
        set_group_name,

        ///<summary>
        ///退出群组
        ///</summary>
        set_group_leave,

        ///<summary>
        ///设置群组专属头衔
        ///</summary>
        set_group_special_title,

        ///<summary>
        ///处理加好友请求
        ///</summary>
        set_friend_add_request,

        ///<summary>
        ///处理加群请求／邀请
        ///</summary>
        set_group_add_request,

        ///<summary>
        ///获取登录号信息
        ///</summary>
        get_login_info,

        ///<summary>
        ///获取陌生人信息
        ///</summary>
        get_stranger_info,

        ///<summary>
        ///获取好友列表
        ///</summary>
        get_friend_list,

        ///<summary>
        ///获取群信息
        ///</summary>
        get_group_info,

        ///<summary>
        ///获取群列表
        ///</summary>
        get_group_list,

        ///<summary>
        ///获取群成员信息
        ///</summary>
        get_group_member_info,

        ///<summary>
        ///获取群成员列表
        ///</summary>
        get_group_member_list,

        ///<summary>
        ///获取群荣誉信息
        ///</summary>
        get_group_honor_info,

        ///<summary>
        ///获取 Cookies
        ///</summary>
        get_cookies,

        ///<summary>
        ///获取 CSRF Token
        ///</summary>
        get_csrf_token,

        ///<summary>
        ///获取 QQ 相关接口凭证
        ///</summary>
        get_credentials,

        ///<summary>
        ///获取语音
        ///</summary>
        get_record,

        ///<summary>
        ///获取图片
        ///</summary>
        get_image,

        ///<summary>
        ///检查是否可以发送图片
        ///</summary>
        can_send_image,

        ///<summary>
        ///检查是否可以发送语音
        ///</summary>
        can_send_record,

        ///<summary>
        ///获取运行状态
        ///</summary>
        get_status,

        ///<summary>
        ///获取版本信息
        ///</summary>
        get_version_info,

        ///<summary>
        ///重启 OneBot 实现
        ///</summary>
        set_restart,

        ///<summary>
        ///清理缓存
        ///</summary>
        clean_cache,
    }
}