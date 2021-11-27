//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using MetalCore.CQS.CommandQuery;
//using MetalCore.CQS.Validation;
//using Moq;
//using Xunit;

//namespace MetalCore.Tests.xUnitMoq.CQS.CommandQuery
//{
//    public class CommandQueryValidatorBaseTests
//    {
//        public abstract class CommandQueryValidatorBaseStub : CommandQueryValidatorBase<ICommandQuery<string>,string>
//        {
//            public ICommandQuery<string> CommandQueryPublic => CommandQuery;
//            public IValidator ValdidatePublic => Validator;

//            public abstract void CreateRulesPublic(CancellationToken token = default);

//            protected override void CreateRules(CancellationToken token = default) =>
//                CreateRulesPublic(token);
//        }

//        public class ValidateCommandQueryAsyncMethods
//        {
//            private readonly Mock<ICommandQuery<string>> _commandQueryMock = new Mock<ICommandQuery<string>>();
//            private readonly Mock<IValidator> _validateMock = new Mock<IValidator>();
//            private readonly CancellationToken _token = default;

//            [Fact]
//            public async Task IfNullCommandQuery_ThenArgumentNullExceptionThrown()
//            {
//                // Arrange
//                var target = new Mock<CommandQueryValidatorBase<ICommandQuery<string>,string>>();

//                // Act
//                Func<Task> act = async () => await target.Object.ValidateCommandQueryAsync(null, _validateMock.Object, _token);

//                // Assert
//                await Assert.ThrowsAsync<ArgumentNullException>("commandQuery", act);
//            }

//            [Fact]
//            public async Task IfNullValidate_ThenArgumentNullExceptionThrown()
//            {
//                // Arrange
//                var target = new Mock<CommandQueryValidatorBase<ICommandQuery<string>,string>>();

//                // Act
//                Func<Task> act = async () => await target.Object.ValidateCommandQueryAsync(_commandQueryMock.Object, null, _token);

//                // Assert
//                await Assert.ThrowsAsync<ArgumentNullException>("validate", act);
//            }

//            [Fact]
//            public async Task IfNonNullCommandQuery_ThenCommandQueryPropertyReturnsSameInstance()
//            {
//                // Arrange
//                var target = new Mock<CommandQueryValidatorBaseStub>();

//                // Act
//                var result = await target.Object.ValidateCommandQueryAsync(_commandQueryMock.Object, _validateMock.Object, _token);

//                // Assert
//                Assert.Equal(_commandQueryMock.Object, target.Object.CommandQueryPublic);
//            }

//            [Fact]
//            public async Task IfNonNullValidate_ThenCValidatePropertyReturnsSameInstance()
//            {
//                // Arrange
//                var target = new Mock<CommandQueryValidatorBaseStub>();

//                // Act
//                var result = await target.Object.ValidateCommandQueryAsync(_commandQueryMock.Object, _validateMock.Object, _token);

//                // Assert
//                Assert.Equal(_validateMock.Object, target.Object.ValdidatePublic);
//            }

//            [Fact]
//            public async Task IfNonNullParameters_ThenEmptyListReturned()
//            {
//                // Arrange
//                var target = new Mock<CommandQueryValidatorBase<ICommandQuery<string>,string>>();

//                // Act
//                var result = await target.Object.ValidateCommandQueryAsync(_commandQueryMock.Object, _validateMock.Object, _token);

//                // Assert
//                Assert.Empty(result);
//            }

//            [Fact]
//            public async Task IfNonNullParameters_ThenCreateRulesCalled()
//            {
//                // Arrange
//                var target = new Mock<CommandQueryValidatorBaseStub>() { CallBase = true };

//                // Act
//                await target.Object.ValidateCommandQueryAsync(_commandQueryMock.Object, _validateMock.Object, _token);

//                // Assert
//                target.Verify(a => a.CreateRulesPublic(_token), Times.Once);
//            }
//        }
//    }
//}