using System.Text;
using Xunit;
using Darabonba.Utils;

namespace DaraUnitTests.Utils
{
    public class DaraBytesUtilTest
    {

        [Fact]
        public void Test_HexEncode()
        {
            byte[] test = Encoding.UTF8.GetBytes("test");
            var res =  BytesUtils.ToHex(test);
            Assert.Equal("74657374", res);
        }

        [Fact]
        public void Test_From()
        {
            string data = "test";
            Assert.Equal(new byte[] { 116, 101, 115, 116 }, BytesUtils.From(data, "utf8"));
            Assert.Equal(new byte[] { 116, 101, 115, 116 }, BytesUtils.From(data, "ASCII"));
            Assert.Equal(new byte[] { 0, 116, 0, 101, 0, 115, 0, 116 }, BytesUtils.From(data, "bigendianunicode"));
            Assert.Equal(new byte[] { 116, 0, 101, 0, 115, 0, 116, 0 }, BytesUtils.From(data, "unicode"));
            Assert.Equal(new byte[] { 116, 0, 0, 0, 101, 0, 0, 0, 115, 0, 0, 0, 116, 0, 0, 0 }, BytesUtils.From(data, "utf32"));
        }
    }
}