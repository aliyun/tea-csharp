using Darabonba;
using Darabonba.Utils;
using Xunit;

namespace DaraUnitTests.Utils
{
    public class UtilityBranchCoverageTest
    {
        [Theory]
        [InlineData("file:///tmp/report.txt", "", "", "")]
        [InlineData("https://example.com/path", "443", "", "")]
        [InlineData("https://example.com:8443/path?x=1#part", "8443", "x=1", "part")]
        public void UrlReturnsOptionalComponentsWithoutDelimiters(string value, string port, string query, string fragment)
        {
            var url = URL.Parse(value);

            Assert.Equal(port, url.Port());
            Assert.Equal(query, url.Search());
            Assert.Equal(fragment, url.Hash());
        }

        [Fact]
        public void UrlEncodingTreatsNullAsEmpty()
        {
            Assert.Equal(string.Empty, URL.UrlEncode(null));
        }

        [Theory]
        [InlineData("utf32")]
        [InlineData("utf-32")]
        [InlineData("UTF-32")]
        public void Utf32AliasesEncodeUnicodeAsLittleEndian(string encoding)
        {
            Assert.Equal(new byte[] { 65, 0, 0, 0, 45, 78, 0, 0 }, BytesUtils.From("A中", encoding));
        }

        [Theory]
        [InlineData(-2, -1, -2, -1)]
        [InlineData(-1, -2, -2, -1)]
        [InlineData(-2, -2, -2, -2)]
        public void MinAndMaxHandleEitherOrderAndEqualValues(int first, int second, int minimum, int maximum)
        {
            Assert.Equal(minimum, MathUtils.Min(first, second));
            Assert.Equal(maximum, MathUtils.Max(first, second));
        }
    }
}
