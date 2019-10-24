using System;
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
            model.testListStr = new List<string> { "str" };
            Dictionary<string, object> dic = model.ToMap();
            Assert.NotNull(dic);
            Assert.IsType<List<Dictionary<string, object>>>(dic["Items"]);
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
            Dictionary<string, object> dicSubNull = null;
            dicItems.Add(dicSub);
            dicItems.Add(dicSubNull);
            dic.Add("items", dicItems);

            dic.Add("testListStr", testListStr);

            TestRegModel model = TeaModel.ToObject<TestRegModel>(dic);
        }

        [Fact]
        public void TestValidator()
        {
            TestRegModel successModel = new TestRegModel();
            successModel.RequestId = "reTest";
            successModel.NextMarker = "nextMarker";
            successModel.testListStr = new List<string> { "listStr1" };
            successModel.Items = new List<TestRegSubModel> { new TestRegSubModel { RequestId = "rTest" } };
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
