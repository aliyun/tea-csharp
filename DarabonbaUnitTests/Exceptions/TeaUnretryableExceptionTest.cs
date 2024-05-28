using System;
using Darabonba;
using Darabonba.Exceptions;
using Darabonba.RetryPolicy;
using Tea;
using Xunit;
using Xunit.Abstractions;

namespace DaraUnitTests.Exceptions
{
    public class TeaUnretryableExceptionTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TeaUnretryableExceptionTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void TestDaraUnRetryableException()
        {
            var retryPolicyContext = new RetryPolicyContext
            {
                Request = new Request(),
                Exception = new DaraException
                {
                    Message = "Exception"
                }
            };
            try
            {
                throw new DaraUnRetryableException(retryPolicyContext);
            }
            catch (TeaUnretryableException e)
            {
                Assert.NotNull(e);
                Assert.NotNull(e.InnerException);
                Assert.Equal(" Retry failed : Exception", e.Message);
                Assert.NotNull(e.LastRequest);
                Assert.True(e.LastRequest != null);
            }

            try
            {
                throw new TestUnRetryableException
                {
                    TestCode = "200"
                };
            }
            catch (TestUnRetryableException e)
            {
                Assert.NotNull(e);
                Assert.Null(e.InnerException);
                Assert.Equal("200", e.TestCode);
            }

            try
            {
                throw new DaraRetryableException();
            }
            catch (DaraRetryableException e)
            {
                Assert.NotNull(e);
                Assert.Equal("Exception of type 'Darabonba.Exceptions.DaraRetryableException' was thrown.", e.Message);
            }
        }
    }
}

internal class TestUnRetryableException : DaraUnRetryableException
{
    public string TestCode { get; set; }
}