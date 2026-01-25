using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using System.IO;
using System.Text;

namespace Another_Mirai_Native.Model
{
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
                $"成员类型: {MemberType.GetDescription()}; " +
                $"专属头衔: {ExclusiveTitle}; " +
                $"专属头衔过期时间: {(ExclusiveTitleExpirationTime != null ? ExclusiveTitleExpirationTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "永久")}; " +
                $"不良记录成员: {(IsBadRecord ? "是" : "否")}; " +
                $"允许修改名片: {(IsAllowEditorCard ? "是" : "否")}"; 
        }

        public byte[] ToNative()
        {
            MemoryStream stream = new();
            BinaryWriter binaryWriter = new(stream);
            BinaryWriterExpand.Write_Ex(binaryWriter, Group);
            BinaryWriterExpand.Write_Ex(binaryWriter, QQ);
            BinaryWriterExpand.Write_Ex(binaryWriter, Nick);
            BinaryWriterExpand.Write_Ex(binaryWriter, Card);
            BinaryWriterExpand.Write_Ex(binaryWriter, (int)Sex);
            BinaryWriterExpand.Write_Ex(binaryWriter, Age);
            BinaryWriterExpand.Write_Ex(binaryWriter, Area);
            BinaryWriterExpand.Write_Ex(binaryWriter, JoinGroupDateTime.ToTimeStamp());
            BinaryWriterExpand.Write_Ex(binaryWriter, LastSpeakDateTime.ToTimeStamp());
            BinaryWriterExpand.Write_Ex(binaryWriter, Level);
            BinaryWriterExpand.Write_Ex(binaryWriter, (int)MemberType);
            BinaryWriterExpand.Write_Ex(binaryWriter, IsBadRecord ? 1 : 0);
            BinaryWriterExpand.Write_Ex(binaryWriter, ExclusiveTitle);
            BinaryWriterExpand.Write_Ex(binaryWriter, ExclusiveTitleExpirationTime?.ToTimeStamp() ?? int.MaxValue);
            BinaryWriterExpand.Write_Ex(binaryWriter, IsAllowEditorCard ? 1 : 0);
            return stream.ToArray();
        }

        public static GroupMemberInfo FromNative(byte[] buffer)
        {
            GroupMemberInfo info = new();

            using BinaryReader binaryReader = new(new MemoryStream(buffer));
            info.Group = binaryReader.ReadInt64_Ex();
            info.QQ = binaryReader.ReadInt64_Ex();
            info.Nick = binaryReader.ReadString_Ex();
            info.Card = binaryReader.ReadString_Ex();
            info.Sex = (QQSex)binaryReader.ReadInt32_Ex();
            info.Age = binaryReader.ReadInt32_Ex();
            info.Area = binaryReader.ReadString_Ex();
            info.JoinGroupDateTime = Helper.TimeStamp2DateTime(binaryReader.ReadInt32_Ex());
            info.LastSpeakDateTime = Helper.TimeStamp2DateTime(binaryReader.ReadInt32_Ex());
            info.Level = binaryReader.ReadString_Ex();
            info.MemberType = (QQGroupMemberType)binaryReader.ReadInt32_Ex();
            info.IsBadRecord = binaryReader.ReadInt32_Ex() == 1;
            info.ExclusiveTitle = binaryReader.ReadString_Ex();
            info.ExclusiveTitleExpirationTime = Helper.TimeStamp2DateTime(binaryReader.ReadInt32_Ex());
            info.IsAllowEditorCard = binaryReader.ReadInt32_Ex() == 1;

            return info;
        }

        public string ToNativeBase64()
        {
            return Convert.ToBase64String(ToNative());
        }

        public static string CollectionToList(List<GroupMemberInfo> list)
        {
            MemoryStream stream = new();
            BinaryWriter binaryWriter = new(stream);
            BinaryWriterExpand.Write_Ex(binaryWriter, list.Count);
            foreach (var item in list)
            {
                var buffer = item.ToNative();
                BinaryWriterExpand.Write_Ex(binaryWriter, (short)buffer.Length);
                binaryWriter.Write(buffer);
            }
            return Convert.ToBase64String(stream.ToArray());
        }

        public static List<GroupMemberInfo> RawToList(byte[] buffer)
        {
            List<GroupMemberInfo> list = [];
            using BinaryReader binaryReader = new(new MemoryStream(buffer));

            int count = binaryReader.ReadInt32_Ex();
            for (int i = 0; i < count; i++)
            {
                list.Add(FromNative(binaryReader.ReadToken_Ex()));
            }
            return list;
        }
    }
}