using AutoFixture.Xunit2;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Validation;
using System.Collections.Generic;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.Results
{
    public class ResultHelperTests
    {
        public class Successful
        {
            [Fact]
            public void WithNoData_ReturnsSuccessfulResult()
            {
                // Arrange

                // Act
                IResult result = ResultHelper.Successful();

                // Assert
                Assert.True(result.IsSuccessful);
            }

            [Fact]
            public void WithNoData_ReturnsIResult()
            {
                // Arrange

                // Act
                IResult result = ResultHelper.Successful();

                // Assert
                Assert.True(result is Result);
                Assert.False(result.GetType().IsGenericType);
            }

            [Fact]
            public void WithNullData_ReturnsNull()
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.Successful<string>(null);

                // Assert
                Assert.Null(result.Data);
            }

            [Theory]
            [AutoData]
            public void WithData_ReturnsSuccessfulResult(string resultData)
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.Successful(resultData);

                // Assert
                Assert.True(result.IsSuccessful);
            }

            [Theory]
            [AutoData]
            public void WithData_ReturnsGivenData(string resultData)
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.Successful(resultData);

                // Assert
                Assert.Equal(resultData, result.Data);
            }

            [Theory]
            [AutoData]
            public void WithData_ReturnsIResultGeneric(string resultData)
            {
                // Arrange

                // Acts
                IResult<string> result = ResultHelper.Successful(resultData);

                // Assert
                Assert.True(result is IResult<string>);
                Assert.True(result.GetType().IsGenericType);
            }
        }

        public class ValidationError
        {
            [Fact]
            public void WithBasic_ReturnsIResult()
            {
                // Arrange

                // Act
                IResult result = ResultHelper.ValidationError(null);

                // Assert
                Assert.True(result is Result);
                Assert.False(result.GetType().IsGenericType);
            }

            [Fact]
            public void WithGeneric_ReturnsIResultGeneric()
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.ValidationError<string>(null);

                // Assert
                Assert.True(result is Result<string>);
                Assert.True(result.GetType().IsGenericType);
            }

            [Fact]
            public void WithNullBrokenRules_ReturnsNonSuccessResult()
            {
                // Arrange

                // Act
                IResult result = ResultHelper.ValidationError(null);

                // Assert
                Assert.False(result.IsSuccessful);
            }

            [Fact]
            public void WithEmptyBrokenRules_ReturnsNonSuccessResult()
            {
                // Arrange
                List<BrokenRule> input = new List<BrokenRule>();

                // Act
                IResult result = ResultHelper.ValidationError(input);

                // Assert
                Assert.False(result.IsSuccessful);
            }

            [Fact]
            public void With1BrokenRule_ReturnsNonSuccessResult()
            {
                // Arrange
                List<BrokenRule> input = new List<BrokenRule>() { new BrokenRule() };

                // Act
                IResult result = ResultHelper.ValidationError(input);

                // Assert
                Assert.False(result.IsSuccessful);
            }

            [Fact]
            public void With2BrokenRules_ReturnsNonSuccessResult()
            {
                // Arrange
                List<BrokenRule> input = new List<BrokenRule>() { new BrokenRule(), new BrokenRule() };

                // Act
                IResult result = ResultHelper.ValidationError(input);

                // Assert
                Assert.False(result.IsSuccessful);
            }

            [Fact]
            public void WithNullBrokenRules_ReturnsNullBrokenRules()
            {
                // Arrange

                // Act
                IResult result = ResultHelper.ValidationError(null);

                // Assert
                Assert.Null(result.BrokenRules);
            }

            [Fact]
            public void WithEmptyBrokenRules_ReturnsGivenBrokenRuleList()
            {
                // Arrange
                List<BrokenRule> input = new List<BrokenRule>();

                // Act
                IResult result = ResultHelper.ValidationError(input);

                // Assert
                Assert.Equal(input, result.BrokenRules);
            }

            [Fact]
            public void With1BrokenRule_ReturnsGivenBrokenRuleList()
            {
                // Arrange
                List<BrokenRule> input = new List<BrokenRule>() { new BrokenRule() };

                // Act
                IResult result = ResultHelper.ValidationError(input);

                // Assert
                Assert.Equal(input, result.BrokenRules);
            }

            [Fact]
            public void With2BrokenRules_ReturnsGivenBrokenRuleList()
            {
                // Arrange
                List<BrokenRule> input = new List<BrokenRule>() { new BrokenRule(), new BrokenRule() };

                // Act
                IResult result = ResultHelper.ValidationError(input);

                // Assert
                Assert.Equal(input, result.BrokenRules);
            }

            [Fact]
            public void WithNullBrokenRulesAndGenericResult_ReturnsNonSuccessResult()
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.ValidationError<string>(null);

                // Assert
                Assert.False(result.IsSuccessful);
            }

            [Fact]
            public void WithEmptyBrokenRulesAndGenericResult_ReturnsNonSuccessResult()
            {
                // Arrange
                List<BrokenRule> input = new List<BrokenRule>();

                // Act
                IResult<string> result = ResultHelper.ValidationError<string>(input);

                // Assert
                Assert.False(result.IsSuccessful);
            }

            [Fact]
            public void With1BrokenRuleAndGenericResult_ReturnsNonSuccessResult()
            {
                // Arrange
                List<BrokenRule> input = new List<BrokenRule>() { new BrokenRule() };

                // Act
                IResult<string> result = ResultHelper.ValidationError<string>(input);

                // Assert
                Assert.False(result.IsSuccessful);
            }

            [Fact]
            public void With2BrokenRulesAndGenericResult_ReturnsNonSuccessResult()
            {
                // Arrange
                List<BrokenRule> input = new List<BrokenRule>() { new BrokenRule(), new BrokenRule() };

                // Act
                IResult<string> result = ResultHelper.ValidationError<string>(input);

                // Assert
                Assert.False(result.IsSuccessful);
            }

            [Fact]
            public void WithNullBrokenRulesAndGenericResult_ReturnsNullBrokenRules()
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.ValidationError<string>(null);

                // Assert
                Assert.Null(result.BrokenRules);
            }

            [Fact]
            public void WithEmptyBrokenRulesAndGenericResult_ReturnsGivenBrokenRuleList()
            {
                // Arrange
                List<BrokenRule> input = new List<BrokenRule>();

                // Act
                IResult<string> result = ResultHelper.ValidationError<string>(input);

                // Assert
                Assert.Equal(input, result.BrokenRules);
            }

            [Fact]
            public void With1BrokenRuleAndGenericResult_ReturnsGivenBrokenRuleList()
            {
                // Arrange
                List<BrokenRule> input = new List<BrokenRule>() { new BrokenRule() };

                // Act
                IResult<string> result = ResultHelper.ValidationError<string>(input);

                // Assert
                Assert.Equal(input, result.BrokenRules);
            }

            [Fact]
            public void With2BrokenRulesAndGenericResult_ReturnsGivenBrokenRuleList()
            {
                // Arrange
                List<BrokenRule> input = new List<BrokenRule>() { new BrokenRule(), new BrokenRule() };

                // Act
                IResult<string> result = ResultHelper.ValidationError<string>(input);

                // Assert
                Assert.Equal(input, result.BrokenRules);
            }
        }

        public class NoDataFoundError
        {
            [Fact]
            public void WithBasic_ReturnsIResult()
            {
                // Arrange

                // Act
                IResult result = ResultHelper.NoDataFoundError();

                // Assert
                Assert.True(result is Result);
                Assert.False(result.GetType().IsGenericType);
            }

            [Fact]
            public void WithGeneric_ReturnsIResultGeneric()
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.NoDataFoundError<string>();

                // Assert
                Assert.True(result is Result<string>);
                Assert.True(result.GetType().IsGenericType);
            }

            [Fact]
            public void WithBasic_ReturnsNonSuccessfulResult()
            {
                // Arrange

                // Act
                IResult result = ResultHelper.NoDataFoundError();

                // Assert
                Assert.False(result.IsSuccessful);
            }

            [Fact]
            public void WithBasic_ReturnsHasDataNotFoundErrorResult()
            {
                // Arrange

                // Act
                IResult result = ResultHelper.NoDataFoundError();

                // Assert
                Assert.True(result.HasDataNotFoundError);
            }

            [Fact]
            public void WithGeneric_ReturnsNonSuccessfulResult()
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.NoDataFoundError<string>();

                // Assert
                Assert.False(result.IsSuccessful);
            }

            [Fact]
            public void WithGeneric_ReturnsHasDataNotFoundErrorResult()
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.NoDataFoundError<string>();

                // Assert
                Assert.True(result.HasDataNotFoundError);
            }
        }

        public class ConcurrencyError
        {
            [Fact]
            public void WithBasic_ReturnsIResult()
            {
                // Arrange

                // Act
                IResult result = ResultHelper.ConcurrencyError();

                // Assert
                Assert.True(result is Result);
                Assert.False(result.GetType().IsGenericType);
            }

            [Fact]
            public void WithGeneric_ReturnsIResultGeneric()
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.ConcurrencyError<string>();

                // Assert
                Assert.True(result is Result<string>);
                Assert.True(result.GetType().IsGenericType);
            }

            [Fact]
            public void WithBasic_ReturnsNonSuccessfulResult()
            {
                // Arrange

                // Act
                IResult result = ResultHelper.ConcurrencyError();

                // Assert
                Assert.False(result.IsSuccessful);
            }

            [Fact]
            public void WithBasic_ReturnsHasConcurrencyErrorResult()
            {
                // Arrange

                // Act
                IResult result = ResultHelper.ConcurrencyError();

                // Assert
                Assert.True(result.HasConcurrencyError);
            }

            [Fact]
            public void WithGeneric_ReturnsNonSuccessfulResult()
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.ConcurrencyError<string>();

                // Assert
                Assert.False(result.IsSuccessful);
            }

            [Fact]
            public void WithGeneric_ReturnsHasConcurrencyErrorResult()
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.ConcurrencyError<string>();

                // Assert
                Assert.True(result.HasConcurrencyError);
            }
        }

        public class NoPermissionError
        {
            [Fact]
            public void WithBasic_ReturnsIResult()
            {
                // Arrange

                // Act
                IResult result = ResultHelper.NoPermissionError();

                // Assert
                Assert.True(result is Result);
                Assert.False(result.GetType().IsGenericType);
            }

            [Fact]
            public void WithGeneric_ReturnsIResultGeneric()
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.NoPermissionError<string>();

                // Assert
                Assert.True(result is Result<string>);
                Assert.True(result.GetType().IsGenericType);
            }

            [Fact]
            public void WithBasic_ReturnsNonSuccessfulResult()
            {
                // Arrange

                // Act
                IResult result = ResultHelper.NoPermissionError();

                // Assert
                Assert.False(result.IsSuccessful);
            }

            [Fact]
            public void WithBasic_ReturnsHasNoPermissionErrorResult()
            {
                // Arrange

                // Act
                IResult result = ResultHelper.NoPermissionError();

                // Assert
                Assert.True(result.HasNoPermissionError);
            }

            [Fact]
            public void WithGeneric_ReturnsNonSuccessfulResult()
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.NoPermissionError<string>();

                // Assert
                Assert.False(result.IsSuccessful);
            }

            [Fact]
            public void WithGeneric_ReturnsHasNoPermissionErrorResult()
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.NoPermissionError<string>();

                // Assert
                Assert.True(result.HasNoPermissionError);
            }
        }

        public class Error
        {
            [Fact]
            public void WithBasic_ReturnsIResult()
            {
                // Arrange

                // Act
                IResult result = ResultHelper.Error(null);

                // Assert
                Assert.True(result is Result);
                Assert.False(result.GetType().IsGenericType);
            }

            [Fact]
            public void WithGeneric_ReturnsIResultGeneric()
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.Error<string>(null);

                // Assert
                Assert.True(result is Result<string>);
                Assert.True(result.GetType().IsGenericType);
            }

            [Fact]
            public void WithNullMessage_ReturnsNonSuccessResult()
            {
                // Arrange

                // Act
                IResult result = ResultHelper.Error(null);

                // Assert
                Assert.False(result.IsSuccessful);
            }

            [Fact]
            public void WithEmptyMessage_ReturnsNonSuccessResult()
            {
                // Arrange

                // Act
                IResult result = ResultHelper.Error(string.Empty);

                // Assert
                Assert.False(result.IsSuccessful);
            }

            [Theory]
            [AutoData]
            public void WithMessage_ReturnsNonSuccessResult(string input)
            {
                // Arrange

                // Act
                IResult result = ResultHelper.Error(input);

                // Assert
                Assert.False(result.IsSuccessful);
            }

            [Fact]
            public void WithNullMessage_ReturnsNullErrorMessage()
            {
                // Arrange

                // Act
                IResult result = ResultHelper.Error(null);

                // Assert
                Assert.Null(result.ErrorMessage);
            }

            [Fact]
            public void WithEmptyMessage_ReturnsGivenErrorMessage()
            {
                // Arrange

                // Act
                IResult result = ResultHelper.Error(string.Empty);

                // Assert
                Assert.Equal(string.Empty, result.ErrorMessage);
            }

            [Theory]
            [AutoData]
            public void With1BrokenRule_ReturnsGivenBrokenRuleList(string input)
            {
                // Arrange

                // Act
                IResult result = ResultHelper.Error(input);

                // Assert
                Assert.Equal(input, result.ErrorMessage);
            }

            [Fact]
            public void WithNullMessageAndGenericResult_ReturnsNonSuccessResult()
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.Error<string>(null);

                // Assert
                Assert.False(result.IsSuccessful);
            }

            [Fact]
            public void WithEmptyMessageAndGenericResult_ReturnsNonSuccessResult()
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.Error<string>(string.Empty);

                // Assert
                Assert.False(result.IsSuccessful);
            }

            [Theory]
            [AutoData]
            public void WithMessageAndGeneircResult_ReturnsNonSuccessResult(string input)
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.Error<string>(input);

                // Assert
                Assert.False(result.IsSuccessful);
            }

            [Fact]
            public void WithNullMessageAndGenericResult_ReturnsNullMessage()
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.Error<string>(null);

                // Assert
                Assert.Null(result.ErrorMessage);
            }

            [Fact]
            public void WithEmptyMessageAndGenericResult_ReturnsGivenBrokenRuleList()
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.Error<string>(string.Empty);

                // Assert
                Assert.Equal(string.Empty, result.ErrorMessage);
            }

            [Theory]
            [AutoData]
            public void WithMessageAndGenericResult_ReturnsGivenBrokenRuleList(string input)
            {
                // Arrange

                // Act
                IResult<string> result = ResultHelper.Error<string>(input);

                // Assert
                Assert.Equal(input, result.ErrorMessage);
            }
        }
    }
}