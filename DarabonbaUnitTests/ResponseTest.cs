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

            Assert.NotNull(response.Body);
            var map = response.ToMap();
            Assert.Equal(200, map["statusCode"]);
            Assert.False(map.ContainsKey("body"));
            Assert.Equal(response.StatusCode, Response.FromMap(map).StatusCode);
            var input = new System.Collections.Generic.Dictionary<string, object> { { "statusCode", 201L }, { "body", response.Body } };
            var copy = Response.FromMap(input);
            Assert.Equal(201, copy.StatusCode);
            Assert.Equal("test", new StreamReader(copy.Body).ReadToEnd());
            Assert.NotNull(Response.FromMap(new System.Collections.Generic.Dictionary<string, object>()));
        }
    }
}
