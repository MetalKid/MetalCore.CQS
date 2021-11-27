using MetalCore.CQS.CommandQuery;
using MetalCore.CQS.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.CommandQuery
{
    public class CommandQueryCacheInvalidationBaseTests
    {
        public abstract class CommandQueryCacheInvalidationStub : CommandQueryCacheInvalidationBase<ICommandQuery<string>, string>
        {
            public ICommandQuery<string> CommandQueryPublic => CommandQuery;

            public CommandQueryCacheInvalidationStub(IQueryCacheRegion queryCacheRegion) : base(queryCacheRegion) { }

            public abstract Task ClearRegionCachePublicAsync(string region, CancellationToken token = default);

            protected override Task ClearRegionCacheAsync(string region, CancellationToken token = default)
            {
                return ClearRegionCachePublicAsync(region, token);
            }

            public abstract Task<ICollection<Type>> GetTypeOfQueriesToInvalidatePublicAsync(CancellationToken token = default);

            protected override Task<ICollection<Type>> GetTypeOfQueriesToInvalidateAsync(CancellationToken token = default)
            {
                return GetTypeOfQueriesToInvalidatePublicAsync(token);
            }
        }

        public class InvalidateCacheAsync
        {
            private readonly Mock<ICommandQuery<string>> _commandQueryMock = new Mock<ICommandQuery<string>>();
            private readonly Mock<IQueryCacheRegion> _queryCacheRegionMock = new Mock<IQueryCacheRegion>();
            private readonly CancellationToken _token = default;

            [Fact]
            public async Task IfNullCommandQuery_ThenArgumentNullExceptionThrown()
            {
                // Arrange
                Mock<CommandQueryCacheInvalidationBase<ICommandQuery<string>, string>> target = new Mock<CommandQueryCacheInvalidationBase<ICommandQuery<string>, string>>(_queryCacheRegionMock.Object)
                {
                    CallBase = true
                };

                // Act
                Func<Task> act = async () => await target.Object.InvalidateCacheAsync(null, _token);

                // Assert
                await Assert.ThrowsAsync<ArgumentNullException>(act);
            }

            [Fact]
            public async Task IfNonNullCommandQueryAnddGetTypeOfQueriesToInvalidateAsyncReturnsNull_GetTypeOfQueriesToInvalidateAsyncCalledOnce()
            {
                // Arrange
                Mock<CommandQueryCacheInvalidationStub> target = new Mock<CommandQueryCacheInvalidationStub>(_queryCacheRegionMock.Object) { CallBase = true };

                // Act
                await target.Object.InvalidateCacheAsync(_commandQueryMock.Object, _token);

                // Assert
                target.Verify(a => a.GetTypeOfQueriesToInvalidatePublicAsync(_token), Times.Once());
            }

            [Fact]
            public async Task IfNonNullCommandQueryAnddGetTypeOfQueriesToInvalidateAsyncReturnsNull_QueryCacheRegionNeverCalled()
            {
                // Arrange
                Mock<CommandQueryCacheInvalidationBase<ICommandQuery<string>, string>> target = new Mock<CommandQueryCacheInvalidationBase<ICommandQuery<string>, string>>(_queryCacheRegionMock.Object);

                // Act
                await target.Object.InvalidateCacheAsync(_commandQueryMock.Object, _token);

                // Assert
                _queryCacheRegionMock.Verify(a => a.GetCacheRegion(It.IsAny<Type>()), Times.Never);
            }

            [Fact]
            public async Task IfNonNullCommandQueryAndGetTypeOfQueriesToInvalidateAsyncReturns0_GetTypeOfQueriesToInvalidateAsyncCalledOnce()
            {
                // Arrange
                Mock<CommandQueryCacheInvalidationStub> target = new Mock<CommandQueryCacheInvalidationStub>(_queryCacheRegionMock.Object) { CallBase = true };
                target.Setup(a => a.GetTypeOfQueriesToInvalidatePublicAsync(_token)).ReturnsAsync(new List<Type>());

                // Act
                await target.Object.InvalidateCacheAsync(_commandQueryMock.Object, _token);

                // Assert
                target.Verify(a => a.GetTypeOfQueriesToInvalidatePublicAsync(_token), Times.Once());
            }

            [Fact]
            public async Task IfNonNullCommandQueryAndGetTypeOfQueriesToInvalidateAsyncReturns0_QueryCacheRegionNeverCalled()
            {
                // Arrange
                Mock<CommandQueryCacheInvalidationStub> target = new Mock<CommandQueryCacheInvalidationStub>(_queryCacheRegionMock.Object) { CallBase = true };
                target.Setup(a => a.GetTypeOfQueriesToInvalidatePublicAsync(_token)).ReturnsAsync(new List<Type>());

                // Act
                await target.Object.InvalidateCacheAsync(_commandQueryMock.Object, _token);

                // Assert
                _queryCacheRegionMock.Verify(a => a.GetCacheRegion(It.IsAny<Type>()), Times.Never);
            }

            [Fact]
            public async Task IfNonNullCommandQueryAndGetTypeOfQueriesToInvalidateAsyncReturn1_IQueryCacheRegionCalledOnce()
            {
                // Arrange
                List<Type> typeList = new List<Type>() { typeof(IQueryCacheRegion) };

                Mock<CommandQueryCacheInvalidationStub> target = new Mock<CommandQueryCacheInvalidationStub>(_queryCacheRegionMock.Object) { CallBase = true };
                target.Setup(a => a.GetTypeOfQueriesToInvalidatePublicAsync(_token)).ReturnsAsync(typeList);

                // Act
                await target.Object.InvalidateCacheAsync(_commandQueryMock.Object, _token);

                // Assert
                _queryCacheRegionMock.Verify(a => a.GetCacheRegion(typeof(IQueryCacheRegion)), Times.Once);
            }

            [Fact]
            public async Task IfNonNullCommandQueryAndGetTypeOfQueriesToInvalidateAsyncReturn1_ClearRegionCacheAsyncCalledOnce()
            {
                // Arrange
                List<Type> typeList = new List<Type>() { typeof(IQueryCacheRegion) };

                Mock<CommandQueryCacheInvalidationStub> target = new Mock<CommandQueryCacheInvalidationStub>(_queryCacheRegionMock.Object) { CallBase = true };
                target.Setup(a => a.GetTypeOfQueriesToInvalidatePublicAsync(_token)).ReturnsAsync(typeList);

                // Act
                await target.Object.InvalidateCacheAsync(_commandQueryMock.Object, _token);

                // Assert
                target.Verify(a => a.ClearRegionCachePublicAsync(It.IsAny<string>(), _token), Times.Once());
            }

            [Fact]
            public async Task IfNonNullCommandQueryAndGetTypeOfQueriesToInvalidateAsyncReturn2_IQueryCacheRegionCalledForEachType()
            {
                // Arrange
                List<Type> typeList = new List<Type>() { typeof(IQueryCacheRegion), typeof(string) };

                Mock<CommandQueryCacheInvalidationStub> target = new Mock<CommandQueryCacheInvalidationStub>(_queryCacheRegionMock.Object) { CallBase = true };
                target.Setup(a => a.GetTypeOfQueriesToInvalidatePublicAsync(_token)).ReturnsAsync(typeList);

                // Act
                await target.Object.InvalidateCacheAsync(_commandQueryMock.Object, _token);

                // Assert
                _queryCacheRegionMock.Verify(a => a.GetCacheRegion(typeof(IQueryCacheRegion)), Times.Once);
                _queryCacheRegionMock.Verify(a => a.GetCacheRegion(typeof(string)), Times.Once);
            }

            [Fact]
            public async Task IfNonNullCommandQueryAndGetTypeOfQueriesToInvalidateAsyncReturn2_ClearRegionCacheAsyncCalledTwice()
            {
                // Arrange
                List<Type> typeList = new List<Type>() { typeof(IQueryCacheRegion), typeof(string) };

                Mock<CommandQueryCacheInvalidationStub> target = new Mock<CommandQueryCacheInvalidationStub>(_queryCacheRegionMock.Object) { CallBase = true };
                target.Setup(a => a.GetTypeOfQueriesToInvalidatePublicAsync(_token)).ReturnsAsync(typeList);

                // Act
                await target.Object.InvalidateCacheAsync(_commandQueryMock.Object, _token);

                // Assert
                target.Verify(a => a.ClearRegionCachePublicAsync(It.IsAny<string>(), _token), Times.Exactly(2));
            }

            [Fact]
            public async Task IfNonNullCommandQueryAndGetTypeOfQueriesToInvalidateAsyncReturn1_CommandQueryPropertyReturnsCommandQuery()
            {
                // Arrange
                Mock<CommandQueryCacheInvalidationStub> target = new Mock<CommandQueryCacheInvalidationStub>(_queryCacheRegionMock.Object) { CallBase = true };

                // Act
                await target.Object.InvalidateCacheAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.Equal(_commandQueryMock.Object, target.Object.CommandQueryPublic);
            }
        }
    }
}