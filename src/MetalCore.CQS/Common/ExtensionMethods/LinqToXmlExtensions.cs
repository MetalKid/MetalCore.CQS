using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MetalCore.CQS.ExtensionMethods
{
    /// <summary>
    /// This class stores Linq to XML methods that allow you to ignore case.
    /// </summary>
    public static class LinqToXmlExtensions
    {
        /// <summary>
        /// Returns a collection of the descendant elements for this document or element, in document 
        /// order and ignoring case.
        /// </summary>
        /// <param name="container">The class being extended.</param>
        /// <param name="name"></param>
        /// <returns>IEnumerable of XElement.</returns>
        public static IEnumerable<XElement> DescendantsIgnoreCase(this XContainer container, XName name)
        {
            foreach (XElement element in container.Descendants())
            {
                if (element.Name.NamespaceName.Equals(name.NamespaceName, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(element.Name.LocalName, name.LocalName, StringComparison.OrdinalIgnoreCase))
                {
                    yield return element;
                }
            }
            yield break;
        }

        /// <summary>
        /// Returns a collection of the descendant elements for this document or element, in 
        /// document order, ignoring case.
        /// </summary>
        /// <param name="container">The class being extended.</param>
        /// <param name="name"></param>
        /// <returns>IEnumerable of XElement.</returns>
        public static IEnumerable<XElement> DescendantsIgnoreCase(this IEnumerable<XElement> container, XName name)
        {
            foreach (XElement element in container.Descendants())
            {
                if (element.Name?.NamespaceName?.Equals(name?.NamespaceName, StringComparison.OrdinalIgnoreCase) == true &&
                    string.Equals(element.Name?.LocalName, name?.LocalName, StringComparison.OrdinalIgnoreCase))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Returns a collection of the child elements of every element and document in the 
        /// source collection, ignoring case.
        /// </summary>
        /// <param name="container">The class being extended.</param>
        /// <param name="name"></param>
        /// <returns>IEnumerable of XElement.</returns>
        public static IEnumerable<XElement> ElementsIgnoreCase(this IEnumerable<XElement> container, XName name)
        {
            foreach (XElement element in container.Elements())
            {
                if (element.Name?.NamespaceName?.Equals(name?.NamespaceName, StringComparison.OrdinalIgnoreCase) == true &&
                    string.Equals(element.Name?.LocalName, name?.LocalName, StringComparison.OrdinalIgnoreCase))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Returns a filtered collection of the child elements of this element or document,
        ///     in document order. Only elements that have a matching System.Xml.Linq.XName
        ///     are included in the collection, ignoring case.
        /// </summary>
        /// <param name="container">The class being extended.</param>
        /// <param name="name"></param>
        /// <returns>IEnumerable of XElement.</returns>
        public static IEnumerable<XElement> ElementsIgnoreCase(this XContainer container, XName name)
        {
            foreach (XElement element in container.Elements())
            {
                if (element.Name?.NamespaceName?.Equals(name?.NamespaceName, StringComparison.OrdinalIgnoreCase) == true &&
                    string.Equals(element.Name?.LocalName, name?.LocalName, StringComparison.OrdinalIgnoreCase))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Gets the first (in document order) child element with the specified System.Xml.Linq.XName, 
        /// ignoring case.
        /// </summary>
        /// <param name="element">The class being extended.</param>
        /// <param name="name"></param>
        /// <returns>XElement.</returns>
        public static XElement ElementIgnoreCase(this XElement element, XName name)
        {
            return element.Element(name) ??
element.Elements()
.FirstOrDefault(
s => s.Name?.NamespaceName?.Equals(name?.NamespaceName, StringComparison.OrdinalIgnoreCase) == true &&
string.Equals(s.Name?.LocalName, name?.LocalName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns a filtered collection of attributes of this element. Only elements that have a matching 
        /// System.Xml.Linq.XName are included in the collection, ignoring case.
        /// </summary>
        /// <param name="element">The class being extended.</param>
        /// <param name="name"></param>
        /// <returns>IEnumerable of XAttribute.</returns>
        public static IEnumerable<XAttribute> AttributesIgnoreCase(this XElement element, XName name)
        {
            foreach (XAttribute attr in element.Attributes())
            {
                if (attr.Name?.NamespaceName?.Equals(name?.NamespaceName, StringComparison.OrdinalIgnoreCase) == true &&
                    string.Equals(attr.Name?.LocalName, name?.LocalName, StringComparison.OrdinalIgnoreCase))
                {
                    yield return attr;
                }
            }
        }

        /// <summary>
        /// Returns a filtered collection of attributes of this element. Only elements
        /// that have a matching System.Xml.Linq.XName are included in the collection, ignoring case.
        /// </summary>
        /// <param name="element">The class being extended.</param>
        /// <param name="name"></param>
        /// <returns>XAttribute</returns>
        public static XAttribute AttributeIgnoreCase(this XElement element, XName name)
        {
            return element.Attributes()
.FirstOrDefault(
s => s.Name?.NamespaceName?.Equals(name?.NamespaceName, StringComparison.OrdinalIgnoreCase) == true &&
string.Equals(s.Name?.LocalName, name?.LocalName, StringComparison.OrdinalIgnoreCase));
        }
    }
}