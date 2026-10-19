using System;
using System.Collections.Generic;
using System.Xml;
using Darabonba;
using Darabonba.Utils;
using Xunit;

namespace DaraUnitTests.Utils
{
    public class XmlUtilsEdgeCaseTest
    {
        public class ScalarXmlModel : Model
        {
            [NameInMap("Message")]
            public string Message { get; set; }
        }

        [Fact]
        public void DeserializeScalarRootUsesMappedName()
        {
            var result = XmlUtils.ParseXml("<Message>hello &amp; goodbye</Message>", typeof(ScalarXmlModel));
            Assert.Single(result);
            Assert.Equal("hello & goodbye", result["Message"]);
        }

        [Fact]
        public void DeserializeRepeatedScalarElementsPreservesOrder()
        {
            var result = XmlUtils.ParseXml("<root><item>first</item><item>second</item><item>third</item></root>", null);
            var root = Assert.IsType<Dictionary<string, object>>(result["root"]);
            Assert.Equal(new object[] { "first", "second", "third" }, Assert.IsType<List<object>>(root["item"]));
        }

        [Fact]
        public void DeserializeRepeatedObjectsPreservesAllItems()
        {
            var result = XmlUtils.ParseXml("<root><item><value>first</value></item><item><value>second</value></item><item><value>third</value></item></root>", null);
            var root = Assert.IsType<Dictionary<string, object>>(result["root"]);
            var items = Assert.IsType<List<object>>(root["item"]);
            Assert.Equal(3, items.Count);
            Assert.Equal("first", Assert.IsType<Dictionary<string, object>>(items[0])["value"]);
            Assert.Equal("second", Assert.IsType<Dictionary<string, object>>(items[1])["value"]);
            Assert.Equal("third", Assert.IsType<Dictionary<string, object>>(items[2])["value"]);
        }

        [Fact]
        public void SerializeEmptyModelAndUnsupportedValueReturnsEmptyString()
        {
            Assert.Equal(string.Empty, XmlUtils.SerializeXml(new Model()));
            Assert.Equal(string.Empty, XmlUtils.SerializeXml(42));
        }

        [Fact]
        public void SerializeRejectsListAtRoot()
        {
            var exception = Assert.Throws<ArgumentException>(() => XmlUtils.ToXML(
                new Dictionary<string, object> { { "root", new List<string> { "item" } } }));
            Assert.Equal("unsupported nest list.", exception.Message);
        }

        [Fact]
        public void DeserializeMalformedXmlThrows()
        {
            Assert.Throws<XmlException>(() => XmlUtils.ParseXml("<root>", null));
        }
    }
}
