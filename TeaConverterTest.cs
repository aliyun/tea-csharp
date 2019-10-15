using Xunit;

using System.Collections.Generic;

namespace Tea
{
    public class TestObject {
        public string name;
    }

    public class TeaConverterTests
    {
        [Fact]
        public void TestToMap()
        {
            TestObject obj = new TestObject();
            obj.name = "JacksonTian";
            var map = TeaConverter.toMap(obj);
            Assert.True(map.ContainsKey("name"));
            Assert.Equal("JacksonTian", map["name"]);
        }

        [Fact]
        public void TestToObject()
        {
            var dict = new Dictionary<string, object>();
            dict.Add("name", "Jackson Tian");
            TestObject obj = TeaConverter.toObject<TestObject>(dict);
            Assert.Equal("Jackson Tian", obj.name);
        }

    }
}