using MetalCore.CQS.Command;
using MetalCore.CQS.Common.Results;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.Command
{
    public class CommandHandlerCacheInvalidationDecoratorTests
    {
        public class ConstructorMethods
        {
            private readonly Mock<ICommandHandler<ICommand>> _commandHandlerMock = new Mock<ICommandHandler<ICommand>>();
            private readonly List<ICommandCacheInvalidation<ICommand>> _CacheInvalidations = new List<ICommandCacheInvalidation<ICommand>>();

            [Fact]
            public void IfNullCommandHandler_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new CommandHandlerCacheInvalidationDecorator<ICommand>(null, _CacheInvalidations);

                // Assert
                Assert.Throws<ArgumentNullException>("commandHandler", act);
            }

            [Fact]
            public void IfNullCacheInvalidations_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new CommandHandlerCacheInvalidationDecorator<ICommand>(_commandHandlerMock.Object, null);

                // Assert
                Assert.Throws<ArgumentNullException>("cacheInvalidators", act);
            }
        }

        public class ExecuteAsyncMethods
        {
            private readonly Mock<ICommandHandler<ICommand>> _commandHandlerMock = new Mock<ICommandHandler<ICommand>>();
            private readonly Mock<ICommand> _commandMock = new Mock<ICommand>();
            private readonly Mock<IResult> _resultMock = new Mock<IResult>();
            private readonly List<ICommandCacheInvalidation<ICommand>> _CacheInvalidations = new List<ICommandCacheInvalidation<ICommand>>();
            private readonly CancellationToken _token = default;

            [Fact]
            public async Task IfNullCommand_ThenCommandPushedToNextCommandHandler()
            {
                // Arrange
                CommandHandlerCacheInvalidationDecorator<ICommand> target = new CommandHandlerCacheInvalidationDecorator<ICommand>(_commandHandlerMock.Object, _CacheInvalidations);

                // Act
                await target.ExecuteAsync(null, _token);

                // Assert
                _commandHandlerMock.Verify(a => a.ExecuteAsync(null, _token), Times.Once);
            }

            [Fact]
            public async Task IfNullCommand_ThenOriginalResultReturned()
            {
                // Arrange
                CommandHandlerCacheInvalidationDecorator<ICommand> target = new CommandHandlerCacheInvalidationDecorator<ICommand>(_commandHandlerMock.Object, _CacheInvalidations);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(null, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult result = await target.ExecuteAsync(null, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfNonNullCommand_ThenCommandPushedToNextCommandHandler()
            {
                // Arrange
                CommandHandlerCacheInvalidationDecorator<ICommand> target = new CommandHandlerCacheInvalidationDecorator<ICommand>(_commandHandlerMock.Object, _CacheInvalidations);

                // Act
                await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                _commandHandlerMock.Verify(a => a.ExecuteAsync(_commandMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfNonNullCommand_ThenOriginalResultReturned()
            {
                // Arrange
                CommandHandlerCacheInvalidationDecorator<ICommand> target = new CommandHandlerCacheInvalidationDecorator<ICommand>(_commandHandlerMock.Object, _CacheInvalidations);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult result = await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGivenNoSuccess_ThenInvalidateCacheAsyncNotCalled()
            {
                // Arrange
                CommandHandlerCacheInvalidationDecorator<ICommand> target = new CommandHandlerCacheInvalidationDecorator<ICommand>(_commandHandlerMock.Object, _CacheInvalidations);

                Mock<ICommandCacheInvalidation<ICommand>> CacheInvalidation = new Mock<ICommandCacheInvalidation<ICommand>>();
                _CacheInvalidations.Add(CacheInvalidation.Object);

                _resultMock.Setup(a => a.IsSuccessful).Returns(false);

                // Act
                await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                CacheInvalidation.Verify(a => a.InvalidateCacheAsync(It.IsAny<ICommand>(), _token), Times.Never);
            }

            [Fact]
            public async Task IfGivenNoSuccess_ThenOriginalResultReturned()
            {
                // Arrange
                CommandHandlerCacheInvalidationDecorator<ICommand> target = new CommandHandlerCacheInvalidationDecorator<ICommand>(_commandHandlerMock.Object, _CacheInvalidations);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));
                _resultMock.Setup(a => a.IsSuccessful).Returns(false);

                // Act
                IResult result = await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGivenNullResult_ThenInvalidateCacheAsyncNotCalled()
            {
                // Arrange
                CommandHandlerCacheInvalidationDecorator<ICommand> target = new CommandHandlerCacheInvalidationDecorator<ICommand>(_commandHandlerMock.Object, _CacheInvalidations);

                Mock<ICommandCacheInvalidation<ICommand>> CacheInvalidation = new Mock<ICommandCacheInvalidation<ICommand>>();
                _CacheInvalidations.Add(CacheInvalidation.Object);

                // Act
                await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                CacheInvalidation.Verify(a => a.InvalidateCacheAsync(It.IsAny<ICommand>(), _token), Times.Never);
            }

            [Fact]
            public async Task IfGiven1CommandCacheInvalidation_ThenInvalidateCacheAsyncCalled()
            {
                // Arrange
                CommandHandlerCacheInvalidationDecorator<ICommand> target = new CommandHandlerCacheInvalidationDecorator<ICommand>(_commandHandlerMock.Object, _CacheInvalidations);

                Mock<ICommandCacheInvalidation<ICommand>> CacheInvalidation = new Mock<ICommandCacheInvalidation<ICommand>>();
                _CacheInvalidations.Add(CacheInvalidation.Object);

                _resultMock.Setup(a => a.IsSuccessful).Returns(true);
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                CacheInvalidation.Verify(a => a.InvalidateCacheAsync(_commandMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven1CommandCacheInvalidation_ThenOriginalResultReturned()
            {
                // Arrange
                CommandHandlerCacheInvalidationDecorator<ICommand> target = new CommandHandlerCacheInvalidationDecorator<ICommand>(_commandHandlerMock.Object, _CacheInvalidations);

                Mock<ICommandCacheInvalidation<ICommand>> CacheInvalidation = new Mock<ICommandCacheInvalidation<ICommand>>();
                _CacheInvalidations.Add(CacheInvalidation.Object);

                _resultMock.Setup(a => a.IsSuccessful).Returns(true);
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult result = await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGiven2CommandCacheInvalidations_ThenInvalidateCacheAsyncCalledForEach()
            {
                // Arrange
                CommandHandlerCacheInvalidationDecorator<ICommand> target = new CommandHandlerCacheInvalidationDecorator<ICommand>(_commandHandlerMock.Object, _CacheInvalidations);

                Mock<ICommandCacheInvalidation<ICommand>> CacheInvalidation = new Mock<ICommandCacheInvalidation<ICommand>>();
                _CacheInvalidations.Add(CacheInvalidation.Object);

                Mock<ICommandCacheInvalidation<ICommand>> CacheInvalidation2 = new Mock<ICommandCacheInvalidation<ICommand>>();
                _CacheInvalidations.Add(CacheInvalidation2.Object);

                _resultMock.Setup(a => a.IsSuccessful).Returns(true);
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                CacheInvalidation.Verify(a => a.InvalidateCacheAsync(_commandMock.Object, _token), Times.Once);
                CacheInvalidation2.Verify(a => a.InvalidateCacheAsync(_commandMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandCacheInvalidations_OriginalResultReturned()
            {
                // Arrange
                CommandHandlerCacheInvalidationDecorator<ICommand> target = new CommandHandlerCacheInvalidationDecorator<ICommand>(_commandHandlerMock.Object, _CacheInvalidations);

                _resultMock.Setup(a => a.IsSuccessful).Returns(true);
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult result = await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGiven2CommandCacheInvalidations_ThenEachRunOnSeparateThreads()
            {
                // Arrange
                const int WAIT_TIME = 500;

                CommandHandlerCacheInvalidationDecorator<ICommand> target = new CommandHandlerCacheInvalidationDecorator<ICommand>(_commandHandlerMock.Object, _CacheInvalidations);

                Mock<ICommandCacheInvalidation<ICommand>> CacheInvalidation = new Mock<ICommandCacheInvalidation<ICommand>>();
                CacheInvalidation.Setup(a => a.InvalidateCacheAsync(_commandMock.Object, _token))
                    .Returns(Task.CompletedTask).Callback(() => Thread.Sleep(WAIT_TIME));
                _CacheInvalidations.Add(CacheInvalidation.Object);

                Mock<ICommandCacheInvalidation<ICommand>> CacheInvalidation2 = new Mock<ICommandCacheInvalidation<ICommand>>();
                CacheInvalidation2.Setup(a => a.InvalidateCacheAsync(_commandMock.Object, _token))
                    .Returns(Task.CompletedTask).Callback(() => Thread.Sleep(WAIT_TIME));
                _CacheInvalidations.Add(CacheInvalidation2.Object);

                _resultMock.Setup(a => a.IsSuccessful).Returns(true);
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await target.ExecuteAsync(_commandMock.Object, _token);
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }
        }
    }
}