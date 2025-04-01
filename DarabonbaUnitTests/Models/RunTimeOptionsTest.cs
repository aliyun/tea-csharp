using System.Collections.Generic;
using Tea.Utils;
using Xunit;

namespace DaraUnitTests.Models
{
    public class RunTimeOptionsTest
    {
        [Fact]
        public void Test_RunTimeOptions()
        {
            var run = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 1000,
                ExtendsParameters = new AlibabaCloud.TeaUtil.Models.ExtendsParameters
                {
                    Headers = new Dictionary<string, string> { { "test", "test" } }
                }
            };
            var runtimeOptions = Test(run);
            Assert.Equal("test", runtimeOptions.ExtendsParameters.Headers.Get("test"));
            Assert.Equal(1000, runtimeOptions.ReadTimeout);
            Assert.Null(runtimeOptions.RetryOptions);
        }

        private static Darabonba.Models.RuntimeOptions Test(Darabonba.Models.RuntimeOptions runtimeOptions)
        {
            return runtimeOptions;
        }
    }
}