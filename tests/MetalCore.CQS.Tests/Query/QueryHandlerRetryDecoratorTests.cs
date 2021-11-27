using MetalCore.CQS.Common;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Exceptions;
using MetalCore.CQS.Query;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.Query
{
    public class QueryHandlerRetryDecoratorTests
    {
        public class ConstructorMethods
        {
            [Fact]
            public void IfNullQueryHandler_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new QueryHandlerRetryDecorator<IQuery<string>, string>(null);

                // Assert
                Assert.Throws<ArgumentNullException>("queryHandler", act);
            }
        }

        public class ExecuteAsyncMethods
        {
            private readonly Mock<IQueryHandler<IQuery<string>, string>> _queryHandlerMock = new Mock<IQueryHandler<IQuery<string>, string>>();
            private readonly Mock<IQuery<string>> _queryMock = new Mock<IQuery<string>>();
            private readonly Mock<IResult<string>> _resultMock = new Mock<IResult<string>>();
            private readonly CancellationToken _token = default;

            [Fact]
            public async Task IfNullQuery_ThenQueryPushedToNextQueryHandler()
            {
                // Arrange
                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                // Act
                await target.ExecuteAsync(null, _token);

                // Assert
                _queryHandlerMock.Verify(a => a.ExecuteAsync(null, _token), Times.Once);
            }

            [Fact]
            public async Task IfNullQuery_ThenOriginalResultReturned()
            {
                // Arrange
                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

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
                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                // Act
                await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                _queryHandlerMock.Verify(a => a.ExecuteAsync(_queryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfNonNullQuery_ThenOriginalResultReturned()
            {
                // Arrange
                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGiven3RetryAndNoException_Then1AttemptPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;

                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                _queryMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() => Thread.Sleep(WAIT_TIME));

                // Act
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                IResult<string> result = await target.ExecuteAsync(_queryMock.Object, _token);
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }

            [Fact]
            public async Task IfGiven1Retry_Then2AttemptsPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;

                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                _queryMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(1);

                Exception exception = new Exception();
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<Exception>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 2);
            }

            [Fact]
            public async Task IfGiven3Retry_Then4AttemptsPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;

                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                _queryMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);

                Exception exception = new Exception();
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<Exception>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 4);
            }

            [Fact]
            public async Task IfGiven3RetryAnd1SecondDelayAndNoException_Then1AttemptPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;
                const int DELAY_TIME = 1000;

                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                _queryMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _queryMock.As<ICqsRetry>().Setup(a => a.RetryDelayMilliseconds).Returns(DELAY_TIME);

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() => Thread.Sleep(WAIT_TIME));

                // Act
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                IResult<string> result = await target.ExecuteAsync(_queryMock.Object, _token);
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }

            [Fact]
            public async Task IfGiven1RetryWith1SecondDelay_Then2AttemptsPerformedWithDelay()
            {
                // Arrange
                const int WAIT_TIME = 500;
                const int DELAY_TIME = 1000;

                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                _queryMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(1);
                _queryMock.As<ICqsRetry>().Setup(a => a.RetryDelayMilliseconds).Returns(DELAY_TIME);

                Exception exception = new Exception();
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<Exception>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 2 + DELAY_TIME);
            }

            [Fact]
            public async Task IfGiven3RetryWith1SecondDelay_Then4AttemptsPerformedWithDelay()
            {
                // Arrange
                const int WAIT_TIME = 500;
                const int DELAY_TIME = 1000;

                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                _queryMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _queryMock.As<ICqsRetry>().Setup(a => a.RetryDelayMilliseconds).Returns(DELAY_TIME);

                Exception exception = new Exception();
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<Exception>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 4 + DELAY_TIME * 3);
            }

            [Fact]
            public async Task IfGiven3RetryWithBrokenRuleExceptionThrown_Then1AttemptsPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;

                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                _queryMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);

                BrokenRuleException exception = new BrokenRuleException();
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<BrokenRuleException>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 1);
            }

            [Fact]
            public async Task IfGiven3RetryWithDataNotFoundExceptionThrown_Then1AttemptsPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;

                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                _queryMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);

                DataNotFoundException exception = new DataNotFoundException();
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<DataNotFoundException>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 1);
            }

            [Fact]
            public async Task IfGiven3RetryWithConcurrencyExceptionThrown_Then1AttemptsPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;

                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                _queryMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);

                ConcurrencyException exception = new ConcurrencyException();
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<ConcurrencyException>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 1);
            }

            [Fact]
            public async Task IfGiven3RetryWithNoPermissionExceptionThrown_Then1AttemptsPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;

                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                _queryMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);

                NoPermissionException exception = new NoPermissionException();
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<NoPermissionException>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 1);
            }

            [Fact]
            public async Task IfGiven3RetryWithSpecificExceptionNotThrown_Then1AttemptsPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;

                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                _queryMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _queryMock.As<ICqsRetrySpecific>()
                    .Setup(a => a.OnlyRetryForExceptionsOfTheseTypes).Returns(new[] { typeof(ArgumentException) });
                _queryMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckBaseTypes).Returns(false);
                _queryMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckInnerExceptions).Returns(false);

                Exception exception = new Exception();
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<Exception>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 1);
            }

            [Fact]
            public async Task IfGiven3RetryWithSpecificExceptionThrown_Then4AttemptsPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;

                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                _queryMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _queryMock.As<ICqsRetrySpecific>()
                    .Setup(a => a.OnlyRetryForExceptionsOfTheseTypes).Returns(new[] { typeof(ArgumentException) });
                _queryMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckBaseTypes).Returns(false);
                _queryMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckInnerExceptions).Returns(false);

                ArgumentException exception = new ArgumentException();
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<ArgumentException>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 4);
            }

            [Fact]
            public async Task IfGiven3RetryWithSpecificInnerExceptionThrownWithIgnoringInner_Then1AttemptsPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;

                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                _queryMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _queryMock.As<ICqsRetrySpecific>()
                    .Setup(a => a.OnlyRetryForExceptionsOfTheseTypes).Returns(new[] { typeof(ArgumentException) });
                _queryMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckBaseTypes).Returns(false);
                _queryMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckInnerExceptions).Returns(false);

                Exception exception = new Exception(null, new ArgumentException());
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<Exception>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 1);
            }

            [Fact]
            public async Task IfGiven3RetryWithSpecificInnerExceptionThrownWithInner_Then4AttemptsPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;

                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                _queryMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _queryMock.As<ICqsRetrySpecific>()
                    .Setup(a => a.OnlyRetryForExceptionsOfTheseTypes).Returns(new[] { typeof(ArgumentException) });
                _queryMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckBaseTypes).Returns(false);
                _queryMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckInnerExceptions).Returns(true);

                Exception exception = new Exception(null, new ArgumentException());
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<Exception>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 4);
            }

            [Fact]
            public async Task IfGiven3RetryWithSpecificInheritedExceptionThrownWithIgnoreBase_Then1AttemptsPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;

                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                _queryMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _queryMock.As<ICqsRetrySpecific>()
                    .Setup(a => a.OnlyRetryForExceptionsOfTheseTypes).Returns(new[] { typeof(SystemException) });
                _queryMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckBaseTypes).Returns(false);
                _queryMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckInnerExceptions).Returns(true);

                ArgumentException exception = new ArgumentException();
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<ArgumentException>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 1);
            }

            [Fact]
            public async Task IfGiven3RetryWithSpecificInheritedExceptionThrownWithBase_Then4AttemptsPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;

                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                _queryMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _queryMock.As<ICqsRetrySpecific>()
                    .Setup(a => a.OnlyRetryForExceptionsOfTheseTypes).Returns(new[] { typeof(SystemException) });
                _queryMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckBaseTypes).Returns(true);
                _queryMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckInnerExceptions).Returns(true);

                ArgumentException exception = new ArgumentException();
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<ArgumentException>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 4);
            }

            [Fact]
            public async Task IfGiven3RetryWithSpecificInheritedExceptionThrownWithBase2LevelsDeep_Then4AttemptsPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;

                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                _queryMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _queryMock.As<ICqsRetrySpecific>()
                    .Setup(a => a.OnlyRetryForExceptionsOfTheseTypes).Returns(new[] { typeof(Exception) });
                _queryMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckBaseTypes).Returns(true);
                _queryMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckInnerExceptions).Returns(false);

                ArgumentException exception = new ArgumentException();
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<ArgumentException>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 4);
            }

            [Fact]
            public async Task IfGiven3RetryWithInnerSpecificInheritedExceptionThrownWithBase2LevelsDeep_Then4AttemptsPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;

                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                _queryMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _queryMock.As<ICqsRetrySpecific>()
                    .Setup(a => a.OnlyRetryForExceptionsOfTheseTypes).Returns(new[] { typeof(SystemException) });
                _queryMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckBaseTypes).Returns(true);
                _queryMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckInnerExceptions).Returns(true);

                Exception exception = new Exception(null, new InvalidOperationException());
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<Exception>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 4);
            }

            [Fact]
            public async Task IfGiven3RetryWithInnerSpecificInheritedExceptionThrownWithBase2LevelsDeepNotFound_Then1AttemptsPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;

                QueryHandlerRetryDecorator<IQuery<string>, string> target = new QueryHandlerRetryDecorator<IQuery<string>, string>(_queryHandlerMock.Object);

                _queryMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _queryMock.As<ICqsRetrySpecific>()
                    .Setup(a => a.OnlyRetryForExceptionsOfTheseTypes).Returns(new[] { typeof(ArgumentNullException) });
                _queryMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckBaseTypes).Returns(true);
                _queryMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckInnerExceptions).Returns(true);

                Exception exception = new Exception(null, new InvalidOperationException());
                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<Exception>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 1);
            }
        }
    }
}