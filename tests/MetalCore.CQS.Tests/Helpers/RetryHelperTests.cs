using MetalCore.CQS.Helpers;
using System;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.Helpers
{
    public class RetryHelperTests
    {
        public class HasAnyExceptionMatch
        {
            [Fact]
            public void WhenNullException_ReturnsFalse()
            {
                // Arrange

                // Act
                bool result = RetryHelper.HasAnyExceptionMatch(false, false, null);

                // Assert
                Assert.False(result);
            }

            [Fact]
            public void WhenNoBaseTypeNorInnerChecksAndExceptionType_ReturnsTrue()
            {
                // Arrange

                // Act
                bool result = RetryHelper.HasAnyExceptionMatch(false, false, new Exception());

                // Assert
                Assert.True(result);
            }

            [Fact]
            public void WhenNoBaseTypeNorInnerChecksAndExceptionTypeAndLimitedToSystemException_ReturnsFalse()
            {
                // Arrange

                // Act
                bool result = RetryHelper.HasAnyExceptionMatch(false, false, new Exception(), typeof(SystemException));

                // Assert
                Assert.False(result);
            }

            [Fact]
            public void WhenNoBaseTypeNorInnerChecksAndSystemExceptionTypeAndLimitedToException_ReturnsFalse()
            {
                // Arrange

                // Act
                bool result = RetryHelper.HasAnyExceptionMatch(false, false, new Exception(), typeof(Exception));

                // Assert
                Assert.True(result);
            }
        }
    }
}
