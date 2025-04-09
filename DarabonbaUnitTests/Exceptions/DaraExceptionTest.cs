using System;
using System.Collections;
using System.Collections.Generic;
using Darabonba.Exceptions;
using Tea;
using Darabonba.Utils;
using Xunit;
using Xunit.Abstractions;

namespace DaraUnitTests
{
    public class DaraExceptionTest
    {
        [Fact]
        public void TestDaraException_new()
        {
            var daraException = new DaraException
            {
                Message = "message",
                Code = "200",
                Data = new Dictionary<string, object>
                {
                    { "test", "test" }
                }
            };
            Assert.NotNull(daraException);
            Assert.Equal("200", daraException.Code);
            Assert.Equal("message", daraException.Message);
            Assert.NotNull(daraException.DataResult);
            Assert.Null(daraException.AccessDeniedDetail);

            daraException = new DaraException
            {
                Message = "message",
                Code = "200",
                AccessDeniedDetail = new Dictionary<string, object>
                {
                    { "NoPermissionType", "ImplicitDeny" }
                }
            };
            Assert.NotNull(daraException);
            Assert.Equal("200", daraException.Code);
            Assert.Equal("message", daraException.Message);
            Assert.Null(daraException.DataResult);
            Assert.NotNull(daraException.AccessDeniedDetail);
            Assert.Equal("ImplicitDeny", DictUtils.GetDicValue(daraException.AccessDeniedDetail, "NoPermissionType"));
        }

        [Fact]
        public void TestDaraException_compatible()
        {
            var daraException = new DaraException(new Dictionary<string, object>
            {
                { "code", "200" },
                { "message", "message" },
                { "data", null }
            });

            Assert.NotNull(daraException);
            Assert.Equal("200", daraException.Code);
            Assert.Equal("message", daraException.Message);
            Assert.Null(daraException.DataResult);

            daraException = new DaraException(new Dictionary<string, object>
            {
                { "code", "200" },
                { "message", "message" },
                {
                    "data",
                    new Dictionary<string, string>
                    {
                        { "test", "test" }
                    }
                }
            });
            Assert.NotNull(daraException);
            Assert.NotNull(daraException.DataResult);
            Assert.Equal("test", daraException.Data["test"]);

            daraException = new DaraException(new Dictionary<string, object>
            {
                { "code", "200" },
                { "message", "message" },
                {
                    "data",
                    new
                    {
                        test = "test"
                    }
                }
            });
            Assert.NotNull(daraException);
            Assert.NotNull(daraException.DataResult);

            daraException = new DaraException(new Dictionary<string, string>
            {
                { "code", "200" }
            });
            Assert.NotNull(daraException);
            Assert.Equal("200", daraException.Code);

            daraException = new DaraException(new Dictionary<string, object>
            {
                { "code", "code" },
                { "message", "message" },
                { "description", "description" },
                {
                    "data",
                    new Dictionary<string, object>
                    {
                        { "test", "test" },
                        { "statusCode", 200 }
                    }
                },
                {
                    "accessDeniedDetail",
                    new Dictionary<string, object>
                    {
                        { "NoPermissionType", "ImplicitDeny" }
                    }
                }
            });
            Assert.NotNull(daraException);
            Assert.Equal("code", daraException.Code);
            Assert.Equal("message", daraException.Message);
            Assert.Equal("description", daraException.Description);
            Assert.Equal(200, daraException.StatusCode);
            Assert.Equal("test", daraException.DataResult["test"]);
            Assert.Equal("ImplicitDeny", DictUtils.GetDicValue(daraException.AccessDeniedDetail, "NoPermissionType"));

            daraException = new DaraException(new Dictionary<string, object>
            {
                { "code", "code" },
                {
                    "accessDeniedDetail", null
                }
            });
            Assert.NotNull(daraException);
            Assert.Null(daraException.AccessDeniedDetail);

            daraException = new DaraException(new Dictionary<string, object>
            {
                { "code", "code" },
                {
                    "accessDeniedDetail", "error type"
                }
            });
            Assert.NotNull(daraException);
            Assert.Null(daraException.AccessDeniedDetail);
        }

        [Fact]
        public void Test_Throw_And_Catch()
        {
            try
            {
                throw new DaraException
                {
                    Message = "msg",
                    Code = "200",
                    AccessDeniedDetail = new Dictionary<string, object>
                    {
                        { "NoPermissionType", "ImplicitDeny" }
                    },
                    Data = new Dictionary<string, object>
                    {
                        { "test", "test" }
                    }
                };
            }
            catch (TeaException e)
            {
                Assert.Equal("msg", e.Message);
                Assert.Equal("200", e.Code);
                Assert.Equal("ImplicitDeny", DictUtils.GetDicValue(e.AccessDeniedDetail, "NoPermissionType"));
                Assert.Equal("test", DictUtils.GetDicValue(e.DataResult, "test"));
                Assert.Equal("test", e.DataResult["test"]);
                Assert.Equal(0, e.StatusCode);
            }

            try
            {
                throw new TestException
                {
                    TestCode = 123,
                    Code = "400"
                };
            }
            catch (TestException e)
            {
                Assert.Null(e.Message);
                Assert.Equal("400", e.Code);
                Assert.Null(e.AccessDeniedDetail);
                Assert.Null(e.DataResult);
                Assert.Equal(123, e.TestCode);
            }
            
            try
            {
                throw new TestException
                {
                    Message = "message",
                    Code = "400",
                    Data = new Dictionary<string, object>
                    {
                        { "test", "test" }
                    }
                };
            }
            catch (DaraException e)
            {
                Assert.Equal("message", e.Message);
                Assert.Equal("400", e.Code);
                Assert.Null(e.AccessDeniedDetail);
                Assert.Equal("test", e.Data["test"]);
            }
        }
    }
}

internal class TestException : DaraException
{
    public int TestCode { get; set; }
}