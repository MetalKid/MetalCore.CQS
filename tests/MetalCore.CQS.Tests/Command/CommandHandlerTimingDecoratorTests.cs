using MetalCore.CQS.Command;
using MetalCore.CQS.Common;
using MetalCore.CQS.Common.Results;
using Moq;
using Moq.Protected;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.Command
{
    public class CommandHandlerTimingDecoratorTests
    {
        public class CommandHandlerTimingDecoratorStub : CommandHandlerTimingDecorator<ICommand>
        {
            public CommandHandlerTimingDecoratorStub(ICommandHandler<ICommand> commandHandler) : base(commandHandler)
            {
            }

            protected override Task HandleTimerEndAsync(ICommand command, IResult result, long totalMilliseconds)
            {
                return Task.CompletedTask;
            }

            protected override Task HandleTimerWarningAsync(ICommand command, IResult result, long totalMilliseconds, ICqsTimingWarningThreshold warningThreshold)
            {
                return Task.CompletedTask;
            }
        }

        public class ConstructorMethods
        {
            [Fact]
            public void IfNullCommandHandler_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new CommandHandlerTimingDecoratorStub(null);

                // Assert
                Assert.Throws<ArgumentNullException>("commandHandler", act);
            }
        }

        public class ExecuteAsyncMethods
        {
            private readonly Mock<ICommandHandler<ICommand>> _commandHandlerMock =
                new Mock<ICommandHandler<ICommand>>();

            private readonly Mock<ICommand> _commandMock = new Mock<ICommand>();
            private readonly Mock<IResult> _resultMock = new Mock<IResult>();
            private readonly CancellationToken _token = default;

            [Fact]
            public async Task IfNullCommand_ThenCommandPushedToNextCommandHandler()
            {
                // Arrange
                Mock<CommandHandlerTimingDecorator<ICommand>> target = new Mock<CommandHandlerTimingDecorator<ICommand>>(
                    _commandHandlerMock.Object);

                // Act
                await target.Object.ExecuteAsync(null, _token);

                // Assert
                _commandHandlerMock.Verify(a => a.ExecuteAsync(null, _token), Times.Once);
            }

            [Fact]
            public async Task IfNullCommand_ThenOriginalResultReturned()
            {
                // Arrange
                Mock<CommandHandlerTimingDecorator<ICommand>> target = new Mock<CommandHandlerTimingDecorator<ICommand>>(
                    _commandHandlerMock.Object);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(null, _token))
                    .Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult result = await target.Object.ExecuteAsync(null, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfNonNullCommand_ThenCommandPushedToNextCommandHandler()
            {
                // Arrange
                Mock<CommandHandlerTimingDecorator<ICommand>> target = new Mock<CommandHandlerTimingDecorator<ICommand>>(
                    _commandHandlerMock.Object);

                // Act
                await target.Object.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                _commandHandlerMock.Verify(a => a.ExecuteAsync(_commandMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfNonNullCommand_ThenOriginalResultReturned()
            {
                // Arrange
                Mock<CommandHandlerTimingDecorator<ICommand>> target = new Mock<CommandHandlerTimingDecorator<ICommand>>(
                    _commandHandlerMock.Object);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult result = await target.Object.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfNonNullCommandAndSuccessful_ThenOriginalResultReturned()
            {
                // Arrange
                Mock<CommandHandlerTimingDecorator<ICommand>> target = new Mock<CommandHandlerTimingDecorator<ICommand>>(
                    _commandHandlerMock.Object);

                _resultMock.Setup(a => a.IsSuccessful).Returns(true);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult result = await target.Object.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfNullCommand_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                Mock<CommandHandlerTimingDecorator<ICommand>> target = new Mock<CommandHandlerTimingDecorator<ICommand>>(
                    _commandHandlerMock.Object);

                // Act
                await target.Object.ExecuteAsync(null, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<ICommand>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Never(), ItExpr.IsAny<ICommand>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNotNullCommand_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                Mock<CommandHandlerTimingDecorator<ICommand>> target = new Mock<CommandHandlerTimingDecorator<ICommand>>(
                    _commandHandlerMock.Object)
                { CallBase = true };

                // Act
                await target.Object.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<ICommand>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Never(), ItExpr.IsAny<ICommand>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNullCommandHasWarningThresholdAndIsUnderThreshold_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<CommandHandlerTimingDecorator<ICommand>> target = new Mock<CommandHandlerTimingDecorator<ICommand>>(
                    _commandHandlerMock.Object);

                _commandMock.As<ICqsTimingWarningThreshold>().Setup(a => a.WarningThresholdMilliseconds)
                    .Returns(WAIT_TIME);

                // Act
                await target.Object.ExecuteAsync(null, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<ICommand>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Never(), ItExpr.IsAny<ICommand>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNullCommandHasWarningThresholdAndIsOverThreshold_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<CommandHandlerTimingDecorator<ICommand>> target = new Mock<CommandHandlerTimingDecorator<ICommand>>(
                    _commandHandlerMock.Object);

                _commandMock.As<ICqsTimingWarningThreshold>().Setup(a => a.WarningThresholdMilliseconds)
                    .Returns(WAIT_TIME);
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object))
                    .Callback(() => Thread.Sleep(WAIT_TIME + 50));

                // Act
                await target.Object.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<ICommand>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Once(), ItExpr.IsAny<ICommand>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNotNullCommandHasWarningThresholdAndIsUnderThreshold_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<CommandHandlerTimingDecorator<ICommand>> target = new Mock<CommandHandlerTimingDecorator<ICommand>>(
                    _commandHandlerMock.Object);

                _commandMock.As<ICqsTimingWarningThreshold>().Setup(a => a.WarningThresholdMilliseconds)
                    .Returns(WAIT_TIME);

                // Act
                await target.Object.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<ICommand>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Never(), ItExpr.IsAny<ICommand>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNotNullCommandHasWarningThresholdAndIsOverThreshold_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<CommandHandlerTimingDecorator<ICommand>> target = new Mock<CommandHandlerTimingDecorator<ICommand>>(
                    _commandHandlerMock.Object);

                _commandMock.As<ICqsTimingWarningThreshold>().Setup(a => a.WarningThresholdMilliseconds)
                    .Returns(WAIT_TIME);
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() => Thread.Sleep(WAIT_TIME + 1));

                // Act
                await target.Object.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<ICommand>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Once(), ItExpr.IsAny<ICommand>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNotNullCommandHasWarningThresholdAndIsOverThresholdAndSuccessful_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<CommandHandlerTimingDecorator<ICommand>> target = new Mock<CommandHandlerTimingDecorator<ICommand>>(
                    _commandHandlerMock.Object);

                _resultMock.Setup(a => a.IsSuccessful).Returns(true);

                _commandMock.As<ICqsTimingWarningThreshold>().Setup(a => a.WarningThresholdMilliseconds)
                    .Returns(WAIT_TIME);
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() => Thread.Sleep(WAIT_TIME + 1));

                // Act
                await target.Object.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<ICommand>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Once(), ItExpr.IsAny<ICommand>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNotNullCommandHasWarningThresholdAndIsOverThresholdAndNullResult_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<CommandHandlerTimingDecorator<ICommand>> target = new Mock<CommandHandlerTimingDecorator<ICommand>>(
                    _commandHandlerMock.Object);

                _commandMock.As<ICqsTimingWarningThreshold>().Setup(a => a.WarningThresholdMilliseconds)
                    .Returns(WAIT_TIME);
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult((IResult)null)).Callback(() => Thread.Sleep(WAIT_TIME + 1));

                // Act
                await target.Object.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<ICommand>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Once(), ItExpr.IsAny<ICommand>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }
        }
    }
}