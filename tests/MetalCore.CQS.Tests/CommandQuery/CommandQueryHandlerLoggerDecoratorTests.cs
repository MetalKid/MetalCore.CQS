using MetalCore.CQS.CommandQuery;
using MetalCore.CQS.Common.Results;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.CommandQuery
{
    public class CommandQueryHandlerLoggerDecoratorTests
    {
        public class ConstructorMethods
        {
            private readonly Mock<ICommandQueryHandler<ICommandQuery<string>, string>> _commandQueryHandlerMock = new Mock<ICommandQueryHandler<ICommandQuery<string>, string>>();
            private readonly List<ICommandQueryLogger<ICommandQuery<string>, string>> _loggers = new List<ICommandQueryLogger<ICommandQuery<string>, string>>();

            [Fact]
            public void IfNullCommandQueryHandler_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(null, _loggers);

                // Assert
                Assert.Throws<ArgumentNullException>("commandQueryHandler", act);
            }

            [Fact]
            public void IfNullCacheInvalidations_ThenNotNull()
            {
                // Arrange

                // Act
                CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string> result = new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, null);

                // Assert
                Assert.NotNull(result);
            }
        }

        public class ExecuteAsyncMethods
        {
            private readonly Mock<ICommandQueryHandler<ICommandQuery<string>, string>> _commandQueryHandlerMock = new Mock<ICommandQueryHandler<ICommandQuery<string>, string>>();
            private readonly Mock<ICommandQuery<string>> _commandQueryMock = new Mock<ICommandQuery<string>>();
            private readonly Mock<IResult<string>> _resultMock = new Mock<IResult<string>>();
            private readonly List<ICommandQueryLogger<ICommandQuery<string>, string>> _loggers = new List<ICommandQueryLogger<ICommandQuery<string>, string>>();
            private readonly CancellationToken _token = default;

            [Fact]
            public async Task IfNullCommandQuery_ThenCommandQueryPushedToNextCommandQueryHandler()
            {
                // Arrange
                CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _loggers);

                // Act
                await target.ExecuteAsync(null, _token);

                // Assert
                _commandQueryHandlerMock.Verify(a => a.ExecuteAsync(null, _token), Times.Once);
            }

            [Fact]
            public async Task IfNullCommandQuery_ThenOriginalResultReturned()
            {
                // Arrange
                CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _loggers);

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
                CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _loggers);

                // Act
                await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                _commandQueryHandlerMock.Verify(a => a.ExecuteAsync(_commandQueryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfNonNullCommandQuery_ThenOriginalResultReturned()
            {
                // Arrange
                CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _loggers);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGiven1CommandQueryLogger_ThenStartMethodCalled()
            {
                // Arrange
                CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _loggers);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                // Act
                await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                loggerMock.Verify(a => a.LogStartAsync(_commandQueryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandQueryLoggers_ThenStartMethodCalledForEach()
            {
                // Arrange
                CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _loggers);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock2 = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                _loggers.Add(loggerMock2.Object);

                // Act
                await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                loggerMock.Verify(a => a.LogStartAsync(_commandQueryMock.Object, _token), Times.Once);
                loggerMock2.Verify(a => a.LogStartAsync(_commandQueryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandQueryLoggers_ThenStartMethodCalledForEachInParallel()
            {
                const int WAIT_TIME = 500;

                // Arrange
                CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _loggers);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                loggerMock.Setup(a => a.LogStartAsync(_commandQueryMock.Object, _token)).Returns(Task.CompletedTask)
                    .Callback(() => Thread.Sleep(WAIT_TIME));
                _loggers.Add(loggerMock.Object);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock2 = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                loggerMock2.Setup(a => a.LogStartAsync(_commandQueryMock.Object, _token)).Returns(Task.CompletedTask)
                    .Callback(() => Thread.Sleep(WAIT_TIME));
                _loggers.Add(loggerMock2.Object);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                await target.ExecuteAsync(_commandQueryMock.Object, _token);
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }

            [Fact]
            public async Task IfGiven1CommandQueryLoggerWithException_ThenStartMethodCalled()
            {
                // Arrange
                CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _loggers);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Throws<Exception>();

                // Act
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_commandQueryMock.Object, _token));

                // Assert
                loggerMock.Verify(a => a.LogStartAsync(_commandQueryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandQueryLoggersWithException_ThenStartMethodCalledForEach()
            {
                // Arrange
                CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _loggers);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock2 = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                _loggers.Add(loggerMock2.Object);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Throws<Exception>();

                // Act
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_commandQueryMock.Object, _token));

                // Assert
                loggerMock.Verify(a => a.LogStartAsync(_commandQueryMock.Object, _token), Times.Once);
                loggerMock2.Verify(a => a.LogStartAsync(_commandQueryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven1CommandQueryLogger_ThenEndMethodCalled()
            {
                // Arrange
                CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _loggers);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                // Act
                IResult<string> result = await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                loggerMock.Verify(a => a.LogEndAsync(_commandQueryMock.Object, result, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandQueryLoggers_ThenEndMethodCalledForEach()
            {
                // Arrange
                CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _loggers);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock2 = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                _loggers.Add(loggerMock2.Object);

                // Act
                IResult<string> result = await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                loggerMock.Verify(a => a.LogEndAsync(_commandQueryMock.Object, result, _token), Times.Once);
                loggerMock2.Verify(a => a.LogEndAsync(_commandQueryMock.Object, result, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandQueryLoggers_ThenEndMethodCalledForEachInParallel()
            {
                const int WAIT_TIME = 500;

                // Arrange
                CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _loggers);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                loggerMock.Setup(a => a.LogEndAsync(_commandQueryMock.Object, It.IsAny<IResult<string>>(), _token)).Returns(Task.CompletedTask)
                    .Callback(() => Thread.Sleep(WAIT_TIME));
                _loggers.Add(loggerMock.Object);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock2 = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                loggerMock2.Setup(a => a.LogEndAsync(_commandQueryMock.Object, It.IsAny<IResult<string>>(), _token)).Returns(Task.CompletedTask)
                    .Callback(() => Thread.Sleep(WAIT_TIME));
                _loggers.Add(loggerMock2.Object);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                IResult<string> result = await target.ExecuteAsync(_commandQueryMock.Object, _token);
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }

            [Fact]
            public async Task IfGiven1CommandQueryLoggerWithException_ThenEndMethodNotCalled()
            {
                // Arrange
                CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _loggers);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Throws<Exception>();

                // Act
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_commandQueryMock.Object, _token));

                // Assert
                loggerMock.Verify(a => a.LogEndAsync(_commandQueryMock.Object, It.IsAny<IResult<string>>(), _token), Times.Never);
            }

            [Fact]
            public async Task IfGiven2CommandQueryLoggersWithException_ThenEndMethodNotCalledForEach()
            {
                // Arrange
                CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _loggers);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock2 = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                _loggers.Add(loggerMock2.Object);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Throws<Exception>();

                // Act
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_commandQueryMock.Object, _token));

                // Assert
                loggerMock.Verify(a => a.LogEndAsync(_commandQueryMock.Object, It.IsAny<IResult<string>>(), _token), Times.Never);
                loggerMock2.Verify(a => a.LogEndAsync(_commandQueryMock.Object, It.IsAny<IResult<string>>(), _token), Times.Never);
            }

            [Fact]
            public async Task IfGiven1CommandQueryLoggerNoException_ThenErrorMethodNotCalled()
            {
                // Arrange
                CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _loggers);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                // Act
                await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                loggerMock.Verify(a => a.LogErrorAsync(_commandQueryMock.Object, It.IsAny<Exception>(), _token), Times.Never);
            }

            [Fact]
            public async Task IfGiven2CommandQueryLoggersNoException_ThenErrortMethodNotCalledForEach()
            {
                // Arrange
                CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _loggers);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock2 = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                _loggers.Add(loggerMock2.Object);

                // Act
                await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                loggerMock.Verify(a => a.LogErrorAsync(_commandQueryMock.Object, It.IsAny<Exception>(), _token), Times.Never);
                loggerMock2.Verify(a => a.LogErrorAsync(_commandQueryMock.Object, It.IsAny<Exception>(), _token), Times.Never);
            }

            [Fact]
            public async Task IfGiven1CommandQueryLoggerWithException_ThenErrorMethodCalled()
            {
                // Arrange
                CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _loggers);
                Exception ex = new Exception();

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Throws(ex);

                // Act
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_commandQueryMock.Object, _token));

                // Assert
                loggerMock.Verify(a => a.LogErrorAsync(_commandQueryMock.Object, ex, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandQueryLoggersWithException_ThenErrortMethodCalledForEach()
            {
                // Arrange
                CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _loggers);
                Exception ex = new Exception();

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock2 = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                _loggers.Add(loggerMock2.Object);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Throws(ex);

                // Act
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_commandQueryMock.Object, _token));

                // Assert
                loggerMock.Verify(a => a.LogErrorAsync(_commandQueryMock.Object, ex, _token), Times.Once);
                loggerMock2.Verify(a => a.LogErrorAsync(_commandQueryMock.Object, ex, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandQueryLoggersWithException_ThenErrortMethodCalledInParallel()
            {
                const int WAIT_TIME = 500;

                // Arrange
                CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerLoggerDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _loggers);
                Exception ex = new Exception();

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                loggerMock.Setup(a => a.LogErrorAsync(_commandQueryMock.Object, ex, _token)).Returns(Task.CompletedTask)
                    .Callback(() => Thread.Sleep(WAIT_TIME));
                _loggers.Add(loggerMock.Object);

                Mock<ICommandQueryLogger<ICommandQuery<string>, string>> loggerMock2 = new Mock<ICommandQueryLogger<ICommandQuery<string>, string>>();
                loggerMock2.Setup(a => a.LogErrorAsync(_commandQueryMock.Object, ex, _token)).Returns(Task.CompletedTask)
                    .Callback(() => Thread.Sleep(WAIT_TIME));
                _loggers.Add(loggerMock2.Object);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Throws(ex);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_commandQueryMock.Object, _token));
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }
        }
    }
}
