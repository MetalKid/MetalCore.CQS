using AutoFixture.Xunit2;
using MetalCore.CQS.CommandQuery;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Validation;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.CommandQuery
{
    public class CommandQueryHandlerValidatorDecoratorTests
    {
        public class ConstructorMethods
        {
            private readonly Mock<ICommandQueryHandler<ICommandQuery<string>, string>> _commandQueryHandlerMock = new Mock<ICommandQueryHandler<ICommandQuery<string>, string>>();
            private readonly List<ICommandQueryValidator<ICommandQuery<string>, string>> _validators = new List<ICommandQueryValidator<ICommandQuery<string>, string>>();

            [Fact]
            public void IfNullCommandQueryHandler_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string>(null, _validators);

                // Assert
                Assert.Throws<ArgumentNullException>("commandQueryHandler", act);
            }

            [Fact]
            public void IfNullValidators_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, null);

                // Assert
                Assert.Throws<ArgumentNullException>("validators", act);
            }
        }

        public class ExecuteAsyncMethods
        {
            private readonly Mock<ICommandQuery<string>> _commandQueryMock = new Mock<ICommandQuery<string>>();
            private readonly Mock<ICommandQueryHandler<ICommandQuery<string>, string>> _commandQueryHandlerMock = new Mock<ICommandQueryHandler<ICommandQuery<string>, string>>();
            private readonly List<ICommandQueryValidator<ICommandQuery<string>, string>> _validators = new List<ICommandQueryValidator<ICommandQuery<string>, string>>();
            private readonly Mock<IResult<string>> _resultMock = new Mock<IResult<string>>();
            private readonly CancellationToken _token = default;

            [Fact]
            public async Task IfNullCommandQuery_ThenCommandQueryPushedToNextCommandQueryHandler()
            {
                // Arrange
                CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _validators);

                // Act
                await target.ExecuteAsync(null, _token);

                // Assert
                _commandQueryHandlerMock.Verify(a => a.ExecuteAsync(null, _token), Times.Once);
            }

            [Fact]
            public async Task IfNullCommandQuery_ThenOriginalResultReturned()
            {
                // Arrange
                CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _validators);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(null, _token))
                    .Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.ExecuteAsync(null, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfNonNullCommandQuery_ThenCommandQueryPushedToNextCommandQueryHandler()
            {
                // Arrange
                CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _validators);

                // Act
                await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                _commandQueryHandlerMock.Verify(a => a.ExecuteAsync(_commandQueryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfNonNullCommandQuery_ThenOriginalResultReturned()
            {
                // Arrange
                CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _validators);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGiven1CommandQueryValidator_ThenValidateCommandQueryAsyncCalled()
            {
                // Arrange
                CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _validators);

                Mock<ICommandQueryValidator<ICommandQuery<string>, string>> commandQueryValidator = new Mock<ICommandQueryValidator<ICommandQuery<string>, string>>();
                _validators.Add(commandQueryValidator.Object);

                // Act
                await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                commandQueryValidator.Verify(a => a.ValidateCommandQueryAsync(_commandQueryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandQueryValidators_ThenValidateCommandQueryAsyncCalledForEach()
            {
                // Arrange
                CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _validators);

                Mock<ICommandQueryValidator<ICommandQuery<string>, string>> commandQueryValidator = new Mock<ICommandQueryValidator<ICommandQuery<string>, string>>();
                _validators.Add(commandQueryValidator.Object);

                Mock<ICommandQueryValidator<ICommandQuery<string>, string>> commandQueryValidator2 = new Mock<ICommandQueryValidator<ICommandQuery<string>, string>>();
                _validators.Add(commandQueryValidator2.Object);

                // Act
                await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                commandQueryValidator.Verify(a => a.ValidateCommandQueryAsync(_commandQueryMock.Object, _token), Times.Once);
                commandQueryValidator2.Verify(a => a.ValidateCommandQueryAsync(_commandQueryMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandQueryValidators_ThenEachRunOnSeparateThreads()
            {
                // Arrange
                const int WAIT_TIME = 500;

                CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _validators);

                IEnumerable<BrokenRule> brokenRules = new List<BrokenRule>();
                Mock<ICommandQueryValidator<ICommandQuery<string>, string>> commandQueryValidator = new Mock<ICommandQueryValidator<ICommandQuery<string>, string>>();
                commandQueryValidator.Setup(a => a.ValidateCommandQueryAsync(_commandQueryMock.Object, _token))
                    .Returns(Task.FromResult(brokenRules)).Callback(() => Thread.Sleep(WAIT_TIME));
                _validators.Add(commandQueryValidator.Object);

                Mock<ICommandQueryValidator<ICommandQuery<string>, string>> commandQueryValidator2 = new Mock<ICommandQueryValidator<ICommandQuery<string>, string>>();
                commandQueryValidator2.Setup(a => a.ValidateCommandQueryAsync(_commandQueryMock.Object, _token))
                    .Returns(Task.FromResult(brokenRules)).Callback(() => Thread.Sleep(WAIT_TIME));
                _validators.Add(commandQueryValidator2.Object);

                // Act
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await target.ExecuteAsync(_commandQueryMock.Object, _token);
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }

            [Theory, AutoData]
            public async Task IfGiven1CommandQueryValidatorWithBrokenRule_ThenBrokenRuleResultReturned(string message, string relation)
            {
                // Arrange
                IEnumerable<BrokenRule> brokenRules = new List<BrokenRule>
                {
                    new BrokenRule(message, relation)
                };

                CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _validators);

                Mock<ICommandQueryValidator<ICommandQuery<string>, string>> commandQueryValidator = new Mock<ICommandQueryValidator<ICommandQuery<string>, string>>();
                commandQueryValidator.Setup(a => a.ValidateCommandQueryAsync(_commandQueryMock.Object, _token))
                    .Returns(Task.FromResult(brokenRules));
                _validators.Add(commandQueryValidator.Object);

                // Act
                IResult<string> result = await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.True(result.BrokenRules.Any());
                Assert.Equal(brokenRules.First(), result.BrokenRules.First());
                Assert.False(result.IsSuccessful);
            }

            [Theory, AutoData]
            public async Task IfGiven2CommandQueryValidatorWithSameBrokenRule_Then1BrokenRuleResultReturned(string message, string relation)
            {
                // Arrange
                IEnumerable<BrokenRule> brokenRules = new List<BrokenRule>
                {
                    new BrokenRule(message, relation)
                };

                CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _validators);

                Mock<ICommandQueryValidator<ICommandQuery<string>, string>> commandQueryValidator = new Mock<ICommandQueryValidator<ICommandQuery<string>, string>>();
                commandQueryValidator.Setup(a => a.ValidateCommandQueryAsync(_commandQueryMock.Object, _token))
                    .Returns(Task.FromResult(brokenRules));
                _validators.Add(commandQueryValidator.Object);

                Mock<ICommandQueryValidator<ICommandQuery<string>, string>> commandQueryValidator2 = new Mock<ICommandQueryValidator<ICommandQuery<string>, string>>();
                commandQueryValidator2.Setup(a => a.ValidateCommandQueryAsync(_commandQueryMock.Object, _token))
                    .Returns(Task.FromResult(brokenRules));
                _validators.Add(commandQueryValidator2.Object);

                // Act
                IResult<string> result = await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.Single(brokenRules);
                Assert.Equal(brokenRules.First(), result.BrokenRules.First());
                Assert.False(result.IsSuccessful);
            }

            [Fact]
            public async Task IfGiven1CommandQueryValidatorWithNullBrokenRule_ThenOriginalResultReturned()
            {
                // Arrange
                IEnumerable<BrokenRule> brokenRules = new List<BrokenRule>
                {
                    null
                };

                CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string> target = new CommandQueryHandlerValidatorDecorator<ICommandQuery<string>, string>(_commandQueryHandlerMock.Object, _validators);

                Mock<ICommandQueryValidator<ICommandQuery<string>, string>> commandQueryValidator = new Mock<ICommandQueryValidator<ICommandQuery<string>, string>>();
                commandQueryValidator.Setup(a => a.ValidateCommandQueryAsync(_commandQueryMock.Object, _token))
                    .Returns(Task.FromResult(brokenRules));
                _validators.Add(commandQueryValidator.Object);

                _commandQueryHandlerMock.Setup(a => a.ExecuteAsync(_commandQueryMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult<string> result = await target.ExecuteAsync(_commandQueryMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }
        }
    }
}