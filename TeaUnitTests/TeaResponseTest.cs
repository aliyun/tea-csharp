using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

using Tea;

using Xunit;

namespace TeaUnitTests
{
    public class TeaResponseTest
    {
        [Fact]
        public void TestTeaResponse()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            httpResponseMessage.StatusCode = HttpStatusCode.OK;
            httpResponseMessage.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes("test")));
            TeaResponse response = new TeaResponse(httpResponseMessage);
            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("", response.StatusMessage);

            TeaResponse teaResponseNull = new TeaResponse(null);
            Assert.Null(teaResponseNull.Body);
        }
    }
}
