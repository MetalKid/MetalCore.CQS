using AutoFixture.Xunit2;
using MetalCore.CQS.Command;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Validation;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.Command
{
    public class CommandHandlerValidatorDecoratorTests
    {
        public class ConstructorMethods
        {
            private readonly Mock<ICommandHandler<ICommand>> _commandHandlerMock = new Mock<ICommandHandler<ICommand>>();
            private readonly List<ICommandValidator<ICommand>> _validators = new List<ICommandValidator<ICommand>>();

            [Fact]
            public void IfNullCommandHandler_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new CommandHandlerValidatorDecorator<ICommand>(null, _validators);

                // Assert
                Assert.Throws<ArgumentNullException>("commandHandler", act);
            }

            [Fact]
            public void IfNullValidators_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new CommandHandlerValidatorDecorator<ICommand>(_commandHandlerMock.Object, null);

                // Assert
                Assert.Throws<ArgumentNullException>("validators", act);
            }
        }

        public class ExecuteAsyncMethods
        {
            private readonly Mock<ICommand> _commandMock = new Mock<ICommand>();
            private readonly Mock<ICommandHandler<ICommand>> _commandHandlerMock = new Mock<ICommandHandler<ICommand>>();
            private readonly List<ICommandValidator<ICommand>> _validators = new List<ICommandValidator<ICommand>>();
            private readonly Mock<IResult> _resultMock = new Mock<IResult>();
            private readonly CancellationToken _token = default;

            [Fact]
            public async Task IfNullCommand_ThenCommandPushedToNextCommandHandler()
            {
                // Arrange
                CommandHandlerValidatorDecorator<ICommand> target = new CommandHandlerValidatorDecorator<ICommand>(_commandHandlerMock.Object, _validators);

                // Act
                await target.ExecuteAsync(null, _token);

                // Assert
                _commandHandlerMock.Verify(a => a.ExecuteAsync(null, _token), Times.Once);
            }

            [Fact]
            public async Task IfNullCommand_ThenOriginalResultReturned()
            {
                // Arrange
                CommandHandlerValidatorDecorator<ICommand> target = new CommandHandlerValidatorDecorator<ICommand>(_commandHandlerMock.Object, _validators);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(null, _token))
                    .Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult result = await target.ExecuteAsync(null, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfNonNullCommand_ThenCommandPushedToNextCommandHandler()
            {
                // Arrange
                CommandHandlerValidatorDecorator<ICommand> target = new CommandHandlerValidatorDecorator<ICommand>(_commandHandlerMock.Object, _validators);

                // Act
                await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                _commandHandlerMock.Verify(a => a.ExecuteAsync(_commandMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfNonNullCommand_ThenOriginalResultReturned()
            {
                // Arrange
                CommandHandlerValidatorDecorator<ICommand> target = new CommandHandlerValidatorDecorator<ICommand>(_commandHandlerMock.Object, _validators);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult result = await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }

            [Fact]
            public async Task IfGiven1CommandValidator_ThenValidateCommandAsyncCalled()
            {
                // Arrange
                CommandHandlerValidatorDecorator<ICommand> target = new CommandHandlerValidatorDecorator<ICommand>(_commandHandlerMock.Object, _validators);

                Mock<ICommandValidator<ICommand>> commandValidator = new Mock<ICommandValidator<ICommand>>();
                _validators.Add(commandValidator.Object);

                // Act
                await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                commandValidator.Verify(a => a.ValidateCommandAsync(_commandMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandValidators_ThenValidateCommandAsyncCalledForEach()
            {
                // Arrange
                CommandHandlerValidatorDecorator<ICommand> target = new CommandHandlerValidatorDecorator<ICommand>(_commandHandlerMock.Object, _validators);

                Mock<ICommandValidator<ICommand>> commandValidator = new Mock<ICommandValidator<ICommand>>();
                _validators.Add(commandValidator.Object);

                Mock<ICommandValidator<ICommand>> commandValidator2 = new Mock<ICommandValidator<ICommand>>();
                _validators.Add(commandValidator2.Object);

                // Act
                await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                commandValidator.Verify(a => a.ValidateCommandAsync(_commandMock.Object, _token), Times.Once);
                commandValidator2.Verify(a => a.ValidateCommandAsync(_commandMock.Object, _token), Times.Once);
            }

            [Fact]
            public async Task IfGiven2CommandValidators_ThenEachRunOnSeparateThreads()
            {
                // Arrange
                const int WAIT_TIME = 500;

                CommandHandlerValidatorDecorator<ICommand> target = new CommandHandlerValidatorDecorator<ICommand>(_commandHandlerMock.Object, _validators);

                IEnumerable<BrokenRule> brokenRules = new List<BrokenRule>();
                Mock<ICommandValidator<ICommand>> commandValidator = new Mock<ICommandValidator<ICommand>>();
                commandValidator.Setup(a => a.ValidateCommandAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(brokenRules)).Callback(() => Thread.Sleep(WAIT_TIME));
                _validators.Add(commandValidator.Object);

                Mock<ICommandValidator<ICommand>> commandValidator2 = new Mock<ICommandValidator<ICommand>>();
                commandValidator2.Setup(a => a.ValidateCommandAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(brokenRules)).Callback(() => Thread.Sleep(WAIT_TIME));
                _validators.Add(commandValidator2.Object);

                // Act
                System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
                await target.ExecuteAsync(_commandMock.Object, _token);
                timer.Stop();

                // Assert
                Assert.True(timer.ElapsedMilliseconds >= WAIT_TIME);
            }

            [Theory, AutoData]
            public async Task IfGiven1CommandValidatorWithBrokenRule_ThenBrokenRuleResultReturned(string message, string relation)
            {
                // Arrange
                IEnumerable<BrokenRule> brokenRules = new List<BrokenRule>
                {
                    new BrokenRule(message, relation)
                };

                CommandHandlerValidatorDecorator<ICommand> target = new CommandHandlerValidatorDecorator<ICommand>(_commandHandlerMock.Object, _validators);

                Mock<ICommandValidator<ICommand>> commandValidator = new Mock<ICommandValidator<ICommand>>();
                commandValidator.Setup(a => a.ValidateCommandAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(brokenRules));
                _validators.Add(commandValidator.Object);

                // Act
                IResult result = await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                Assert.True(result.BrokenRules.Any());
                Assert.Equal(brokenRules.First(), result.BrokenRules.First());
                Assert.False(result.IsSuccessful);
            }

            [Theory, AutoData]
            public async Task IfGiven2CommandValidatorWithSameBrokenRule_Then1BrokenRuleResultReturned(string message, string relation)
            {
                // Arrange
                IEnumerable<BrokenRule> brokenRules = new List<BrokenRule>
                {
                    new BrokenRule(message, relation)
                };

                CommandHandlerValidatorDecorator<ICommand> target = new CommandHandlerValidatorDecorator<ICommand>(_commandHandlerMock.Object, _validators);

                Mock<ICommandValidator<ICommand>> commandValidator = new Mock<ICommandValidator<ICommand>>();
                commandValidator.Setup(a => a.ValidateCommandAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(brokenRules));
                _validators.Add(commandValidator.Object);

                Mock<ICommandValidator<ICommand>> commandValidator2 = new Mock<ICommandValidator<ICommand>>();
                commandValidator2.Setup(a => a.ValidateCommandAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(brokenRules));
                _validators.Add(commandValidator2.Object);

                // Act
                IResult result = await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                Assert.Single(brokenRules);
                Assert.Equal(brokenRules.First(), result.BrokenRules.First());
                Assert.False(result.IsSuccessful);
            }

            [Fact]
            public async Task IfGiven1CommandValidatorWithNullBrokenRule_ThenOriginalResultReturned()
            {
                // Arrange
                IEnumerable<BrokenRule> brokenRules = new List<BrokenRule>
                {
                    null
                };

                CommandHandlerValidatorDecorator<ICommand> target = new CommandHandlerValidatorDecorator<ICommand>(_commandHandlerMock.Object, _validators);

                Mock<ICommandValidator<ICommand>> commandValidator = new Mock<ICommandValidator<ICommand>>();
                commandValidator.Setup(a => a.ValidateCommandAsync(_commandMock.Object, _token))
                    .Returns(Task.FromResult(brokenRules));
                _validators.Add(commandValidator.Object);

                _commandHandlerMock.Setup(a => a.ExecuteAsync(_commandMock.Object, _token)).Returns(Task.FromResult(_resultMock.Object));

                // Act
                IResult result = await target.ExecuteAsync(_commandMock.Object, _token);

                // Assert
                Assert.Equal(_resultMock.Object, result);
            }
        }
    }
}