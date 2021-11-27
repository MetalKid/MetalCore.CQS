using MetalCore.CQS.Command;
using MetalCore.CQS.Common;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Exceptions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.Command
{
    public class CommandHandlerRetryDecoratorTests
    {
        public class ConstructorMethods
        {
            [Fact]
            public void IfNullCommandHandler_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new CommandHandlerRetryDecorator<ICommand>(null);

                // Assert
                Assert.Throws<ArgumentNullException>("commandHandler", act);
            }
        }

        public class ExecuteAsyncMethods
        {
            private readonly Mock<ICommandHandler<ICommand>> _commandHandlerMock = new Mock<ICommandHandler<ICommand>>();
            private readonly Mock<ICommand> _commandMock = new Mock<ICommand>();
            private readonly Mock<IResult> _resultMock = new Mock<IResult>();
            private readonly CancellationToken _token = default;

            [Fact]
            public async Task IfNullCommand_ThenCommandPushedToNextCommandHandler()
            {
                // Arrange
                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                // Act
                await target.ExecuteAsync(null, _token);

                // Assert
                _commandHandlerMock.Verify(a => a.ExecuteAsync(null, _token), Times.Once);
            }

            [Fact]
            public async Task IfNullCommand_ThenOriginalResultReturned()
            {
                // Arrange
                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

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
                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                // Act
                await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                _commandHandlerMock.Verify(a => a.ExecuteAsync(_commandMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfNonNullCommand_ThenOriginalResultReturned()
            {
                // Arrange
                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult result = await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGiven3RetryAndNoException_Then1AttemptPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;

                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                _commandMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() => Thread.Sleep(WAIT_TIME));

                // Act
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                IResult result = await target.ExecuteAsync(_commandMock.Object, _token);
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }

            [Fact]
            public async Task IfGiven1Retry_Then2AttemptsPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;

                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                _commandMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(1);

                Exception exception = new Exception();
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<Exception>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 2);
            }

            [Fact]
            public async Task IfGiven3RetryAnd1SecondDelayAndNoException_Then1AttemptPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;
                const int DELAY_TIME = 1000;

                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                _commandMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _commandMock.As<ICqsRetry>().Setup(a => a.RetryDelayMilliseconds).Returns(DELAY_TIME);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() => Thread.Sleep(WAIT_TIME));

                // Act
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                IResult result = await target.ExecuteAsync(_commandMock.Object, _token);
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

                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                _commandMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(1);
                _commandMock.As<ICqsRetry>().Setup(a => a.RetryDelayMilliseconds).Returns(DELAY_TIME);

                Exception exception = new Exception();
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_commandMock.Object, _token);

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

                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                _commandMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _commandMock.As<ICqsRetry>().Setup(a => a.RetryDelayMilliseconds).Returns(DELAY_TIME);

                Exception exception = new Exception();
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<Exception>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 4 + DELAY_TIME * 3);
            }

            [Fact]
            public async Task IfGiven3Retry_Then4AttemptsPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;

                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                _commandMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);

                Exception exception = new Exception();
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<Exception>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 4);
            }

            [Fact]
            public async Task IfGiven3RetryWithBrokenRuleExceptionThrown_Then1AttemptsPerformed()
            {
                // Arrange
                const int WAIT_TIME = 500;

                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                _commandMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);

                BrokenRuleException exception = new BrokenRuleException();
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_commandMock.Object, _token);

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

                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                _commandMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);

                DataNotFoundException exception = new DataNotFoundException();
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_commandMock.Object, _token);

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

                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                _commandMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);

                ConcurrencyException exception = new ConcurrencyException();
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_commandMock.Object, _token);

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

                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                _commandMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);

                NoPermissionException exception = new NoPermissionException();
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_commandMock.Object, _token);

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

                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                _commandMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _commandMock.As<ICqsRetrySpecific>()
                    .Setup(a => a.OnlyRetryForExceptionsOfTheseTypes).Returns(new[] { typeof(ArgumentException) });
                _commandMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckBaseTypes).Returns(false);
                _commandMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckInnerExceptions).Returns(false);

                Exception exception = new Exception();
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_commandMock.Object, _token);

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

                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                _commandMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _commandMock.As<ICqsRetrySpecific>()
                    .Setup(a => a.OnlyRetryForExceptionsOfTheseTypes).Returns(new[] { typeof(ArgumentException) });
                _commandMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckBaseTypes).Returns(false);
                _commandMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckInnerExceptions).Returns(false);

                ArgumentException exception = new ArgumentException();
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_commandMock.Object, _token);

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

                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                _commandMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _commandMock.As<ICqsRetrySpecific>()
                    .Setup(a => a.OnlyRetryForExceptionsOfTheseTypes).Returns(new[] { typeof(ArgumentException) });
                _commandMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckBaseTypes).Returns(false);
                _commandMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckInnerExceptions).Returns(false);

                Exception exception = new Exception(null, new ArgumentException());
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_commandMock.Object, _token);

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

                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                _commandMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _commandMock.As<ICqsRetrySpecific>()
                    .Setup(a => a.OnlyRetryForExceptionsOfTheseTypes).Returns(new[] { typeof(ArgumentException) });
                _commandMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckBaseTypes).Returns(false);
                _commandMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckInnerExceptions).Returns(true);

                Exception exception = new Exception(null, new ArgumentException());
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_commandMock.Object, _token);

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

                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                _commandMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _commandMock.As<ICqsRetrySpecific>()
                    .Setup(a => a.OnlyRetryForExceptionsOfTheseTypes).Returns(new[] { typeof(SystemException) });
                _commandMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckBaseTypes).Returns(false);
                _commandMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckInnerExceptions).Returns(true);

                ArgumentException exception = new ArgumentException();
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_commandMock.Object, _token);

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

                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                _commandMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _commandMock.As<ICqsRetrySpecific>()
                    .Setup(a => a.OnlyRetryForExceptionsOfTheseTypes).Returns(new[] { typeof(SystemException) });
                _commandMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckBaseTypes).Returns(true);
                _commandMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckInnerExceptions).Returns(true);

                ArgumentException exception = new ArgumentException();
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_commandMock.Object, _token);

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

                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                _commandMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _commandMock.As<ICqsRetrySpecific>()
                    .Setup(a => a.OnlyRetryForExceptionsOfTheseTypes).Returns(new[] { typeof(Exception) });
                _commandMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckBaseTypes).Returns(true);
                _commandMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckInnerExceptions).Returns(false);

                ArgumentException exception = new ArgumentException();
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_commandMock.Object, _token);

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

                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                _commandMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _commandMock.As<ICqsRetrySpecific>()
                    .Setup(a => a.OnlyRetryForExceptionsOfTheseTypes).Returns(new[] { typeof(SystemException) });
                _commandMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckBaseTypes).Returns(true);
                _commandMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckInnerExceptions).Returns(true);

                Exception exception = new Exception(null, new InvalidOperationException());
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_commandMock.Object, _token);

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

                CommandHandlerRetryDecorator<ICommand> target = new CommandHandlerRetryDecorator<ICommand>(_commandHandlerMock.Object);

                _commandMock.As<ICqsRetry>().Setup(a => a.MaxRetries).Returns(3);
                _commandMock.As<ICqsRetrySpecific>()
                    .Setup(a => a.OnlyRetryForExceptionsOfTheseTypes).Returns(new[] { typeof(ArgumentNullException) });
                _commandMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckBaseTypes).Returns(true);
                _commandMock.As<ICqsRetrySpecific>().Setup(a => a.RetryCheckInnerExceptions).Returns(true);

                Exception exception = new Exception(null, new InvalidOperationException());
                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object)).Callback(() =>
                    {
                        Thread.Sleep(WAIT_TIME);
                        throw exception;
                    });

                // Act
                Func<Task> act = async () => await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await Assert.ThrowsAsync<Exception>(act);
                timer.Stop();

                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME * 1);
            }
        }
    }
}