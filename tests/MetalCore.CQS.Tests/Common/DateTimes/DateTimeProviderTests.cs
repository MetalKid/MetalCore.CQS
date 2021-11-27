using MetalCore.CQS.DateTimes;
using System;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.DateTimes
{
    public class DateTimeProviderTests
    {
        public class NowProperty
        {
            [Fact]
            public void IfInvoked_ThenReturnsNow()
            {
                // Arrange
                DateTimeProvider target = new DateTimeProvider();
                DateTime before = DateTime.Now;

                // Act
                DateTime result = target.Now;
                DateTime after = DateTime.Now;

                // Assert
                Assert.True(result >= before);
                Assert.True(result <= after);
            }
        }

        public class UtcNowProperty
        {
            [Fact]
            public void IfInvoked_ThenReturnsUtcNow()
            {
                // Arrange
                DateTimeProvider target = new DateTimeProvider();
                DateTime before = DateTime.UtcNow;

                // Act
                DateTime result = target.UtcNow;
                DateTime after = DateTime.UtcNow;

                // Assert
                Assert.True(result >= before);
                Assert.True(result <= after);
            }
        }

        public class TodayProperty
        {
            [Fact]
            public void IfInvoked_ThenReturnsToday()
            {
                // Arrange
                DateTimeProvider target = new DateTimeProvider();
                DateTime today = DateTime.Today;

                // Act
                DateTime result = target.Today;

                // Assert
                Assert.Equal(result, today);
            }
        }

        public class UtcTodayProperty
        {
            [Fact]
            public void IfInvoked_ThenReturnsUtcToday()
            {
                // Arrange
                DateTimeProvider target = new DateTimeProvider();
                DateTime today = DateTime.UtcNow.Date;

                // Act
                DateTime result = target.UtcToday;

                // Assert
                Assert.Equal(result, today);
            }
        }

        public class MinValueProperty
        {
            [Fact]
            public void IfInvoked_ThenReturnsMinValue()
            {
                // Arrange
                DateTimeProvider target = new DateTimeProvider();
                DateTime min = DateTime.MinValue;

                // Act
                DateTime result = target.MinValue;

                // Assert
                Assert.Equal(result, min);
            }
        }

        public class MaxValueProperty
        {
            [Fact]
            public void IfInvoked_ThenReturnsMaxValue()
            {
                // Arrange
                DateTimeProvider target = new DateTimeProvider();
                DateTime max = DateTime.MaxValue;

                // Act
                DateTime result = target.MaxValue;

                // Assert
                Assert.Equal(result, max);
            }
        }
    }
}