using System.Collections.Generic;
using Tea.Utils;

using Xunit;

namespace TeaUnitTests.Utils
{
    public class ExtensionsTest
    {
        [Fact]
        public void TestToSafeString()
        {
            string strNull = null;
            Assert.Null(strNull.ToSafeString());
            Assert.Empty(strNull.ToSafeString(string.Empty));

            string strTest = "test";
            Assert.Equal("test", strTest.ToSafeString());
            Assert.Equal("test", strTest.ToSafeString(string.Empty));
        }

        [Fact]
        public void TestToSafeInt()
        {
            string intNull = null;
            Assert.Null(intNull.ToSafeInt());
            Assert.Equal(0, intNull.ToSafeInt(0));

            string intTest = "99";
            Assert.Equal(99, intTest.ToSafeInt());
            Assert.Equal(99, intTest.ToSafeInt(0));

            string intErr = "str";
            Assert.Null(intErr.ToSafeInt());
            Assert.Equal(0, intErr.ToSafeInt(0));
        }

        [Fact]
        public void TestToSafeBool()
        {
            string boolNull = null;
            Assert.False(boolNull.ToSafeBool());

            string boolTest = "true";
            Assert.True(boolTest.ToSafeBool());
            Assert.True(boolTest.ToSafeBool(false));

            string boolErr = "str";
            Assert.False(boolErr.ToSafeBool());
            Assert.True(boolErr.ToSafeBool(true));
        }

        [Fact]
        public void TestToSafeDouble()
        {
            string doubleNull = null;
            Assert.Null(doubleNull.ToSafeDouble());
            Assert.Equal(0, doubleNull.ToSafeDouble(0));

            string doubleTest = "99.99";
            Assert.Equal(99.99, doubleTest.ToSafeDouble());
            Assert.Equal(99.99, doubleTest.ToSafeDouble(0));

            string doubleErr = "str";
            Assert.Null(doubleErr.ToSafeDouble());
            Assert.Equal(0, doubleErr.ToSafeDouble(0));
        }

        [Fact]
        public void TestToSafeFloat()
        {
            string floatNull = null;
            Assert.Null(floatNull.ToSafeFloat());
            Assert.Equal(0F, floatNull.ToSafeFloat(0));

            string floatTest = "99.99";
            Assert.Equal(99.99F, floatTest.ToSafeFloat());
            Assert.Equal(99.99F, floatTest.ToSafeFloat(0));

            string floatErr = "str";
            Assert.Null(floatErr.ToSafeFloat());
            Assert.Equal(0F, floatErr.ToSafeFloat(0));
        }

        [Fact]
        public void TestDicGet()
        {
            Dictionary<string, string> dicStr = new Dictionary<string, string>();
            dicStr["test"] = "test";
            dicStr["key"] = "value";
            Assert.Null(dicStr.Get("testNull"));
            Assert.Equal("test","test");

            Dictionary<string, object> dicObj = new Dictionary<string, object>();
            dicObj["test"] = 1;
            Assert.Equal(1,dicObj.Get("test"));
        }
    }
}
