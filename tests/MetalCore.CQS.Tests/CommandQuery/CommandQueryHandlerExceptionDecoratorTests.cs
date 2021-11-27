using AutoFixture.Xunit2;
using MetalCore.CQS.CommandQuery;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Exceptions;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.CommandQuery
{
    public class CommandQueryHandlerExceptionDecoratorTests
    {
        public class CommandQueryHandlerExceptionDecoratorStub : CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>
        {
            public CommandQueryHandlerExceptionDecoratorStub(ICommandQueryHandler<ICommandQuery<string>, string> commandQueryHandler) : base(commandQueryHandler)
            {
            }

            protected override Task HandleBrokenRuleExceptionAsync(ICommandQuery<string> commandQuery, BrokenRuleException ex)
            {
                return Task.CompletedTask;
            }

            protected override Task HandleConcurrencyExceptionAsync(ICommandQuery<string> commandQuery, ConcurrencyException ex)
            {
                return Task.CompletedTask;
            }

            protected override Task HandleDataNotFoundExceptionAsync(ICommandQuery<string> commandQuery, DataNotFoundException ex)
            {
                return Task.CompletedTask;
            }

            protected override Task HandleNoPermissionExceptionAsync(ICommandQuery<string> commandQuery, NoPermissionException ex)
            {
                return Task.CompletedTask;
            }

            protected override Task HandleUserFriendlyExceptionAsync(ICommandQuery<string> commandQuery, UserFriendlyException ex)
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
                Action act = () => new CommandQueryHandlerExceptionDecoratorStub(null);

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
                Mock<CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object)
                {
                    CallBase = true
                };

                // Act
                await target.Object.ExecuteAsync(null, _token);

                // Assert
                _commandQueryHandlerMock.Verify(a => a.ExecuteAsync(null, _token), Times.Once);
            }

            [Fact]
            public async Task IfNullCommandQuery_ThenOriginalResultReturned()
            {
                // Arrange
                Mock<CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object)
                {
                    CallBase = true
                };

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
                Mock<CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object)
                {
                    CallBase = true
                };

                // Act
                await target.Object.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                _commandQueryHandlerMock.Verify(a => a.ExecuteAsync(_commandQueryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfNonNullCommandQuery_ThenOriginalResultReturned()
            {
                // Arrange
                Mock<CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object)
                {
                    CallBase = true
                };

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.Object.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Theory, AutoData]
            public async Task IfBrokenRuleExceptionThrown_ThenBrokenRuleResultReturned(
                BrokenRuleException exception)
            {
                // Arrange
                Mock<CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object)
                { CallBase = true };

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Throws(exception);

                // Act
                IResult<string> result = await target.Object.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.NotEqual(_resultMock.Object, result);
                Assert.True(result.BrokenRules.Any());
                Assert.False(result.IsSuccessful);
            }


            [Theory, AutoData]
            public async Task IfUserFriendlyExceptionThrown_ThenErrorResultReturned(
                UserFriendlyException exception)
            {
                // Arrange
                Mock<CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object)
                {
                    CallBase = true
                };

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Throws(exception);

                // Act
                IResult<string> result = await target.Object.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.NotEqual(_resultMock.Object, result);
                Assert.Equal(exception.Message, result.ErrorMessage);
                Assert.False(result.IsSuccessful);
            }

            [Theory, AutoData]
            public async Task IfNoPermissionExceptionThrown_ThenNoPermissionResultReturned(
                NoPermissionException exception)
            {
                // Arrange
                Mock<CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object)
                {
                    CallBase = true
                };

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Throws(exception);

                // Act
                IResult<string> result = await target.Object.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.NotEqual(_resultMock.Object, result);
                Assert.True(result.HasNoPermissionError);
                Assert.False(result.IsSuccessful);
            }

            [Theory, AutoData]
            public async Task IfDataNotFoundExceptionThrown_ThenDataNotFoundResultReturned(
                DataNotFoundException exception)
            {
                // Arrange
                Mock<CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object)
                {
                    CallBase = true
                };

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Throws(exception);

                // Act
                IResult<string> result = await target.Object.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.NotEqual(_resultMock.Object, result);
                Assert.True(result.HasDataNotFoundError);
                Assert.False(result.IsSuccessful);
            }

            [Theory, AutoData]
            public async Task IfConcurrencyExceptionThrown_ThenConcurrencyResultReturned(
                ConcurrencyException exception)
            {
                // Arrange
                Mock<CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object)
                {
                    CallBase = true
                };

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Throws(exception);

                // Act
                IResult<string> result = await target.Object.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.NotEqual(_resultMock.Object, result);
                Assert.True(result.HasConcurrencyError);
                Assert.False(result.IsSuccessful);
            }

            [Theory, AutoData]
            public async Task IfExceptionThrown_ThenErrorResultReturned(Exception exception)
            {
                // Arrange
                Mock<CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>> target = new Mock<CommandQueryHandlerExceptionDecorator<ICommandQuery<string>, string>>(
                    _commandQueryHandlerMock.Object)
                {
                    CallBase = true
                };

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Throws(exception);

                // Act
                Func<Task> act = async () => await target.Object.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                await Assert.ThrowsAsync<Exception>(act);
            }
        }
    }
}