using System;

using Tea;

using Xunit;

namespace TeaUnitTests
{
    public class TeaUnretryableExceptionTest
    {
        [Fact]
        public void TestTeaUnretryableException()
        {
            TeaUnretryableException teaUnretryableExceptionEmpty = new TeaUnretryableException();
            Assert.NotNull(teaUnretryableExceptionEmpty);

            TeaUnretryableException teaUnretryableException = new TeaUnretryableException(new TeaRequest(), new Exception("Exception"));
            Assert.NotNull(teaUnretryableException);
            Assert.Equal(" Retry failed : Exception", teaUnretryableException.Message);
            Assert.NotNull(teaUnretryableException.LastRequest);
        }
    }
}
