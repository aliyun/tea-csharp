using System;
using System.Collections.Generic;
using System.Linq;
using Darabonba;
using Darabonba.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace DaraUnitTests.Utils
{
    public class JSONUtilTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public JSONUtilTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test_SerializeObject()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>
            { { "key", "value" }
            };
            string jsonStr = JSONUtils.SerializeObject(dict);
            Assert.NotNull(jsonStr);
            Assert.NotEmpty(jsonStr);
            Assert.Equal("{\"key\":\"value\"}", jsonStr);
            Assert.Equal("{}", JSONUtils.SerializeObject(new Dictionary<string, object>()));
            Assert.Equal("test str", JSONUtils.SerializeObject("test str"));
            Assert.Equal("1", JSONUtils.SerializeObject(1));
            Assert.Equal("true", JSONUtils.SerializeObject(true));
            Assert.Equal("null", JSONUtils.SerializeObject(null));
            Dictionary<string, object> unicode = new Dictionary<string, object>
            { { "str", "test&<>://中文" }
            };
            Assert.Equal("{\"key\":\"value\",\"map\":{\"str\":\"test&<>://中文\"},\"num\":1}", JSONUtils.SerializeObject(
                new Dictionary<string, object>
                {
                    { "key", "value" },
                    { "map", unicode },
                    { "num", 1 }
                }));
        }

        [Fact]
        public void TestDeserializeToDic()
        {
            Assert.Null(JSONUtils.Deserialize(null));

            string jsonStr = "{\"arrayObj\":[[{\"itemName\":\"item\",\"itemInt\":1},{\"itemName\":\"item2\",\"itemInt\":2}],[{\"itemName\":\"item3\",\"itemInt\":3}]],\"arrayList\":[[[1,2],[3,4]],[[5,6],[7]],[]],\"listStr\":[1,2,3],\"items\":[{\"total_size\":18,\"partNumber\":1,\"tags\":[{\"aa\":\"11\"}]},{\"total_size\":20,\"partNumber\":2,\"tags\":[{\"aa\":\"22\"}]}],\"next_marker\":\"\",\"test\":{\"total_size\":19,\"partNumber\":1,\"tags\":[{\"aa\":\"11\"}]}}";
            JObject jObject = JObject.Parse(jsonStr);
            Dictionary<string, object> dic = (Dictionary<string, object>) JSONUtils.Deserialize(jObject);
            Assert.NotNull(dic);
            List<object> listResult = (List<object>) dic["items"];
            Dictionary<string, object> item1 = (Dictionary<string, object>) listResult[0];
            Assert.Equal(18L, item1["total_size"]);
            Assert.Empty((string) dic["next_marker"]);
            Assert.Equal(2, ((List<object>) dic["arrayObj"]).Count);
        }

        [Fact]
        public void TestReadPath()
        {
            var jsonStr = "{\"testBool\":true,\"arrayObj\":[[{\"itemName\":\"item\",\"itemInt\":1},{\"itemName\":\"item2\",\"itemInt\":2}],[{\"itemName\":\"item3\",\"itemInt\":3}]],\"arrayList\":[[[1,2],[3,4]],[[5,6],[7]],[]],\"listStr\":[1,2,3],\"items\":[{\"total_size\":18,\"partNumber\":1,\"tags\":[{\"aa\":\"11\"}]},{\"total_size\":20,\"partNumber\":2,\"tags\":[{\"aa\":\"22\"}]}],\"next_marker\":\"\",\"test\":{\"total_size\":19,\"partNumber\":1,\"tags\":[{\"aa\":\"11\"}]}}";
            var jObject = JObject.Parse(jsonStr);
            _testOutputHelper.WriteLine("objjj----{0}", jObject);
            var res = JSONUtils.ReadPath(jObject, "$.arrayObj[0]");
            Assert.Equal("[{\"itemName\":\"item\",\"itemInt\":1},{\"itemName\":\"item2\",\"itemInt\":2}]", JsonConvert.SerializeObject(res));
            res = JSONUtils.ReadPath(jObject, "$.arrayObj[0][0].itemInt");
            Assert.Equal(1L, res);
            res = JSONUtils.ReadPath(jObject, "$.testBool");
            Assert.True((bool)res);
        }
        
        [Fact]
        public void Test_ReadPath()
        {
            var context = new Context
            {
                Str = "test",
                TestBool = true,
                ContextInteger = 123,
                ContextLong = 123L,
                ContextDouble = 1.123d,
                ContextFloat = 3.456f,
                ContextListLong = new List<long?> { 123L, 456L },
                ListList = new List<List<int?>>
                {
                    new List<int?> { 789, 123 },
                    new List<int?> { 8, 9 }
                },
                IntegerListMap = new Dictionary<string, List<int?>>
                {
                    { "integerList", new List<int?> { 123, 456 } }
                }
            };

            Assert.Null(JSONUtils.ReadPath(context, "$.notExist"));
            _testOutputHelper.WriteLine("bool----{0}", JSONUtils.ReadPath(context, "$.testBool"));
            Assert.True(JSONUtils.ReadPath(context, "$.testBool") is bool);
            Assert.True(JSONUtils.ReadPath(context, "$.listList") is List<object>);
            Assert.True(JSONUtils.ReadPath(context, "$.contextInteger") is long);
            Assert.True(JSONUtils.ReadPath(context, "$.contextLong") is long);
            Assert.True(JSONUtils.ReadPath(context, "$.contextDouble") is double);
            Assert.True(JSONUtils.ReadPath(context, "$.contextFloat") is double);
            Assert.True(JSONUtils.ReadPath(context, "$.contextListLong") is List<object>);
            Assert.True(JSONUtils.ReadPath(context, "$.integerListMap") is Dictionary<string, object>);

            Assert.Equal(true, JSONUtils.ReadPath(context, "$.testBool"));
            Assert.Equal("test", JSONUtils.ReadPath(context, "$.testStr"));
            Assert.Equal(123L, JSONUtils.ReadPath(context, "$.contextLong"));
            var listLong = JSONUtils.ReadPath(context, "$.contextListLong") as List<object>;
            Assert.Equal(123L, listLong[0]);

            var listList = JSONUtils.ReadPath(context, "$.listList") as List<object>;
            Assert.Equal(789L, (listList[0] as List<object>)[0]);

            var map = JSONUtils.ReadPath(context, "$.integerListMap") as Dictionary<string, object>;
            Assert.Equal(123L, (map["integerList"] as List<object>)[0]);

            var realListList = new List<List<int?>>();
            foreach (var itemList in listList)
            {
                var intList = itemList as List<object>;
                var nullableIntList = new List<int?>();
                if (intList != null)
                {
                    foreach (var item in intList)
                    {
                        var intValue = (int?)(item as long?);
                        nullableIntList.Add(intValue);
                    }
                }

                realListList.Add(nullableIntList);
            }


            var realIntegerListMap = new Dictionary<string, List<int?>>();
            foreach (var kvp in map)
            {
                string key = kvp.Key;
                object value = kvp.Value;

                var intList = value as List<object>;
                var nullableIntList = new List<int?>();
                if (intList != null)
                {
                    foreach (var item in intList)
                    {
                        nullableIntList.Add((int?)(item as long?));
                    }
                }
                realIntegerListMap[key] = nullableIntList;
            }
            var context1 = new Context
            {
                ContextLong = JSONUtils.ReadPath(context, "$.contextLong") as long?,
                ContextInteger = (int?)(JSONUtils.ReadPath(context, "$.contextInteger") as long?),
                ContextFloat = (float?)(JSONUtils.ReadPath(context, "$.contextFloat") as double?),
                ContextDouble = JSONUtils.ReadPath(context, "$.contextDouble") as double?,
                ContextListLong = (JSONUtils.ReadPath(context, "$.contextListLong") as List<object>)
                    .Select(item => item is long longValue ? longValue : (long?)null)
                    .ToList(),
                ListList = realListList,
                IntegerListMap = realIntegerListMap
            };

            Assert.Equal(123L, context1.ContextLong);
            Assert.Equal(123, context1.ContextInteger);
            Assert.Equal(3.456f, context1.ContextFloat);
            Assert.Equal(1.123d, context1.ContextDouble);
            Assert.Equal(new List<long?> { 123L, 456L }, context1.ContextListLong);
            Assert.Equal(new List<List<int?>>
            {
                new List<int?> { 789, 123 },
                new List<int?> { 8, 9 }
            }, context1.ListList);
            Assert.Equal(123, (context1.IntegerListMap["integerList"] as List<int?>)[0]);
        }
    }
    
    public class Context : Model
    {
        [NameInMap("testStr")] 
        public string Str { get; set; }
        
        [NameInMap("testBool")] 
        public bool? TestBool { get; set; }

        [NameInMap("contextInteger")] 
        public int? ContextInteger { get; set; }

        [NameInMap("contextLong")] 
        public long? ContextLong { get; set; }

        [NameInMap("contextListLong")] 
        public List<long?> ContextListLong { get; set; }

        [NameInMap("listList")] 
        public List<List<int?>> ListList { get; set; }

        [NameInMap("contextDouble")] 
        public double? ContextDouble { get; set; }

        [NameInMap("contextFloat")] 
        public float? ContextFloat { get; set; }

        [NameInMap("integerListMap")] 
        public Dictionary<string, List<int?>> IntegerListMap { get; set; }
    }
}
