using MetalCore.CQS.Command;
using MetalCore.CQS.Common.Results;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.Command
{
    public class CommandHandlerLoggerDecoratorTests
    {
        public class ConstructorMethods
        {
            private readonly Mock<ICommandHandler<ICommand>> _commandHandlerMock = new Mock<ICommandHandler<ICommand>>();
            private readonly List<ICommandLogger<ICommand>> _loggers = new List<ICommandLogger<ICommand>>();

            [Fact]
            public void IfNullCommandHandler_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new CommandHandlerLoggerDecorator<ICommand>(null, _loggers);

                // Assert
                Assert.Throws<ArgumentNullException>("commandHandler", act);
            }

            [Fact]
            public void IfNullCacheInvalidations_ThenNotNull()
            {
                // Arrange

                // Act
                CommandHandlerLoggerDecorator<ICommand> result = new CommandHandlerLoggerDecorator<ICommand>(_commandHandlerMock.Object, null);

                // Assert
                Assert.NotNull(result);
            }
        }

        public class ExecuteAsyncMethods
        {
            private readonly Mock<ICommandHandler<ICommand>> _commandHandlerMock = new Mock<ICommandHandler<ICommand>>();
            private readonly Mock<ICommand> _commandMock = new Mock<ICommand>();
            private readonly Mock<IResult> _resultMock = new Mock<IResult>();
            private readonly List<ICommandLogger<ICommand>> _loggers = new List<ICommandLogger<ICommand>>();
            private readonly CancellationToken _token = default;

            [Fact]
            public async Task IfNullCommand_ThenCommandPushedToNextCommandHandler()
            {
                // Arrange
                CommandHandlerLoggerDecorator<ICommand> target = new CommandHandlerLoggerDecorator<ICommand>(_commandHandlerMock.Object, _loggers);

                // Act
                await target.ExecuteAsync(null, _token);

                // Assert
                _commandHandlerMock.Verify(a => a.ExecuteAsync(null, _token), Times.Once);
            }

            [Fact]
            public async Task IfNullCommand_ThenOriginalResultReturned()
            {
                // Arrange
                CommandHandlerLoggerDecorator<ICommand> target = new CommandHandlerLoggerDecorator<ICommand>(_commandHandlerMock.Object, _loggers);

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
                CommandHandlerLoggerDecorator<ICommand> target = new CommandHandlerLoggerDecorator<ICommand>(_commandHandlerMock.Object, _loggers);

                // Act
                await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                _commandHandlerMock.Verify(a => a.ExecuteAsync(_commandMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfNonNullCommand_ThenOriginalResultReturned()
            {
                // Arrange
                CommandHandlerLoggerDecorator<ICommand> target = new CommandHandlerLoggerDecorator<ICommand>(_commandHandlerMock.Object, _loggers);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult result = await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGiven1CommandLogger_ThenStartMethodCalled()
            {
                // Arrange
                CommandHandlerLoggerDecorator<ICommand> target = new CommandHandlerLoggerDecorator<ICommand>(_commandHandlerMock.Object, _loggers);

                Mock<ICommandLogger<ICommand>> loggerMock = new Mock<ICommandLogger<ICommand>>();
                _loggers.Add(loggerMock.Object);

                // Act
                await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                loggerMock.Verify(a => a.LogStartAsync(_commandMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandLoggers_ThenStartMethodCalledForEach()
            {
                // Arrange
                CommandHandlerLoggerDecorator<ICommand> target = new CommandHandlerLoggerDecorator<ICommand>(_commandHandlerMock.Object, _loggers);

                Mock<ICommandLogger<ICommand>> loggerMock = new Mock<ICommandLogger<ICommand>>();
                _loggers.Add(loggerMock.Object);

                Mock<ICommandLogger<ICommand>> loggerMock2 = new Mock<ICommandLogger<ICommand>>();
                _loggers.Add(loggerMock2.Object);

                // Act
                await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                loggerMock.Verify(a => a.LogStartAsync(_commandMock.Object, _token), Times.Once);
                loggerMock2.Verify(a => a.LogStartAsync(_commandMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandLoggers_ThenStartMethodCalledForEachInParallel()
            {
                const int WAIT_TIME = 500;

                // Arrange
                CommandHandlerLoggerDecorator<ICommand> target = new CommandHandlerLoggerDecorator<ICommand>(_commandHandlerMock.Object, _loggers);

                Mock<ICommandLogger<ICommand>> loggerMock = new Mock<ICommandLogger<ICommand>>();
                loggerMock.Setup(a => a.LogStartAsync(_commandMock.Object, _token)).Returns(Task.CompletedTask)
                    .Callback(() => Thread.Sleep(WAIT_TIME));
                _loggers.Add(loggerMock.Object);

                Mock<ICommandLogger<ICommand>> loggerMock2 = new Mock<ICommandLogger<ICommand>>();
                loggerMock2.Setup(a => a.LogStartAsync(_commandMock.Object, _token)).Returns(Task.CompletedTask)
                    .Callback(() => Thread.Sleep(WAIT_TIME));
                _loggers.Add(loggerMock2.Object);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                await target.ExecuteAsync(_commandMock.Object, _token);
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }

            [Fact]
            public async Task IfGiven1CommandLoggerWithException_ThenStartMethodCalled()
            {
                // Arrange
                CommandHandlerLoggerDecorator<ICommand> target = new CommandHandlerLoggerDecorator<ICommand>(_commandHandlerMock.Object, _loggers);

                Mock<ICommandLogger<ICommand>> loggerMock = new Mock<ICommandLogger<ICommand>>();
                _loggers.Add(loggerMock.Object);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Throws<Exception>();

                // Act
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_commandMock.Object, _token));

                // Assert
                loggerMock.Verify(a => a.LogStartAsync(_commandMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandLoggersWithException_ThenStartMethodCalledForEach()
            {
                // Arrange
                CommandHandlerLoggerDecorator<ICommand> target = new CommandHandlerLoggerDecorator<ICommand>(_commandHandlerMock.Object, _loggers);

                Mock<ICommandLogger<ICommand>> loggerMock = new Mock<ICommandLogger<ICommand>>();
                _loggers.Add(loggerMock.Object);

                Mock<ICommandLogger<ICommand>> loggerMock2 = new Mock<ICommandLogger<ICommand>>();
                _loggers.Add(loggerMock2.Object);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Throws<Exception>();

                // Act
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_commandMock.Object, _token));

                // Assert
                loggerMock.Verify(a => a.LogStartAsync(_commandMock.Object, _token), Times.Once);
                loggerMock2.Verify(a => a.LogStartAsync(_commandMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven1CommandLogger_ThenEndMethodCalled()
            {
                // Arrange
                CommandHandlerLoggerDecorator<ICommand> target = new CommandHandlerLoggerDecorator<ICommand>(_commandHandlerMock.Object, _loggers);

                Mock<ICommandLogger<ICommand>> loggerMock = new Mock<ICommandLogger<ICommand>>();
                _loggers.Add(loggerMock.Object);

                // Act
                IResult result = await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                loggerMock.Verify(a => a.LogEndAsync(_commandMock.Object, result, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandLoggers_ThenEndMethodCalledForEach()
            {
                // Arrange
                CommandHandlerLoggerDecorator<ICommand> target = new CommandHandlerLoggerDecorator<ICommand>(_commandHandlerMock.Object, _loggers);

                Mock<ICommandLogger<ICommand>> loggerMock = new Mock<ICommandLogger<ICommand>>();
                _loggers.Add(loggerMock.Object);

                Mock<ICommandLogger<ICommand>> loggerMock2 = new Mock<ICommandLogger<ICommand>>();
                _loggers.Add(loggerMock2.Object);

                // Act
                IResult result = await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                loggerMock.Verify(a => a.LogEndAsync(_commandMock.Object, result, _token), Times.Once);
                loggerMock2.Verify(a => a.LogEndAsync(_commandMock.Object, result, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandLoggers_ThenEndMethodCalledForEachInParallel()
            {
                const int WAIT_TIME = 500;

                // Arrange
                CommandHandlerLoggerDecorator<ICommand> target = new CommandHandlerLoggerDecorator<ICommand>(_commandHandlerMock.Object, _loggers);

                Mock<ICommandLogger<ICommand>> loggerMock = new Mock<ICommandLogger<ICommand>>();
                loggerMock.Setup(a => a.LogEndAsync(_commandMock.Object, It.IsAny<IResult>(), _token)).Returns(Task.CompletedTask)
                    .Callback(() => Thread.Sleep(WAIT_TIME));
                _loggers.Add(loggerMock.Object);

                Mock<ICommandLogger<ICommand>> loggerMock2 = new Mock<ICommandLogger<ICommand>>();
                loggerMock2.Setup(a => a.LogEndAsync(_commandMock.Object, It.IsAny<IResult>(), _token)).Returns(Task.CompletedTask)
                    .Callback(() => Thread.Sleep(WAIT_TIME));
                _loggers.Add(loggerMock2.Object);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                IResult result = await target.ExecuteAsync(_commandMock.Object, _token);
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }

            [Fact]
            public async Task IfGiven1CommandLoggerWithException_ThenEndMethodNotCalled()
            {
                // Arrange
                CommandHandlerLoggerDecorator<ICommand> target = new CommandHandlerLoggerDecorator<ICommand>(_commandHandlerMock.Object, _loggers);

                Mock<ICommandLogger<ICommand>> loggerMock = new Mock<ICommandLogger<ICommand>>();
                _loggers.Add(loggerMock.Object);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Throws<Exception>();

                // Act
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_commandMock.Object, _token));

                // Assert
                loggerMock.Verify(a => a.LogEndAsync(_commandMock.Object, It.IsAny<IResult>(), _token), Times.Never);
            }

            [Fact]
            public async Task IfGiven2CommandLoggersWithException_ThenEndMethodNotCalledForEach()
            {
                // Arrange
                CommandHandlerLoggerDecorator<ICommand> target = new CommandHandlerLoggerDecorator<ICommand>(_commandHandlerMock.Object, _loggers);

                Mock<ICommandLogger<ICommand>> loggerMock = new Mock<ICommandLogger<ICommand>>();
                _loggers.Add(loggerMock.Object);

                Mock<ICommandLogger<ICommand>> loggerMock2 = new Mock<ICommandLogger<ICommand>>();
                _loggers.Add(loggerMock2.Object);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Throws<Exception>();

                // Act
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_commandMock.Object, _token));

                // Assert
                loggerMock.Verify(a => a.LogEndAsync(_commandMock.Object, It.IsAny<IResult>(), _token), Times.Never);
                loggerMock2.Verify(a => a.LogEndAsync(_commandMock.Object, It.IsAny<IResult>(), _token), Times.Never);
            }

            [Fact]
            public async Task IfGiven1CommandLoggerNoException_ThenErrorMethodNotCalled()
            {
                // Arrange
                CommandHandlerLoggerDecorator<ICommand> target = new CommandHandlerLoggerDecorator<ICommand>(_commandHandlerMock.Object, _loggers);

                Mock<ICommandLogger<ICommand>> loggerMock = new Mock<ICommandLogger<ICommand>>();
                _loggers.Add(loggerMock.Object);

                // Act
                await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                loggerMock.Verify(a => a.LogErrorAsync(_commandMock.Object, It.IsAny<Exception>(), _token), Times.Never);
            }

            [Fact]
            public async Task IfGiven2CommandLoggersNoException_ThenErrortMethodNotCalledForEach()
            {
                // Arrange
                CommandHandlerLoggerDecorator<ICommand> target = new CommandHandlerLoggerDecorator<ICommand>(_commandHandlerMock.Object, _loggers);

                Mock<ICommandLogger<ICommand>> loggerMock = new Mock<ICommandLogger<ICommand>>();
                _loggers.Add(loggerMock.Object);

                Mock<ICommandLogger<ICommand>> loggerMock2 = new Mock<ICommandLogger<ICommand>>();
                _loggers.Add(loggerMock2.Object);

                // Act
                await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                loggerMock.Verify(a => a.LogErrorAsync(_commandMock.Object, It.IsAny<Exception>(), _token), Times.Never);
                loggerMock2.Verify(a => a.LogErrorAsync(_commandMock.Object, It.IsAny<Exception>(), _token), Times.Never);
            }

            [Fact]
            public async Task IfGiven1CommandLoggerWithException_ThenErrorMethodCalled()
            {
                // Arrange
                CommandHandlerLoggerDecorator<ICommand> target = new CommandHandlerLoggerDecorator<ICommand>(_commandHandlerMock.Object, _loggers);
                Exception ex = new Exception();

                Mock<ICommandLogger<ICommand>> loggerMock = new Mock<ICommandLogger<ICommand>>();
                _loggers.Add(loggerMock.Object);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Throws(ex);

                // Act
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_commandMock.Object, _token));

                // Assert
                loggerMock.Verify(a => a.LogErrorAsync(_commandMock.Object, ex, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandLoggersWithException_ThenErrortMethodCalledForEach()
            {
                // Arrange
                CommandHandlerLoggerDecorator<ICommand> target = new CommandHandlerLoggerDecorator<ICommand>(_commandHandlerMock.Object, _loggers);
                Exception ex = new Exception();

                Mock<ICommandLogger<ICommand>> loggerMock = new Mock<ICommandLogger<ICommand>>();
                _loggers.Add(loggerMock.Object);

                Mock<ICommandLogger<ICommand>> loggerMock2 = new Mock<ICommandLogger<ICommand>>();
                _loggers.Add(loggerMock2.Object);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Throws(ex);

                // Act
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_commandMock.Object, _token));

                // Assert
                loggerMock.Verify(a => a.LogErrorAsync(_commandMock.Object, ex, _token), Times.Once);
                loggerMock2.Verify(a => a.LogErrorAsync(_commandMock.Object, ex, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandLoggersWithException_ThenErrortMethodCalledInParallel()
            {
                const int WAIT_TIME = 500;

                // Arrange
                CommandHandlerLoggerDecorator<ICommand> target = new CommandHandlerLoggerDecorator<ICommand>(_commandHandlerMock.Object, _loggers);
                Exception ex = new Exception();

                Mock<ICommandLogger<ICommand>> loggerMock = new Mock<ICommandLogger<ICommand>>();
                loggerMock.Setup(a => a.LogErrorAsync(_commandMock.Object, ex, _token)).Returns(Task.CompletedTask)
                    .Callback(() => Thread.Sleep(WAIT_TIME));
                _loggers.Add(loggerMock.Object);

                Mock<ICommandLogger<ICommand>> loggerMock2 = new Mock<ICommandLogger<ICommand>>();
                loggerMock2.Setup(a => a.LogErrorAsync(_commandMock.Object, ex, _token)).Returns(Task.CompletedTask)
                    .Callback(() => Thread.Sleep(WAIT_TIME));
                _loggers.Add(loggerMock2.Object);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Throws(ex);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_commandMock.Object, _token));
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }
        }
    }
}
