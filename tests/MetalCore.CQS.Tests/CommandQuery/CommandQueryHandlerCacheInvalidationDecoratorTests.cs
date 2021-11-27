using MetalCore.CQS.CommandQuery;
using MetalCore.CQS.Common.Results;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.CommandQuery
{
    public class CommandQueryHandlerCacheInvalidationDecoratorTests
    {
        public class ConstructorMethods
        {
            private readonly Mock<ICommandQueryHandler<ICommandQuery<string>, string>> _commandQueryHandlerMock = new Mock<ICommandQueryHandler<ICommandQuery<string>, string>>();
            private readonly List<ICommandQueryCacheInvalidation<ICommandQuery<string>, string>> _CacheInvalidations = new List<ICommandQueryCacheInvalidation<ICommandQuery<string>, string>>();

            [Fact]
            public void IfNullCommandQueryHandler_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string>(null, _CacheInvalidations);

                // Assert
                Assert.Throws<ArgumentNullException>("commandQueryHandler", act);
            }

            [Fact]
            public void IfNullCacheInvalidations_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, null);

                // Assert
                Assert.Throws<ArgumentNullException>("cacheInvalidators", act);
            }
        }

        public class ExecuteAsyncMethods
        {
            private readonly Mock<ICommandQueryHandler<ICommandQuery<string>, string>> _commandQueryHandlerMock = new Mock<ICommandQueryHandler<ICommandQuery<string>, string>>();
            private readonly Mock<ICommandQuery<string>> _commandQueryMock = new Mock<ICommandQuery<string>>();
            private readonly Mock<IResult<string>> _resultMock = new Mock<IResult<string>>();
            private readonly List<ICommandQueryCacheInvalidation<ICommandQuery<string>, string>> _CacheInvalidations = new List<ICommandQueryCacheInvalidation<ICommandQuery<string>, string>>();
            private readonly CancellationToken _token = default;

            [Fact]
            public async Task IfNullCommandQuery_ThenCommandQueryPushedToNextCommandQueryHandler()
            {
                // Arrange
                CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _CacheInvalidations);

                // Act
                await target.ExecuteAsync(null, _token);

                // Assert
                _commandQueryHandlerMock.Verify(a => a.ExecuteAsync(null, _token), Times.Once);
            }

            [Fact]
            public async Task IfNullCommandQuery_ThenOriginalResultReturned()
            {
                // Arrange
                CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _CacheInvalidations);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(null, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.ExecuteAsync(null, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfNonNullCommandQuery_ThenCommandQueryPushedToNextCommandQueryHandler()
            {
                // Arrange
                CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _CacheInvalidations);

                // Act
                await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                _commandQueryHandlerMock.Verify(a => a.ExecuteAsync(_commandQueryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfNonNullCommandQuery_ThenOriginalResultReturned()
            {
                // Arrange
                CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _CacheInvalidations);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGivenNoSuccess_ThenInvalidateCacheAsyncNotCalled()
            {
                // Arrange
                CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _CacheInvalidations);

                Mock<ICommandQueryCacheInvalidation<ICommandQuery<string>, string>> CacheInvalidation = new Mock<ICommandQueryCacheInvalidation<ICommandQuery<string>, string>>();
                _CacheInvalidations.Add(CacheInvalidation.Object);

                _resultMock.Setup(a => a.IsSuccessful).Returns(false);

                // Act
                await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                CacheInvalidation.Verify(a => a.InvalidateCacheAsync(It.IsAny<ICommandQuery<string>>(), _token), Times.Never);
            }

            [Fact]
            public async Task IfGivenNoSuccess_ThenOriginalResultReturned()
            {
                // Arrange
                CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _CacheInvalidations);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));
                _resultMock.Setup(a => a.IsSuccessful).Returns(false);

                // Act
                IResult<string> result = await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGivenNullResult_ThenInvalidateCacheAsyncNotCalled()
            {
                // Arrange
                CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _CacheInvalidations);

                Mock<ICommandQueryCacheInvalidation<ICommandQuery<string>, string>> CacheInvalidation = new Mock<ICommandQueryCacheInvalidation<ICommandQuery<string>, string>>();
                _CacheInvalidations.Add(CacheInvalidation.Object);

                // Act
                await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                CacheInvalidation.Verify(a => a.InvalidateCacheAsync(It.IsAny<ICommandQuery<string>>(), _token), Times.Never);
            }

            [Fact]
            public async Task IfGiven1CommandQueryCacheInvalidation_ThenInvalidateCacheAsyncCalled()
            {
                // Arrange
                CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _CacheInvalidations);

                Mock<ICommandQueryCacheInvalidation<ICommandQuery<string>, string>> CacheInvalidation = new Mock<ICommandQueryCacheInvalidation<ICommandQuery<string>, string>>();
                _CacheInvalidations.Add(CacheInvalidation.Object);

                _resultMock.Setup(a => a.IsSuccessful).Returns(true);
                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                CacheInvalidation.Verify(a => a.InvalidateCacheAsync(_commandQueryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven1CommandQueryCacheInvalidation_ThenOriginalResultReturned()
            {
                // Arrange
                CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _CacheInvalidations);

                Mock<ICommandQueryCacheInvalidation<ICommandQuery<string>, string>> CacheInvalidation = new Mock<ICommandQueryCacheInvalidation<ICommandQuery<string>, string>>();
                _CacheInvalidations.Add(CacheInvalidation.Object);

                _resultMock.Setup(a => a.IsSuccessful).Returns(true);
                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGiven2CommandQueryCacheInvalidations_ThenInvalidateCacheAsyncCalledForEach()
            {
                // Arrange
                CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _CacheInvalidations);

                Mock<ICommandQueryCacheInvalidation<ICommandQuery<string>, string>> CacheInvalidation = new Mock<ICommandQueryCacheInvalidation<ICommandQuery<string>, string>>();
                _CacheInvalidations.Add(CacheInvalidation.Object);

                Mock<ICommandQueryCacheInvalidation<ICommandQuery<string>, string>> CacheInvalidation2 = new Mock<ICommandQueryCacheInvalidation<ICommandQuery<string>, string>>();
                _CacheInvalidations.Add(CacheInvalidation2.Object);

                _resultMock.Setup(a => a.IsSuccessful).Returns(true);
                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                CacheInvalidation.Verify(a => a.InvalidateCacheAsync(_commandQueryMock.Object, _token), Times.Once);
                CacheInvalidation2.Verify(a => a.InvalidateCacheAsync(_commandQueryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandQueryCacheInvalidations_OriginalResultReturned()
            {
                // Arrange
                CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _CacheInvalidations);

                _resultMock.Setup(a => a.IsSuccessful).Returns(true);
                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGiven2CommandQueryCacheInvalidations_ThenEachRunOnSeparateThreads()
            {
                // Arrange
                const int WAIT_TIME = 500;

                CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerCacheInvalidationDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _CacheInvalidations);

                Mock<ICommandQueryCacheInvalidation<ICommandQuery<string>, string>> CacheInvalidation = new Mock<ICommandQueryCacheInvalidation<ICommandQuery<string>, string>>();
                CacheInvalidation.Setup(a => a.InvalidateCacheAsync(_commandQueryMock.Object, _token))
                    .Returns(Task.CompletedTask).Callback(() => Thread.Sleep(WAIT_TIME));
                _CacheInvalidations.Add(CacheInvalidation.Object);

                Mock<ICommandQueryCacheInvalidation<ICommandQuery<string>, string>> CacheInvalidation2 = new Mock<ICommandQueryCacheInvalidation<ICommandQuery<string>, string>>();
                CacheInvalidation2.Setup(a => a.InvalidateCacheAsync(_commandQueryMock.Object, _token))
                    .Returns(Task.CompletedTask).Callback(() => Thread.Sleep(WAIT_TIME));
                _CacheInvalidations.Add(CacheInvalidation2.Object);

                _resultMock.Setup(a => a.IsSuccessful).Returns(true);
                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await target.ExecuteAsync(_commandQueryMock.Object, _token);
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }
        }
    }
}