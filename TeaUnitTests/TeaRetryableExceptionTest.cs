using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

using Tea;

using Xunit;

namespace TeaUnitTests
{
    public class TeaRetryableExceptionTest
    {
        [Fact]
        public void TestTeaRetryableException()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            httpResponseMessage.StatusCode = HttpStatusCode.OK;
            httpResponseMessage.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes("test")));
            TeaResponse response = new TeaResponse(httpResponseMessage);
            TeaRetryableException teaRetryableException = new TeaRetryableException(new TeaRequest(), response);
            Assert.NotNull(teaRetryableException);
            Assert.NotNull(teaRetryableException.Request);
            Assert.NotNull(teaRetryableException.Response);
        }
    }
}
