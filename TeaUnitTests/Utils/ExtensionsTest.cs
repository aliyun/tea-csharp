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
    }
}
