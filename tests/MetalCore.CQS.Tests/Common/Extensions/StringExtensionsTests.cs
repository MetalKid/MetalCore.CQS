using MetalCore.CQS.ExtensionMethods;
using System;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.Extensions
{
    public class StringExtensionsTests
    {
        public class RemoveDiacritics
        {
            [Fact]
            public void IfNull_ThenNullReturned()
            {
                // Arrange
                string input = null;

                // Act
                input = input.RemoveDiacritics();

                // Assert
                Assert.Null(input);
            }

            [Fact]
            public void IfNoDiacritics_ThenSameStringReturned()
            {
                // Arrange
                string input = "abc";

                // Act
                input = input.RemoveDiacritics();

                // Assert
                Assert.Equal("abc", input);
            }

            [Fact]
            public void IfDiacritics_ThenDifferentStringReturned()
            {
                // Arrange
                string input = "abç";

                // Act
                input = input.RemoveDiacritics();

                // Assert
                Assert.Equal("abc", input);
                Assert.NotEqual("abç", input);
            }
        }
    }
}
