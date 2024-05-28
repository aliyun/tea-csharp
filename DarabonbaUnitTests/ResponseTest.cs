using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

using Darabonba;

using Xunit;

namespace DaraUnitTests
{
    public class ResponseTest
    {
        [Fact]
        public void TestDaraResponse()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes("test")))
            };
            Response response = new Response(httpResponseMessage);
            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("", response.StatusMessage);

            Response responseNull = new Response(null);
            Assert.Null(responseNull.Body);
        }
    }
}
