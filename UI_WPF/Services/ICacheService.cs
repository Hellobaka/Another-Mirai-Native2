using System.Threading.Tasks;

namespace Another_Mirai_Native.UI.Services
{
    /// <summary>
    /// 缓存服务接口，统一管理好友、群、群成员信息缓存
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// 获取好友昵称
        /// </summary>
        /// <param name="qq">好友QQ号</param>
        /// <returns>昵称，失败时返回QQ号</returns>
        Task<string> GetFriendNickAsync(long qq);

        /// <summary>
        /// 获取群名称
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <returns>群名称，失败时返回群号</returns>
        Task<string> GetGroupNameAsync(long groupId);

        /// <summary>
        /// 获取群成员昵称（名片优先，否则返回昵称）
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <param name="qq">成员QQ号</param>
        /// <returns>名片或昵称，失败时返回QQ号</returns>
        Task<string> GetGroupMemberNickAsync(long groupId, long qq);

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        void ClearCache();

        /// <summary>
        /// 清空好友缓存
        /// </summary>
        void ClearFriendCache();

        /// <summary>
        /// 清空群缓存
        /// </summary>
        void ClearGroupCache();

        /// <summary>
        /// 清空指定群的成员缓存
        /// </summary>
        /// <param name="groupId">群号</param>
        void ClearGroupMemberCache(long groupId);
    }
}
