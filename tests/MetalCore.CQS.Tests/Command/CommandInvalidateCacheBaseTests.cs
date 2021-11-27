using MetalCore.CQS.Command;
using MetalCore.CQS.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.Command
{
    public class CommandCacheInvalidationBaseTests
    {
        public abstract class CommandCacheInvalidationStub : CommandCacheInvalidationBase<ICommand>
        {
            public ICommand CommandPublic => Command;

            public CommandCacheInvalidationStub(IQueryCacheRegion queryCacheRegion) : base(queryCacheRegion) { }

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
            private readonly Mock<ICommand> _commandMock = new Mock<ICommand>();
            private readonly Mock<IQueryCacheRegion> _queryCacheRegionMock = new Mock<IQueryCacheRegion>();
            private readonly CancellationToken _token = default;

            [Fact]
            public async Task IfNullCommand_ThenArgumentNullExceptionThrown()
            {
                // Arrange
                Mock<CommandCacheInvalidationStub> target = new Mock<CommandCacheInvalidationStub>(_queryCacheRegionMock.Object) { CallBase = true };

                // Act
                Func<Task> act = async () => await target.Object.InvalidateCacheAsync(null, _token);

                // Assert
                await Assert.ThrowsAsync<ArgumentNullException>(act);
            }

            [Fact]
            public async Task IfNonNullCommandAnddGetTypeOfQueriesToInvalidateAsyncReturnsNull_GetTypeOfQueriesToInvalidateAsyncCalledOnce()
            {
                // Arrange
                Mock<CommandCacheInvalidationStub> target = new Mock<CommandCacheInvalidationStub>(_queryCacheRegionMock.Object) { CallBase = true };

                // Act
                await target.Object.InvalidateCacheAsync(_commandMock.Object, _token);

                // Assert
                target.Verify(a => a.GetTypeOfQueriesToInvalidatePublicAsync(_token), Times.Once());
            }

            [Fact]
            public async Task IfNonNullCommandAnddGetTypeOfQueriesToInvalidateAsyncReturnsNull_QueryCacheRegionNeverCalled()
            {
                // Arrange
                Mock<CommandCacheInvalidationBase<ICommand>> target = new Mock<CommandCacheInvalidationBase<ICommand>>(_queryCacheRegionMock.Object);

                // Act
                await target.Object.InvalidateCacheAsync(_commandMock.Object, _token);

                // Assert
                _queryCacheRegionMock.Verify(a => a.GetCacheRegion(It.IsAny<Type>()), Times.Never);
            }

            [Fact]
            public async Task IfNonNullCommandAndGetTypeOfQueriesToInvalidateAsyncReturns0_GetTypeOfQueriesToInvalidateAsyncCalledOnce()
            {
                // Arrange
                Mock<CommandCacheInvalidationStub> target = new Mock<CommandCacheInvalidationStub>(_queryCacheRegionMock.Object) { CallBase = true };
                target.Setup(a => a.GetTypeOfQueriesToInvalidatePublicAsync(_token)).ReturnsAsync(new List<Type>());

                // Act
                await target.Object.InvalidateCacheAsync(_commandMock.Object, _token);

                // Assert
                target.Verify(a => a.GetTypeOfQueriesToInvalidatePublicAsync(_token), Times.Once());
            }

            [Fact]
            public async Task IfNonNullCommandAndGetTypeOfQueriesToInvalidateAsyncReturns0_QueryCacheRegionNeverCalled()
            {
                // Arrange
                Mock<CommandCacheInvalidationStub> target = new Mock<CommandCacheInvalidationStub>(_queryCacheRegionMock.Object) { CallBase = true };
                target.Setup(a => a.GetTypeOfQueriesToInvalidatePublicAsync(_token)).ReturnsAsync(new List<Type>());

                // Act
                await target.Object.InvalidateCacheAsync(_commandMock.Object, _token);

                // Assert
                _queryCacheRegionMock.Verify(a => a.GetCacheRegion(It.IsAny<Type>()), Times.Never);
            }

            [Fact]
            public async Task IfNonNullCommandAndGetTypeOfQueriesToInvalidateAsyncReturn1_IQueryCacheRegionCalledOnce()
            {
                // Arrange
                List<Type> typeList = new List<Type>() { typeof(IQueryCacheRegion) };

                Mock<CommandCacheInvalidationStub> target = new Mock<CommandCacheInvalidationStub>(_queryCacheRegionMock.Object) { CallBase = true };
                target.Setup(a => a.GetTypeOfQueriesToInvalidatePublicAsync(_token)).ReturnsAsync(typeList);

                // Act
                await target.Object.InvalidateCacheAsync(_commandMock.Object, _token);

                // Assert
                _queryCacheRegionMock.Verify(a => a.GetCacheRegion(typeof(IQueryCacheRegion)), Times.Once);
            }

            [Fact]
            public async Task IfNonNullCommandAndGetTypeOfQueriesToInvalidateAsyncReturn1_ClearRegionCacheAsyncCalledOnce()
            {
                // Arrange
                List<Type> typeList = new List<Type>() { typeof(IQueryCacheRegion) };

                Mock<CommandCacheInvalidationStub> target = new Mock<CommandCacheInvalidationStub>(_queryCacheRegionMock.Object) { CallBase = true };
                target.Setup(a => a.GetTypeOfQueriesToInvalidatePublicAsync(_token)).ReturnsAsync(typeList);

                // Act
                await target.Object.InvalidateCacheAsync(_commandMock.Object, _token);

                // Assert
                target.Verify(a => a.ClearRegionCachePublicAsync(It.IsAny<string>(), _token), Times.Once());
            }

            [Fact]
            public async Task IfNonNullCommandAndGetTypeOfQueriesToInvalidateAsyncReturn2_IQueryCacheRegionCalledForEachType()
            {
                // Arrange
                List<Type> typeList = new List<Type>() { typeof(IQueryCacheRegion), typeof(string) };

                Mock<CommandCacheInvalidationStub> target = new Mock<CommandCacheInvalidationStub>(_queryCacheRegionMock.Object) { CallBase = true };
                target.Setup(a => a.GetTypeOfQueriesToInvalidatePublicAsync(_token)).ReturnsAsync(typeList);

                // Act
                await target.Object.InvalidateCacheAsync(_commandMock.Object, _token);

                // Assert
                _queryCacheRegionMock.Verify(a => a.GetCacheRegion(typeof(IQueryCacheRegion)), Times.Once);
                _queryCacheRegionMock.Verify(a => a.GetCacheRegion(typeof(string)), Times.Once);
            }

            [Fact]
            public async Task IfNonNullCommandAndGetTypeOfQueriesToInvalidateAsyncReturn2_ClearRegionCacheAsyncCalledTwice()
            {
                // Arrange
                List<Type> typeList = new List<Type>() { typeof(IQueryCacheRegion), typeof(string) };

                Mock<CommandCacheInvalidationStub> target = new Mock<CommandCacheInvalidationStub>(_queryCacheRegionMock.Object) { CallBase = true };
                target.Setup(a => a.GetTypeOfQueriesToInvalidatePublicAsync(_token)).ReturnsAsync(typeList);

                // Act
                await target.Object.InvalidateCacheAsync(_commandMock.Object, _token);

                // Assert
                target.Verify(a => a.ClearRegionCachePublicAsync(It.IsAny<string>(), _token), Times.Exactly(2));
            }

            [Fact]
            public async Task IfNonNullCommandAndGetTypeOfQueriesToInvalidateAsyncReturn1_CommandPropertyReturnsCommand()
            {
                // Arrange
                Mock<CommandCacheInvalidationStub> target = new Mock<CommandCacheInvalidationStub>(_queryCacheRegionMock.Object) { CallBase = true };

                // Act
                await target.Object.InvalidateCacheAsync(_commandMock.Object, _token);

                // Assert
                Assert.Equal(_commandMock.Object, target.Object.CommandPublic);
            }
        }
    }
}