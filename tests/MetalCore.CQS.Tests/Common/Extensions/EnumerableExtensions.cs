using AutoFixture.Xunit2;
using MetalCore.CQS.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.Extensions
{
    public class EnumerableExtensions
    {
        public class Batch
        {
            [Fact]
            public void IfNull_ReturnsEmptyList()
            {
                // Arrange
                const int BATCH_SIZE = 10;

                List<string> list = null;

                // Act
                List<IEnumerable<string>> result = list.Batch(BATCH_SIZE).ToList();

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public void IfEmptyList_ReturnsEmptyList()
            {
                // Arrange
                const int BATCH_SIZE = 10;

                List<string> list = new List<string>();

                // Act
                List<IEnumerable<string>> result = list.Batch(BATCH_SIZE).ToList();

                // Assert
                Assert.Empty(result);
            }

            [Theory]
            [AutoData]
            public void If1ItemInList_ReturnsListWith1Item(string input)
            {
                // Arrange
                const int BATCH_SIZE = 10;

                List<string> list = new List<string>() { input };

                // Act
                List<IEnumerable<string>> result = list.Batch(BATCH_SIZE).ToList();

                // Assert
                Assert.Single(result);
            }

            [Theory]
            [AutoData]
            public void If10ItemInList_ReturnsListWith1Item(string input)
            {
                // Arrange
                const int BATCH_SIZE = 10;

                List<string> list = new List<string>() { };
                for (int i = 1; i <= 10; i++)
                {
                    list.Add(input);
                }

                // Act
                List<IEnumerable<string>> result = list.Batch(BATCH_SIZE).ToList();

                // Assert
                Assert.Single(result);
            }

            [Theory]
            [AutoData]
            public void If11ItemInList_ReturnsListWith2Items(string input)
            {
                // Arrange
                const int BATCH_SIZE = 10;

                List<string> list = new List<string>() { };
                for (int i = 1; i <= 11; i++)
                {
                    list.Add(input);
                }

                // Act
                List<IEnumerable<string>> result = list.Batch(BATCH_SIZE).ToList();

                // Assert
                Assert.Equal(2, result.Count);
            }
        }
    }
}
