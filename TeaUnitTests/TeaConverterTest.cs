using System.Collections.Generic;

using Tea;

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
            Dictionary<string, object> dic = new Dictionary<string, object>();
            Dictionary<string, object> dicNull = null;
            Dictionary<string, object> dicMerge = new Dictionary<string, object>();
            dic.Add("testNull", null);
            dic.Add("testExist", "testExist");
            dic.Add("test", "test");
            dicMerge.Add("testMerge", "testMerge");
            dicMerge.Add("testExist", "IsExist");
            Dictionary<string, object> dicResult = TeaConverter.merge(dic, dicNull, dicMerge);
            Assert.NotNull(dicResult);
            Assert.Equal(4, dicResult.Count);

            Assert.Empty(TeaConverter.merge(null));
        }

    }
}
