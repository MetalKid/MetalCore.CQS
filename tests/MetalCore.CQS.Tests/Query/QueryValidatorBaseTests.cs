using System;
using System.Threading;
using System.Threading.Tasks;
using MetalCore.CQS.Query;
using MetalCore.CQS.Query.Decorators;
using Moq;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.Query
{
    public class QueryValidatorBaseTests
    {
        public abstract class QueryValidatorBaseStub : QueryValidatorBase<IQuery<string>, string>
        {
            public IQuery<string> QueryPublic => Query;

            public abstract void CreateRulesPublic(CancellationToken token = default);

            protected override void CreateRules(IQuery<string> input, CancellationToken token = default) =>
                CreateRulesPublic(token);
        }

        public class ValidateQueryAsyncMethods
        {
            private readonly Mock<IQuery<string>> _queryMock = new Mock<IQuery<string>>();
            private readonly CancellationToken _token = default;

            [Fact]
            public async Task IfNullQuery_ThenArgumentNullExceptionThrown()
            {
                // Arrange
                var target = new Mock<QueryValidatorBase<IQuery<string>, string>>();

                // Act
                Func<Task> act = async () => await target.Object.ValidateQueryAsync(null, _token);

                // Assert
                await Assert.ThrowsAsync<ArgumentNullException>("query", act);
            }

            [Fact]
            public async Task IfNonNullQuery_ThenQueryPropertyReturnsSameInstance()
            {
                // Arrange
                var target = new Mock<QueryValidatorBaseStub>();

                // Act
                var result = await target.Object.ValidateQueryAsync(_queryMock.Object, _token);

                // Assert
                Assert.Equal(_queryMock.Object, target.Object.QueryPublic);
            }

            [Fact]
            public async Task IfNonNullParameters_ThenEmptyListReturned()
            {
                // Arrange
                var target = new Mock<QueryValidatorBase<IQuery<string>, string>>();

                // Act
                var result = await target.Object.ValidateQueryAsync(_queryMock.Object, _token);

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public async Task IfNonNullParameters_ThenCreateRulesCalled()
            {
                // Arrange
                var target = new Mock<QueryValidatorBaseStub>() { CallBase = true };

                // Act
                await target.Object.ValidateQueryAsync(_queryMock.Object, _token);

                // Assert
                target.Verify(a => a.CreateRulesPublic(_token), Times.Once);
            }
        }
    }
}