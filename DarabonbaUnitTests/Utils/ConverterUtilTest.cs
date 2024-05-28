using System;
using System.Collections.Generic;
using Darabonba.Exceptions;
using Darabonba.Utils;
using DaraUnitTests.Models;
using Xunit;

namespace DaraUnitTests.Utils
{
    public class TestObject
    {
        public string name { get; set; }
    }

    public class ConverterUtilTests
    {
        [Fact]
        public void TestMergeMap()
        {
            Dictionary<string, object> dict1 = new Dictionary<string, object>
            { 
                { "key1", "value1" },
                { "key2", "value2" }
            };
            Dictionary<string, object> dict2 = new Dictionary<string, object>
            { 
                { "key2", "value22" },
                { "key3", "value3" }
            };
            Dictionary<string, object> res = ConverterUtil.Merge(dict1, dict2);
            Assert.Equal("value1", res["key1"]);
            Assert.Equal("value22", res["key2"]);
            Assert.Equal("value3", res["key3"]);
        }

        [Fact]
        public void TestMergeListMap()
        {
            Assert.Empty(ConverterUtil.Merge<object>(null));

            Dictionary<string, string> dic = new Dictionary<string, string>();
            Dictionary<string, object> dicNull = null;
            Dictionary<string, object> dicMerge = new Dictionary<string, object>();
            TestRegModel model = new TestRegModel();

            dic.Add("testNull", null);
            dic.Add("testExist", "testExist");
            dic.Add("test", "test");
            dicMerge.Add("testMerge", "testMerge");
            dicMerge.Add("testExist", "IsExist");
            Dictionary<string, string> dicResult = ConverterUtil.Merge<string>(dic, dicNull, dicMerge, null);
            Assert.NotNull(dicResult);
            Assert.Equal(4, dicResult.Count);

            Dictionary<string, object> dicModelMerge = ConverterUtil.Merge<object>(dic, dicNull, dicMerge, model);
            Assert.NotNull(dicResult);

            Assert.Throws<ArgumentException>(() => { ConverterUtil.Merge<object>(dic, 1); });
        }

        [Fact]
        public void TestStrToLower()
        {
            Assert.Empty(ConverterUtil.StrToLower(null));

            Assert.Equal("test", ConverterUtil.StrToLower("TEST"));
        }

        [Fact]
        public void Test_ParseMethods()
        {
            Assert.Equal(123, ConverterUtil.ParseInt("123"));
            Assert.Equal(123, ConverterUtil.ParseInt("123.0123"));
            Assert.Equal(123, ConverterUtil.ParseLong("123"));
            Assert.Equal(123, ConverterUtil.ParseLong("123.0123"));
            Assert.Equal(123.0123, Math.Round(ConverterUtil.ParseFloat("123.0123"), 4));
            var ex = Assert.Throws<DaraException>(() => { ConverterUtil.ParseLong((string)null); });
            Assert.Equal("Data is null.", ex.Message);
            var ex1 = Assert.Throws<FormatException>(() => { ConverterUtil.ParseLong("test"); });
            Assert.Equal("Input string was not in a correct format.", ex1.Message);
        }
    }
}
