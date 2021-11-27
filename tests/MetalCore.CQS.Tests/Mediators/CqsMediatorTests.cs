using MetalCore.CQS.Command;
using MetalCore.CQS.CommandQuery;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Mediators;
using MetalCore.CQS.Query;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.Mediators
{
    public class CqsMediatorTests
    {
        public class ContstructorMethods
        {
            [Fact]
            public void IfNullCallback_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new CqsMediator(null);

                // Assert
                Assert.Throws<ArgumentNullException>("getInstanceCallback", act);
            }

            [Fact]
            public void IfNotNullCallback_ThenNoExceptionThrownAndNotNullInstance()
            {
                // Arrange

                // Act
                CqsMediator result = new CqsMediator(t => null);

                // Assert
                Assert.NotNull(result);
            }
        }

        public class ExecuteAsync_Command
        {
            private readonly Mock<ICommandHandler<ICommand>> _commandHander = new Mock<ICommandHandler<ICommand>>();
            private readonly CqsMediator _mediator;
            private readonly Mock<ICommand> _command = new Mock<ICommand>();
            private readonly CancellationToken _token = CancellationToken.None;
            private readonly Mock<IResult> _result = new Mock<IResult>();

            public ExecuteAsync_Command()
            {
                _mediator = new CqsMediator(t => _commandHander.Object);
            }

            [Fact]
            public async Task IfNullCommand_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Func<Task> act = async () => await _mediator.ExecuteAsync((ICommand)null, CancellationToken.None);

                // Assert
                await Assert.ThrowsAsync<ArgumentNullException>("command", act);
            }

            [Fact]
            public async Task IfCommandHandlerNotFoundInCallback_ThenTypeLoadExceptionThrown()
            {
                // Arrange

                // Act
                Func<Task> act = async () => await new CqsMediator(t => null).ExecuteAsync(_command.Object, CancellationToken.None);

                // Assert
                await Assert.ThrowsAsync<TypeLoadException>(act);
            }

            [Fact]
            public async Task IfCommandHandlerFoundInCallback_ThenExecuteAsyncCalled()
            {
                // Arrange
                _commandHander.Setup(a => a.ExecuteAsync(_command.Object, _token)).Returns(Task.FromResult(_result.Object));

                // Act
                IResult result = await _mediator.ExecuteAsync(_command.Object, _token);

                // Assert
                Assert.Equal(_result.Object, result);
            }

            [Fact]
            public async Task IfCommandHandlerFoundInCallbackAndNoToken_ThenExecuteAsyncCalled()
            {
                // Arrange
                _commandHander.Setup(a => a.ExecuteAsync(_command.Object, It.IsAny<CancellationToken>())).Returns(Task.FromResult(_result.Object));

                // Act
                IResult result = await _mediator.ExecuteAsync(_command.Object, _token);

                // Assert
                Assert.Equal(_result.Object, result);
            }
        }

        public class ExecuteAsync_CommandQuery
        {
            private readonly Mock<ICommandQueryHandler<ICommandQuery<string>, string>> _commandHander = new Mock<ICommandQueryHandler<ICommandQuery<string>, string>>();
            private readonly CqsMediator _mediator;
            private readonly Mock<ICommandQuery<string>> _commandQuery = new Mock<ICommandQuery<string>>();
            private readonly CancellationToken _token = CancellationToken.None;
            private readonly Mock<IResult<string>> _result = new Mock<IResult<string>>();

            public ExecuteAsync_CommandQuery()
            {
                _mediator = new CqsMediator(t => _commandHander.Object);
            }

            [Fact]
            public async Task IfNullCommandQuery_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Func<Task> act = async () => await _mediator.ExecuteAsync((ICommandQuery<string>)null, CancellationToken.None);

                // Assert
                await Assert.ThrowsAsync<ArgumentNullException>("commandQuery", act);
            }

            [Fact]
            public async Task IfCommandQueryHandlerNotFoundInCallback_ThenTypeLoadExceptionThrown()
            {
                // Arrange

                // Act
                Func<Task> act = async () => await new CqsMediator(t => null).ExecuteAsync(_commandQuery.Object, CancellationToken.None);

                // Assert
                await Assert .ThrowsAsync<TypeLoadException>(act);
            }

            [Fact]
            public async Task IfCommandQueryHandlerFoundInCallback_ThenExecuteAsyncCalled()
            {
                // Arrange
                _commandHander.Setup(a => a.ExecuteAsync(_commandQuery.Object, _token))
                    .Returns(Task.FromResult(_result.Object));

                // Act
                IResult<string> result = await _mediator.ExecuteAsync(_commandQuery.Object, _token);

                // Assert
                Assert.Equal(_result.Object, result);
            }

            [Fact]
            public async Task IfCommandQueryHandlerFoundInCallbackAndNoToken_ThenExecuteAsyncCalled()
            {
                // Arrange
                _commandHander.Setup(a => a.ExecuteAsync(_commandQuery.Object, It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(_result.Object));

                // Act
                IResult<string> result = await _mediator.ExecuteAsync(_commandQuery.Object, _token);

                // Assert
                Assert.Equal(_result.Object, result);
            }
        }

        public class ExecuteAsync_Query
        {
            private readonly Mock<IQueryHandler<IQuery<string>, string>> _queryHander =
                new Mock<IQueryHandler<IQuery<string>, string>>();
            private readonly CqsMediator _mediator;
            private readonly Mock<IQuery<string>> _query = new Mock<IQuery<string>>();
            private readonly CancellationToken _token = CancellationToken.None;
            private readonly Mock<IResult<string>> _result = new Mock<IResult<string>>();

            public ExecuteAsync_Query()
            {
                _mediator = new CqsMediator(t => _queryHander.Object);
            }

            [Fact]
            public async Task IfNullQuery_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Func<Task> act = async () => await _mediator.ExecuteAsync((IQuery<string>)null, CancellationToken.None);

                // Assert
                await Assert.ThrowsAsync<ArgumentNullException>("query", act);
            }

            [Fact]
            public async Task IfQueryHandlerNotFoundInCallback_ThenTypeLoadExceptionThrown()
            {
                // Arrange

                // Act
                Func<Task> act = async () => await new CqsMediator(t => null).ExecuteAsync(_query.Object, CancellationToken.None);

                // Assert
                await Assert.ThrowsAsync<TypeLoadException>(act);
            }

            [Fact]
            public async Task IfQueryHandlerFoundInCallback_ThenExecuteAsyncCalled()
            {
                // Arrange
                _queryHander.Setup(a => a.ExecuteAsync(_query.Object, _token))
                    .Returns(Task.FromResult(_result.Object));

                // Act
                IResult<string> result = await _mediator.ExecuteAsync(_query.Object, _token);

                // Assert
                Assert.Equal(_result.Object, result);
            }

            [Fact]
            public async Task IfQueryHandlerFoundInCallbackAndNoToken_ThenExecuteAsyncCalled()
            {
                // Arrange
                _queryHander.Setup(a => a.ExecuteAsync(_query.Object, It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(_result.Object));

                // Act
                IResult<string> result = await _mediator.ExecuteAsync(_query.Object, _token);

                // Assert
                Assert.Equal(_result.Object, result);
            }
        }
    }
}
