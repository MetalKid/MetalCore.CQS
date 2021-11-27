using MetalCore.CQS.CommandQuery;
using MetalCore.CQS.Common;
using MetalCore.CQS.Common.Results;
using Moq;
using Moq.Protected;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.CommandQuery
{
    public class CommandQueryHandlerTimingDecoratorTests
    {
        public class CommandQueryHandlerTimingDecoratorStub : CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>
        {
            public CommandQueryHandlerTimingDecoratorStub(ICommandQueryHandler<ICommandQuery<string>, string> commandQueryHandler)
                : base(commandQueryHandler)
            {
            }

            protected override Task HandleTimerEndAsync(ICommandQuery<string> commandQuery, IResult result, long totalMilliseconds)
            {
                return Task.CompletedTask;
            }

            protected override Task HandleTimerWarningAsync(ICommandQuery<string> commandQuery, IResult result, long totalMilliseconds, ICqsTimingWarningThreshold warningThreshold)
            {
                return Task.CompletedTask;
            }
        }

        public class ConstructorMethods
        {
            [Fact]
            public void IfNullCommandQueryHandler_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new CommandQueryHandlerTimingDecoratorStub(null);

                // Assert
                Assert.Throws<ArgumentNullException>("commandQueryHandler", act);
            }
        }


        public class ExecuteAsyncMethods
        {
            private readonly Mock<ICommandQueryHandler<ICommandQuery<string>, string>> _commandQueryHandlerMock =
                new Mock<ICommandQueryHandler<ICommandQuery<string>, string>>();

            private readonly Mock<ICommandQuery<string>> _commandQueryMock = new Mock<ICommandQuery<string>>();
            private readonly Mock<IResult<string>> _resultMock = new Mock<IResult<string>>();
            private readonly CancellationToken _token = default;

            [Fact]
            public async Task IfNullCommandQuery_ThenCommandQueryPushedToNextCommandQueryHandler()
            {
                // Arrange
                Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object);

                // Act
                await target.Object.ExecuteAsync(null, _token);

                // Assert
                _commandQueryHandlerMock.Verify(a => a.ExecuteAsync(null, _token), Times.Once());
            }

            [Fact]
            public async Task IfNullCommandQuery_ThenOriginalResultReturned()
            {
                // Arrange
                Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(null, _token))
                    .Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.Object.ExecuteAsync(null, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfNonNullCommandQuery_ThenCommandQueryPushedToNextCommandQueryHandler()
            {
                // Arrange
                Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object);

                // Act
                await target.Object.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                _commandQueryHandlerMock.Verify(a => a.ExecuteAsync(_commandQueryMock.Object, _token), Times.Once());
            }

            [Fact]
            public async Task IfNonNullCommandQuery_ThenOriginalResultReturned()
            {
                // Arrange
                Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.Object.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfNonNullCommandQueryAndSuccessful_ThenOriginalResultReturned()
            {
                // Arrange
                Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object);

                _resultMock.Setup(a => a.IsSuccessful).Returns(true);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.Object.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfNullCommandQuery_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object);

                // Act
                await target.Object.ExecuteAsync(null, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<ICommandQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Never(), ItExpr.IsAny<ICommandQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNotNullCommandQuery_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object);

                // Act
                await target.Object.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<ICommandQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Never(), ItExpr.IsAny<ICommandQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNullCommandQueryHasWarningThresholdAndIsUnderThreshold_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object);

                _commandQueryMock.As<ICqsTimingWarningThreshold>().Setup(a => a.WarningThresholdMilliseconds)
                    .Returns(WAIT_TIME);

                // Act
                await target.Object.ExecuteAsync(null, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<ICommandQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Never(), ItExpr.IsAny<ICommandQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNullCommandQueryHasWarningThresholdAndIsOverThreshold_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object);

                _commandQueryMock.As<ICqsTimingWarningThreshold>().Setup(a => a.WarningThresholdMilliseconds)
                    .Returns(WAIT_TIME);
                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object))
                    .Callback(() => Thread.Sleep(WAIT_TIME + 50));

                // Act
                await target.Object.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<ICommandQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Once(), ItExpr.IsAny<ICommandQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNotNullCommandQueryHasWarningThresholdAndIsUnderThreshold_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object);

                _commandQueryMock.As<ICqsTimingWarningThreshold>().Setup(a => a.WarningThresholdMilliseconds)
                    .Returns(WAIT_TIME);

                // Act
                await target.Object.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<ICommandQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Never(), ItExpr.IsAny<ICommandQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNotNullCommandQueryHasWarningThresholdAndIsOverThreshold_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object);

                _commandQueryMock.As<ICqsTimingWarningThreshold>().Setup(a => a.WarningThresholdMilliseconds)
                    .Returns(WAIT_TIME);
                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() => Thread.Sleep(WAIT_TIME + 1));

                // Act
                await target.Object.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<ICommandQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Once(), ItExpr.IsAny<ICommandQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNotNullCommandQueryHasWarningThresholdAndIsOverThresholdAndSuccessful_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object);

                _resultMock.Setup(a => a.IsSuccessful).Returns(true);

                _commandQueryMock.As<ICqsTimingWarningThreshold>().Setup(a => a.WarningThresholdMilliseconds)
                    .Returns(WAIT_TIME);
                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() => Thread.Sleep(WAIT_TIME + 1));

                // Act
                await target.Object.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<ICommandQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Once(), ItExpr.IsAny<ICommandQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNotNullCommandQueryHasWarningThresholdAndIsOverThresholdAndNullResult_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerTimingDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object);

                _commandQueryMock.As<ICqsTimingWarningThreshold>().Setup(a => a.WarningThresholdMilliseconds)
                    .Returns(WAIT_TIME);
                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token))
                    .Returns(Task.FromResult((IResult<string>)null)).Callback(() => Thread.Sleep(WAIT_TIME + 1));

                // Act
                await target.Object.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<ICommandQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Once(), ItExpr.IsAny<ICommandQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }
        }
    }
}