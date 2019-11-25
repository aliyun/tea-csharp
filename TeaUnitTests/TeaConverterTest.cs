using System;
using System.Collections.Generic;

using Tea;

using TeaUnitTests.Models;

using Xunit;

namespace TeaUnitTests
{
    public class TestObject
    {
        public string name { get; set; }
    }

    public class TeaConverterTests
    {
        [Fact]
        public void TestMerge()
        {
            Assert.Empty(TeaConverter.merge<object>(null));

            Dictionary<string, string> dic = new Dictionary<string, string>();
            Dictionary<string, object> dicNull = null;
            Dictionary<string, object> dicMerge = new Dictionary<string, object>();
            TestRegModel model = new TestRegModel();

            dic.Add("testNull", null);
            dic.Add("testExist", "testExist");
            dic.Add("test", "test");
            dicMerge.Add("testMerge", "testMerge");
            dicMerge.Add("testExist", "IsExist");
            Dictionary<string, string> dicResult = TeaConverter.merge<string>(dic, dicNull, dicMerge, null);
            Assert.NotNull(dicResult);
            Assert.Equal(4, dicResult.Count);

            Dictionary<string, object> dicModelMerge = TeaConverter.merge<object>(dic, dicNull, dicMerge, model);
            Assert.NotNull(dicResult);

            Assert.Throws<ArgumentException>(() => { TeaConverter.merge<object>(dic, 1); });
        }

        [Fact]
        public void TestStrToLower()
        {
            Assert.Empty(TeaConverter.StrToLower(null));

            Assert.Equal("test", TeaConverter.StrToLower("TEST"));
        }

    }
}
