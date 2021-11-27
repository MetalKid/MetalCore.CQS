using MetalCore.CQS.Common;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Query;
using Moq;
using Moq.Protected;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.Query
{
    public class QueryHandlerTimingDecoratorTests
    {
        public class QueryHandlerTimingDecoratorStub : QueryHandlerTimingDecorator<IQuery<string>, string>
        {
            public QueryHandlerTimingDecoratorStub(IQueryHandler<IQuery<string>, string> queryHandler) : base(queryHandler)
            {
            }

            protected override Task HandleTimerEndAsync(IQuery<string> query, IResult result, long totalMilliseconds)
            {
                return Task.CompletedTask;
            }

            protected override Task HandleTimerWarningAsync(IQuery<string> query, IResult result, long totalMilliseconds, ICqsTimingWarningThreshold warningThreshold)
            {
                return Task.CompletedTask;
            }
        }

        public class ConstructorMethods
        {
            [Fact]
            public void IfNullQueryHandler_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new QueryHandlerTimingDecoratorStub(null);

                // Assert
                Assert.Throws<ArgumentNullException>("queryHandler", act);
            }
        }

        public class ExecuteAsyncMethods
        {
            private readonly Mock<IQueryHandler<IQuery<string>, string>> _queryHandlerMock =
                new Mock<IQueryHandler<IQuery<string>, string>>();

            private readonly Mock<IQuery<string>> _queryMock = new Mock<IQuery<string>>();
            private readonly Mock<IResult<string>> _resultMock = new Mock<IResult<string>>();
            private readonly CancellationToken _token = default;

            [Fact]
            public async Task IfNullQuery_ThenQueryPushedToNextQueryHandler()
            {
                // Arrange
                Mock<QueryHandlerTimingDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerTimingDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object);

                // Act
                await target.Object.ExecuteAsync(null, _token);

                // Assert
                _queryHandlerMock.Verify(a => a.ExecuteAsync(null, _token), Times.Once);
            }

            [Fact]
            public async Task IfNullQuery_ThenOriginalResultReturned()
            {
                // Arrange
                Mock<QueryHandlerTimingDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerTimingDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object);

                _queryHandlerMock.Setup(a => a.ExecuteAsync(null, _token))
                    .Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.Object.ExecuteAsync(null, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfNonNullQuery_ThenQueryPushedToNextQueryHandler()
            {
                // Arrange
                Mock<QueryHandlerTimingDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerTimingDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object);

                // Act
                await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                _queryHandlerMock.Verify(a => a.ExecuteAsync(_queryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfNonNullQuery_ThenOriginalResultReturned()
            {
                // Arrange
                Mock<QueryHandlerTimingDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerTimingDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object);

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfNonNullQueryAndSuccessful_ThenOriginalResultReturned()
            {
                // Arrange
                Mock<QueryHandlerTimingDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerTimingDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object);

                _resultMock.Setup(a => a.IsSuccessful).Returns(true);

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfNullQuery_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                Mock<QueryHandlerTimingDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerTimingDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object);

                // Act
                await target.Object.ExecuteAsync(null, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<IQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Never(), ItExpr.IsAny<IQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNotNullQuery_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                Mock<QueryHandlerTimingDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerTimingDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object);

                // Act
                await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<IQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Never(), ItExpr.IsAny<IQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNullQueryHasWarningThresholdAndIsUnderThreshold_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<QueryHandlerTimingDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerTimingDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object);

                _queryMock.As<ICqsTimingWarningThreshold>().Setup(a => a.WarningThresholdMilliseconds)
                    .Returns(WAIT_TIME);

                // Act
                await target.Object.ExecuteAsync(null, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<IQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Never(), ItExpr.IsAny<IQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNullQueryHasWarningThresholdAndIsOverThreshold_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<QueryHandlerTimingDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerTimingDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object)
                {
                    CallBase = true
                };
                Moq.Language.Flow.IReturnsResult<ICqsTimingWarningThreshold> input =
                _queryMock.As<ICqsTimingWarningThreshold>().Setup(a => a.WarningThresholdMilliseconds)
                    .Returns(WAIT_TIME);
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object))
                    .Callback(() => Thread.Sleep(WAIT_TIME + 50));

                // Act
                await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<IQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Once(), ItExpr.IsAny<IQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNotNullQueryHasWarningThresholdAndIsUnderThreshold_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<QueryHandlerTimingDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerTimingDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object);

                _queryMock.As<ICqsTimingWarningThreshold>().Setup(a => a.WarningThresholdMilliseconds)
                    .Returns(WAIT_TIME);

                // Act
                await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<IQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Never(), ItExpr.IsAny<IQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNotNullQueryHasWarningThresholdAndIsOverThreshold_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<QueryHandlerTimingDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerTimingDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object);

                _queryMock.As<ICqsTimingWarningThreshold>().Setup(a => a.WarningThresholdMilliseconds)
                    .Returns(WAIT_TIME);
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() => Thread.Sleep(WAIT_TIME + 1));

                // Act
                await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<IQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Once(), ItExpr.IsAny<IQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNotNullQueryHasWarningThresholdAndIsOverThresholdAndSuccessful_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<QueryHandlerTimingDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerTimingDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object);

                _resultMock.Setup(a => a.IsSuccessful).Returns(true);

                _queryMock.As<ICqsTimingWarningThreshold>().Setup(a => a.WarningThresholdMilliseconds)
                    .Returns(WAIT_TIME);
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() => Thread.Sleep(WAIT_TIME + 1));

                // Act
                await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<IQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Once(), ItExpr.IsAny<IQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }

            [Fact]
            public async Task IfNotNullQueryHasWarningThresholdAndIsOverThresholdAndNullResult_ThenAppropriateAbstractMethodsCalled()
            {
                // Arrange
                const int WAIT_TIME = 500;

                Mock<QueryHandlerTimingDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerTimingDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object);

                _queryMock.As<ICqsTimingWarningThreshold>().Setup(a => a.WarningThresholdMilliseconds)
                    .Returns(WAIT_TIME);
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult((IResult<string>)null)).Callback(() => Thread.Sleep(WAIT_TIME + 1));

                // Act
                await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                target.Protected().Verify<Task>("HandleTimerEndAsync", Times.Once(), ItExpr.IsAny<IQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>());
                target.Protected().Verify<Task>("HandleTimerWarningAsync", Times.Once(), ItExpr.IsAny<IQuery<string>>(), ItExpr.IsAny<IResult>(), ItExpr.IsAny<long>(), ItExpr.IsAny<ICqsTimingWarningThreshold>());
            }
        }
    }
}