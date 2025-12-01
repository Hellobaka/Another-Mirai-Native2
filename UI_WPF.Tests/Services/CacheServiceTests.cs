using Another_Mirai_Native.UI.Services;
using Xunit;

namespace Another_Mirai_Native.UI.Tests.Services
{
    /// <summary>
    /// CacheService 单元测试
    /// 注意：由于 CacheService 依赖于 ProtocolManager 和 AppConfig，
    /// 某些测试需要在完整的运行时环境中执行。
    /// 此处测试主要验证缓存逻辑和接口行为。
    /// </summary>
    public class CacheServiceTests
    {
        #region 缓存清理测试

        [Fact]
        public void ClearCache_ShouldNotThrow()
        {
            // Arrange
            var cacheService = new CacheService();

            // Act & Assert - 不应抛出异常
            cacheService.ClearCache();
        }

        [Fact]
        public void ClearFriendCache_ShouldNotThrow()
        {
            // Arrange
            var cacheService = new CacheService();

            // Act & Assert
            cacheService.ClearFriendCache();
        }

        [Fact]
        public void ClearGroupCache_ShouldNotThrow()
        {
            // Arrange
            var cacheService = new CacheService();

            // Act & Assert
            cacheService.ClearGroupCache();
        }

        [Fact]
        public void ClearGroupMemberCache_ShouldNotThrow()
        {
            // Arrange
            var cacheService = new CacheService();
            long groupId = 12345;

            // Act & Assert
            cacheService.ClearGroupMemberCache(groupId);
        }

        [Fact]
        public void ClearCache_CanBeCalledMultipleTimes()
        {
            // Arrange
            var cacheService = new CacheService();

            // Act & Assert - 可以多次调用而不抛出异常
            cacheService.ClearCache();
            cacheService.ClearCache();
            cacheService.ClearCache();
        }

        [Fact]
        public void ClearGroupMemberCache_WithNonExistentGroup_ShouldNotThrow()
        {
            // Arrange
            var cacheService = new CacheService();
            long nonExistentGroupId = 99999999;

            // Act & Assert - 清理不存在的群组成员缓存不应抛出异常
            cacheService.ClearGroupMemberCache(nonExistentGroupId);
        }

        #endregion

        #region 接口实现验证

        [Fact]
        public void CacheService_ShouldImplementICacheService()
        {
            // Arrange & Act
            var cacheService = new CacheService();

            // Assert
            Assert.IsAssignableFrom<ICacheService>(cacheService);
        }

        #endregion

        #region 批量查询接口存在性验证

        [Fact]
        public async Task GetFriendNicksBatchAsync_WithEmptyList_ShouldReturnEmptyDictionary()
        {
            // Arrange
            var cacheService = new CacheService();
            var emptyList = new List<long>();

            // Act
            var result = await cacheService.GetFriendNicksBatchAsync(emptyList);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetGroupNamesBatchAsync_WithEmptyList_ShouldReturnEmptyDictionary()
        {
            // Arrange
            var cacheService = new CacheService();
            var emptyList = new List<long>();

            // Act
            var result = await cacheService.GetGroupNamesBatchAsync(emptyList);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetGroupMemberNicksBatchAsync_WithEmptyList_ShouldReturnEmptyDictionary()
        {
            // Arrange
            var cacheService = new CacheService();
            long groupId = 12345;
            var emptyList = new List<long>();

            // Act
            var result = await cacheService.GetGroupMemberNicksBatchAsync(groupId, emptyList);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region 多线程安全性测试

        [Fact]
        public async Task ClearCache_CalledFromMultipleThreads_ShouldNotThrow()
        {
            // Arrange
            var cacheService = new CacheService();
            var tasks = new List<Task>();

            // Act - 从多个线程同时调用清理方法
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() => cacheService.ClearCache()));
            }

            // Assert - 所有任务都应该成功完成
            await Task.WhenAll(tasks);
        }

        [Fact]
        public async Task ClearGroupMemberCache_CalledFromMultipleThreads_ShouldNotThrow()
        {
            // Arrange
            var cacheService = new CacheService();
            var tasks = new List<Task>();
            long groupId = 12345;

            // Act
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() => cacheService.ClearGroupMemberCache(groupId)));
            }

            // Assert
            await Task.WhenAll(tasks);
        }

        #endregion
    }
}
