namespace Another_Mirai_Native.Model
{
    /// <summary>
    /// 用于表示群信息的类
    /// </summary>
    public class GroupInfo
    {
        /// <summary>
        /// 获取一个值, 指示当前QQ群对象
        /// </summary>
        public long Group { get; set; } = 10001;

        /// <summary>
        /// 获取当前QQ群的名称
        /// </summary>
        public string Name { get; set; } = "Err";

        /// <summary>
        /// 获取一个值, 指示QQ群的当前人数;
        /// </summary>
        public int CurrentMemberCount { get; set; } = 1;

        /// <summary>
        /// 获取一个值, 指示当前QQ群最大可容纳的人数;
        /// </summary>
        public int MaxMemberCount { get; set; } = 10;

        /// <summary>
        /// 最后更新时间(时间戳)
        /// </summary>
        public long LastUpdateTime { get; set; }

        /// <summary>
        /// ToString 方法的重写, 用于提供当前群信息的字符串表示形式
        /// </summary>
        public override string ToString()
        {
            return $"ID={Group}; 名称={Name}; 当前人数={CurrentMemberCount}; 最大人数={MaxMemberCount}";
        }
    }
}