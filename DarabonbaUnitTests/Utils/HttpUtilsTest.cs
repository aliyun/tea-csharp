using System.Net.Http;

using Darabonba.Utils;

using Xunit;

namespace DaraUnitTests.Utils
{
    public class HttpUtilsTest
    {
        [Fact]
        public void Test_GetHttpMethod()
        {
            Assert.Equal(HttpMethod.Get, HttpUtils.GetHttpMethod("GET"));
            Assert.Equal(HttpMethod.Post, HttpUtils.GetHttpMethod("post"));
            Assert.Equal(HttpMethod.Delete, HttpUtils.GetHttpMethod("delete"));
            Assert.Equal(HttpMethod.Put, HttpUtils.GetHttpMethod("put"));
            Assert.Equal(HttpMethod.Head, HttpUtils.GetHttpMethod("head"));
            Assert.Equal(HttpMethod.Options, HttpUtils.GetHttpMethod("options"));
            Assert.Equal(HttpMethod.Trace, HttpUtils.GetHttpMethod("trace"));
            Assert.Equal(HttpMethod.Get, HttpUtils.GetHttpMethod("test"));
        }
    }
}
