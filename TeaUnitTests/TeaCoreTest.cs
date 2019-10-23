using System;
using System.Collections.Generic;

using TeaUnitTests.Models;

using Xunit;

namespace TeaUnitTests
{
    public class TeaCoreTest
    {
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
