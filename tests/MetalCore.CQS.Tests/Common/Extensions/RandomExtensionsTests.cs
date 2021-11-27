using MetalCore.CQS.ExtensionMethods;
using System;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.Extensions
{
    public class RandomExtensionsTests
    {
        public class NextString
        {
            [Fact]
            public void If0Size_ThenArgumentOutOfRangeException()
            {
                // Arrange
                int size = 0;
                bool requiresAtLeastOneNumber = true;
                bool requiresAtLeastOneUpperCaseLetter = true;
                Random random = new Random();

                // Act
                Action act = () => random.NextString(size, requiresAtLeastOneNumber, requiresAtLeastOneUpperCaseLetter);

                // Assert
                Assert.Throws<ArgumentOutOfRangeException>(act);
            }

            [Fact]
            public void If1SizeAndAtLeastOneUpperAndNumber_ThenArgumentOutOfRangeException()
            {
                // Arrange
                int size = 1;
                bool requiresAtLeastOneNumber = true;
                bool requiresAtLeastOneUpperCaseLetter = true;
                Random random = new Random();

                // Act
                Action act = () => random.NextString(size, requiresAtLeastOneNumber, requiresAtLeastOneUpperCaseLetter);

                // Assert
                Assert.Throws<ArgumentOutOfRangeException>(act);
            }

            [Fact]
            public void If1SizeAndAtLeastOneUpperTrueAndNumberFalse_ThenResultReturned()
            {
                // Arrange
                int size = 1;
                bool requiresAtLeastOneNumber = false;
                bool requiresAtLeastOneUpperCaseLetter = true;
                Random random = new Random();

                // Act
                string result = random.NextString(size, requiresAtLeastOneNumber, requiresAtLeastOneUpperCaseLetter);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.Length);
                Assert.Equal(result.ToUpper(), result);
            }

            [Fact]
            public void If1SizeAndAtLeastOneUpperFalseAndNumberTrue_ThenResultReturned()
            {
                // Arrange
                int size = 1;
                bool requiresAtLeastOneNumber = true;
                bool requiresAtLeastOneUpperCaseLetter = false;
                Random random = new Random();

                // Act
                string result = random.NextString(size, requiresAtLeastOneNumber, requiresAtLeastOneUpperCaseLetter);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.Length);
                Assert.True(int.TryParse(result, out _));
            }

            [Fact]
            public void If1SizeAndAtLeastOneUpperFalseAndNumberFalse_ThenResultReturned()
            {
                // Arrange
                int size = 1;
                bool requiresAtLeastOneNumber = false;
                bool requiresAtLeastOneUpperCaseLetter = false;
                Random random = new Random();

                // Act
                string result = random.NextString(size, requiresAtLeastOneNumber, requiresAtLeastOneUpperCaseLetter);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.Length);
            }

            [Fact]
            public void If2SizeAndAtLeastOneUpperTrueAndNumberTrue_ThenResultReturned()
            {
                // Arrange
                int size = 2;
                bool requiresAtLeastOneNumber = true;
                bool requiresAtLeastOneUpperCaseLetter = true;
                Random random = new Random();

                // Act
                string result = null;
                for (int i = 0; i < 1000; i++)
                {
                    result = random.NextString(size, requiresAtLeastOneNumber, requiresAtLeastOneUpperCaseLetter);
                }

                // Assert
                Assert.NotNull(result);
            }

            [Fact]
            public void If10SizeAndAtLeastOneUpperTrueAndNumberTrue_ThenResultReturned()
            {
                // Arrange
                int size = 10;
                bool requiresAtLeastOneNumber = true;
                bool requiresAtLeastOneUpperCaseLetter = true;
                Random random = new Random();

                // Act
                string result = random.NextString(size, requiresAtLeastOneNumber, requiresAtLeastOneUpperCaseLetter);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(10, result.Length);
            }
        }
    }
}