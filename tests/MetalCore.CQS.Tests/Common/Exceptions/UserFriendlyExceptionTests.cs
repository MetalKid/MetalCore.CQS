using MetalCore.CQS.Exceptions;
using System;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.Exceptions
{
    public class UserFriendlyExceptionTests
    {
        public class Constructor
        {
            [Fact]
            public void IfDefault_ThenNonNullMessageAndNullInnerException()
            {
                // Arrange

                // Act
                UserFriendlyException result = new UserFriendlyException();

                // Assert
                Assert.NotNull(result.Message);
                Assert.Null(result.InnerException);
            }

            [Fact]
            public void IfGivenMessage_ThenMessageAndNullInnerException()
            {
                // Arrange
                string message = "Test";

                // Act
                UserFriendlyException result = new UserFriendlyException(message);

                // Assert
                Assert.Equal(message, result.Message);
                Assert.Null(result.InnerException);
            }

            [Fact]
            public void IfGivenMessageAndInnerException_ThenMessageAndInnerException()
            {
                // Arrange
                string message = "Test";
                Exception innerException = new Exception();

                // Act
                UserFriendlyException result = new UserFriendlyException(message, innerException);

                // Assert
                Assert.Equal(message, result.Message);
                Assert.Equal(innerException, result.InnerException);
            }

            [Fact]
            public void IfNullMessageAndNotNullInnerException_ThenDefaultMessageAndInnerException()
            {
                // Arrange
                Exception innerException = new Exception();

                // Act
                UserFriendlyException result = new UserFriendlyException(null, innerException);

                // Assert
                Assert.NotNull(result.Message);
                Assert.Equal(innerException, result.InnerException);
            }
        }
    }
}
