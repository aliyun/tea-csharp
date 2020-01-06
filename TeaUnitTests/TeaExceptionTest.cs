using System.Collections.Generic;

using Tea;

using Xunit;

namespace TeaUnitTests
{
    public class TeaExceptionTest
    {
        [Fact]
        public void TestTeaException()
        {
            TeaException teaException = new TeaException(new Dictionary<string, object>
            { { "code", "200" },
                { "message", "message" },
                { "data", null }
            });

            Assert.NotNull(teaException);
            Assert.Equal("200", teaException.Code);
            Assert.Equal("message", teaException.Message);
            Assert.Null(teaException.Data);

            teaException = new TeaException(new Dictionary<string, object>
            { { "code", "200" },
                { "message", "message" },
                {
                    "data",
                    new Dictionary<string, string>
                    { { "test", "test" }
                    }
                }
            });
            Assert.NotNull(teaException);
            Assert.NotNull(teaException.Data);

            teaException = new TeaException(new Dictionary<string, object>
            { { "code", "200" },
                { "message", "message" },
                {
                    "data",
                    new
                    {
                        test = "test"
                    }
                }
            });
            Assert.NotNull(teaException);
            Assert.NotNull(teaException.Data);

            teaException = new TeaException(new Dictionary<string, string>
            { { "code", "200" }
            });
            Assert.NotNull(teaException);
            Assert.Equal("200", teaException.Code);
        }
    }
}
