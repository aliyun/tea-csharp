using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Darabonba;
using Darabonba.Exceptions;
using Xunit;

namespace DaraUnitTests.Exceptions
{
    public class DaraRetryableExceptionTest
    {
        [Fact]
        public void TestDaraRetryableException()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            httpResponseMessage.StatusCode = HttpStatusCode.OK;
            httpResponseMessage.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes("test")));
            Response response = new Response(httpResponseMessage);
            DaraRetryableException daraRetryableException = new DaraRetryableException();
            Assert.NotNull(daraRetryableException);
        }
    }
}
