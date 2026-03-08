namespace Another_Mirai_Native.Abstractions.Models
{
    /// <summary>
    /// 表示描述 群文件信息 的类
    /// </summary>
    public class GroupFileInfo
    {
        /// <summary>
        /// 获取一个值, 指示当前文件的 Busid (唯一标识符)
        /// </summary>
        public int Id { get; private set; }
        
        /// <summary>
        /// 获取一个值, 指示当前文件的名称
        /// </summary>
        public string FileName { get; private set; }
        
        /// <summary>
        /// 获取一个值, 指示当前文件的Id
        /// </summary>
        public string FileId { get; private set; }
        
        /// <summary>
        /// 获取一个值, 指示当前文件的大小
        /// </summary>
        public long FileSize { get; private set; }
    }
}
