using AutoFixture.Xunit2;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Exceptions;
using MetalCore.CQS.Query;
using Moq;
using Moq.Protected;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.Query
{
    public class QueryHandlerExceptionDecoratorTests
    {
        public class QueryHandlerExceptionDecoratorStub : QueryHandlerExceptionDecorator<IQuery<string>, string>
        {
            public QueryHandlerExceptionDecoratorStub(IQueryHandler<IQuery<string>, string> queryHandler) : base(queryHandler)
            {
            }

            protected override Task HandleBrokenRuleExceptionAsync(IQuery<string> query, BrokenRuleException ex)
            {
                return Task.CompletedTask;
            }

            protected override Task HandleConcurrencyExceptionAsync(IQuery<string> query, ConcurrencyException ex)
            {
                return Task.CompletedTask;
            }

            protected override Task HandleDataNotFoundExceptionAsync(IQuery<string> query, DataNotFoundException ex)
            {
                return Task.CompletedTask;
            }

            protected override Task HandleNoPermissionExceptionAsync(IQuery<string> query, NoPermissionException ex)
            {
                return Task.CompletedTask;
            }

            protected override Task HandleUserFriendlyExceptionAsync(IQuery<string> query, UserFriendlyException ex)
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
                Action act = () => new QueryHandlerExceptionDecoratorStub(null);

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
                Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object)
                {
                    CallBase = true
                };

                // Act
                await target.Object.ExecuteAsync(null, _token);

                // Assert
                _queryHandlerMock.Verify(a => a.ExecuteAsync(null, _token), Times.Once);
            }

            [Fact]
            public async Task IfNullQuery_ThenOriginalResultReturned()
            {
                // Arrange
                Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object)
                {
                    CallBase = true
                };

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
                Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object)
                {
                    CallBase = true
                };

                // Act
                await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                _queryHandlerMock.Verify(a => a.ExecuteAsync(_queryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfNonNullQuery_ThenOriginalResultReturned()
            {
                // Arrange
                Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object)
                {
                    CallBase = true
                };

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Theory, AutoData]
            public async Task IfBrokenRuleExceptionThrown_ThenBrokenRuleResultReturned(
                BrokenRuleException exception)
            {
                // Arrange
                Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object)
                {
                    CallBase = true
                };

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Throws(exception);

                // Act
                IResult<string> result = await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                Assert.NotEqual(_resultMock.Object, result);
                Assert.False(result.IsSuccessful);
            }

            [Theory, AutoData]
            public async Task IfUserFriendlyExceptionThrown_ThenErrorResultReturned(
                UserFriendlyException exception)
            {
                // Arrange
                Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object)
                {
                    CallBase = true
                };

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Throws(exception);

                // Act
                IResult<string> result = await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                Assert.NotEqual(_resultMock.Object, result);
                Assert.Equal(exception.Message, result.ErrorMessage);
                Assert.False(result.IsSuccessful);
            }

            [Theory, AutoData]
            public async Task IfUserFriendlyExceptionThrown_ThenHandleUserFriendlyExceptionAsyncCalled(
                UserFriendlyException exception)
            {
                // Arrange
                Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object)
                {
                    CallBase = true
                };

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Throws(exception);

                // Act
                await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                target.Protected().Verify(
                    "HandleUserFriendlyExceptionAsync",
                    Times.Once(),
                    _queryMock.Object,
                    ItExpr.IsAny<UserFriendlyException>());

            }

            [Theory, AutoData]
            public async Task IfNoPermissionExceptionThrown_ThenNoPermissionResultReturned(
                NoPermissionException exception)
            {
                // Arrange
                Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object)
                {
                    CallBase = true
                };

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Throws(exception);

                // Act
                IResult<string> result = await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                Assert.NotEqual(_resultMock.Object, result);
                Assert.True(result.HasNoPermissionError);
                Assert.False(result.IsSuccessful);
            }

            [Theory, AutoData]
            public async Task IfNoPermissionExceptionThrown_ThenHandleNoPermissionExceptionAsyncCalled(
                NoPermissionException exception)
            {
                // Arrange
                Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object)
                {
                    CallBase = true
                };

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Throws(exception);

                // Act
                await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                target.Protected().Verify(
                 "HandleNoPermissionExceptionAsync",
                 Times.Once(),
                 _queryMock.Object,
                 ItExpr.IsAny<NoPermissionException>());
            }

            [Theory, AutoData]
            public async Task IfDataNotFoundExceptionThrown_ThenDataNotFoundResultReturned(
                DataNotFoundException exception)
            {
                // Arrange
                Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object)
                {
                    CallBase = true
                };

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Throws(exception);

                // Act
                IResult<string> result = await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                Assert.NotEqual(_resultMock.Object, result);
                Assert.True(result.HasDataNotFoundError);
                Assert.False(result.IsSuccessful);
            }

            [Theory, AutoData]
            public async Task IfDataNotFoundExceptionThrown_ThenHandleDataNotFoundExceptionAsyncCalled(
                DataNotFoundException exception)
            {
                // Arrange
                Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object)
                {
                    CallBase = true
                };

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Throws(exception);

                // Act
                IResult<string> result = await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                target.Protected().Verify(
                  "HandleDataNotFoundExceptionAsync",
                  Times.Once(),
                  _queryMock.Object,
                  ItExpr.IsAny<DataNotFoundException>());
            }

            [Theory, AutoData]
            public async Task IfConcurrencyExceptionThrown_ThenConcurrencyResultReturned(
                ConcurrencyException exception)
            {
                // Arrange
                Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object)
                {
                    CallBase = true
                };

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Throws(exception);

                // Act
                IResult<string> result = await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                Assert.NotEqual(_resultMock.Object, result);
                Assert.True(result.HasConcurrencyError);
                Assert.False(result.IsSuccessful);
            }

            [Theory, AutoData]
            public async Task IfConcurrencyExceptionThrown_ThenHandleConcurrencyExceptionAsyncCalled(
                ConcurrencyException exception)
            {
                // Arrange
                Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object)
                {
                    CallBase = true
                };

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Throws(exception);

                // Act
                await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                target.Protected().Verify(
                    "HandleConcurrencyExceptionAsync",
                    Times.Once(),
                    _queryMock.Object,
                    ItExpr.IsAny<ConcurrencyException>());
            }

            [Theory, AutoData]
            public async Task IfExceptionThrown_ThenThrowsException(Exception exception)
            {
                // Arrange
                Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>> target = new Mock<QueryHandlerExceptionDecorator<IQuery<string>, string>>(
                    _queryHandlerMock.Object)
                {
                    CallBase = true
                };

                _queryHandlerMock.Setup(a => a.ExecuteAsync(_queryMock.Object, _token)).Throws(exception);

                // Act
                Func<Task> act = async () => await target.Object.ExecuteAsync(_queryMock.Object, _token);

                // Assert
                await Assert.ThrowsAsync<Exception>(act);
            }
        }
    }
}