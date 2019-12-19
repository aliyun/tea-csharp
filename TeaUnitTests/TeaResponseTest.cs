using System.Net;

using Moq;

using Tea;

using Xunit;

namespace TeaUnitTests
{
    public class TeaResponseTest
    {
        [Fact]
        public void TestTeaResponse()
        {
            Mock<HttpWebResponse> mockHttpWebResponse = new Mock<HttpWebResponse>();
            mockHttpWebResponse.Setup(p => p.StatusCode).Returns(HttpStatusCode.OK);
            mockHttpWebResponse.Setup(p => p.StatusDescription).Returns("StatusDescription");
            mockHttpWebResponse.Setup(p => p.Headers).Returns(new WebHeaderCollection());
            TeaResponse teaResponse = new TeaResponse(mockHttpWebResponse.Object);
            Assert.NotNull(teaResponse);
            Assert.Equal(200, teaResponse.StatusCode);
            Assert.Equal("StatusDescription", teaResponse.StatusMessage);
            Assert.Empty(teaResponse.Headers);

            TeaResponse teaResponseNull = new TeaResponse(null);
            Assert.Null(teaResponseNull.Body);
        }
    }
}
