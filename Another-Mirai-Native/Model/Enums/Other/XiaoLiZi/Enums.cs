using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Model.Enums.Other.XiaoLiZi
{
    /// <summary>
    /// 启动事件处理
    /// </summary>
    public enum EventProcessEnum
    {
        /// <summary>
        /// 阻止其他插件继续处理此事件
        /// </summary>
        Block = 8,

        /// <summary>
        /// 允许其他插件继续处理此事件
        /// </summary>
        Ignore = 0
    }

    public enum FreeGiftEnum
    {
        /// <summary>
        /// 牵你的手
        /// </summary>
        Gift_280 = 280,

        /// <summary>
        /// 可爱猫咪
        /// </summary>
        Gift_281 = 281,

        /// <summary>
        /// 神秘面具
        /// </summary>
        Gift_284 = 284,

        /// <summary>
        /// 甜wink
        /// </summary>
        Gift_285 = 285,

        /// <summary>
        /// 我超忙的
        /// </summary>
        Gift_286 = 286,

        /// <summary>
        /// 快乐肥宅水
        /// </summary>
        Gift_289 = 289,

        /// <summary>
        /// 幸运手链
        /// </summary>
        Gift_290 = 290,

        /// <summary>
        /// 卡布奇诺
        /// </summary>
        Gift_299 = 299,

        /// <summary>
        /// 猫咪手表
        /// </summary>
        Gift_302 = 302,

        /// <summary>
        /// 绒绒手套
        /// </summary>
        Gift_307 = 307,

        /// <summary>
        /// 彩虹糖果
        /// </summary>
        Gift_308 = 308,

        /// <summary>
        /// 爱心口罩
        /// </summary>
        Gift_312 = 312,

        /// <summary>
        /// 坚强
        /// </summary>
        Gift_313 = 313,

        /// <summary>
        /// 告白话筒
        /// </summary>
        Gift_367 = 367
    }

    /// <summary>
    /// 好友验证方式
    /// </summary>
    public enum FriendAuthenticationModeEnum
    {
        /// <summary>
        /// 禁止任何人添加
        /// </summary>
        disable = 1,

        /// <summary>
        /// 允许任何人添加
        /// </summary>
        allow = 2,

        /// <summary>
        /// 需要验证信息
        /// </summary>
        verification = 3,

        /// <summary>
        /// 需要正确回答问题
        /// </summary>
        ATQC = 4,

        /// <summary>
        /// 需要回答问题并由我确认
        /// </summary>
        ATQACPM = 5
    }

    public enum FriendVerificationOperateEnum
    {
        /// <summary>
        /// 同意
        /// </summary>
        Agree = 1,

        /// <summary>
        /// 拒绝
        /// </summary>
        Deny = 2
    }

    public enum GroupFindMethodEnum
    {
        /// <summary>
        /// 不允许
        /// </summary>
        NotAllowed = 0,

        /// <summary>
        /// 通过群号或关键词
        /// </summary>
        GroupName = 1,

        /// <summary>
        /// 仅可通过群号
        /// </summary>
        GroupQQ = 2
    }

    public enum AddGroupMethodEnum
    {
        /// <summary>
        /// 不允许
        /// </summary>
        Allow = 0,

        /// <summary>
        /// 需要发送验证消息
        /// </summary>
        Verification = 1,

        /// <summary>
        /// 需要回答问题并由管理员审核
        /// </summary>
        AnswerTheQuestions = 2,

        /// <summary>
        /// 需要正确回答问题
        /// </summary>
        AnswerTheQuestionCorrectly = 3,

        /// <summary>
        /// 不允许任何人加群
        /// </summary>
        NotAllowed = 4
    }

    public enum GroupNoticeMethodEnum
    {
        /// <summary>
        /// 不接收此人消息
        /// </summary>
        no = 0,

        /// <summary>
        /// 特别关注
        /// </summary>
        SpecialAttention = 1,

        /// <summary>
        /// 默认
        /// </summary>
        Default = 2,
    }

    /// <summary>
    /// 群邀请权限
    /// </summary>
    public enum GroupPermission_SetInviteMethodEnum
    {
        /// <summary>
        /// 无需审核
        /// </summary>
        No_review_required = 1,

        /// <summary>
        /// 需要管理员审核
        /// </summary>
        Admin_review = 2,

        /// <summary>
        /// 100人以内无需审核
        /// </summary>
        No_review_required_within_100_people = 3
    }

    /// <summary>
    /// 表示群成员对于所在群的群成员类型
    /// </summary>
    public enum GroupPosition
    {
        /// <summary>
        /// 普通用户
        /// </summary>
        [Description("普通用户")]
        Member = 1,

        /// <summary>
        /// 管理员
        /// </summary>
        [Description("管理员")]
        Manage = 2,

        /// <summary>
        /// 群主
        /// </summary>
        [Description("群主")]
        Creator = 3
    }

    /// 群验证信息操作类型
    public enum GroupVerificationOperateEnum
    {
        /// <summary>
        /// 同意
        /// </summary>
        Agree = 11,

        /// <summary>
        /// 拒绝
        /// </summary>
        Deny = 12,

        /// <summary>
        /// 忽略
        /// </summary>
        Ignore = 14
    }

    public enum IMEStatusEnum
    {
        /// <summary>
        /// 正在输入
        /// </summary>
        Input = 1,

        /// <summary>
        /// 关闭显示
        /// </summary>
        CloseShow = 2,

        /// <summary>
        /// 正在说话
        /// </summary>
        Talking = 3
    }

    public enum IsGroupRecviceEnum
    {
        /// <summary>
        /// 接收并提醒
        /// </summary>
        recAndRemind = 1,

        /// <summary>
        /// 收进群助手
        /// </summary>
        shrinkGroup = 2,

        /// <summary>
        /// 屏蔽群消息
        /// </summary>
        shieldGroup = 3,

        /// <summary>
        /// 接收不提醒
        /// </summary>
        recNoRemind = 4
    }

    /// <summary>
    /// 消息子类型
    /// </summary>
    public enum MessageSubTypeEnum
    {
        /// <summary>
        /// 临时会话_群临时
        /// </summary>
        Temporary_Group = 0,

        /// <summary>
        /// 临时会话_公众号
        /// </summary>
        Temporary_PublicAccount = 129,

        /// <summary>
        /// 临时会话_网页QQ咨询
        /// </summary>
        Temporary_WebQQConsultation = 201,

        /// <summary>
        /// 临时会话
        /// </summary>
        Temporary_onversation = 141,

        /// <summary>
        /// 临时会话_讨论组临时
        /// </summary>
        temporary_conversation_group = 1,

        /// <summary>
        /// 好友通常消息
        /// </summary>
        friend_usualMsg = 166,

        /// <summary>
        /// 讨论组消息
        /// </summary>
        discussion_group_message = 83,
    }

    /// 消息类型
    public enum MessageTypeEnum
    {
        /// <summary>
        /// 临时会话
        /// </summary>
        Temporary = 141,

        /// <summary>
        /// 好友通常消息
        /// </summary>
        FriendUsualMessage = 166,

        /// <summary>
        /// 好友文件
        /// </summary>
        FriendFile = 529,

        /// <summary>
        /// 好友语音
        /// </summary>
        FriendAudio = 208,

        /// <summary>
        ///  群红包
        /// </summary>
        GroupRedEnvelope = 78,

        /// <summary>
        /// 群聊通常消息
        /// </summary>
        GroupUsualMessage = 134
    }

    public enum MusicAppTypeEnum
    {
        /// <summary>
        /// QQ音乐
        /// </summary>
        QQMusic = 0,

        /// <summary>
        /// 虾米音乐
        /// </summary>
        XiaMiMusic = 1,

        /// <summary>
        /// 酷我音乐
        /// </summary>
        KuWoMusic = 2,

        /// <summary>
        /// 酷狗音乐
        /// </summary>
        KuGouMusic = 3,

        /// <summary>
        /// 网易云音乐
        /// </summary>
        WangYiMusic = 4
    }

    public enum MusicShare_Type
    {
        /// <summary>
        /// 私聊
        /// </summary>
        PrivateMsg = 0,

        /// <summary>
        /// 群聊
        /// </summary>
        GroupMsg = 1
    }

    public enum PermissionEnum
    {
        // 输出日志
        OutputLog = 0,

        // 发送好友消息
        SendFriendMessage = 1,

        // 发送群消息
        SendGroupMessage = 2,

        // 发送群临时消息
        SendGroupTemporaryMessage = 3,

        // 添加好友
        AddFriend = 4,

        // 添加群
        AddGroup = 5,

        // 删除好友！
        RemoveFriend = 6,

        // 置屏蔽好友！
        SetBlockFriend = 7,

        // 置特别关心好友
        SetSpecialFriend = 8,

        // 发送好友json消息
        SendFriendJSONMessage = 11,

        // 发送群json消息
        SendGroupJSONMessage = 12,

        // 上传好友图片
        UploadFriendPicture = 13,

        // 上传群图片
        UploadGroupPicture = 14,

        // 上传好友语音
        UploadFriendAudio = 15,

        // 上传群语音
        UploadGroupAudio = 16,

        // 上传头像！
        UploadAvatar = 17,

        // 设置群名片
        SetGroupMemberNickname = 18,

        // 取昵称_从缓存
        GetNameFromCache = 19,

        // 强制取昵称
        GetNameForce = 20,

        // 获取skey！
        GetSKey = 21,

        // 获取pskey！
        GetPSKey = 22,

        // 获取clientkey！
        GetClientKey = 23,

        // 取框架QQ
        GetThisQQ = 24,

        // 取好友列表
        GetFriendList = 25,

        // 取群列表
        GetGroupList = 26,

        // 取群成员列表
        GetGroupMemberList = 27,

        // 设置管理员
        SetAdministrator = 28,

        // 取管理层列表
        GetAdministratorList = 29,

        // 取群名片
        GetGroupMemberNickname = 30,

        // 取个性签名
        GetSignature = 31,

        // 修改昵称！
        SetName = 32,

        // 修改个性签名！
        SetSignature = 33,

        // 删除群成员
        KickGroupMember = 34,

        // 禁言群成员
        BanGroupMember = 35,

        // 退群！
        QuitGroup = 36,

        // 解散群！
        DissolveGroup = 37,

        // 上传群头像
        UploadGroupAvatar = 38,

        // 全员禁言
        BanAll = 39,

        // 群权限_发起新的群聊
        Group_Create = 40,

        // 群权限_发起临时会话
        Group_CreateTemporary = 41,

        // 群权限_上传文件
        Group_UploadFile = 42,

        // 群权限_上传相册
        Group_UploadPicture = 43,

        // 群权限_邀请好友加群
        Group_InviteFriend = 44,

        // 群权限_匿名聊天
        Group_Anonymous = 45,

        // 群权限_坦白说
        Group_ChatFrankly = 46,

        // 群权限_新成员查看历史消息
        Group_NewMemberReadChatHistory = 47,

        // 群权限_邀请方式设置
        Group_SetInviteMethod = 48,

        // 撤回消息_群聊
        Undo_Group = 49,

        // 撤回消息_私聊本身
        Undo_Private = 50,

        // 设置位置共享
        SetLocationShare = 51,

        // 上报当前位置
        ReportCurrentLocation = 52,

        // 是否被禁言
        IsShutUp = 53,

        // 处理好友验证事件
        ProcessFriendVerification = 54,

        // 处理群验证事件
        ProcessGroupVerification = 55,

        // 查看转发聊天记录内容
        ReadForwardedChatHistory = 56,

        // 上传群文件
        UploadGroupFile = 57,

        // 创建群文件夹
        CreateGroupFolder = 58,

        // 设置在线状态
        SetStatus = 59,

        // QQ点赞！
        QQLike = 60,

        // 取图片下载地址
        GetImageDownloadLink = 61,

        // 查询好友信息
        QueryFriendInformation = 63,

        // 查询群信息
        QueryGroupInformation = 64,

        // 框架重启！
        Reboot = 65,

        // 群文件转发至群
        GroupFileForwardToGroup = 66,

        // 群文件转发至好友
        GroupFileForwardToFriend = 67,

        // 好友文件转发至好友
        FriendFileForwardToFriend = 68,

        // 置群消息接收
        SetGroupMessageReceive = 69,

        // 取群名称_从缓存
        GetGroupNameFromCache = 70,

        // 发送免费礼物
        SendFreeGift = 71,

        // 取好友在线状态
        GetFriendStatus = 72,

        // 取QQ钱包个人信息！
        GetQQWalletPersonalInformation = 73,

        // 获取订单详情
        GetOrderDetail = 74,

        // 提交支付验证码
        SubmitPaymentCaptcha = 75,

        // 分享音乐
        ShareMusic = 77,

        // 更改群聊消息内容！
        ModifyGroupMessageContent = 78,

        // 更改私聊消息内容！
        ModifyPrivateMessageContent = 79,

        // 群聊口令红包
        GroupPasswordRedEnvelope = 80,

        // 群聊拼手气红包
        GroupRandomRedEnvelope = 81,

        // 群聊普通红包
        GroupNormalRedEnvelope = 82,

        // 群聊画图红包
        GroupDrawRedEnvelope = 83,

        // 群聊语音红包
        GroupAudioRedEnvelope = 84,

        // 群聊接龙红包
        GroupFollowRedEnvelope = 85,

        // 群聊专属红包
        GroupExclusiveRedEnvelope = 86,

        // 好友口令红包
        FriendPasswordRedEnvelope = 87,

        // 好友普通红包
        FriendNormalRedEnvelope = 88,

        // 好友画图红包
        FriendDrawRedEnvelope = 89,

        // 好友语音红包
        FriendAudioRedEnvelope = 90,

        // 好友接龙红包
        FriendFollowRedEnvelope = 91,

        //.常量 权限_重命名群文件夹, "92", 公开
        Grour_RenmeFolder = 92,

        //.常量 权限_删除群文件夹, "93", 公开
        Grour_DeleteFolder = 93,

        //.常量 权限_删除群文件, "94", 公开
        Grour_DeleteFile = 94,

        //.常量 权限_保存文件到微云, "95", 公开
        Grour_Save2WwiYun = 95,

        //.常量 权限_移动群文件, "96", 公开
        Grour_MoveFile = 96,

        //.常量 权限_取群文件列表, "97", 公开
        Grour_GetFileList = 97,

        //.常量 权限_设置专属头衔, "98", 公开
        Grour_SetMemerTitle = 98,

        //.常量 权限_下线指定QQ, "99", 公开
        Grour_OfflineQQ = 99,

        //.常量 权限_登录指定QQ, "100", 公开
        Grour_LoginQQ = 100,

        //.常量 权限_取群未领红包, "101", 公开
        Grour_UnclaimedRedEnvelope = 101,

        //.常量 权限_发送输入状态, "102", 公开
        Grour_SendInputStatusr = 102,

        //.常量 权限_修改资料, "103", 公开
        Grour_ModifyInformation = 103,

        //.常量 权限_打好友电话, "104", 公开
        Grour_CallFriendsTel = 104,

        //.常量 权限_取群文件下载地址, "105", 公开
        Grour_GetFileDownload = 105,

        //.常量 权限_头像双击_好友, "106", 公开
        DoubleClickFriend = 106,

        //.常量 权限_头像双击_群, "107", 公开
        DoubleClickGroup = 107,

        //.常量 权限_取群成员简略信息, "108", 公开
        Grour_SimpleInfo = 108,

        //.常量 权限_群聊置顶, "109", 公开
        Grour_ChatTop = 109,

        //.常量 权限_私聊置顶, "110", 公开
        Grour_PrivateTop = 110,

        //.常量 权限_取加群链接, "111", 公开
        Grour_AddUrl = 111,

        //.常量 权限_设为精华, "112", 公开
        Grour_SetEssence = 112,

        //.常量 权限_群权限_设置群昵称规则, "113", 公开
        Grour_SetNiceRules = 113,

        //.常量 权限_群权限_设置群发言频率, "114", 公开
        Grour_SpeckFrequency = 114,

        //.常量 权限_群权限_设置群查找方式, "115", 公开
        Grour_FindMethod = 115,

        //.常量 权限_邀请好友加群, "116", 公开
        Grour_SetInviteFrindeAdd = 116,

        //.常量 权限_置群内消息通知, "117", 公开
        Grour_Notification = 117,

        //.常量 权限_修改群名称, "118", 公开
        Grour_UpdateName = 118,
    }

    public enum ProfessionEnum
    {
        /// <summary>
        /// IT
        /// </summary>
        IT = 1,

        /// <summary>
        /// 制造
        /// </summary>
        Manufacturing = 2,

        /// <summary>
        /// 医疗
        /// </summary>
        Medical = 3,

        /// <summary>
        /// 金融
        /// </summary>
        Finance = 4,

        /// <summary>
        /// 商业
        /// </summary>
        Business = 5,

        /// <summary>
        /// 文化
        /// </summary>
        Culture = 6,

        /// <summary>
        /// 艺术
        /// </summary>
        Art = 7,

        /// <summary>
        /// 法律
        /// </summary>
        Law = 8,

        /// <summary>
        /// 教育
        /// </summary>
        Education = 9,

        /// <summary>
        /// 行政
        /// </summary>
        Administration = 10,

        /// <summary>
        /// 模特
        /// </summary>
        Model = 11,

        /// <summary>
        /// 空姐
        /// </summary>
        Stewardess = 12,

        /// <summary>
        /// 学生
        /// </summary>
        Student = 13,

        /// <summary>
        /// 其他职业
        /// </summary>
        OtherOccupations = 14
    }

    /// <summary>
    /// 红包转发类型
    /// </summary>
    public enum RedE2TypeEnum
    {
        /// <summary>
        /// 好友
        /// </summary>
        Friend = 1,

        /// <summary>
        /// 群
        /// </summary>
        Group = 2
    }

    public enum ServiceInformation
    {
        SVIP = 1, //SVIP

        VIDEO_VIP = 4, //视频会员

        MUSIC_PACK = 6, //音乐包

        STAR = 105, //star

        YELLOW_DIAMOND = 102, //黄钻

        GREEN_DIAMOND = 103, //绿钻

        RED_DIAMOND = 101, //红钻

        YELLOWLOVE = 104, //yellowlove

        SVIP_WITH_VIDEO = 107, //SVIP&视频会员

        SVIP_WITH_GREEN = 109, //SVIP&绿钻

        SVIP_WITH_MUSIC = 110 //SVIP&音乐包
    }

    public enum SexEnum
    {
        /// <summary>
        /// 男
        /// </summary>
        M = 1,

        /// <summary>
        /// 女
        /// </summary>
        F = 2
    }

    public enum ShowPicEnum
    {
        /// <summary>
        /// 40000普通
        /// </summary>
        normal = 4000,

        /// <summary>
        /// 40001幻影
        /// </summary>
        phantom = 40001,

        /// <summary>
        /// 40002抖动
        /// </summary>
        jitter = 40002,

        /// <summary>
        /// 40003生日
        /// </summary>
        birthday = 40003,

        /// <summary>
        /// 40004爱你
        /// </summary>
        loveyou = 40004,

        /// <summary>
        /// 40005征友
        /// </summary>
        friends = 40005
    }

    /// <summary>
    /// 转账类型
    /// </summary>
    public enum TransferTypeEnum
    {
        /// <summary>
        /// 好友转账
        /// </summary>
        Firend = 0,

        /// <summary>
        /// 陌生人转账
        /// </summary>
        Stranger = 1
    }

    /// 主要在线状态
    public enum StatusTypeEnum
    {
        /// <summary>
        /// 在线
        /// </summary>
        Online = 11,

        /// <summary>
        /// 离开
        /// </summary>
        Away = 31,

        /// <summary>
        /// 隐身
        /// </summary>
        Invisible = 41,

        /// <summary>
        /// 忙碌
        /// </summary>
        Busy = 50,

        /// <summary>
        /// Q我吧
        /// </summary>
        TalkToMe = 60,

        /// <summary>
        /// 请勿打扰
        /// </summary>
        DoNotDisturb = 70
    }

    /// 详细在线状态
    public enum StatusOnlineTypeEnum
    {
        /// <summary>
        /// 普通在线
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 我的电量
        /// </summary>
        Battery = 1000,

        /// <summary>
        /// 信号弱
        /// </summary>
        WeakSignal = 1011,

        /// <summary>
        /// 睡觉中
        /// </summary>
        Sleeping = 1016,

        /// <summary>
        /// 游戏中
        /// </summary>
        Gaming = 1017,

        /// <summary>
        /// 学习中
        /// </summary>
        Studying = 1018,

        /// <summary>
        /// 吃饭中
        /// </summary>
        Eating = 1019,

        /// <summary>
        /// 煲剧中
        /// </summary>
        WatchingTVSeries = 1021,

        /// <summary>
        /// 度假中
        /// </summary>
        OnVacation = 1022,

        /// <summary>
        ///  在线学习
        /// </summary>
        OnlineStudying = 1024,

        /// <summary>
        /// 在家旅游
        /// </summary>
        TravelAtHome = 1025,

        /// <summary>
        /// TiMi中
        /// </summary>
        TiMiing = 1027,

        /// <summary>
        /// 我在听歌
        /// </summary>
        ListeningToMusic = 1028,

        /// <summary>
        /// 熬夜中
        /// </summary>
        StayingUpLate = 1032,

        /// <summary>
        /// 打球中
        /// </summary>
        PlayingBall = 1050,

        /// <summary>
        /// 恋爱中
        /// </summary>
        FallInLove = 1051,

        /// <summary>
        /// 我没事(实际上有事)
        /// </summary>
        ImOK = 1052,

        /// <summary>
        /// 汪汪汪
        /// </summary>
        Barking = 1053,

        /// <summary>
        /// 40001在地球
        /// </summary>
        OnEarth = 40001,

        /// <summary>
        /// 移动中
        /// </summary>
        Moving = 4102,

        /// <summary>
        /// 1033在小区
        /// </summary>
        IntheCommunity = 1033,

        /// <summary>
        /// 41034在学校
        /// </summary>
        inSchool = 41034,

        /// <summary>
        /// 41035在公园
        /// </summary>
        inthePark = 41035,

        /// <summary>
        /// 41036在海边
        /// </summary>
        attheSeaside = 41036,

        /// <summary>
        /// 41037在机场
        /// </summary>
        attheAirport = 41037,

        /// <summary>
        /// 41038在商场
        /// </summary>
        AttheMall = 41038,

        /// <summary>
        /// 41039在咖啡厅
        /// </summary>
        intheCoffeeShop = 41039,

        /// <summary>
        /// 41041在餐厅
        /// </summary>
        intheRestaurant = 41041,

        /// <summary>
        /// 1022度假中
        /// </summary>
        onVacation = 1022,

        /// <summary>
        /// 1020健身中
        /// </summary>
        onFitness = 1020,

        /// <summary>
        /// 1056嗨到起飞
        /// </summary>
        toTakeOff = 1056,

        /// <summary>
        /// 1058元气满满
        /// </summary>
        fullOfVitality = 1058,

        /// <summary>
        /// 1057美滋滋
        /// </summary>
        beautiful = 1057,

        /// <summary>
        /// 1059悠哉哉
        /// </summary>
        leisurely = 1059,

        /// <summary>
        /// 1060无聊中
        /// </summary>
        boring = 1060,

        /// <summary>
        /// 1061想静静
        /// </summary>
        wantToTeQuiet = 1061,

        /// <summary>
        /// 1062我太难了
        /// </summary>
        IamTooHard = 1062,

        /// <summary>
        /// 1063一言难尽
        /// </summary>
        hardToSay = 1063,

        /// <summary>
        /// 1064吃鸡中
        /// </summary>
        eatChicken = 1064,

        /// <summary>
        /// 1069遇见春天
        /// </summary>
        meetSpring = 1069,
    }

    public enum FiletypeEnum
    {
        /// <summary>
        /// 文件
        /// </summary>
        file = 1,

        /// <summary>
        /// 文件夹
        /// </summary>
        folder = 2
    }

    /// <summary>
    /// 消息事件处理
    /// </summary>
    public enum EventMessageEnum
    {
        /// <summary>
        /// 阻止其他插件继续处理此事件
        /// </summary>
        Block = 1,

        /// <summary>
        /// 允许其他插件继续处理此事件
        /// </summary>
        Ignore = 0
    }

    public enum EventTypeEnum
    {
        /// <summary>
        /// 好友事件_被好友删除
        /// </summary>
        Friend_Removed = 100,

        /// <summary>
        /// 好友事件_签名变更
        /// </summary>
        Friend_SignatureChanged = 101,

        /// <summary>
        /// 好友事件_昵称改变
        /// </summary>
        Friend_NameChanged = 102,

        /// <summary>
        /// 好友事件_某人撤回事件
        /// </summary>
        Friend_UserUndid = 103,

        /// <summary>
        /// 好友事件_有新好友
        /// </summary>
        Friend_NewFriend = 104,

        /// <summary>
        /// 好友事件_好友请求
        /// </summary>
        Friend_FriendRequest = 105,

        /// <summary>
        /// 好友事件_对方同意了您的好友请求
        /// </summary>
        Friend_FriendRequestAccepted = 106,

        /// <summary>
        /// 好友事件_对方拒绝了您的好友请求
        /// </summary>
        Friend_FriendRequestRefused = 107,

        /// <summary>
        /// 好友事件_资料卡点赞
        /// </summary>
        Friend_InformationLiked = 108,

        /// <summary>
        /// 好友事件_签名点赞
        /// </summary>
        Friend_SignatureLiked = 109,

        /// <summary>
        /// 好友事件_签名回复
        /// </summary>
        Friend_SignatureReplied = 110,

        /// <summary>
        /// 好友事件_个性标签点赞
        /// </summary>
        Friend_TagLiked = 111,

        /// <summary>
        /// 好友事件_随心贴回复
        /// </summary>
        Friend_StickerLiked = 112,

        /// <summary>
        /// 好友事件_随心贴增添
        /// </summary>
        Friend_StickerAdded = 113,

        /// <summary>
        /// 好友事件_系统提示
        /// </summary>
        Friend_SystmHint = 114,

        /// <summary>
        /// 好友事件_随心贴点赞
        /// </summary>
        Friend_link = 115,

        /// <summary>
        /// 好友事件_匿名提问_被提问
        /// </summary>
        Friend_AnonymousQuestioned = 116,

        /// <summary>
        /// 好友事件_匿名提问_被点赞
        /// </summary>
        Friend_Anonymouslink = 117,

        /// <summary>
        /// 好友事件_匿名提问_被回复
        /// </summary>
        Friend_AnonymousResponded = 118,

        /// <summary>
        /// 好友事件_输入状态
        /// </summary>
        Friend_inputStatus = 119,

        /// <summary>
        /// 群事件_我被邀请加入群
        /// </summary>
        Group_Invited = 1,

        /// <summary>
        /// 群事件_某人加入了群
        /// </summary>
        Group_MemberJoined = 2,

        /// <summary>
        /// 群事件_某人申请加群
        /// </summary>
        Group_MemberVerifying = 3,

        /// <summary>
        /// 群事件_群被解散
        /// </summary>
        Group_GroupDissolved = 4,

        /// <summary>
        /// 群事件_某人退出了群
        /// </summary>
        Group_MemberQuit = 5,

        /// <summary>
        /// 群事件_某人被踢出群
        /// </summary>
        Group_MemberKicked = 6,

        /// <summary>
        /// 群事件_某人被禁言
        /// </summary>
        Group_MemberShutUp = 7,

        /// <summary>
        /// 群事件_某人撤回事件
        /// </summary>
        Group_MemberUndid = 8,

        /// <summary>
        /// 群事件_某人被取消管理
        /// </summary>
        Group_AdministratorTook = 9,

        /// <summary>
        /// 群事件_某人被赋予管理
        /// </summary>
        Group_AdministratorGave = 10,

        /// <summary>
        /// 群事件_开启全员禁言
        /// </summary>
        Group_EnableAllShutUp = 11,

        /// <summary>
        /// 群事件_关闭全员禁言
        /// </summary>
        Group_DisableAllShutUp = 12,

        /// <summary>
        /// 群事件_开启匿名聊天
        /// </summary>
        Group_EnableAnonymous = 13,

        /// <summary>
        /// 群事件_关闭匿名聊天
        /// </summary>
        Group_DisableAnonymous = 14,

        /// <summary>
        /// 群事件_开启坦白说
        /// </summary>
        Group_EnableChatFrankly = 15,

        /// <summary>
        /// 群事件_关闭坦白说
        /// </summary>
        Group_DisableChatFrankly = 16,

        /// <summary>
        /// 群事件_允许群临时会话
        /// </summary>
        Group_AllowGroupTemporary = 17,

        /// <summary>
        /// 群事件_禁止群临时会话
        /// </summary>
        Group_ForbidGroupTemporary = 18,

        /// <summary>
        /// 群事件_允许发起新的群聊
        /// </summary>
        Group_AllowCreateGroup = 19,

        /// <summary>
        /// 群事件_禁止发起新的群聊
        /// </summary>
        Group_ForbidCreateGroup = 20,

        /// <summary>
        /// 群事件_允许上传群文件
        /// </summary>
        Group_AllowUploadFile = 21,

        /// <summary>
        /// 群事件_禁止上传群文件
        /// </summary>
        Group_ForbidUploadFile = 22,

        /// <summary>
        /// 群事件_允许上传相册
        /// </summary>
        Group_AllowUploadPicture = 23,

        /// <summary>
        /// 群事件_禁止上传相册
        /// </summary>
        Group_ForbidUploadPicture = 24,

        /// <summary>
        /// 群事件_某人被邀请入群
        /// </summary>
        Group_MemberInvited = 25,

        /// <summary>
        /// 群事件_展示成员群头衔
        /// </summary>
        Group_ShowMemberTitle = 26,

        /// <summary>
        /// 群事件_隐藏成员群头衔
        /// </summary>
        Group_HideMemberTitle = 27,

        /// <summary>
        /// 群事件_某人被解除禁言
        /// </summary>
        Group_MemberNotShutUp = 28,

        /// <summary>
        /// 空间事件_与我相关
        /// </summary>
        QZone_Related = 29,

        /// <summary>
        /// 群事件_我被踢出
        /// </summary>
        Group_MemberKickOut = 30,

        /// <summary>
        /// 群事件_群名变更
        /// </summary>
        Group_GroupNameUpdate = 32,

        /// <summary>
        /// 群事件_系统提示
        /// </summary>
        Group_SystmHint = 33,

        /// <summary>
        /// 群事件_群头像事件
        /// </summary>
        Group_Face = 34,

        /// <summary>
        /// 群事件_入场特效
        /// </summary>
        Group_AdmissionSpecialEffects = 35,

        /// <summary>
        /// 群事件_修改群名片
        /// </summary>
        Group_ModifyBusinessCard = 36,

        /// <summary>
        /// 群事件_群被转让
        /// </summary>
        Group_Transfer = 37,

        /// <summary>
        /// 框架事件_登录成功
        /// </summary>
        This_SignInSuccess = 31,

        /// <summary>
        /// 框架事件_登录失败
        /// </summary>
        This_LoginFailed = 38,

        /// <summary>
        /// 框架事件_即将重启更新自身
        /// </summary>
        This_Reboot = 39,

        /// <summary>
        /// 登录事件_电脑上线
        /// </summary>
        login_pcOnline = 200,

        /// <summary>
        /// 登录事件_电脑下线
        /// </summary>
        login_pcOffline = 201,

        /// <summary>
        /// 登录事件_移动设备上线
        /// </summary>
        login_mobileOnline = 202,

        /// <summary>
        /// 登录事件_移动设备下线
        /// </summary>
        login_mobileOffline = 203,

        /// <summary>
        /// PCQQ登录验证请求
        /// </summary>
        login_PCVerification = 204,

        /// <summary>
        /// 讨论组事件_讨论组名变更
        /// </summary>
        Discussion_NameChange = 300,

        /// <summary>
        /// 讨论组事件_某人撤回事件
        /// </summary>
        Discussion_Withdraw = 301,

        /// <summary>
        /// 讨论组事件_某人被邀请入群
        /// </summary>
        Discussion_beInvited = 302,

        /// <summary>
        /// 讨论组事件_某人退出了群
        /// </summary>
        Discussion_Out = 303,

        /// <summary>
        /// 讨论组事件_某人被踢出群
        /// </summary>
        Discussion_KickedOut = 304,
    }

    public enum AudioTypeEnum
    {
        /// <summary>
        /// 普通语音
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 变声语音
        /// </summary>
        Change = 1,

        /// <summary>
        /// 文字语音
        /// </summary>
        Text = 2,

        /// <summary>
        /// (红包)匹配语音
        /// </summary>
        Match = 3,
    }
}