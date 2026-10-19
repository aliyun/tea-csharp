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
            httpResponseMessage.Content.Headers.TryAddWithoutValidation("Content-Type", "text/xml;charset=utf-8");
            Response response = new Response(httpResponseMessage);
            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("", response.StatusMessage);
            Assert.Equal(httpResponseMessage.Content.Headers.ContentType.ToString(), response.Headers["content-type"]);

            Response responseNull = new Response(null);
            Assert.Null(responseNull.Body);

            Assert.NotNull(response.Body);
            Assert.Empty(response.ToMap());
            Assert.Empty(response.ToMap(true));
            Assert.NotNull(Response.FromMap(new System.Collections.Generic.Dictionary<string, object>()));
        }
    }
}
