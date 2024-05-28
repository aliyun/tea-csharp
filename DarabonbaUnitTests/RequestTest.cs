using Darabonba;

using Xunit;

namespace DaraUnitTests
{
    public class RequestTest
    {
        [Fact]
        public void TestDaraRequest()
        {
            Request request = new Request();
            Assert.NotNull(request);
            Assert.NotNull(request.Headers);
            Assert.NotNull(request.Query);
            Assert.Equal("http", request.Protocol);
            request.Headers = null;
            Assert.NotNull(request.Headers);
            Assert.Equal("GET", request.Method);

            request.Method = "POST";
            Assert.Equal("POST", request.Method);

            request.Query = null;
            Assert.NotNull(request.Query);
        }
    }
}
