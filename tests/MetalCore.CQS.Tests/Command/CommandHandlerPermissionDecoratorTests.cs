using MetalCore.CQS.Command;
using MetalCore.CQS.Common.Results;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.Command
{
    public class CommandHandlerPermissionDecoratorTests
    {
        public class ConstructorMethods
        {
            private readonly Mock<ICommandHandler<ICommand>> _commandHandlerMock = new Mock<ICommandHandler<ICommand>>();
            private readonly List<ICommandPermission<ICommand>> _commandPermissions = new List<ICommandPermission<ICommand>>();

            [Fact]
            public void IfNullCommandHandler_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new CommandHandlerPermissionDecorator<ICommand>(null, _commandPermissions);

                // Assert
                Assert.Throws<ArgumentNullException>("commandHandler", act);
            }

            [Fact]
            public void IfNullPermissions_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new CommandHandlerPermissionDecorator<ICommand>(_commandHandlerMock.Object, null);

                // Assert
                Assert.Throws<ArgumentNullException>("permissions", act);
            }
        }

        public class ExecuteAsyncMethods
        {
            private readonly Mock<ICommandHandler<ICommand>> _commandHandlerMock = new Mock<ICommandHandler<ICommand>>();
            private readonly Mock<ICommand> _commandMock = new Mock<ICommand>();
            private readonly List<ICommandPermission<ICommand>> _commandPermissions = new List<ICommandPermission<ICommand>>();
            private readonly Mock<IResult> _resultMock = new Mock<IResult>();
            private readonly CancellationToken _token = default;

            [Fact]
            public async Task IfNullCommand_ThenCommandPushedToNextCommandHandler()
            {
                // Arrange
                CommandHandlerPermissionDecorator<ICommand> target = new CommandHandlerPermissionDecorator<ICommand>(_commandHandlerMock.Object, _commandPermissions);

                // Act
                await target.ExecuteAsync(null, _token);

                // Assert
                _commandHandlerMock.Verify(a => a.ExecuteAsync(null, _token), Times.Once);
            }

            [Fact]
            public async Task IfNullCommand_ThenOriginalResultReturned()
            {
                // Arrange
                CommandHandlerPermissionDecorator<ICommand> target = new CommandHandlerPermissionDecorator<ICommand>(_commandHandlerMock.Object, _commandPermissions);

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
                CommandHandlerPermissionDecorator<ICommand> target = new CommandHandlerPermissionDecorator<ICommand>(_commandHandlerMock.Object, _commandPermissions);

                // Act
                await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                _commandHandlerMock.Verify(a => a.ExecuteAsync(_commandMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfNonNullCommand_ThenOriginalResultReturned()
            {
                // Arrange
                CommandHandlerPermissionDecorator<ICommand> target = new CommandHandlerPermissionDecorator<ICommand>(_commandHandlerMock.Object, _commandPermissions);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult result = await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGiven1CommandPermission_ThenVerifyPermissionAsyncCalled()
            {
                // Arrange
                CommandHandlerPermissionDecorator<ICommand> target = new CommandHandlerPermissionDecorator<ICommand>(_commandHandlerMock.Object, _commandPermissions);

                Mock<ICommandPermission<ICommand>> permission = new Mock<ICommandPermission<ICommand>>();
                _commandPermissions.Add(permission.Object);

                // Act
                await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                permission.Verify(a => a.HasPermissionAsync(_commandMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven1CommandPermission_ThenOriginalResultReturned()
            {
                // Arrange
                CommandHandlerPermissionDecorator<ICommand> target = new CommandHandlerPermissionDecorator<ICommand>(_commandHandlerMock.Object, _commandPermissions);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                Mock<ICommandPermission<ICommand>> permission = new Mock<ICommandPermission<ICommand>>();
                permission.Setup(a => a.HasPermissionAsync(_commandMock.Object, _token)).Returns(Task.FromResult(true));
                _commandPermissions.Add(permission.Object);

                // Act
                IResult result = await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGiven2CommandPermissions_ThenVerifyPermissionAsyncCalledForEach()
            {
                // Arrange
                CommandHandlerPermissionDecorator<ICommand> target = new CommandHandlerPermissionDecorator<ICommand>(_commandHandlerMock.Object, _commandPermissions);

                Mock<ICommandPermission<ICommand>> permission = new Mock<ICommandPermission<ICommand>>();
                _commandPermissions.Add(permission.Object);

                Mock<ICommandPermission<ICommand>> permission2 = new Mock<ICommandPermission<ICommand>>();
                _commandPermissions.Add(permission2.Object);

                // Act
                await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                permission.Verify(a => a.HasPermissionAsync(_commandMock.Object, _token), Times.Once);
                permission2.Verify(a => a.HasPermissionAsync(_commandMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandPermissions_ThenOriginalResultReturned()
            {
                // Arrange
                CommandHandlerPermissionDecorator<ICommand> target = new CommandHandlerPermissionDecorator<ICommand>(_commandHandlerMock.Object, _commandPermissions);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                Mock<ICommandPermission<ICommand>> permission = new Mock<ICommandPermission<ICommand>>();
                permission.Setup(a => a.HasPermissionAsync(_commandMock.Object, _token)).Returns(Task.FromResult(true));
                _commandPermissions.Add(permission.Object);

                Mock<ICommandPermission<ICommand>> permission2 = new Mock<ICommandPermission<ICommand>>();
                permission2.Setup(a => a.HasPermissionAsync(_commandMock.Object, _token)).Returns(Task.FromResult(true));
                _commandPermissions.Add(permission2.Object);

                // Act
                IResult result = await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGiven2CommandPermissionsAndOneReturnsFalse_ThenReturnsNotOriginalResult()
            {
                // Arrange
                CommandHandlerPermissionDecorator<ICommand> target = new CommandHandlerPermissionDecorator<ICommand>(_commandHandlerMock.Object, _commandPermissions);

                Mock<ICommandPermission<ICommand>> permission = new Mock<ICommandPermission<ICommand>>();
                permission.Setup(a => a.HasPermissionAsync(_commandMock.Object, _token)).Returns(Task.FromResult(true));
                _commandPermissions.Add(permission.Object);

                Mock<ICommandPermission<ICommand>> permission2 = new Mock<ICommandPermission<ICommand>>();
                permission2.Setup(a => a.HasPermissionAsync(_commandMock.Object, _token)).Returns(Task.FromResult(false));
                _commandPermissions.Add(permission2.Object);

                // Act
                IResult result = await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                Assert.NotEqual(_resultMock.Object, result);
                Assert.True(result.HasNoPermissionError);
            }

            [Fact]
            public async Task IfGiven2CommandPermissions_ThenEachRunOnSeparateThreads()
            {
                // Arrange
                const int WAIT_TIME = 500;

                CommandHandlerPermissionDecorator<ICommand> target = new CommandHandlerPermissionDecorator<ICommand>(_commandHandlerMock.Object, _commandPermissions);

                Mock<ICommandPermission<ICommand>> permission = new Mock<ICommandPermission<ICommand>>();
                permission.Setup(a => a.HasPermissionAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(true)).Callback(() => Thread.Sleep(WAIT_TIME));
                _commandPermissions.Add(permission.Object);

                Mock<ICommandPermission<ICommand>> permission2 = new Mock<ICommandPermission<ICommand>>();
                permission2.Setup(a => a.HasPermissionAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(true)).Callback(() => Thread.Sleep(WAIT_TIME));
                _commandPermissions.Add(permission2.Object);

                // Act
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await target.ExecuteAsync(_commandMock.Object, _token);
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }
        }
    }
}