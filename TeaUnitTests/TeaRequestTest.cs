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
        }
    }
}
