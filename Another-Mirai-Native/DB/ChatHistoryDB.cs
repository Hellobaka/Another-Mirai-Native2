using SqlSugar;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Model;

namespace Another_Mirai_Native.DB
{
    public static class ChatHistoryDB
    {
        private static readonly object _lockObj = new();
        private static SqlSugarClient? _dbInstance;

        public static string GetDBPath() => Path.Combine("logs", "chat_history.db");

        public static SqlSugarClient GetInstance()
        {
            if (_dbInstance != null)
            {
                return _dbInstance;
            }

            lock (_lockObj)
            {
                if (_dbInstance != null)
                {
                    return _dbInstance;
                }

                string dbPath = GetDBPath();
                bool isNewDb = !File.Exists(dbPath);

                _dbInstance = new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = $"data source={dbPath}",
                    DbType = DbType.Sqlite,
                    IsAutoCloseConnection = false,
                    InitKeyType = InitKeyType.Attribute,
                });

                InitializeDatabase(_dbInstance);
                return _dbInstance;
            }
        }

        private static void InitializeDatabase(SqlSugarClient db)
        {
            db.CodeFirst.InitTables(
                typeof(ChatHistoryEntity),
                typeof(FriendEntity),
                typeof(GroupEntity),
                typeof(GroupMemberEntity),
                typeof(ChatCategoryEntity)
            );
        }
    }

    /// <summary>
    /// 聊天记录表 - 核心消息表
    /// 整合所有类型的聊天记录(群聊/私聊/通知等)到单一表中
    /// </summary>
    [SugarTable("ChatHistory")]
    [SugarIndex("IX_History_Parent_Type_Time", nameof(ParentID), OrderByType.Asc, nameof(Type), OrderByType.Asc, nameof(Time), OrderByType.Desc)]
    [SugarIndex("IX_History_Parent_Type_MsgId", nameof(ParentID), OrderByType.Asc, nameof(Type), OrderByType.Asc, nameof(MsgId), OrderByType.Asc)]
    [SugarIndex("IX_History_Sender_Time", nameof(SenderID), OrderByType.Asc, nameof(Time), OrderByType.Desc)]
    public class ChatHistoryEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        /// <summary>
        /// 消息时间
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long Time { get; set; }

        /// <summary>
        /// 聊天类型(群聊/私聊/通知/其他)
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public ChatHistoryType Type { get; set; }

        /// <summary>
        /// 父ID: 群号或QQ号
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long ParentID { get; set; }

        /// <summary>
        /// 发送者QQ号
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long SenderID { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 消息ID
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int MsgId { get; set; }

        /// <summary>
        /// 是否撤回
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public bool Recalled { get; set; }

        /// <summary>
        /// 插件名称
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string PluginName { get; set; } = string.Empty;
    }

    /// <summary>
    /// 好友信息表 - 缓存好友数据
    /// 持久化 FriendInfoCache 的内容
    /// </summary>
    [SugarTable("Friends")]
    [SugarIndex("IX_Friend_QQ", nameof(FriendEntity.QQ), OrderByType.Asc, true)]
    public class FriendEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        /// <summary>
        /// 好友QQ号
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long QQ { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Nick { get; set; } = string.Empty;

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Postscript { get; set; } = string.Empty;

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long LastUpdateTime { get; set; }
    }

    /// <summary>
    /// 群信息表 - 缓存群数据
    /// 持久化 GroupInfoCache 的内容
    /// </summary>
    [SugarTable("Groups")]
    [SugarIndex("IX_Group_GroupID", nameof(GroupEntity.GroupID), OrderByType.Asc, true)]
    public class GroupEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        /// <summary>
        /// 群号
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long GroupID { get; set; }

        /// <summary>
        /// 群名称
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 当前成员数
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int CurrentMemberCount { get; set; }

        /// <summary>
        /// 最大成员数
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int MaxMemberCount { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long LastUpdateTime { get; set; }
    }

    /// <summary>
    /// 群成员信息表 - 缓存群成员数据
    /// 持久化 GroupMemberCache 的内容
    /// </summary>
    [SugarTable("GroupMembers")]
    [SugarIndex("IX_GroupMember_Composite", nameof(GroupID), OrderByType.Asc, nameof(QQ), OrderByType.Asc, true)]
    [SugarIndex("IX_GroupMember_Group", nameof(GroupID), OrderByType.Asc)]
    public class GroupMemberEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        /// <summary>
        /// 群号
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long GroupID { get; set; }

        /// <summary>
        /// 成员QQ号
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long QQ { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Nick { get; set; } = string.Empty;

        /// <summary>
        /// 群名片
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Card { get; set; } = string.Empty;

        /// <summary>
        /// 成员类型(1:成员 2:管理 3:群主)
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public QQGroupMemberType MemberType { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public QQSex Sex { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int Age { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Area { get; set; } = string.Empty;

        /// <summary>
        /// 加群时间戳
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long JoinGroupTime { get; set; }

        /// <summary>
        /// 最后发言时间戳
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long LastSpeakTime { get; set; }

        /// <summary>
        /// 群等级
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Level { get; set; } = string.Empty;

        /// <summary>
        /// 专属头衔
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ExclusiveTitle { get; set; } = string.Empty;

        /// <summary>
        /// 专属头衔过期时间戳(0表示永久)
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long ExclusiveTitleExpirationTime { get; set; }

        /// <summary>
        /// 是否不良记录成员
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public bool IsBadRecord { get; set; }

        /// <summary>
        /// 是否允许修改名片
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public bool IsAllowEditorCard { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long LastUpdateTime { get; set; }
    }

    /// <summary>
    /// 聊天会话分类表 - 记录最近会话
    /// 对应原来的 ChatHistoryType.Other 类型记录
    /// </summary>
    [SugarTable("ChatCategories")]
    [SugarIndex("IX_Category_Composite", nameof(ParentID), OrderByType.Asc, nameof(Type), OrderByType.Asc, true)]
    public class ChatCategoryEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        /// <summary>
        /// 会话ID(群号或QQ号)
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long ParentID { get; set; }

        /// <summary>
        /// 会话ID(QQ号)
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long SenderID { get; set; }

        /// <summary>
        /// 会话类型
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public ChatHistoryType Type { get; set; }

        /// <summary>
        /// 最后一条消息时间
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long Time { get; set; }

        /// <summary>
        /// 最后一条消息内容
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 未读消息数
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int UnreadCount { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public bool IsPinned { get; set; }
    }
}