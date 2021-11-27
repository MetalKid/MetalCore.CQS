using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.Query
{
    public class QueryHandlerPermissionDecoratorTests
    {
        public class ConstructorMethods
        {
            private readonly Mock<IQueryHandler<IQuery<string>, string>> _queryHandlerMock = new Mock<IQueryHandler<IQuery<string>, string>>();
            private readonly List<IQueryPermission<IQuery<string>, string>> _queryPermissions = new List<IQueryPermission<IQuery<string>, string>>();

            [Fact]
            public void IfNullQueryHandler_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new QueryHandlerPermissionDecorator<IQuery<string>, string>(null, _queryPermissions);

                // Assert
                Assert.Throws<ArgumentNullException>("queryHandler", act);
            }

            [Fact]
            public void IfNullPermissions_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new QueryHandlerPermissionDecorator<IQuery<string>, string>(_queryHandlerMock.Object, null);

                // Assert
                Assert.Throws<ArgumentNullException>("permissions", act);
            }
        }

        public class ExecuteAsyncMethods
        {
            private readonly Mock<IQueryHandler<IQuery<string>, string>> _queryHandlerMock = new Mock<IQueryHandler<IQuery<string>, string>>();
            private readonly Mock<IQuery<string>> _queryMock = new Mock<IQuery<string>>();
            private readonly List<IQueryPermission<IQuery<string>, string>> _queryPermissions = new List<IQueryPermission<IQuery<string>, string>>();
            private readonly Mock<IResult<string>> _resultMock = new Mock<IResult<string>>();
            private readonly CancellationToken _token = default;

            [Fact]
            public async Task IfNullQuery_ThenQueryPushedToNextQueryHandler()
            {
                // Arrange
                QueryHandlerPermissionDecorator<IQuery<string>, string> target = new QueryHandlerPermissionDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _queryPermissions);

                // Act
                await target.ExecuteAsync(null, _token);

                // Assert
                _queryHandlerMock.Verify(a => a.ExecuteAsync(null, _token), Times.Once);
            }

            [Fact]
            public async Task IfNullQuery_ThenOriginalResultReturned()
            {
                // Arrange
                QueryHandlerPermissionDecorator<IQuery<string>, string> target = new QueryHandlerPermissionDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _queryPermissions);

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
                QueryHandlerPermissionDecorator<IQuery<string>, string> target = new QueryHandlerPermissionDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _queryPermissions);

                // Act
                await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                _queryHandlerMock.Verify(a => a.ExecuteAsync(_queryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfNonNullQuery_ThenOriginalResultReturned()
            {
                // Arrange
                QueryHandlerPermissionDecorator<IQuery<string>, string> target = new QueryHandlerPermissionDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _queryPermissions);

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGiven1QueryPermission_ThenVerifyPermissionAsyncCalled()
            {
                // Arrange
                QueryHandlerPermissionDecorator<IQuery<string>, string> target = new QueryHandlerPermissionDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _queryPermissions);

                Mock<IQueryPermission<IQuery<string>, string>> permission = new Mock<IQueryPermission<IQuery<string>, string>>();
                _queryPermissions.Add(permission.Object);

                // Act
                await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                permission.Verify(a => a.HasPermissionAsync(_queryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven1QueryPermission_ThenOriginalResultReturned()
            {
                // Arrange
                QueryHandlerPermissionDecorator<IQuery<string>, string> target = new QueryHandlerPermissionDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _queryPermissions);

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                Mock<IQueryPermission<IQuery<string>, string>> permission = new Mock<IQueryPermission<IQuery<string>, string>>();
                permission.Setup(a => a.HasPermissionAsync(_queryMock.Object, _token)).Returns(Task.FromResult(true));
                _queryPermissions.Add(permission.Object);

                // Act
                IResult<string> result = await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGiven2QueryPermissions_ThenVerifyPermissionAsyncCalledForEach()
            {
                // Arrange
                QueryHandlerPermissionDecorator<IQuery<string>, string> target = new QueryHandlerPermissionDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _queryPermissions);

                Mock<IQueryPermission<IQuery<string>, string>> permission = new Mock<IQueryPermission<IQuery<string>, string>>();
                _queryPermissions.Add(permission.Object);

                Mock<IQueryPermission<IQuery<string>, string>> permission2 = new Mock<IQueryPermission<IQuery<string>, string>>();
                _queryPermissions.Add(permission2.Object);

                // Act
                await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                permission.Verify(a => a.HasPermissionAsync(_queryMock.Object, _token), Times.Once);
                permission2.Verify(a => a.HasPermissionAsync(_queryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2QueryPermissions_ThenOriginalResultReturned()
            {
                // Arrange
                QueryHandlerPermissionDecorator<IQuery<string>, string> target = new QueryHandlerPermissionDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _queryPermissions);

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                Mock<IQueryPermission<IQuery<string>, string>> permission = new Mock<IQueryPermission<IQuery<string>, string>>();
                permission.Setup(a => a.HasPermissionAsync(_queryMock.Object, _token)).Returns(Task.FromResult(true));
                _queryPermissions.Add(permission.Object);

                Mock<IQueryPermission<IQuery<string>, string>> permission2 = new Mock<IQueryPermission<IQuery<string>, string>>();
                permission2.Setup(a => a.HasPermissionAsync(_queryMock.Object, _token)).Returns(Task.FromResult(true));
                _queryPermissions.Add(permission2.Object);

                // Act
                IResult<string> result = await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGiven2QueryPermissionsAndOneReturnsFalse_ThenReturnsNotOriginalResult()
            {
                // Arrange
                QueryHandlerPermissionDecorator<IQuery<string>, string> target = new QueryHandlerPermissionDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _queryPermissions);

                Mock<IQueryPermission<IQuery<string>, string>> permission = new Mock<IQueryPermission<IQuery<string>, string>>();
                permission.Setup(a => a.HasPermissionAsync(_queryMock.Object, _token)).Returns(Task.FromResult(true));
                _queryPermissions.Add(permission.Object);

                Mock<IQueryPermission<IQuery<string>, string>> permission2 = new Mock<IQueryPermission<IQuery<string>, string>>();
                permission2.Setup(a => a.HasPermissionAsync(_queryMock.Object, _token)).Returns(Task.FromResult(false));
                _queryPermissions.Add(permission2.Object);

                // Act
                IResult<string> result = await target.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                Assert.NotEqual(_resultMock.Object, result);
                Assert.True(result.HasNoPermissionError);
            }

            [Fact]
            public async Task IfGiven2QueryPermissions_ThenEachRunOnSeparateThreads()
            {
                // Arrange
                const int WAIT_TIME = 500;

                QueryHandlerPermissionDecorator<IQuery<string>, string> target = new QueryHandlerPermissionDecorator<IQuery<string>, string>(_queryHandlerMock.Object, _queryPermissions);

                Mock<IQueryPermission<IQuery<string>, string>> permission = new Mock<IQueryPermission<IQuery<string>, string>>();
                permission.Setup(a => a.HasPermissionAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(true)).Callback(() => Thread.Sleep(WAIT_TIME));
                _queryPermissions.Add(permission.Object);

                Mock<IQueryPermission<IQuery<string>, string>> permission2 = new Mock<IQueryPermission<IQuery<string>, string>>();
                permission2.Setup(a => a.HasPermissionAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(true)).Callback(() => Thread.Sleep(WAIT_TIME));
                _queryPermissions.Add(permission2.Object);

                // Act
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await target.ExecuteAsync(_queryMock.Object, _token);
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }
        }
    }
}