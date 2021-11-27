using MetalCore.CQS.Query;
using Moq;
using System;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.CQS.Query
{
    public class QueryCacheRegionTests
    {
        public class ConstructorMethods
        {
            [Fact]
            public void IfNullUserContext_ThenNoExceptionThrownAndNotNullInstance()
            {
                // Arrange

                // Act
                Mock<QueryCacheRegion> result = new Mock<QueryCacheRegion>();

                // Assert
                Assert.NotNull(result);
            }
        }

        public class GetCacheRegion
        {
            public class UserSpecificType : IQueryCacheableByUser { }
            public class LanguageSpecificType : IQueryCacheableByLanguage { }
            public class UserLanguageSpecificType : IQueryCacheableByUser, IQueryCacheableByLanguage { }

            [Fact]
            public void IfNullUserContextAndNullType_ThenArgumentNullExceptionThrown()
            {
                // Arrange
                Mock<QueryCacheRegion> queryCacheRegion = new Mock<QueryCacheRegion>();

                // Act
                Action act = () => queryCacheRegion.Object.GetCacheRegion(null);

                // Assert
                Assert.Throws<ArgumentNullException>("queryType", act);
            }

            [Fact]
            public void IfNullUserContextAndNotNullType_ThenNameIsFullTypeName()
            {
                // Arrange
                Type type = typeof(GetCacheRegion);
                Mock<QueryCacheRegion> queryCacheRegion = new Mock<QueryCacheRegion>()
                {
                    CallBase = true
                };

                // Act
                string result = queryCacheRegion.Object.GetCacheRegion(type);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(type.FullName, result);
            }

            [Fact]
            public void IfNullUserContextAndUserSpecificType_ThenNameIsFullTypeName()
            {
                // Arrange
                Type type = typeof(UserSpecificType);
                Mock<QueryCacheRegion> queryCacheRegion = new Mock<QueryCacheRegion>()
                {
                    CallBase = true
                };

                // Act
                string result = queryCacheRegion.Object.GetCacheRegion(type);

                // Assert
                Assert.NotNull(result);
                Assert.Equal($"{type.FullName}-", result);
            }

            [Fact]
            public void IfNullUserContextAndLanguageSpecificType_ThenNameIsFullTypeName()
            {
                // Arrange
                Type type = typeof(LanguageSpecificType);
                Mock<QueryCacheRegion> queryCacheRegion = new Mock<QueryCacheRegion>()
                {
                    CallBase = true
                };

                // Act
                string result = queryCacheRegion.Object.GetCacheRegion(type);

                // Assert
                Assert.NotNull(result);
                Assert.Equal($"{type.FullName}-", result);
            }

            [Fact]
            public void IfNullUserContextAndUserLanguageSpecificType_ThenNameIsFullTypeName()
            {
                // Arrange
                Type type = typeof(UserLanguageSpecificType);
                Mock<QueryCacheRegion> queryCacheRegion = new Mock<QueryCacheRegion>()
                {
                    CallBase = true
                };

                // Act
                string result = queryCacheRegion.Object.GetCacheRegion(type);

                // Assert
                Assert.NotNull(result);
                Assert.Equal($"{type.FullName}--", result);
            }
        }
    }
}