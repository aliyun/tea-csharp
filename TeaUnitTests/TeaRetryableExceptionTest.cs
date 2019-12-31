using System.IO;
using System.Net;
using System.Text;

using Moq;

using Tea;

using Xunit;

namespace TeaUnitTests
{
    public class TeaRetryableExceptionTest
    {
        [Fact]
        public void TestTeaRetryableException()
        {
            Mock<HttpWebResponse> mock = new Mock<HttpWebResponse>();
            mock.Setup(p => p.StatusCode).Returns(HttpStatusCode.OK);
            mock.Setup(p => p.Headers).Returns(new WebHeaderCollection());
            mock.Setup(p => p.GetResponseStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("test")));
            TeaResponse response = new TeaResponse(mock.Object);
            TeaRetryableException teaRetryableException = new TeaRetryableException(new TeaRequest(), response);
            Assert.NotNull(teaRetryableException);
            Assert.NotNull(teaRetryableException.Request);
            Assert.NotNull(teaRetryableException.Response);
        }
    }
}
