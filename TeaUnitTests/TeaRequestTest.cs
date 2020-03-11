using Tea;

using Xunit;

namespace TeaUnitTests
{
    public class TeaRequestTest
    {
        [Fact]
        public void TestTeaRequest()
        {
            TeaRequest teaRequest = new TeaRequest();
            Assert.NotNull(teaRequest);
            Assert.NotNull(teaRequest.Headers);
            Assert.NotNull(teaRequest.Query);
            Assert.Equal("http", teaRequest.Protocol);
            teaRequest.Headers = null;
            Assert.NotNull(teaRequest.Headers);
            Assert.Equal("GET", teaRequest.Method);

            teaRequest.Method = "POST";
            Assert.Equal("POST", teaRequest.Method);

            teaRequest.Query = null;
            Assert.NotNull(teaRequest.Query);
        }
    }
}
