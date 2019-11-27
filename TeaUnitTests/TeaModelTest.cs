﻿using System;
using System.Collections.Generic;

using Tea;

using TeaUnitTests.Models;

using Xunit;

namespace TeaUnitTests
{
    public class TeaModelTest
    {
        [Fact]
        public void TestToMap()
        {
            TestRegModel model = new TestRegModel();
            model.RequestId = "requestID";
            model.Items = new List<TestRegSubModel> { new TestRegSubModel { RequestId = "sub" }, null };
            model.NextMarker = "next";
            model.testNoAttr = "noAttr";
            model.subModel = new TestRegSubModel();
            model.testListStr = new List<string> { "str" };
            Dictionary<string, object> dic = model.ToMap();
            Assert.NotNull(dic);
            Assert.IsType<List<Dictionary<string, object>>>(dic["items"]);

            TestRegModel modelNull = new TestRegModel();
            modelNull.RequestId = "1";
            Dictionary<string, object> dicNull = modelNull.ToMap();
            Assert.Null(dicNull["items"]);
            Assert.Null(dicNull["subModel"]);
        }

        [Fact]
        public void TestToObject()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            List<Dictionary<string, object>> dicItems = new List<Dictionary<string, object>>();
            List<string> testListStr = new List<string> { "str" };
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

            TestRegModel model = TeaModel.ToObject<TestRegModel>(dic);
            Assert.NotNull(model);
            Assert.Equal("requestID", model.RequestId);
            Assert.Equal("next", model.NextMarker);
            Assert.Equal("noAttr", model.testNoAttr);
            Assert.Equal("sub", model.Items[0].RequestId);
            Assert.Equal(100, model.Items[0].testInt);
            Assert.NotNull(model.Items[1]);
            Assert.NotEqual(model.Items[0].RequestId, model.Items[1].RequestId);
            Assert.Null(model.Items[2]);
            Assert.Equal("str", model.testListStr[0]);
            Assert.NotNull(model.subModel);

        }

        [Fact]
        public void TestValidator()
        {
            TestRegModel successModel = new TestRegModel();
            successModel.RequestId = "reTest";
            successModel.NextMarker = "nextMarker";
            successModel.testListStr = new List<string> { "listStr1" };
            successModel.Items = new List<TestRegSubModel> { new TestRegSubModel { RequestId = "rTest" } };
            successModel.subModel = new TestRegSubModel { RequestId = "rTest", testInt = 10 };
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

        [Fact]
        public void TestBuildMap()
        {
            Assert.Empty(TeaModel.BuildMap(null));

            TestRegModel model = new TestRegModel();
            model.RequestId = "requestID";
            model.Items = new List<TestRegSubModel> { new TestRegSubModel { RequestId = "sub" }, null };
            model.NextMarker = "next";
            model.testNoAttr = "noAttr";
            model.subModel = new TestRegSubModel();
            model.testListStr = new List<string> { "str" };
            Dictionary<string, object> dic = TeaModel.BuildMap(model);
            Assert.NotNull(dic);
            Assert.IsType<List<Dictionary<string, object>>>(dic["items"]);
        }
    }
}
