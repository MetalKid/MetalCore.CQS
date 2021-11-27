using MetalCore.CQS.CommandQuery;
using MetalCore.CQS.Common.Results;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.CommandQuery
{
    public class CommandQueryHandlerPermissionDecoratorTests
    {
        public class ConstructorMethods
        {
            private readonly Mock<ICommandQueryHandler<ICommandQuery<string>, string>> _commandQueryHandlerMock = new Mock<ICommandQueryHandler<ICommandQuery<string>, string>>();
            private readonly List<ICommandQueryPermission<ICommandQuery<string>, string>> _commandQueryPermissions = new List<ICommandQueryPermission<ICommandQuery<string>, string>>();

            [Fact]
            public void IfNullCommandQueryHandler_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string>(null, _commandQueryPermissions);

                // Assert
                Assert.Throws<ArgumentNullException>("commandQueryHandler", act);
            }

            [Fact]
            public void IfNullPermissions_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, null);

                // Assert
                Assert.Throws<ArgumentNullException>("permissions", act);
            }
        }

        public class ExecuteAsyncMethods
        {
            private readonly Mock<ICommandQueryHandler<ICommandQuery<string>, string>> _commandQueryHandlerMock = new Mock<ICommandQueryHandler<ICommandQuery<string>, string>>();
            private readonly Mock<ICommandQuery<string>> _commandQueryMock = new Mock<ICommandQuery<string>>();
            private readonly List<ICommandQueryPermission<ICommandQuery<string>, string>> _commandQueryPermissions = new List<ICommandQueryPermission<ICommandQuery<string>, string>>();
            private readonly Mock<IResult<string>> _resultMock = new Mock<IResult<string>>();
            private readonly CancellationToken _token = default;

            [Fact]
            public async Task IfNullCommandQuery_ThenCommandQueryPushedToNextCommandQueryHandler()
            {
                // Arrange
                CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _commandQueryPermissions);

                // Act
                await target.ExecuteAsync(null, _token);

                // Assert
                _commandQueryHandlerMock.Verify(a => a.ExecuteAsync(null, _token), Times.Once);
            }

            [Fact]
            public async Task IfNullCommandQuery_ThenOriginalResultReturned()
            {
                // Arrange
                CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _commandQueryPermissions);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(null, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.ExecuteAsync(null, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfNonNullCommandQuery_ThenCommandQueryPushedToNextCommandQueryHandler()
            {
                // Arrange
                CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _commandQueryPermissions);

                // Act
                await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                _commandQueryHandlerMock.Verify(a => a.ExecuteAsync(_commandQueryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfNonNullCommandQuery_ThenOriginalResultReturned()
            {
                // Arrange
                CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _commandQueryPermissions);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGiven1CommandQueryPermission_ThenVerifyPermissionAsyncCalled()
            {
                // Arrange
                CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _commandQueryPermissions);

                Mock<ICommandQueryPermission<ICommandQuery<string>, string>> permission = new Mock<ICommandQueryPermission<ICommandQuery<string>, string>>();
                _commandQueryPermissions.Add(permission.Object);

                // Act
                await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                permission.Verify(a => a.HasPermissionAsync(_commandQueryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven1CommandQueryPermission_ThenOriginalResultReturned()
            {
                // Arrange
                CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _commandQueryPermissions);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                Mock<ICommandQueryPermission<ICommandQuery<string>, string>> permission = new Mock<ICommandQueryPermission<ICommandQuery<string>, string>>();
                permission.Setup(a => a.HasPermissionAsync(_commandQueryMock.Object, _token)).Returns(Task.FromResult(true));
                _commandQueryPermissions.Add(permission.Object);

                // Act
                IResult<string> result = await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGiven2CommandQueryPermissions_ThenVerifyPermissionAsyncCalledForEach()
            {
                // Arrange
                CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _commandQueryPermissions);

                Mock<ICommandQueryPermission<ICommandQuery<string>, string>> permission = new Mock<ICommandQueryPermission<ICommandQuery<string>, string>>();
                _commandQueryPermissions.Add(permission.Object);

                Mock<ICommandQueryPermission<ICommandQuery<string>, string>> permission2 = new Mock<ICommandQueryPermission<ICommandQuery<string>, string>>();
                _commandQueryPermissions.Add(permission2.Object);

                // Act
                await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                permission.Verify(a => a.HasPermissionAsync(_commandQueryMock.Object, _token), Times.Once);
                permission2.Verify(a => a.HasPermissionAsync(_commandQueryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandQueryPermissions_ThenOriginalResultReturned()
            {
                // Arrange
                CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _commandQueryPermissions);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                Mock<ICommandQueryPermission<ICommandQuery<string>, string>> permission = new Mock<ICommandQueryPermission<ICommandQuery<string>, string>>();
                permission.Setup(a => a.HasPermissionAsync(_commandQueryMock.Object, _token)).Returns(Task.FromResult(true));
                _commandQueryPermissions.Add(permission.Object);

                Mock<ICommandQueryPermission<ICommandQuery<string>, string>> permission2 = new Mock<ICommandQueryPermission<ICommandQuery<string>, string>>();
                permission2.Setup(a => a.HasPermissionAsync(_commandQueryMock.Object, _token)).Returns(Task.FromResult(true));
                _commandQueryPermissions.Add(permission2.Object);

                // Act
                IResult<string> result = await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGiven2CommandQueryPermissionsAndOneReturnsFalse_ThenReturnsNotOriginalResult()
            {
                // Arrange
                CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _commandQueryPermissions);

                Mock<ICommandQueryPermission<ICommandQuery<string>, string>> permission = new Mock<ICommandQueryPermission<ICommandQuery<string>, string>>();
                permission.Setup(a => a.HasPermissionAsync(_commandQueryMock.Object, _token)).Returns(Task.FromResult(true));
                _commandQueryPermissions.Add(permission.Object);

                Mock<ICommandQueryPermission<ICommandQuery<string>, string>> permission2 = new Mock<ICommandQueryPermission<ICommandQuery<string>, string>>();
                permission2.Setup(a => a.HasPermissionAsync(_commandQueryMock.Object, _token)).Returns(Task.FromResult(false));
                _commandQueryPermissions.Add(permission2.Object);

                // Act
                IResult<string> result = await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.NotEqual(_resultMock.Object, result);
                Assert.True(result.HasNoPermissionError);
            }

            [Fact]
            public async Task IfGiven2CommandQueryPermissions_ThenEachRunOnSeparateThreads()
            {
                // Arrange
                const int WAIT_TIME = 500;

                CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerPermissionDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _commandQueryPermissions);

                Mock<ICommandQueryPermission<ICommandQuery<string>, string>> permission = new Mock<ICommandQueryPermission<ICommandQuery<string>, string>>();
                permission.Setup(a => a.HasPermissionAsync(_commandQueryMock.Object, _token))
                    .Returns(Task.FromResult(true)).Callback(() => Thread.Sleep(WAIT_TIME));
                _commandQueryPermissions.Add(permission.Object);

                Mock<ICommandQueryPermission<ICommandQuery<string>, string>> permission2 = new Mock<ICommandQueryPermission<ICommandQuery<string>, string>>();
                permission2.Setup(a => a.HasPermissionAsync(_commandQueryMock.Object, _token))
                    .Returns(Task.FromResult(true)).Callback(() => Thread.Sleep(WAIT_TIME));
                _commandQueryPermissions.Add(permission2.Object);

                // Act
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await target.ExecuteAsync(_commandQueryMock.Object, _token);
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }
        }
    }
}