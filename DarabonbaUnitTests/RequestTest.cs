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

            request.Protocol = "https";
            Assert.Equal("https", request.Protocol);

            var map = request.ToMap();
            Assert.Equal("https", map["protocol"]);
            Assert.Equal("POST", map["method"]);
            Assert.False(map.ContainsKey("body"));
            var copy = Request.FromMap(map);
            Assert.Equal(request.Protocol, copy.Protocol);
            Assert.Equal(request.Method, copy.Method);
            Assert.Empty(copy.Query);
            Assert.NotNull(Request.FromMap(new System.Collections.Generic.Dictionary<string, object>()));
        }
    }
}
