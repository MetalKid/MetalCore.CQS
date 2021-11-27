using AutoFixture.Xunit2;
using MetalCore.CQS.Mapper;
using MetalCore.CQS.Mediators;
using Moq;
using System;
using System.Threading;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.Mediators
{
    public class MapperMediatorTests
    {
        public class ContstructorMethods
        {
            [Fact]
            public void IfNullCallback_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => new MapperMediator(null);

                // Assert
                Assert.Throws<ArgumentNullException>("getInstanceCallback", act);
            }

            [Fact]
            public void IfNotNullCallback_ThenNoExceptionThrownAndNotNullInstance()
            {
                // Arrange

                // Act
                MapperMediator result = new MapperMediator(t => null);

                // Assert
                Assert.NotNull(result);
            }
        }

        public class Map
        {
            private readonly MapperMediator _mediator;
            private readonly Mock<IMapper<string, string>> _mapper = new Mock<IMapper<string, string>>();

            public Map()
            {
                _mediator = new MapperMediator(x => _mapper.Object);
            }

            [Fact]
            public void IfNullFromObject_ThenArgumentNullExceptionThrown()
            {
                // Arrange

                // Act
                Action act = () => _mediator.Map<string>(null);

                // Assert
                Assert.Throws<ArgumentNullException>("from", act);
            }

            [Theory]
            [AutoData]
            public void IfNotFoundInCallback_ThenTypeLoadExceptionThrown(string data)
            {
                // Arrange

                // Act
                Action act = () => new MapperMediator(t => null).Map<string>(data);

                // Assert
                Assert.Throws<TypeLoadException>(act);
            }

            [Theory]
            [AutoData]
            public void IfFoundInCallback_ThenDataMapped(string data, string output)
            {
                // Arrange
                _mapper.Setup(a => a.Map(data, null)).Returns(output);

                // Act
                string result = _mediator.Map<string>(data);

                // Assert
                Assert.Equal(output, result);
            }

            [Theory]
            [AutoData]
            public void IfFoundInCallback_ThenDataMappedWithDefault(string data, string output)
            {
                // Arrange
                _mapper.Setup(a => a.Map(data, data)).Returns(output);

                // Act
                string result = _mediator.Map<string>(data, data);

                // Assert
                Assert.Equal(output, result);
            }
        }
    }
}
