using MetalCore.CQS.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Xunit;

namespace MetalCore.Tests.xUnitMoq.Extensions
{
    public class LinqToXmlExtensionsTests
    {
        public class DescendantsIgnoreCase_XContainer
        {
            [Fact]
            public void IfNull_ReturnsNull()
            {
                // Arrange
                XContainer input = null;

                // Act
                List<XElement> result = input?.DescendantsIgnoreCase(null)?.ToList();

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public void IfNoDescendents_ReturnsEmptyList()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<test></test>")));
                XContainer input = xml.Root;

                // Act
                List<XElement> result = input.DescendantsIgnoreCase(null)?.ToList();

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public void IfOneDescendentsWithMatchingCase_ReturnsOneElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<test><two></two></test>")));
                XContainer input = xml.Root;

                // Act
                List<XElement> result = input.DescendantsIgnoreCase("two")?.ToList();

                // Assert
                Assert.Single(result);
                Assert.Equal("two", result.Single().Name);
            }

            [Fact]
            public void IfOneDescendentsWithNonMatchingCase_ReturnsOneElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO></TWO></TEST>")));
                XContainer input = xml.Root;

                // Act
                List<XElement> result = input.DescendantsIgnoreCase("two")?.ToList();

                // Assert
                Assert.Single(result);
                Assert.Equal("TWO", result.Single().Name);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithNonMatchingCaseAndNonMatchingNamespace_ReturnsNoElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                XContainer input = xml.Root;

                // Act
                List<XElement> result = input.DescendantsIgnoreCase(XName.Get("TWO", "pp"))?.ToList();

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithNonMatchingCaseAndNonMatchingCaseNamespace_ReturnsOneElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                XContainer input = xml.Root;

                // Act
                List<XElement> result = input.DescendantsIgnoreCase(XName.Get("TWO", "CRAP"))?.ToList();

                // Assert
                Assert.Single(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithMatchingCaseAndNonMatchingCaseNamespace_ReturnsNoElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                XContainer input = xml.Root;

                // Act
                List<XElement> result = input.DescendantsIgnoreCase(XName.Get("two", "CRAP"))?.ToList();

                // Assert
                Assert.Single(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithAndNullNamespace_ThenThrowsArgumentNullException()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                XContainer input = xml.Root;

                // Act
                Action act = () => input.DescendantsIgnoreCase(XName.Get("TWO", null))?.ToList();

                // Assert
                Assert.Throws<ArgumentNullException>("namespaceName", act);
            }

            [Fact]
            public void IfOneDescendentsWithNullLocalNameWithAndNamespace_ThenThrowsArgumentNullException()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                XContainer input = xml.Root;

                // Act
                Action act = () => input.DescendantsIgnoreCase(XName.Get(null, "crap"))?.ToList();

                // Assert
                Assert.Throws<ArgumentNullException>("localName", act);
            }

        }

        public class DescendantsIgnoreCase_IEnumerable_XElement
        {
            [Fact]
            public void IfNull_ReturnsNull()
            {
                // Arrange
                IEnumerable<XElement> input = null;

                // Act
                List<XElement> result = input?.DescendantsIgnoreCase(null)?.ToList();

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public void IfNoDescendents_ReturnsEmptyList()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<test></test>")));
                IEnumerable<XElement> input = xml.Elements();

                // Act
                List<XElement> result = input.DescendantsIgnoreCase(null)?.ToList();

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public void IfOneDescendentsWithMatchingCase_ReturnsOneElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<test><two></two></test>")));
                IEnumerable<XElement> input = xml.Elements();

                // Act
                List<XElement> result = input.DescendantsIgnoreCase("two")?.ToList();

                // Assert
                Assert.Single(result);
                Assert.Equal("two", result.Single().Name);
            }

            [Fact]
            public void IfOneDescendentsWithNonMatchingCase_ReturnsOneElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO></TWO></TEST>")));
                IEnumerable<XElement> input = xml.Elements();

                // Act
                List<XElement> result = input.DescendantsIgnoreCase("two")?.ToList();

                // Assert
                Assert.Single(result);
                Assert.Equal("TWO", result.Single().Name);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithNonMatchingCaseAndNonMatchingNamespace_ReturnsNoElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                IEnumerable<XElement> input = xml.Elements();

                // Act
                List<XElement> result = input.DescendantsIgnoreCase(XName.Get("TWO", "pp"))?.ToList();

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithNonMatchingCaseAndNonMatchingCaseNamespace_ReturnsOneElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                IEnumerable<XElement> input = xml.Elements();

                // Act
                List<XElement> result = input.DescendantsIgnoreCase(XName.Get("TWO", "CRAP"))?.ToList();

                // Assert
                Assert.Single(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithMatchingCaseAndNonMatchingCaseNamespace_ReturnsNoElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                IEnumerable<XElement> input = xml.Elements();

                // Act
                List<XElement> result = input.DescendantsIgnoreCase(XName.Get("two", "CRAP"))?.ToList();

                // Assert
                Assert.Single(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithAndNullNamespace_ThenThrowsArgumentNullException()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                IEnumerable<XElement> input = xml.Elements();

                // Act
                Action act = () => input.DescendantsIgnoreCase(XName.Get("TWO", null))?.ToList();

                // Assert
                Assert.Throws<ArgumentNullException>("namespaceName", act);
            }

            [Fact]
            public void IfOneDescendentsWithNullLocalNameWithAndNamespace_ThenThrowsArgumentNullException()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                IEnumerable<XElement> input = xml.Elements();

                // Act
                Action act = () => input.DescendantsIgnoreCase(XName.Get(null, "crap"))?.ToList();

                // Assert
                Assert.Throws<ArgumentNullException>("localName", act);
            }

        }

        public class ElementsIgnoreCase_IEnumerable_XElement
        {
            [Fact]
            public void IfNull_ReturnsNull()
            {
                // Arrange
                IEnumerable<XElement> input = null;

                // Act
                List<XElement> result = input?.ElementsIgnoreCase(null)?.ToList();

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public void IfNoDescendents_ReturnsEmptyList()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<test></test>")));
                IEnumerable<XElement> input = xml.Elements();

                // Act
                List<XElement> result = input.ElementsIgnoreCase(null)?.ToList();

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public void IfOneDescendentsWithMatchingCase_ReturnsOneElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<test><two></two></test>")));
                IEnumerable<XElement> input = xml.Elements();

                // Act
                List<XElement> result = input.ElementsIgnoreCase("two")?.ToList();

                // Assert
                Assert.Single(result);
                Assert.Equal("two", result.Single().Name);
            }

            [Fact]
            public void IfOneDescendentsWithNonMatchingCase_ReturnsOneElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO></TWO></TEST>")));
                IEnumerable<XElement> input = xml.Elements();

                // Act
                List<XElement> result = input.ElementsIgnoreCase("two")?.ToList();

                // Assert
                Assert.Single(result);
                Assert.Equal("TWO", result.Single().Name);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithNonMatchingCaseAndNonMatchingNamespace_ReturnsNoElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                IEnumerable<XElement> input = xml.Elements();

                // Act
                List<XElement> result = input.ElementsIgnoreCase(XName.Get("TWO", "pp"))?.ToList();

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithNonMatchingCaseAndNonMatchingCaseNamespace_ReturnsOneElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                IEnumerable<XElement> input = xml.Elements();

                // Act
                List<XElement> result = input.ElementsIgnoreCase(XName.Get("TWO", "CRAP"))?.ToList();

                // Assert
                Assert.Single(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithMatchingCaseAndNonMatchingCaseNamespace_ReturnsNoElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                IEnumerable<XElement> input = xml.Elements();

                // Act
                List<XElement> result = input.ElementsIgnoreCase(XName.Get("two", "CRAP"))?.ToList();

                // Assert
                Assert.Single(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithAndNullNamespace_ThenThrowsArgumentNullException()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                IEnumerable<XElement> input = xml.Elements();

                // Act
                Action act = () => input.ElementsIgnoreCase(XName.Get("TWO", null))?.ToList();

                // Assert
                Assert.Throws<ArgumentNullException>("namespaceName", act);
            }

            [Fact]
            public void IfOneDescendentsWithNullLocalNameWithAndNamespace_ThenThrowsArgumentNullException()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                IEnumerable<XElement> input = xml.Elements();

                // Act
                Action act = () => input.ElementsIgnoreCase(XName.Get(null, "crap"))?.ToList();

                // Assert
                Assert.Throws<ArgumentNullException>("localName", act);
            }

        }

        public class ElementsIgnoreCase_XContainer
        {
            [Fact]
            public void IfNull_ReturnsNull()
            {
                // Arrange
                XContainer input = null;

                // Act
                List<XElement> result = input?.ElementsIgnoreCase(null)?.ToList();

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public void IfNoDescendents_ReturnsEmptyList()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<test></test>")));
                XContainer input = xml.Root;

                // Act
                List<XElement> result = input.ElementsIgnoreCase(null)?.ToList();

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public void IfOneDescendentsWithMatchingCase_ReturnsOneElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<test><two></two></test>")));
                XContainer input = xml.Root;

                // Act
                List<XElement> result = input.ElementsIgnoreCase("two")?.ToList();

                // Assert
                Assert.Single(result);
                Assert.Equal("two", result.Single().Name);
            }

            [Fact]
            public void IfOneDescendentsWithNonMatchingCase_ReturnsOneElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO></TWO></TEST>")));
                XContainer input = xml.Root;

                // Act
                List<XElement> result = input.ElementsIgnoreCase("two")?.ToList();

                // Assert
                Assert.Single(result);
                Assert.Equal("TWO", result.Single().Name);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithNonMatchingCaseAndNonMatchingNamespace_ReturnsNoElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                XContainer input = xml.Root;

                // Act
                List<XElement> result = input.ElementsIgnoreCase(XName.Get("TWO", "pp"))?.ToList();

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithNonMatchingCaseAndNonMatchingCaseNamespace_ReturnsOneElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                XContainer input = xml.Root;

                // Act
                List<XElement> result = input.ElementsIgnoreCase(XName.Get("TWO", "CRAP"))?.ToList();

                // Assert
                Assert.Single(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithMatchingCaseAndNonMatchingCaseNamespace_ReturnsNoElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                XContainer input = xml.Root;

                // Act
                List<XElement> result = input.ElementsIgnoreCase(XName.Get("two", "CRAP"))?.ToList();

                // Assert
                Assert.Single(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithAndNullNamespace_ThenThrowsArgumentNullException()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                XContainer input = xml.Root;

                // Act
                Action act = () => input.ElementsIgnoreCase(XName.Get("TWO", null))?.ToList();

                // Assert
                Assert.Throws<ArgumentNullException>("namespaceName", act);
            }

            [Fact]
            public void IfOneDescendentsWithNullLocalNameWithAndNamespace_ThenThrowsArgumentNullException()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                XContainer input = xml.Root;

                // Act
                Action act = () => input.ElementsIgnoreCase(XName.Get(null, "crap"))?.ToList();

                // Assert
                Assert.Throws<ArgumentNullException>("localName", act);
            }

        }

        public class ElementIgnoreCase_XContainer
        {
            [Fact]
            public void IfNull_ReturnsNull()
            {
                // Arrange
                XElement input = null;

                // Act
                XElement result = input?.ElementIgnoreCase(null);

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public void IfNoDescendents_ReturnsEmptyList()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<test></test>")));
                XElement input = xml.Root;

                // Act
                XElement result = input.ElementIgnoreCase(null);

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public void IfOneDescendentsWithMatchingCase_ReturnsOneElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<test><two></two></test>")));
                XElement input = xml.Root;

                // Act
                XElement result = input.ElementIgnoreCase("two");

                // Assert
                Assert.NotNull(result);
                Assert.Equal("two", result.Name);
            }

            [Fact]
            public void IfOneDescendentsWithNonMatchingCase_ReturnsOneElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO></TWO></TEST>")));
                XElement input = xml.Root;

                // Act
                XElement result = input.ElementIgnoreCase("two");

                // Assert
                Assert.NotNull(result);
                Assert.Equal("TWO", result.Name);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithNonMatchingCaseAndNonMatchingNamespace_ReturnsNoElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                XElement input = xml.Root;

                // Act
                XElement result = input.ElementIgnoreCase(XName.Get("TWO", "pp"));

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithNonMatchingCaseAndNonMatchingCaseNamespace_ReturnsOneElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                XElement input = xml.Root;

                // Act
                XElement result = input.ElementIgnoreCase(XName.Get("TWO", "CRAP"));

                // Assert
                Assert.NotNull(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithMatchingCaseAndNonMatchingCaseNamespace_ReturnsNoElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                XElement input = xml.Root;

                // Act
                XElement result = input.ElementIgnoreCase(XName.Get("two", "CRAP"));

                // Assert
                Assert.NotNull(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithAndNullNamespace_ThenThrowsArgumentNullException()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                XElement input = xml.Root;

                // Act
                Action act = () => input.ElementIgnoreCase(XName.Get("TWO", null));

                // Assert
                Assert.Throws<ArgumentNullException>("namespaceName", act);
            }

            [Fact]
            public void IfOneDescendentsWithNullLocalNameWithAndNamespace_ThenThrowsArgumentNullException()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\"></TWO></TEST>")));
                XElement input = xml.Root;

                // Act
                Action act = () => input.ElementIgnoreCase(XName.Get(null, "crap"));

                // Assert
                Assert.Throws<ArgumentNullException>("localName", act);
            }

        }

        public class AttributesIgnoreCase_XContainer
        {
            [Fact]
            public void IfNull_ReturnsNull()
            {
                // Arrange
                XElement input = null;

                // Act
                List<XAttribute> result = input?.AttributesIgnoreCase(null)?.ToList();

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public void IfNoDescendents_ReturnsEmptyList()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<test></test>")));
                XElement input = xml.Root;

                // Act
                List<XAttribute> result = input.AttributesIgnoreCase(null)?.ToList();

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public void IfOneDescendentsWithMatchingCase_ReturnsOneElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<test tt='true'><two></two></test>")));
                XElement input = xml.Root;

                // Act
                List<XAttribute> result = input.AttributesIgnoreCase("tt")?.ToList();

                // Assert
                Assert.Single(result);
                Assert.Equal("tt", result.Single().Name);
                Assert.Equal("true", result.Single().Value);
            }

            [Fact]
            public void IfOneDescendentsWithNonMatchingCase_ReturnsOneElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST TT='TRUE'><TWO></TWO></TEST>")));
                XElement input = xml.Root;

                // Act
                List<XAttribute> result = input.AttributesIgnoreCase("tt")?.ToList();

                // Assert
                Assert.Single(result);
                Assert.Equal("TT", result.Single().Name);
                Assert.Equal("TRUE", result.Single().Value);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithNonMatchingCaseAndNonMatchingNamespace_ReturnsNoElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST xmlns=\"crap\" TT='TRUE'><TWO></TWO></TEST>")));
                XElement input = xml.Root;

                // Act
                List<XAttribute> result = input.AttributesIgnoreCase(XName.Get("TWO", "pp"))?.ToList();

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithNonMatchingCaseAndNonMatchingCaseNamespace_ReturnsOneElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST xmlns:tt=\"crap\" tt:tt='TRUE'><TWO></TWO></TEST>")));
                XElement input = xml.Root;

                // Act
                List<XAttribute> result = input.AttributesIgnoreCase(XName.Get("tt", "CRAP"))?.ToList();

                // Assert
                Assert.Single(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithMatchingCaseAndNonMatchingCaseNamespace_ReturnsOneElement()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST xmlns:tt=\"crap\" tt:TT='TRUE'><TWO></TWO></TEST>")));
                XElement input = xml.Root;

                // Act
                List<XAttribute> result = input.AttributesIgnoreCase(XName.Get("TT", "CRAP"))?.ToList();

                // Assert
                Assert.Single(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithAndNullNamespace_ThenThrowsArgumentNullException()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\" tt='true'></TWO></TEST>")));
                XElement input = xml.Root;

                // Act
                Action act = () => input.AttributesIgnoreCase(XName.Get("TWO", null))?.ToList();

                // Assert
                Assert.Throws<ArgumentNullException>("namespaceName", act);
            }

            [Fact]
            public void IfOneDescendentsWithNullLocalNameWithAndNamespace_ThenThrowsArgumentNullException()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST><TWO xmlns=\"crap\" tt='true'></TWO></TEST>")));
                XElement input = xml.Root;

                // Act
                Action act = () => input.AttributesIgnoreCase(XName.Get(null, "crap"))?.ToList();

                // Assert
                Assert.Throws<ArgumentNullException>("localName", act);
            }

        }

        public class AttributeIgnoreCase_XContainer
        {
            [Fact]
            public void IfNull_ReturnsNull()
            {
                // Arrange
                XElement input = null;

                // Act
                XAttribute result = input?.AttributeIgnoreCase(null);

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public void IfNoDescendents_ReturnsEmptyList()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<test></test>")));
                XElement input = xml.Root;

                // Act
                XAttribute result = input.AttributeIgnoreCase(null);

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public void IfOneDescendentsWithMatchingCase_ReturnsOneAttribute()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<test tt='true'><two></two></test>")));
                XElement input = xml.Root;

                // Act
                XAttribute result = input.AttributeIgnoreCase("tt");

                // Assert
                Assert.NotNull(result);
                Assert.Equal("tt", result.Name);
                Assert.Equal("true", result.Value);
            }

            [Fact]
            public void IfOneDescendentsWithNonMatchingCase_ReturnsOneAttribute()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST TT='TRUE'><TWO></TWO></TEST>")));
                XElement input = xml.Root;

                // Act
                XAttribute result = input.AttributeIgnoreCase("tt");

                // Assert
                Assert.NotNull(result);
                Assert.Equal("TT", result.Name);
                Assert.Equal("TRUE", result.Value);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithNonMatchingCaseAndNonMatchingNamespace_ReturnsNoAttribute()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST xmlns:tt=\"crap\" tt:tt='true'><TWO></TWO></TEST>")));
                XElement input = xml.Root;

                // Act
                XAttribute result = input.AttributeIgnoreCase(XName.Get("TWO", "pp"));

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithNonMatchingCaseAndNonMatchingCaseNamespace_ReturnsOneAttribute()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST xmlns:tt=\"crap\" tt:tt='true'><TWO></TWO></TEST>")));
                XElement input = xml.Root;

                // Act
                XAttribute result = input.AttributeIgnoreCase(XName.Get("TT", "CRAP"));

                // Assert
                Assert.NotNull(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithMatchingCaseAndNonMatchingCaseNamespace_ReturnsNoAttribute()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST xmlns:tt=\"crap\" tt:tt='true'><TWO xmlns=\"crap\"></TWO></TEST>")));
                XElement input = xml.Root;

                // Act
                XAttribute result = input.AttributeIgnoreCase(XName.Get("tt", "CRAP"));

                // Assert
                Assert.NotNull(result);
            }

            [Fact]
            public void IfOneDescendentsWithLocalNameWithAndNullNamespace_ThenThrowsArgumentNullException()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST xmlns:tt=\"crap\" tt:tt='true'><TWO></TWO></TEST>")));
                XElement input = xml.Root;

                // Act
                Action act = () => input.AttributeIgnoreCase(XName.Get("TT", null));

                // Assert
                Assert.Throws<ArgumentNullException>("namespaceName", act);
            }

            [Fact]
            public void IfOneDescendentsWithNullLocalNameWithAndNamespace_ThenThrowsArgumentNullException()
            {
                // Arrange
                XDocument xml = XDocument.Load(XmlReader.Create(new StringReader("<TEST xmlns=\"crap\"><TWO></TWO></TEST>")));
                XElement input = xml.Root;

                // Act
                Action act = () => input.AttributeIgnoreCase(XName.Get(null, "crap"));

                // Assert
                Assert.Throws<ArgumentNullException>("localName", act);
            }

        }

    }
}
