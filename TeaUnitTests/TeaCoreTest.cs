using System;
using System.Collections.Generic;
using Tea;
using TeaUnitTests.Models;
using Xunit;

namespace TeaUnitTests
{
    public class TeaCoreTest
    {
        [Fact]
        public void TestValidator()
        {
            TestRegModel model=new TestRegModel();
            model.RequestId="re123";
            List<TestRegSubModel> items=new List<TestRegSubModel>();
            items.Add(new TestRegSubModel{RequestId="r"});
            model.Items=items;
            model.NextMarker="next";
            model.testNoAttr="testNoAttr";
            TeaCore.ValidateModel(model);

            model.NextMarker="test";
            Assert.Throws<ArgumentException>(()=>{TeaCore.ValidateModel(model);});
            
            model.NextMarker="next";
            items.Add(new TestRegSubModel{RequestId="test"});
            Assert.Throws<ArgumentException>(()=>{TeaCore.ValidateModel(model);});

        }
    }
}