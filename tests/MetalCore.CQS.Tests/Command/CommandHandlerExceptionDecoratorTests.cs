using AutoFixture.Xunit2;
using MetalCore.CQS.Command;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Exceptions;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.Command
{
    public class CommandHandlerExceptionDecoratorTests
    {
        internal class CommandHandlerExceptionDecoratorStub : CommandHandlerExceptionDecorator<ICommand>
        {
            public CommandHandlerExceptionDecoratorStub(ICommandHandler<ICommand> commandHandler) : base(commandHandler)
            {
            }

            protected override Task HandleBrokenRuleExceptionAsync(ICommand command, BrokenRuleException ex)
            {
                return Task.CompletedTask;
            }

            protected override Task HandleConcurrencyExceptionAsync(ICommand command, ConcurrencyException ex)
            {
                return Task.CompletedTask;
            }

            protected override Task HandleDataNotFoundExceptionAsync(ICommand command, DataNotFoundException ex)
            {
                return Task.CompletedTask;
            }

            protected override Task HandleNoPermissionExceptionAsync(ICommand command, NoPermissionException ex)
            {
                return Task.CompletedTask;
            }

            protected override Task HandleUserFriendlyExceptionAsync(ICommand command, UserFriendlyException ex)
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
                Action act = () =>
                    new CommandHandlerExceptionDecoratorStub(null);

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
                Mock<CommandHandlerExceptionDecorator<ICommand>> target = new Mock<CommandHandlerExceptionDecorator<ICommand>>(
                    _commandHandlerMock.Object)
                {
                    CallBase = true
                };

                // Act
                await target.Object.ExecuteAsync(null, _token);

                // Assert
                _commandHandlerMock.Verify(a => a.ExecuteAsync(null, _token), Times.Once);
            }

            [Fact]
            public async Task IfNullCommand_ThenOriginalResultReturned()
            {
                // Arrange
                Mock<CommandHandlerExceptionDecorator<ICommand>> target = new Mock<CommandHandlerExceptionDecorator<ICommand>>(
                    _commandHandlerMock.Object)
                {
                    CallBase = true
                };

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
                Mock<CommandHandlerExceptionDecorator<ICommand>> target = new Mock<CommandHandlerExceptionDecorator<ICommand>>(
                    _commandHandlerMock.Object)
                {
                    CallBase = true
                };

                // Act
                await target.Object.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                _commandHandlerMock.Verify(a => a.ExecuteAsync(_commandMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfNonNullCommand_ThenOriginalResultReturned()
            {
                // Arrange
                Mock<CommandHandlerExceptionDecorator<ICommand>> target = new Mock<CommandHandlerExceptionDecorator<ICommand>>(
                    _commandHandlerMock.Object)
                {
                    CallBase = true
                };

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult result = await target.Object.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Theory, AutoData]
            public async Task IfBrokenRuleExceptionThrown_ThenBrokenRuleResultReturned(
                BrokenRuleException exception)
            {
                // Arrange
                Mock<CommandHandlerExceptionDecorator<ICommand>> target = new Mock<CommandHandlerExceptionDecorator<ICommand>>(
                    _commandHandlerMock.Object)
                {
                    CallBase = true
                };

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Throws(exception);

                // Act
                IResult result = await target.Object.ExecuteAsync(_commandMock.Object, _token);

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
                Mock<CommandHandlerExceptionDecorator<ICommand>> target = new Mock<CommandHandlerExceptionDecorator<ICommand>>(
                    _commandHandlerMock.Object)
                {
                    CallBase = true
                };

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Throws(exception);

                // Act
                IResult result = await target.Object.ExecuteAsync(_commandMock.Object, _token);

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
                Mock<CommandHandlerExceptionDecorator<ICommand>> target = new Mock<CommandHandlerExceptionDecorator<ICommand>>(
                    _commandHandlerMock.Object)
                {
                    CallBase = true
                };

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Throws(exception);

                // Act
                IResult result = await target.Object.ExecuteAsync(_commandMock.Object, _token);

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
                Mock<CommandHandlerExceptionDecorator<ICommand>> target = new Mock<CommandHandlerExceptionDecorator<ICommand>>(
                    _commandHandlerMock.Object)
                {
                    CallBase = true
                };

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Throws(exception);

                // Act
                IResult result = await target.Object.ExecuteAsync(_commandMock.Object, _token);

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
                Mock<CommandHandlerExceptionDecorator<ICommand>> target = new Mock<CommandHandlerExceptionDecorator<ICommand>>(
                    _commandHandlerMock.Object)
                {
                    CallBase = true
                };

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Throws(exception);

                // Act
                IResult result = await target.Object.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                Assert.NotEqual(_resultMock.Object, result);
                Assert.True(result.HasConcurrencyError);
                Assert.False(result.IsSuccessful);
            }

            [Theory, AutoData]
            public async Task IfExceptionThrown_ThenErrorResultReturned(Exception exception)
            {
                // Arrange
                Mock<CommandHandlerExceptionDecorator<ICommand>> target = new Mock<CommandHandlerExceptionDecorator<ICommand>>(
                    _commandHandlerMock.Object)
                {
                    CallBase = true
                };

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Throws(exception);

                // Act
                Func<Task> act = async () => await target.Object.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                await Assert.ThrowsAsync<Exception>(act);
            }
        }
    }
}