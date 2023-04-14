using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tea;

using TeaUnitTests.Models;

using Xunit;

namespace TeaUnitTests
{
    public class TeaModelTest
    {
        public class NestedTest : TeaModel
        {
            [NameInMap("MapNestedMap")]
            public Dictionary<string, Dictionary<string, TestRegSubModel>> mapnestedMap { get; set; }

            [NameInMap("ListNestedList")]
            public List<List<IDictionary>> listNestedList { get; set; }

        }

        [Fact]
        public void TestToMap()
        {
            TeaModel modelNull = null;
            Assert.Null(modelNull.ToMap());

            TestRegModel model = new TestRegModel();
            model.RequestId = "requestID";
            model.Items = new List<TestRegSubModel> { new TestRegSubModel { RequestId = "sub" }, null };
            model.NextMarker = "next";
            model.testNoAttr = "noAttr";
            model.subModel = new TestRegSubModel();
            model.testListStr = new List<string> { "str" };
            model.bytes = Encoding.UTF8.GetBytes("test");
            TestRegSubModel dicSubModel = new TestRegSubModel
            {
                RequestId = "requestDic"
            };
            Dictionary<string, TestRegSubModel> dicSub = new Dictionary<string, TestRegSubModel>
            {
                { "subDic", dicSubModel },
                { "subNull", null }
            };
            var dicMap = new Dictionary<string, Dictionary<string, TestRegSubModel>>
            {
                { "map", dicSub }
            };
            Dictionary<string, string> map = new Dictionary<string, string>
            {
                { "test", "test" }
            };
            List<IDictionary> list = new List<IDictionary>();
            list.Add(map);
            List<List<IDictionary>> listNestedList = new List<List<IDictionary>>();
            listNestedList.Add(list);
            model.dicNestDic = dicMap;
            model.listIDic = listNestedList;
            Dictionary<string, object> dic = model.ToMap();
            Assert.NotNull(dic);

            var from = TeaModel.ToObject<TestRegModel>(dic);
            Assert.Equal("test", from.listIDic[0][0]["test"]);
            Assert.Equal("requestDic", from.dicNestDic["map"]["subDic"].RequestId);

            TestRegModel modelEmpty = new TestRegModel();
            modelEmpty.RequestId = "1";
            Dictionary<string, object> dicEmpty = modelEmpty.ToMap();
            Assert.Null(dicEmpty["items"]);
            Assert.Null(dicEmpty["subModel"]);
        }

        [Fact]
        public void TestToObject()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            List<Dictionary<string, object>> dicItems = new List<Dictionary<string, object>>();
            List<string> testListStr = new List<string> { "str","testStr"};
            dic.Add("requestId", "requestID");
            dic.Add("next_marker", "next");
            dic.Add("testNoAttr", "noAttr");

            Dictionary<string, object> dicSub = new Dictionary<string, object>();
            dicSub.Add("requestId", "sub");
            dicSub.Add("testInt", 100);
            Dictionary<string, object> dicSubNull = null;
            Dictionary<string, object> dicSubRe = new Dictionary<string, object>();
            dicSubRe.Add("requestId", "subRe");
            dicSubRe.Add("testInt", 500);
            dicItems.Add(dicSub);
            dicItems.Add(dicSubRe);
            dicItems.Add(dicSubNull);
            dic.Add("items", dicItems);

            dic.Add("testListStr", testListStr);

            Dictionary<string, object> dicSubModel = new Dictionary<string, object>();
            dicSubModel.Add("requestId", "subModel");
            dic.Add("subModel", dicSubModel);

            Dictionary<string, object> dicDict = new Dictionary<string, object>();
            dicDict.Add("test", 1);
            dicDict.Add("testListStr", testListStr);
            dicDict.Add("testDict", dicSubModel);
            dic.Add("dict", dicDict);

            dic.Add("testInt32", "-32");
            dic.Add("testLong", -64L);
            dic.Add("testFloat", 11.11);
            dic.Add("testDouble", 12.12);
            dic.Add("testBool", "false");
            dic.Add("testShort", -16);
            dic.Add("testUShort", 16);
            dic.Add("testUInt", 32);
            dic.Add("testULong", 64);
            dic.Add("testNull", null);
            Dictionary<string, object> nullValueDic = new Dictionary<string, object>();
            nullValueDic.Add("testNullValueDic", new Dictionary<string, object>());
            dic.Add("Content", nullValueDic);

            TestRegModel model = TeaModel.ToObject<TestRegModel>(dic);
            var testNullValueDic = model.Content["testNullValueDic"] as Dictionary<string, object>;
            Assert.True(testNullValueDic.Count==0);
            Assert.NotNull(model);
            Assert.Equal("requestID", model.RequestId);
            Assert.Equal("next", model.NextMarker);
            Assert.Equal("noAttr", model.testNoAttr);
            Assert.Equal("sub", model.Items[0].RequestId);
            Assert.Equal(testListStr, model.dict["testListStr"]);
            Assert.Equal(dicSubModel, model.dict["testDict"]);
            Assert.Equal(100, model.Items[0].testInt);
            Assert.NotNull(model.Items[1]);
            Assert.NotEqual(model.Items[0].RequestId, model.Items[1].RequestId);
            Assert.Null(model.Items[2]);
            Assert.Equal("str", model.testListStr[0]);
            Assert.NotNull(model.subModel);
            Assert.NotNull(model.dict);
            Assert.Null(model.testNull);

            Dictionary<string, string> dicString = null;
            Assert.Null(TeaModel.ToObject<TestDicStringModel>(dicString));
            dicString = new Dictionary<string, string>();
            dicString.Add("requestId", "test");
            dicString.Add("count", "1");
            TestDicStringModel stringModel = TeaModel.ToObject<TestDicStringModel>(dicString);
            Assert.NotNull(stringModel);
            Assert.Equal("test", stringModel.RequestId);

            Dictionary<string, object> dicConvert = new Dictionary<string, object>();
            dicConvert.Add("requestId", dicSub);
            dicConvert.Add("count", "1");

            stringModel = TeaModel.ToObject<TestDicStringModel>(dicConvert);
            Assert.NotNull(stringModel);
            Assert.Equal("{\"requestId\":\"sub\",\"testInt\":100}", stringModel.RequestId);
            Assert.Equal(1, stringModel.Count);

            dicConvert.Remove("requestId");
            dicConvert.Add("requestId", dicItems);
            stringModel = TeaModel.ToObject<TestDicStringModel>(dicConvert);
            Assert.Equal("[{\"requestId\":\"sub\",\"testInt\":100},{\"requestId\":\"subRe\",\"testInt\":500},null]", stringModel.RequestId);

            dicConvert.Remove("requestId");
            string[] array = new string[] { "a", "b" };
            dicConvert.Add("requestId", array);
            stringModel = TeaModel.ToObject<TestDicStringModel>(dicConvert);
            Assert.Equal("[\"a\",\"b\"]", stringModel.RequestId);

            dicConvert.Remove("requestId");
            dicConvert.Add("requestId", 1.1);
            stringModel = TeaModel.ToObject<TestDicStringModel>(dicConvert);
            Assert.Equal("1.1", stringModel.RequestId);

            dicConvert.Remove("requestId");
            dicConvert.Add("requestId", 11111111111111111111L);
            stringModel = TeaModel.ToObject<TestDicStringModel>(dicConvert);
            Assert.Equal("11111111111111111111", stringModel.RequestId);

            dicConvert.Remove("requestId");
            dicConvert.Add("requestId", null);
            stringModel = TeaModel.ToObject<TestDicStringModel>(dicConvert);
            Assert.Null(stringModel.RequestId);

            dicConvert.Remove("requestId");
            dicConvert.Add("requestId", true);
            stringModel = TeaModel.ToObject<TestDicStringModel>(dicConvert);
            Assert.Equal("True", stringModel.RequestId);

        }

        [Fact]
        public void TestValidator()
        {
            TeaModel modelNull = null;
            Assert.Throws<ArgumentException>(() => { modelNull.Validate(); });

            TestRegModel successModel = new TestRegModel();
            successModel.RequestId = "reTest";
            successModel.NextMarker = "nextMarker";
            successModel.testListStr = new List<string> { "listStr1" };
            successModel.Items = new List<TestRegSubModel> { new TestRegSubModel { RequestId = "rTest" } };
            successModel.subModel = new TestRegSubModel { RequestId = "rTest", testInt = 10 };
            successModel.bytes = Encoding.UTF8.GetBytes("test");
            successModel.Validate();

            successModel.testListStr = null;
            successModel.Validate();

            TestRegModel modelRequired = new TestRegModel();
            Assert.Equal("RequestId is required.",
                Assert.Throws<ArgumentException>(() => { modelRequired.Validate(); }).Message
            );

            modelRequired.RequestId = "reTest";
            modelRequired.NextMarker = "nextMarker";
            Assert.Equal("Items is required.",
                Assert.Throws<ArgumentException>(() => { modelRequired.Validate(); }).Message
            );

            TestRegModel modelReg = new TestRegModel();
            modelReg.RequestId = "123";
            modelReg.Items = new List<TestRegSubModel> { new TestRegSubModel { RequestId = "rTest" } };
            modelReg.NextMarker = "nextMarker";
            Assert.Equal("RequestId is not match re",
                Assert.Throws<ArgumentException>(() => { modelReg.Validate(); }).Message
            );

            modelReg.RequestId = "reTest";
            modelReg.testListStr = new List<string> { "test" };
            Assert.Equal("testListStr is not match listStr",
                Assert.Throws<ArgumentException>(() => { modelReg.Validate(); }).Message
            );
        }
    }
}
