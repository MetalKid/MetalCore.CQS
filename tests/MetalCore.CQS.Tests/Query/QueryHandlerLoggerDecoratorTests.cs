using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.Query
{
    public class QueryHandlerLoggerDecoratorTests
    {
        public class ConstructorMethods
        {
            private readonly Mock<IQueryHandler<IQuery<string>, string>> _queryHandlerMock = new Mock<IQueryHandler<IQuery<string>, string>>();
            private readonly List<IQueryLogger<IQuery<string>, string>> _loggers = new List<IQueryLogger<IQuery<string>, string>>();

            [Fact]
            public void IfNullQueryHandler_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new QueryHandlerLoggerDecorator<IQuery<string>, string>(null, _loggers);

                // Assert
                Assert.Throws<ArgumentNullException>("queryHandler", act);
            }

            [Fact]
            public void IfNullCacheInvalidations_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                QueryHandlerLoggerDecorator<IQuery<string>, string> result = new QueryHandlerLoggerDecorator<IQuery<string>, string>(_queryHandlerMock.Object, null);

                // Assert
                Assert.NotNull(result);
            }
        }

        public class ExecuteAsyncMethods
        {
            private readonly Mock<IQueryHandler<IQuery<string>, string>> _queryHandlerMock = new Mock<IQueryHandler<IQuery<string>, string>>();
            private readonly Mock<IQuery<string>> _queryMock = new Mock<IQuery<string>>();
            private readonly Mock<IResult<string>> _resultMock = new Mock<IResult<string>>();
            private readonly List<IQueryLogger<IQuery<string>, string>> _loggers = new List<IQueryLogger<IQuery<string>, string>>();
            private readonly CancellationToken _token = default;

            [Fact]
            public async Task IfNullQuery_ThenQueryPushedToNextQueryHandler()
            {
                // Arrange
                QueryHandlerLoggerDecorator<IQuery<string>, string> target = new QueryHandlerLoggerDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _loggers);

                // Act
                await target.ExecuteAsync(null, _token);

                // Assert
                _queryHandlerMock.Verify(a => a.ExecuteAsync(null, _token), Times.Once);
            }

            [Fact]
            public async Task IfNullQuery_ThenOriginalResultReturned()
            {
                // Arrange
                QueryHandlerLoggerDecorator<IQuery<string>, string> target = new QueryHandlerLoggerDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _loggers);

                _queryHandlerMock.Setup(a => a.ExecuteAsync(null, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.ExecuteAsync(null, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfNonNullQuery_ThenQueryPushedToNextQueryHandler()
            {
                // Arrange
                QueryHandlerLoggerDecorator<IQuery<string>, string> target = new QueryHandlerLoggerDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _loggers);

                // Act
                await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                _queryHandlerMock.Verify(a => a.ExecuteAsync(_queryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfNonNullQuery_ThenOriginalResultReturned()
            {
                // Arrange
                QueryHandlerLoggerDecorator<IQuery<string>, string> target = new QueryHandlerLoggerDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _loggers);

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGiven1QueryLogger_ThenStartMethodCalled()
            {
                // Arrange
                QueryHandlerLoggerDecorator<IQuery<string>, string> target = new QueryHandlerLoggerDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _loggers);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock = new Mock<IQueryLogger<IQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                // Act
                await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                loggerMock.Verify(a => a.LogStartAsync(_queryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2QueryLoggers_ThenStartMethodCalledForEach()
            {
                // Arrange
                QueryHandlerLoggerDecorator<IQuery<string>, string> target = new QueryHandlerLoggerDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _loggers);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock = new Mock<IQueryLogger<IQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock2 = new Mock<IQueryLogger<IQuery<string>, string>>();
                _loggers.Add(loggerMock2.Object);

                // Act
                await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                loggerMock.Verify(a => a.LogStartAsync(_queryMock.Object, _token), Times.Once);
                loggerMock2.Verify(a => a.LogStartAsync(_queryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2QueryLoggers_ThenStartMethodCalledForEachInParallel()
            {
                const int WAIT_TIME = 500;

                // Arrange
                QueryHandlerLoggerDecorator<IQuery<string>, string> target = new QueryHandlerLoggerDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _loggers);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock = new Mock<IQueryLogger<IQuery<string>, string>>();
                loggerMock.Setup(a => a.LogStartAsync(_queryMock.Object, _token)).Returns(Task.CompletedTask)
                    .Callback(() => Thread.Sleep(WAIT_TIME));
                _loggers.Add(loggerMock.Object);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock2 = new Mock<IQueryLogger<IQuery<string>, string>>();
                loggerMock2.Setup(a => a.LogStartAsync(_queryMock.Object, _token)).Returns(Task.CompletedTask)
                    .Callback(() => Thread.Sleep(WAIT_TIME));
                _loggers.Add(loggerMock2.Object);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                await target.ExecuteAsync(_queryMock.Object, _token);
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }

            [Fact]
            public async Task IfGiven1QueryLoggerWithException_ThenStartMethodCalled()
            {
                // Arrange
                QueryHandlerLoggerDecorator<IQuery<string>, string> target = new QueryHandlerLoggerDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _loggers);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock = new Mock<IQueryLogger<IQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Throws<Exception>();

                // Act
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_queryMock.Object, _token));

                // Assert
                loggerMock.Verify(a => a.LogStartAsync(_queryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2QueryLoggersWithException_ThenStartMethodCalledForEach()
            {
                // Arrange
                QueryHandlerLoggerDecorator<IQuery<string>, string> target = new QueryHandlerLoggerDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _loggers);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock = new Mock<IQueryLogger<IQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock2 = new Mock<IQueryLogger<IQuery<string>, string>>();
                _loggers.Add(loggerMock2.Object);

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Throws<Exception>();

                // Act
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_queryMock.Object, _token));

                // Assert
                loggerMock.Verify(a => a.LogStartAsync(_queryMock.Object, _token), Times.Once);
                loggerMock2.Verify(a => a.LogStartAsync(_queryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven1QueryLogger_ThenEndMethodCalled()
            {
                // Arrange
                QueryHandlerLoggerDecorator<IQuery<string>, string> target = new QueryHandlerLoggerDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _loggers);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock = new Mock<IQueryLogger<IQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                // Act
                IResult<string> result = await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                loggerMock.Verify(a => a.LogEndAsync(_queryMock.Object, result, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2QueryLoggers_ThenEndMethodCalledForEach()
            {
                // Arrange
                QueryHandlerLoggerDecorator<IQuery<string>, string> target = new QueryHandlerLoggerDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _loggers);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock = new Mock<IQueryLogger<IQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock2 = new Mock<IQueryLogger<IQuery<string>, string>>();
                _loggers.Add(loggerMock2.Object);

                // Act
                IResult<string> result = await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                loggerMock.Verify(a => a.LogEndAsync(_queryMock.Object, result, _token), Times.Once);
                loggerMock2.Verify(a => a.LogEndAsync(_queryMock.Object, result, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2QueryLoggers_ThenEndMethodCalledForEachInParallel()
            {
                const int WAIT_TIME = 500;

                // Arrange
                QueryHandlerLoggerDecorator<IQuery<string>, string> target = new QueryHandlerLoggerDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _loggers);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock = new Mock<IQueryLogger<IQuery<string>, string>>();
                loggerMock.Setup(a => a.LogEndAsync(_queryMock.Object, It.IsAny<IResult<string>>(), _token)).Returns(Task.CompletedTask)
                    .Callback(() => Thread.Sleep(WAIT_TIME));
                _loggers.Add(loggerMock.Object);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock2 = new Mock<IQueryLogger<IQuery<string>, string>>();
                loggerMock2.Setup(a => a.LogEndAsync(_queryMock.Object, It.IsAny<IResult<string>>(), _token)).Returns(Task.CompletedTask)
                    .Callback(() => Thread.Sleep(WAIT_TIME));
                _loggers.Add(loggerMock2.Object);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                IResult<string> result = await target.ExecuteAsync(_queryMock.Object, _token);
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }

            [Fact]
            public async Task IfGiven1QueryLoggerWithException_ThenEndMethodNotCalled()
            {
                // Arrange
                QueryHandlerLoggerDecorator<IQuery<string>, string> target = new QueryHandlerLoggerDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _loggers);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock = new Mock<IQueryLogger<IQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Throws<Exception>();

                // Act
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_queryMock.Object, _token));

                // Assert
                loggerMock.Verify(a => a.LogEndAsync(_queryMock.Object, It.IsAny<IResult<string>>(), _token), Times.Never);
            }

            [Fact]
            public async Task IfGiven2QueryLoggersWithException_ThenEndMethodNotCalledForEach()
            {
                // Arrange
                QueryHandlerLoggerDecorator<IQuery<string>, string> target = new QueryHandlerLoggerDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _loggers);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock = new Mock<IQueryLogger<IQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock2 = new Mock<IQueryLogger<IQuery<string>, string>>();
                _loggers.Add(loggerMock2.Object);

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Throws<Exception>();

                // Act
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_queryMock.Object, _token));

                // Assert
                loggerMock.Verify(a => a.LogEndAsync(_queryMock.Object, It.IsAny<IResult<string>>(), _token), Times.Never);
                loggerMock2.Verify(a => a.LogEndAsync(_queryMock.Object, It.IsAny<IResult<string>>(), _token), Times.Never);
            }

            [Fact]
            public async Task IfGiven1QueryLoggerNoException_ThenErrorMethodNotCalled()
            {
                // Arrange
                QueryHandlerLoggerDecorator<IQuery<string>, string> target = new QueryHandlerLoggerDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _loggers);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock = new Mock<IQueryLogger<IQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                // Act
                await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                loggerMock.Verify(a => a.LogErrorAsync(_queryMock.Object, It.IsAny<Exception>(), _token), Times.Never);
            }

            [Fact]
            public async Task IfGiven2QueryLoggersNoException_ThenErrortMethodNotCalledForEach()
            {
                // Arrange
                QueryHandlerLoggerDecorator<IQuery<string>, string> target = new QueryHandlerLoggerDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _loggers);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock = new Mock<IQueryLogger<IQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock2 = new Mock<IQueryLogger<IQuery<string>, string>>();
                _loggers.Add(loggerMock2.Object);

                // Act
                await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                loggerMock.Verify(a => a.LogErrorAsync(_queryMock.Object, It.IsAny<Exception>(), _token), Times.Never);
                loggerMock2.Verify(a => a.LogErrorAsync(_queryMock.Object, It.IsAny<Exception>(), _token), Times.Never);
            }

            [Fact]
            public async Task IfGiven1QueryLoggerWithException_ThenErrorMethodCalled()
            {
                // Arrange
                QueryHandlerLoggerDecorator<IQuery<string>, string> target = new QueryHandlerLoggerDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _loggers);
                Exception ex = new Exception();

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock = new Mock<IQueryLogger<IQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Throws(ex);

                // Act
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_queryMock.Object, _token));

                // Assert
                loggerMock.Verify(a => a.LogErrorAsync(_queryMock.Object, ex, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2QueryLoggersWithException_ThenErrortMethodCalledForEach()
            {
                // Arrange
                QueryHandlerLoggerDecorator<IQuery<string>, string> target = new QueryHandlerLoggerDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _loggers);
                Exception ex = new Exception();

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock = new Mock<IQueryLogger<IQuery<string>, string>>();
                _loggers.Add(loggerMock.Object);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock2 = new Mock<IQueryLogger<IQuery<string>, string>>();
                _loggers.Add(loggerMock2.Object);

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Throws(ex);

                // Act
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_queryMock.Object, _token));

                // Assert
                loggerMock.Verify(a => a.LogErrorAsync(_queryMock.Object, ex, _token), Times.Once);
                loggerMock2.Verify(a => a.LogErrorAsync(_queryMock.Object, ex, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2QueryLoggersWithException_ThenErrortMethodCalledInParallel()
            {
                const int WAIT_TIME = 500;

                // Arrange
                QueryHandlerLoggerDecorator<IQuery<string>, string> target = new QueryHandlerLoggerDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _loggers);
                Exception ex = new Exception();

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock = new Mock<IQueryLogger<IQuery<string>, string>>();
                loggerMock.Setup(a => a.LogErrorAsync(_queryMock.Object, ex, _token)).Returns(Task.CompletedTask)
                    .Callback(() => Thread.Sleep(WAIT_TIME));
                _loggers.Add(loggerMock.Object);

                Mock<IQueryLogger<IQuery<string>, string>> loggerMock2 = new Mock<IQueryLogger<IQuery<string>, string>>();
                loggerMock2.Setup(a => a.LogErrorAsync(_queryMock.Object, ex, _token)).Returns(Task.CompletedTask)
                    .Callback(() => Thread.Sleep(WAIT_TIME));
                _loggers.Add(loggerMock2.Object);

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Throws(ex);

                // Act
                Stopwatch timer = Stopwatch.StartNew();
                await Assert.ThrowsAsync<Exception>(async () => await target.ExecuteAsync(_queryMock.Object, _token));
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }
        }
    }
}
