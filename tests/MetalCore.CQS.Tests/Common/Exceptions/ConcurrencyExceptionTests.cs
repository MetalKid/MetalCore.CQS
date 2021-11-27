using MetalCore.CQS.Exceptions;
using System;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.Exceptions
{
    public class ConcurrencyExceptionTests
    {
        public class Constructor
        {
            [Fact]
            public void IfDefault_ThenNonNullMessageAndNullInnerException()
            {
                // Arrange

                // Act
                ConcurrencyException result = new ConcurrencyException();

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
                ConcurrencyException result = new ConcurrencyException(message);

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
                ConcurrencyException result = new ConcurrencyException(message, innerException);

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
                ConcurrencyException result = new ConcurrencyException(null, innerException);

                // Assert
                Assert.NotNull(result.Message);
                Assert.Equal(innerException, result.InnerException);
            }
        }
    }
}
