using AutoFixture.Xunit2;
using MetalCore.CQS.Mediators;
using MetalCore.CQS.Repository;
using MetalCore.CQS.Validation;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.Mediators
{
    public class ValidationRepositoryMediatorTests
    {
        public class ContstructorMethods
        {
            [Fact]
            public void IfNullCallback_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new ValidationRepositoryMediator(null);

                // Assert
                Assert.Throws<ArgumentNullException>("getInstanceCallback", act);
            }

            [Fact]
            public void IfNotNullCallback_ThenNoExceptionThrownAndNotNullInstance()
            {
                // Arrange

                // Act
                ValidationRepositoryMediator result = new ValidationRepositoryMediator(t => null);

                // Assert
                Assert.NotNull(result);
            }
        }

        public class GetValidationRules
        {
            private readonly Mock<IValidationRepository<string>> _commandHandler = new Mock<IValidationRepository<string>>();
            private readonly ValidationRepositoryMediator _mediator;
            private readonly CancellationToken _token = CancellationToken.None;
            private readonly ICollection<Func<Task<BrokenRule>>> _result = new List<Func<Task<BrokenRule>>>();

            public GetValidationRules()
            {
                _mediator = new ValidationRepositoryMediator(t => _commandHandler.Object);
            }

            [Fact]
            public void IfNullCommand_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => _mediator.GetValidationRules<string>(null, CancellationToken.None);

                // Assert
                Assert.Throws<ArgumentNullException>("request", act);
            }

            [Theory]
            [AutoData]
            public void IfCommandHandlerNotFoundInCallback_ThenTypeLoadExceptionThrown(string input)
            {
                // Arrange

                // Act
                Action act = () => new ValidationRepositoryMediator(t => null).GetValidationRules(input, CancellationToken.None);

                // Assert
                Assert.Throws<TypeLoadException>(act);
            }

            [Theory]
            [AutoData]
            public void IfCommandHandlerFoundInCallback_ThenExecuteAsyncCalled(string input)
            {
                // Arrange
                _commandHandler.Setup(a => a.GetValidationRules(input, _token)).Returns(_result);

                // Act
                ICollection<Func<Task<BrokenRule>>> result = _mediator.GetValidationRules(input, _token);

                // Assert
                Assert.Equal(_result, result);
            }

            [Theory]
            [AutoData]
            public void IfCommandHandlerFoundInCallbackAndNoToken_ThenExecuteAsyncCalled(string input)
            {
                // Arrange
                _commandHandler.Setup(a => a.GetValidationRules(input, It.IsAny<CancellationToken>())).Returns(_result);

                // Act
                ICollection<Func<Task<BrokenRule>>> result = _mediator.GetValidationRules(input);

                // Assert
                Assert.Equal(_result, result);
            }
        }
    }
}
