using System.Collections.Generic;

using Tea;
using Tea.Utils;

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
            Assert.Null(teaException.DataResult);

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
            Assert.NotNull(teaException.DataResult);

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
            Assert.NotNull(teaException.DataResult);

            teaException = new TeaException(new Dictionary<string, string>
            { { "code", "200" }
            });
            Assert.NotNull(teaException);
            Assert.Equal("200", teaException.Code);

            teaException = new TeaException(new Dictionary<string, object>
            { { "code", "code" },
                { "message", "message" },
                { "description", "description" },
                {
                    "data",
                    new Dictionary<string, object>
                    { { "test", "test" },
                        {"statusCode", 200}
                    }
                },
                {
                    "accessDeniedDetail",
                    new Dictionary<string, object>
                    { { "NoPermissionType", "ImplicitDeny" }
                    }
                }
            });
            Assert.NotNull(teaException);
            Assert.Equal("code", teaException.Code);
            Assert.Equal("message", teaException.Message);
            Assert.Equal("description", teaException.Description);
            Assert.Equal(200, teaException.StatusCode);
            Assert.Equal("ImplicitDeny", DictUtils.GetDicValue(teaException.AccessDeniedDetail, "NoPermissionType"));

            teaException = new TeaException(new Dictionary<string, object>
            { { "code", "code" },
                {
                    "accessDeniedDetail", null
                }
            });
            Assert.NotNull(teaException);
            Assert.Null(teaException.AccessDeniedDetail);

            teaException = new TeaException(new Dictionary<string, object>
            { { "code", "code" },
                {
                    "accessDeniedDetail", "error type"
                }
            });
            Assert.NotNull(teaException);
            Assert.Null(teaException.AccessDeniedDetail);
        }
    }
}
