using System.Collections.Generic;

using Xunit;

namespace Tea
{
    public class TestObject
    {
        public string name;
    }

    public class TeaConverterTests
    {

        [Fact]
        public void TestToObject()
        {
            var dict = new Dictionary<string, object>();
            dict.Add("name", "Jackson Tian");
            TestObject obj = TeaModel.ToObject<TestObject>(dict);
            Assert.Equal("Jackson Tian", obj.name);
        }

    }
}
