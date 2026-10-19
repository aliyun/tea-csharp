using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Darabonba.Utils;
using Newtonsoft.Json.Linq;
using Xunit;

namespace DaraUnitTests.Utils
{
    public class UtilityEdgeCaseTest
    {
        private class InvalidString
        {
            public override string ToString()
            {
                throw new InvalidOperationException("Cannot format value");
            }
        }

        [Fact]
        public void SafeStringReturnsDefaultWhenFormattingFails()
        {
            Assert.Null(new InvalidString().ToSafeString());
            Assert.Equal("fallback", new InvalidString().ToSafeString("fallback"));
        }

        [Fact]
        public void DictionaryGetHandlesNullDictionariesAndKeys()
        {
            Dictionary<string, string> generic = null;
            Assert.Null(generic.Get("key", "fallback"));

            IDictionary dictionary = null;
            Assert.Null(dictionary.Get("key", "fallback"));
            dictionary = new Hashtable { { "key", "value" } };
            Assert.Equal("fallback", dictionary.Get(null, "fallback"));
            Assert.Equal("value", dictionary.Get("key"));
            Assert.Null(dictionary.Get("missing"));
        }

        [Theory]
        [InlineData("utf-8")]
        [InlineData("unknown")]
        public void BytesUseUtf8ForAliasesAndUnknownEncodings(string encoding)
        {
            Assert.Equal(Encoding.UTF8.GetBytes("中文"), BytesUtils.From("中文", encoding));
        }

        [Fact]
        public void HexHandlesEmptyInputAndPadsBytes()
        {
            Assert.Equal(string.Empty, BytesUtils.ToHex(null));
            Assert.Equal(string.Empty, BytesUtils.ToHex(new byte[0]));
            Assert.Equal("00010fff", BytesUtils.ToHex(new byte[] { 0, 1, 15, 255 }));
        }

        [Fact]
        public void JsonPreservesExplicitNullAndHandlesMissingPaths()
        {
            Assert.Null(JSONUtils.ParseToMap(null));
            var json = JObject.Parse("{\"value\":null}");
            Assert.Equal(JTokenType.Null, Assert.IsType<JValue>(JSONUtils.ReadPath(json, "value")).Type);
            Assert.Null(JSONUtils.ReadPath(json, "missing"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("/")]
        public void PathEncodingPreservesEmptyAndRootPaths(string path)
        {
            Assert.Equal(path, Darabonba.URL.PathEncode(path));
        }
    }
}
