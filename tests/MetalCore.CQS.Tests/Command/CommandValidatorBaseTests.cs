using System;
using System.Threading;
using System.Threading.Tasks;
using MetalCore.CQS.Command;
using Moq;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.Command
{
    public class CommandValidatorBaseTests
    {
        public abstract class CommandValidatorBaseStub : CommandValidatorBase<ICommand>
        {
            public ICommand CommandPublic => Command;

            public abstract void CreateRulesPublic(CancellationToken token = default);

            protected override void CreateRules(ICommand input, CancellationToken token = default) =>
                CreateRulesPublic(token);
        }

        public class ValidateCommandAsyncMethods
        {
            private readonly Mock<ICommand> _commandMock = new Mock<ICommand>();
            private readonly CancellationToken _token = default;

            [Fact]
            public async Task IfNullCommand_ThenArgumentNullExceptionThrown()
            {
                // Arrange
                var target = new Mock<CommandValidatorBase<ICommand>>();

                // Act
                Func<Task> act = async () => await target.Object.ValidateCommandAsync(null, _token);

                // Assert
                await Assert.ThrowsAsync<ArgumentNullException>("command", act);
            }

            [Fact]
            public async Task IfNonNullCommand_ThenCommandPropertyReturnsSameInstance()
            {
                // Arrange
                var target = new Mock<CommandValidatorBaseStub>();

                // Act
                var result = await target.Object.ValidateCommandAsync(_commandMock.Object, _token);

                // Assert
                Assert.Equal(_commandMock.Object, target.Object.CommandPublic);
            }

            [Fact]
            public async Task IfNonNullParameters_ThenEmptyListReturned()
            {
                // Arrange
                var target = new Mock<CommandValidatorBase<ICommand>>();

                // Act
                var result = await target.Object.ValidateCommandAsync(_commandMock.Object, _token);

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public async Task IfNonNullParameters_ThenCreateRulesCalled()
            {
                // Arrange
                var target = new Mock<CommandValidatorBaseStub>() { CallBase = true };

                // Act
                await target.Object.ValidateCommandAsync(_commandMock.Object, _token);

                // Assert
                target.Verify(a => a.CreateRulesPublic(_token), Times.Once);
            }
        }
    }
}