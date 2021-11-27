using AutoFixture.Xunit2;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Mediators;
using MetalCore.CQS.Repository;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.Mediators
{
    public class RepositoryMediatorTests
    {
        public class ContstructorMethods
        {
            [Fact]
            public void IfNullCallback_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new RepositoryMediator(null);

                // Assert
                Assert.Throws<ArgumentNullException>("getInstanceCallback", act);
            }

            [Fact]
            public void IfNotNullCallback_ThenNoExceptionThrownAndNotNullInstance()
            {
                // Arrange

                // Act
                RepositoryMediator result = new RepositoryMediator(t => null);

                // Assert
                Assert.NotNull(result);
            }
        }

        public class ExecuteAsync_Command
        {
            private readonly Mock<IRepository<string>> _commandHandler = new Mock<IRepository<string>>();
            private readonly RepositoryMediator _mediator;
            private readonly CancellationToken _token = CancellationToken.None;

            public ExecuteAsync_Command()
            {
                _mediator = new RepositoryMediator(t => _commandHandler.Object);
            }

            [Fact]
            public void IfNullCommand_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => _mediator.ExecuteAsync(null, CancellationToken.None);

                // Assert
                Assert.Throws<ArgumentNullException>("request", act);
            }

            [Theory]
            [AutoData]
            public void IfCommandHandlerNotFoundInCallback_ThenTypeLoadExceptionThrown(string input)
            {
                // Arrange

                // Act
                Action act = () => new RepositoryMediator(t => null).ExecuteAsync(input, CancellationToken.None);

                // Assert
                Assert.Throws<TypeLoadException>(act);
            }

            [Theory]
            [AutoData]
            public async Task IfCommandHandlerFoundInCallback_ThenExecuteAsyncCalled(string input)
            {
                // Arrange
                _commandHandler.Setup(a => a.ExecuteAsync(input, _token)).Returns(Task.CompletedTask);

                // Act
                await _mediator.ExecuteAsync(input, _token);

                // Assert
                _commandHandler.Verify(a => a.ExecuteAsync(input, _token), Times.Once);
            }

            [Theory]
            [AutoData]
            public async Task IfCommandHandlerFoundInCallbackAndNoToken_ThenExecuteAsyncCalled(string input)
            {
                // Arrange
                _commandHandler.Setup(a => a.ExecuteAsync(input, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

                // Act
                await _mediator.ExecuteAsync(input, _token);

                // Assert
                _commandHandler.Verify(a => a.ExecuteAsync(input, _token), Times.Once);
            }
        }

        public class ExecuteAsync_Query
        {
            private readonly Mock<IRepository<string, string>> _queryHandler = new Mock<IRepository<string, string>>();
            private readonly RepositoryMediator _mediator;
            private readonly CancellationToken _token = CancellationToken.None;

            public ExecuteAsync_Query()
            {
                _mediator = new RepositoryMediator(t => _queryHandler.Object);
            }

            [Fact]
            public void IfNullCommand_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => _mediator.ExecuteAsync(null, CancellationToken.None);

                // Assert
                Assert.Throws<ArgumentNullException>("request", act);
            }

            [Theory]
            [AutoData]
            public void IfCommandHandlerNotFoundInCallback_ThenTypeLoadExceptionThrown(string input)
            {
                // Arrange

                // Act
                Action act = () => new RepositoryMediator(t => null).ExecuteAsync(input, CancellationToken.None);

                // Assert
                Assert.Throws<TypeLoadException>(act);
            }

            [Theory]
            [AutoData]
            public async Task IfCommandHandlerFoundInCallback_ThenExecuteAsyncCalled(string input, string output)
            {
                // Arrange
                _queryHandler.Setup(a => a.ExecuteAsync(input, _token)).Returns(Task.FromResult(output));

                // Act
                string result = await _mediator.ExecuteAsync<string>(input, _token);

                // Assert
                Assert.Equal(output, result);
            }

            [Theory]
            [AutoData]
            public async Task IfCommandHandlerFoundInCallbackAndToken_ThenExecuteAsyncCalled(string input, string output)
            {
                // Arrange
                _queryHandler.Setup(a => a.ExecuteAsync(input, It.IsAny<CancellationToken>())).Returns(Task.FromResult(output));

                // Act
                string result = await _mediator.ExecuteAsync<string>(input, _token);

                // Assert
                Assert.Equal(output, result);
            }
        }
    }
}
