using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Darabonba;
using Darabonba.RetryPolicy;
using Darabonba.Exceptions;
using Darabonba.Utils;
using DaraUnitTests.Utils;
using Xunit;

namespace DaraUnitTests
{
    public class CoreTest
    {
        [Fact]
        public void TestComposeUrl()
        {
            Request request = new Request();

            var url = Core.ComposeUrl(request);
            Assert.Equal("http://", url);

            request.Headers["host"] = "fake.domain.com";
            url = Core.ComposeUrl(request);
            Assert.Equal("http://fake.domain.com", url);

            request.Port = 8080;
            url = Core.ComposeUrl(request);
            Assert.Equal("http://fake.domain.com:8080", url);

            request.Pathname = "/index.html";
            url = Core.ComposeUrl(request);
            Assert.Equal("http://fake.domain.com:8080/index.html", url);

            request.Query["foo"] = "";
            url = Core.ComposeUrl(request);
            Assert.Equal("http://fake.domain.com:8080/index.html?foo=", url);

            request.Query["foo"] = "bar";
            url = Core.ComposeUrl(request);
            Assert.Equal("http://fake.domain.com:8080/index.html?foo=bar", url);

            request.Pathname = "/index.html?a=b";
            url = Core.ComposeUrl(request);
            Assert.Equal("http://fake.domain.com:8080/index.html?a=b&foo=bar", url);

            request.Pathname = "/index.html?a=b&";
            url = Core.ComposeUrl(request);
            Assert.Equal("http://fake.domain.com:8080/index.html?a=b&foo=bar", url);

            request.Query["fake"] = null;
            url = Core.ComposeUrl(request);
            Assert.Equal("http://fake.domain.com:8080/index.html?a=b&foo=bar", url);

            request.Query["fake"] = "val*";
            url = Core.ComposeUrl(request);
            Assert.Equal("http://fake.domain.com:8080/index.html?a=b&foo=bar&fake=val%2A", url);
        }

        [Fact]
        public void TestDoAction()
        {
            Request request = new Request
            {
                Protocol = "http",
                Method = "GET",
                Headers = new Dictionary<string, string>()
            };
            request.Headers["host"] = "www.alibabacloud.com";
            request.Pathname = "/s/zh";
            request.Query = new Dictionary<string, string>
            {
                { "k", "ecs" }
            };
            Dictionary<string, object> runtime = new Dictionary<string, object>
            {
                { "readTimeout", 7000 },
                { "connectTimeout", 7000 },
                // result in failure on .NET framework 45
                // { "httpsProxy", "http://www.alibabacloud.com/s/zh?k=ecs" },
                { "ignoreSSL", true }
            };

            Response response = Core.DoAction(request, runtime);
            Assert.NotNull(response);

            request.Protocol = "https";
            response = Core.DoAction(request);
            Assert.NotNull(response);


            string bodyStr = Core.GetResponseBody(response);
            Assert.NotNull(bodyStr);

            request.Method = "POST";
            request.Body = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            request.Headers["content-test"] = "test";
            response = Core.DoAction(request, runtime);
            Assert.NotNull(response);

            Request request404 = new Request
            {
                Protocol = "https",
                Method = "GET",
                Headers = new Dictionary<string, string>()
            };
            request404.Headers["host"] = "www.alibabacloud404.com";
            request404.Pathname = "/s/zh";
            request404.Query = new Dictionary<string, string>
            {
                { "k", "ecs" }
            };
            Dictionary<string, object> runtime404 = new Dictionary<string, object>
            {
                { "readTimeout", 7000 },
                { "connectTimeout", 7000 }
            };
            Assert.Throws<AggregateException>(() => { Core.DoAction(request404, runtime404); });

            Request requestProxy = new Request();


            Request requestException = new Request
            {
                Protocol = "http",
                Method = "GET",
                Pathname = "/test"
            };
            Dictionary<string, object> runtimeException = new Dictionary<string, object>();
            requestException.Headers["host"] = "www.aliyun.com";
            Response responseException = Core.DoAction(requestException, runtimeException);
            Assert.NotNull(responseException);
        }

        [Fact]
        public async Task TestDoActionAsync()
        {
            Request request = new Request
            {
                Protocol = "https",
                Method = "GET",
                Headers = new Dictionary<string, string>()
            };
            request.Headers["host"] = "www.alibabacloud.com";
            request.Pathname = "/s/zh";
            request.Query = new Dictionary<string, string>
            {
                { "k", "ecs" }
            };

            Response response = await Core.DoActionAsync(request);
            Assert.NotNull(response);

            Dictionary<string, object> runtime = new Dictionary<string, object>
            {
                { "readTimeout", 4000 },
                { "connectTimeout", 0 }
            };

            response = await Core.DoActionAsync(request, runtime);
            Assert.NotNull(response);

            string bodyStr = Core.GetResponseBody(response);
            Assert.NotNull(bodyStr);

            request.Method = "POST";
            request.Body = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            response = await Core.DoActionAsync(request, runtime);
            Assert.NotNull(response);

            Request request404 = new Request
            {
                Protocol = "https",
                Method = "GET",
                Headers = new Dictionary<string, string>()
            };
            request404.Headers["host"] = "www.alibabacloud404.com";
            request404.Pathname = "/s/zh";
            request404.Query = new Dictionary<string, string>
            {
                { "k", "ecs" }
            };
            Dictionary<string, object> runtime404 = new Dictionary<string, object>
            {
                { "readTimeout", 7000 },
                { "connectTimeout", 7000 }
            };
            await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await Core.DoActionAsync(request404, runtime);
            });

            Request requestException = new Request
            {
                Protocol = "http",
                Method = "GET",
                Pathname = "/test"
            };
            Dictionary<string, object> runtimeException = new Dictionary<string, object>();
            requestException.Headers["host"] = "www.aliyun.com";
            Response responseException = await Core.DoActionAsync(requestException, runtimeException);
        }

        [Fact]
        public void TestConvertHeaders()
        {
            WebHeaderCollection headers = new WebHeaderCollection
            {
                { "testKey", "testValue" }
            };
            Dictionary<string, string> dic = Core.ConvertHeaders(headers);
            Assert.NotNull(dic);
            Assert.True(dic.ContainsKey("testkey"));
            Assert.Equal("testValue", dic["testkey"]);
        }

        [Fact]
        public void TestAllowRetry()
        {
            long _now = DateTime.Now.Millisecond;

            Assert.True(Core.AllowRetry(null, 0, _now));

            Assert.False(Core.AllowRetry(null, 3, _now));

            Dictionary<string, object> dic = new Dictionary<string, object>();
            Assert.False(Core.AllowRetry(dic, 3, _now));

            dic.Add("retryable", true);
            dic.Add("maxAttempts", null);
            Assert.False(Core.AllowRetry(dic, 3, _now));

            dic["maxAttempts"] = 5;
            Assert.True(Core.AllowRetry(dic, 3, _now));

            Dictionary<string, int?> dicInt = new Dictionary<string, int?>();
            Assert.False(Core.AllowRetry(dicInt, 3, _now));
        }

        public class AException : DaraException
        {
            public AException() : base()
            {
            }

            public AException(IDictionary dict) : base(dict)
            {
            }
        }

        public class BException : DaraException
        {
            public BException() : base()
            {
            }

            public BException(IDictionary dict) : base(dict)
            {
            }
        }

        public class CException : DaraException
        {
            public CException() : base()
            {
            }

            public CException(IDictionary dict) : base(dict)
            {
            }
        }

        [Fact]
        public void TestShouldRetry()
        {
            var backoffPolicy = new ExponentialBackoffPolicy(2, 60 * 1000);

            var retryCondition1 = new RetryCondition
            {
                MaxAttempts = 1,
                Backoff = backoffPolicy,
                Exception = new List<string> { "AException" },
                ErrorCode = new List<string> { "BExceptionCode" }
            };

            var retryCondition2 = new RetryCondition
            {
                MaxAttempts = 2,
                Backoff = backoffPolicy,
                Exception = new List<string> { "AException", "CException" }
            };

            var retryCondition3 = new RetryCondition
            {
                Exception = new List<string> { "BException" }
            };

            var retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 0,
                Exception = new AException
                {
                    Message = "AException",
                    Code = "AExceptionCode"
                }
            };

            Assert.True(Core.ShouldRetry(null, retryPolicyContext));

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 1,
                Exception = new AException
                {
                    Message = "AException",
                    Code = "AExceptionCode"
                }
            };

            Assert.False(Core.ShouldRetry(null, retryPolicyContext));

            var retryOptions = new RetryOptions
            {
                Retryable = false,
                RetryCondition = null,
                NoRetryCondition = null
            };

            Assert.False(Core.ShouldRetry(retryOptions, retryPolicyContext));

            retryOptions = new RetryOptions
            {
                Retryable = true,
                RetryCondition = null,
                NoRetryCondition = null
            };

            Assert.False(Core.ShouldRetry(retryOptions, retryPolicyContext));

            retryOptions = new RetryOptions
            {
                Retryable = true,
                RetryCondition = new List<RetryCondition> { retryCondition1, retryCondition2 },
                NoRetryCondition = new List<RetryCondition> { retryCondition3 }
            };

            Assert.True(Core.ShouldRetry(retryOptions, retryPolicyContext));

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 2,
                Exception = new AException
                {
                    Message = "AException",
                    Code = "AExceptionCode"
                }
            };

            Assert.False(Core.ShouldRetry(retryOptions, retryPolicyContext));

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 0,
                Exception = new BException
                {
                    Message = "BException",
                    Code = "BExceptionCode"
                }
            };

            Assert.True(Core.ShouldRetry(retryOptions, retryPolicyContext));

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 1,
                Exception = new BException
                {
                    Message = "BException",
                    Code = "BExceptionCode"
                }
            };

            Assert.False(Core.ShouldRetry(retryOptions, retryPolicyContext));

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 1,
                Exception = new CException
                {
                    Message = "CException",
                    Code = "CExceptionCode"
                }
            };

            Assert.True(Core.ShouldRetry(retryOptions, retryPolicyContext));

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 2,
                Exception = new CException
                {
                    Message = "CException",
                    Code = "CExceptionCode"
                }
            };

            Assert.True(Core.ShouldRetry(retryOptions, retryPolicyContext));

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 3,
                Exception = new CException
                {
                    Message = "CException",
                    Code = "CExceptionCode"
                }
            };

            Assert.False(Core.ShouldRetry(retryOptions, retryPolicyContext));
        }

        [Fact]
        public void TestGetBackoffTime()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            Assert.Equal(0, Core.GetBackoffTime(dic, 1));

            dic.Add("policy", null);
            Assert.Equal(0, Core.GetBackoffTime(dic, 1));

            dic["policy"] = string.Empty;
            Assert.Equal(0, Core.GetBackoffTime(dic, 1));

            dic["policy"] = "no";
            Assert.Equal(0, Core.GetBackoffTime(dic, 1));

            dic["policy"] = "yes";
            Assert.Equal(0, Core.GetBackoffTime(dic, 1));

            dic.Add("period", null);
            Assert.Equal(0, Core.GetBackoffTime(dic, 1));

            dic["period"] = -1;
            Assert.Equal(1, Core.GetBackoffTime(dic, 1));

            dic["period"] = 1000;
            Assert.Equal(1000, Core.GetBackoffTime(dic, 1));
        }

        [Fact]
        public void TestGetBackoffDelay()
        {
            BackoffPolicy backoffPolicy = new ExponentialBackoffPolicy(200, 60 * 1000);

            RetryCondition retryCondition1 = new RetryCondition
            {
                MaxAttempts = 1,
                Backoff = backoffPolicy,
                Exception = new List<string> { "AException" },
                ErrorCode = new List<string> { "BExceptionCode" }
            };

            RetryCondition retryCondition2 = new RetryCondition
            {
                MaxAttempts = 2,
                Backoff = backoffPolicy,
                Exception = new List<string> { "AException", "CException" }
            };

            RetryCondition retryCondition3 = new RetryCondition
            {
                Exception = new List<string> { "BException" }
            };

            RetryPolicyContext retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 0,
                Exception = new AException
                {
                    Message = "AException",
                    Code = "AExceptionCode"
                }
            };

            Assert.Equal(0, Core.GetBackoffDelay(null, retryPolicyContext));

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 1,
                Exception = new AException
                {
                    Message = "AException",
                    Code = "AExceptionCode"
                }
            };

            Assert.Equal(100, Core.GetBackoffDelay(null, retryPolicyContext));

            RetryOptions retryOptions = new RetryOptions
            {
                Retryable = false,
                RetryCondition = null,
                NoRetryCondition = null
            };

            Assert.Equal(100, Core.GetBackoffDelay(retryOptions, retryPolicyContext));

            retryOptions = new RetryOptions
            {
                Retryable = true,
                RetryCondition = new List<RetryCondition> { retryCondition1, retryCondition2 },
                NoRetryCondition = new List<RetryCondition> { retryCondition3 }
            };

            Assert.Equal(400, Core.GetBackoffDelay(retryOptions, retryPolicyContext));

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 2,
                Exception = new AException
                {
                    Message = "AException",
                    Code = "AExceptionCode"
                }
            };

            Assert.Equal(800, Core.GetBackoffDelay(retryOptions, retryPolicyContext));

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 1,
                Exception = new BException
                {
                    Message = "BException",
                    Code = "BExceptionCode"
                }
            };

            Assert.Equal(400, Core.GetBackoffDelay(retryOptions, retryPolicyContext));


            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 1,
                Exception = new CException
                {
                    Message = "CException",
                    Code = "CExceptionCode"
                }
            };

            Assert.Equal(400, Core.GetBackoffDelay(retryOptions, retryPolicyContext));

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 2,
                Exception = new CException
                {
                    Message = "CException",
                    Code = "CExceptionCode"
                }
            };
            Assert.Equal(800, Core.GetBackoffDelay(retryOptions, retryPolicyContext));

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 3,
                Exception = new CException
                {
                    Message = "CException",
                    Code = "CExceptionCode"
                }
            };
            Assert.Equal(1600, Core.GetBackoffDelay(retryOptions, retryPolicyContext));

            RetryCondition retryCondition4 = new RetryCondition
            {
                MaxAttempts = 20,
                Backoff = backoffPolicy,
                Exception = new List<string> { "AException" }
            };

            retryOptions = new RetryOptions
            {
                RetryCondition = new List<RetryCondition> { retryCondition4 }
            };

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 10,
                Exception = new AException
                {
                    Message = "AException",
                    Code = "AExceptionCode"
                }
            };
            Assert.Equal(60000, Core.GetBackoffDelay(retryOptions, retryPolicyContext));

            backoffPolicy = new ExponentialBackoffPolicy(200, 180 * 1000);

            retryCondition4 = new RetryCondition
            {
                MaxAttempts = 20,
                Backoff = backoffPolicy,
                Exception = new List<string> { "AException" }
            };

            retryOptions = new RetryOptions
            {
                Retryable = true,
                RetryCondition = new List<RetryCondition> { retryCondition4 }
            };

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 10,
                Exception = new AException
                {
                    Message = "AException",
                    Code = "AExceptionCode"
                }
            };

            Assert.Equal(120000, Core.GetBackoffDelay(retryOptions, retryPolicyContext));


            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 15,
                Exception = new AException
                {
                    Message = "AException",
                    Code = "AExceptionCode"
                }
            };
            Assert.Equal(120000, Core.GetBackoffDelay(retryOptions, retryPolicyContext));

            retryCondition4 = new RetryCondition
            {
                MaxAttempts = 20,
                MaxDelayTimeMillis = 30 * 1000,
                Backoff = backoffPolicy,
                Exception = new List<string> { "AException" }
            };

            retryOptions = new RetryOptions
            {
                Retryable = true,
                RetryCondition = new List<RetryCondition> { retryCondition4 }
            };

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 10,
                Exception = new AException
                {
                    Message = "AException",
                    Code = "AExceptionCode"
                }
            };

            Assert.Equal(30000, Core.GetBackoffDelay(retryOptions, retryPolicyContext));

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 15,
                Exception = new AException
                {
                    Message = "AException",
                    Code = "AExceptionCode"
                }
            };

            Assert.Equal(30000, Core.GetBackoffDelay(retryOptions, retryPolicyContext));

            retryCondition4 = new RetryCondition
            {
                MaxAttempts = 20,
                Backoff = null,
                Exception = new List<string> { "AException" }
            };

            retryOptions = new RetryOptions
            {
                Retryable = true,
                RetryCondition = new List<RetryCondition> { retryCondition4 }
            };

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 10,
                Exception = new AException
                {
                    Message = "AException",
                    Code = "AExceptionCode"
                }
            };
            Assert.Equal(100, Core.GetBackoffDelay(retryOptions, retryPolicyContext));
        }


        public class ThrottlingException : DaraResponseException
        {
            public ThrottlingException() : base()
            {
            }
        }

        [Fact]
        public void TestThrottlingBackoffDelay()
        {
            BackoffPolicy backoffPolicy = new ExponentialBackoffPolicy(200, 60 * 1000);
            RetryCondition retryCondition = new RetryCondition
            {
                MaxAttempts = 1,
                Backoff = backoffPolicy,
                Exception = new List<string> { "ThrottlingException" }
            };

            RetryPolicyContext retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 1,
                Exception = new ThrottlingException { }
            };
            Assert.Equal(100, Core.GetBackoffDelay(null, retryPolicyContext));

            RetryOptions retryOptions = new RetryOptions
            {
                Retryable = false,
                RetryCondition = new List<RetryCondition> { retryCondition },
                NoRetryCondition = null
            };
            Assert.Equal(100, Core.GetBackoffDelay(retryOptions, retryPolicyContext));

            retryOptions = new RetryOptions
            {
                Retryable = true,
                RetryCondition = new List<RetryCondition> { retryCondition },
                NoRetryCondition = null
            };
            Assert.Equal(100, Core.GetBackoffDelay(retryOptions, retryPolicyContext));

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 1,
                Exception = new ThrottlingException
                {
                    Message = "ThrottlingException"
                }
            };
            Assert.Equal(400, Core.GetBackoffDelay(retryOptions, retryPolicyContext));

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 1,
                Exception = new ThrottlingException
                {
                    // TODO retryable true
                    Message = "ThrottlingException",
                    RetryAfter = 2000L
                }
            };
            Assert.Equal(2000, Core.GetBackoffDelay(retryOptions, retryPolicyContext));

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 1,
                Exception = new ThrottlingException
                {
                    // TODO retryable true
                    Message = "ThrottlingException",
                    RetryAfter = 320 * 1000L
                }
            };
            Assert.Equal(120000, Core.GetBackoffDelay(retryOptions, retryPolicyContext));

            retryCondition = new RetryCondition
            {
                MaxAttempts = 1,
                Backoff = backoffPolicy,
                ErrorCode = new List<string> { "Throttling", "Throttling.User", "Throttling.Api" }
            };
            retryOptions = new RetryOptions
            {
                Retryable = true,
                RetryCondition = new List<RetryCondition> { retryCondition },
                NoRetryCondition = null
            };
            Assert.Equal(100, Core.GetBackoffDelay(retryOptions, retryPolicyContext));

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 1,
                Exception = new ThrottlingException
                {
                    // TODO retryable true
                    Code = "Throttling",
                    RetryAfter = 2000L
                }
            };
            Assert.Equal(2000, Core.GetBackoffDelay(retryOptions, retryPolicyContext));

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 1,
                Exception = new ThrottlingException
                {
                    // TODO retryable true
                    Code = "Throttling.User",
                    RetryAfter = 2000L
                }
            };
            Assert.Equal(2000, Core.GetBackoffDelay(retryOptions, retryPolicyContext));

            retryPolicyContext = new RetryPolicyContext
            {
                RetriesAttempted = 1,
                Exception = new ThrottlingException
                {
                    // TODO retryable true
                    Code = "Throttling.Api",
                    RetryAfter = 2000L
                }
            };
            Assert.Equal(2000, Core.GetBackoffDelay(retryOptions, retryPolicyContext));
        }

        [Fact]
        public void TestSleep()
        {
            TimeSpan tsBefore = new TimeSpan(DateTime.Now.Ticks);
            Core.Sleep(1000);
            TimeSpan tsAfter = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan tsSubtract = tsBefore.Subtract(tsAfter).Duration();
            Assert.InRange(tsSubtract.TotalMilliseconds, 990, 1100);
        }

        [Fact]
        public async void TestSleepAsync()
        {
            TimeSpan tsBefore = new TimeSpan(DateTime.Now.Ticks);
            await Core.SleepAsync(1000);
            TimeSpan tsAfter = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan tsSubtract = tsBefore.Subtract(tsAfter).Duration();
            Assert.InRange(tsSubtract.TotalMilliseconds, 990, 1000000);
        }

        [Fact]
        public void TestIsRetryable()
        {
            Exception ex = new Exception();
            Assert.False(Core.IsRetryable(ex));

            DaraRetryableException webEx = new DaraRetryableException();
            Assert.True(Core.IsRetryable(webEx));
        }

        [Fact]
        public void TestBytesReadable()
        {
            string str = "test";
            Stream stream = StreamUtils.BytesReadable(str);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            string bytesStr = Encoding.UTF8.GetString(bytes);
            Assert.Equal("test", bytesStr);
        }

        [Fact]
        public void Test_PercentEncode()
        {
            Assert.Null(Core.PercentEncode(null));

            Assert.Equal("test%3D", Core.PercentEncode("test="));
        }

        [Fact]
        public void Test_ThrowException()
        {
            var retryPolicyContext = new RetryPolicyContext
            {
                Request = new Request(),
                Exception = new DaraException()
            };
            try
            {
                throw Core.ThrowException(retryPolicyContext);
            }
            catch (Exception e)
            {
                Assert.Equal("Darabonba.Exceptions.DaraException", e.GetType().ToString());
            }

            retryPolicyContext = new RetryPolicyContext
            {
                Request = new Request(),
                Exception = new IOException()
            };
            try
            {
                throw Core.ThrowException(retryPolicyContext);
            }
            catch (Exception e)
            {
                Assert.Equal("Darabonba.Exceptions.DaraUnRetryableException", e.GetType().ToString());
            }
        }

        [Fact]
        public void Test_GetDefaultValue()
        {
            byte? byteVal = 0;
            object obj = byteVal;
            var res = Core.GetDefaultValue(obj, 100);
            Assert.Equal(100, res);
            byteVal = 1;
            obj = byteVal;
            res = Core.GetDefaultValue(obj, 100);
            Assert.Equal((byte)1, res);
            
            byteVal = null;
            res = Core.GetDefaultValue(byteVal, 100);
            Assert.Equal(100, res);
            
            sbyte sbyteVal = 0;
            obj = sbyteVal;
            res = Core.GetDefaultValue(obj, 100);
            Assert.Equal(100, res);
            sbyteVal = 1;
            obj = sbyteVal;
            res = Core.GetDefaultValue(obj, 100);
            Assert.Equal((sbyte)1, res);

            int? intVal = 0;
            obj = intVal;
            res = Core.GetDefaultValue(obj, 100);
            Assert.Equal(100, res);
            intVal = 1;
            obj = intVal;
            res = Core.GetDefaultValue(obj, 100);
            Assert.Equal(1, res);
            
            uint? uintVal = 0;
            obj = uintVal;
            res = Core.GetDefaultValue(obj, 100);
            Assert.Equal(100, res);
            uintVal = 1;
            obj = uintVal;
            res = Core.GetDefaultValue(obj, 100);
            Assert.Equal((uint)1, res);
            
            short shortVal = 0;
            obj = shortVal;
            res = Core.GetDefaultValue(obj, 100);
            Assert.Equal(100, res);
            shortVal = 1;
            obj = shortVal;
            res = Core.GetDefaultValue(obj, 100);
            Assert.Equal((short)1, res);
            
            ushort ushortVal = 0;
            obj = ushortVal;
            res = Core.GetDefaultValue(obj, 100);
            Assert.Equal(100, res);
            ushortVal = 1;
            obj = ushortVal;
            res = Core.GetDefaultValue(obj, 100);
            Assert.Equal((ushort)1, res);
            
            long longVal = 0;
            obj = longVal;
            res = Core.GetDefaultValue(obj, 100);
            Assert.Equal(100, res);
            longVal = 1;
            obj = longVal;
            res = Core.GetDefaultValue(obj, 100);
            Assert.Equal(1L, res);
            
            ulong ulongVal = 0;
            obj = ulongVal;
            res = Core.GetDefaultValue(obj, 100);
            Assert.Equal(100, res);
            ulongVal = 1;
            obj = ulongVal;
            res = Core.GetDefaultValue(obj, 100);
            Assert.Equal((ulong)1, res);
            
            float floatVal = 0;
            obj = floatVal;
            res = Core.GetDefaultValue(obj, 100);
            Assert.Equal(100, res);
            floatVal = 1;
            obj = floatVal;
            res = Core.GetDefaultValue(obj, 100);
            Assert.Equal(1f, res);
            
            double doubleVal = 0;
            obj = doubleVal;
            res = Core.GetDefaultValue(obj, 100);
            Assert.Equal(100, res);
            doubleVal = 1;
            obj = doubleVal;
            res = Core.GetDefaultValue(obj, 100);
            Assert.Equal(1d, res);
            
            var str = "";
            obj = str;
            res = Core.GetDefaultValue(obj, "defaultStr");
            Assert.Equal("defaultStr", res);
            str = "test";
            obj = str;
            res = Core.GetDefaultValue(obj, "defaultStr");
            Assert.Equal("test", res);

            var boolVal = false;
            obj = boolVal;
            res = Core.GetDefaultValue(obj, true);
            Assert.True((bool)res);
            boolVal = true;
            obj = boolVal;
            res = Core.GetDefaultValue(obj, false);
            Assert.True((bool)res);

            Context context = null;
            res = Core.GetDefaultValue(context, true);
            Assert.True((bool)res);
        }
    }
}