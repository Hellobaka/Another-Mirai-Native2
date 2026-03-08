using Another_Mirai_Native.Model.Enums;
using System;

namespace Another_Mirai_Native.Abstractions.Models
{
    /// <summary>
    /// 表示群成员信息的类
    /// </summary>
    public class GroupMemberInfo
    {
        /// <summary>
        /// 获取一个值, 指示成员所在群的实例
        /// </summary>
        public long Group { get; set; }

        /// <summary>
        /// 获取一个值, 指示当前成员的QQ号的实例
        /// </summary>
        public long QQ { get; set; }

        /// <summary>
        /// 获取一个值, 指示当前成员的QQ昵称
        /// </summary>
        public string Nick { get; set; } = "";

        /// <summary>
        /// 获取一个值, 指示当前成员在此群的群名片
        /// </summary>
        public string Card { get; set; } = "";

        /// <summary>
        /// 获取一个值, 指示当前群成员的性别
        /// </summary>
        public QQSex Sex { get; set; }

        /// <summary>
        /// 获取一个值, 指示当前群成员年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 获取一个值, 指示当前成员所在地区
        /// </summary>
        public string Area { get; set; } = "";

        /// <summary>
        /// 获取一个值, 指示当前成员加入群的日期和时间
        /// </summary>
        public DateTime JoinGroupDateTime { get; set; }

        /// <summary>
        /// 获取一个值, 指示当前群成员最后一次发言的日期和时间
        /// </summary>
        public DateTime LastSpeakDateTime { get; set; }

        /// <summary>
        /// 获取一个值, 指示当前群成员的等级
        /// </summary>
        public string Level { get; set; } = "";

        /// <summary>
        /// 获取一个值, 指示当前的群成员类型
        /// </summary>
        public QQGroupMemberType MemberType { get; set; }

        /// <summary>
        /// 获取一个值, 指示当前群成员是否为不良记录群成员
        /// </summary>
        public bool IsBadRecord { get; set; }

        /// <summary>
        /// 获取一个值, 指示当前群成员在此群获得的专属头衔
        /// </summary>
        public string ExclusiveTitle { get; set; } = "";

        /// <summary>
        /// 获取一个值, 指示当前群成员在此群的专属头衔过期时间, 若本属性为 null 则表示无期限
        /// </summary>
        public DateTime? ExclusiveTitleExpirationTime { get; set; }

        /// <summary>
        /// 获取一个值, 指示当前群成员是否允许修改群名片
        /// </summary>
        public bool IsAllowEditorCard { get; set; }

        /// <summary>
        /// 最后更新时间(时间戳)
        /// </summary>
        public long LastUpdateTime { get; set; }

        /// <summary>
        /// ToString 的重写，用于提供当前群成员信息的字符串表示形式
        /// </summary>
        public override string ToString()
        {
            return $"群: {Group}; " +
                $"QQ: {QQ}; " +
                $"昵称: {Nick}; " +
                $"名片: {Card}; " +
                $"性别: {Sex}; " +
                $"地区: {Area}; " +
                $"入群时间: {JoinGroupDateTime:yyyy-MM-dd HH:mm:ss}; " +
                $"最后发言时间: {LastSpeakDateTime:yyyy-MM-dd HH:mm:ss}; " +
                $"成员等级: {Level}; " +
                $"成员类型: {MemberType}; " +
                $"专属头衔: {ExclusiveTitle}; " +
                $"专属头衔过期时间: {(ExclusiveTitleExpirationTime != null ? ExclusiveTitleExpirationTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "永久")}; " +
                $"不良记录成员: {(IsBadRecord ? "是" : "否")}; " +
                $"允许修改名片: {(IsAllowEditorCard ? "是" : "否")}"; 
        }
    }
}