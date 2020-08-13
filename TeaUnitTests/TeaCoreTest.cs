using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Tea;

using Xunit;

namespace TeaUnitTests
{
    public class TeaCoreTest
    {
        [Fact]
        public void TestComposeUrl()
        {
            TeaRequest teaRequest = new TeaRequest();

            var url = TeaCore.ComposeUrl(teaRequest);
            Assert.Equal("http://", url);

            teaRequest.Headers["host"] = "fake.domain.com";
            url = TeaCore.ComposeUrl(teaRequest);
            Assert.Equal("http://fake.domain.com", url);

            teaRequest.Port = 8080;
            url = TeaCore.ComposeUrl(teaRequest);
            Assert.Equal("http://fake.domain.com:8080", url);

            teaRequest.Pathname = "/index.html";
            url = TeaCore.ComposeUrl(teaRequest);
            Assert.Equal("http://fake.domain.com:8080/index.html", url);

            teaRequest.Query["foo"] = "";
            url = TeaCore.ComposeUrl(teaRequest);
            Assert.Equal("http://fake.domain.com:8080/index.html", url);

            teaRequest.Query["foo"] = "bar";
            url = TeaCore.ComposeUrl(teaRequest);
            Assert.Equal("http://fake.domain.com:8080/index.html?foo=bar", url);

            teaRequest.Pathname = "/index.html?a=b";
            url = TeaCore.ComposeUrl(teaRequest);
            Assert.Equal("http://fake.domain.com:8080/index.html?a=b&foo=bar", url);

            teaRequest.Pathname = "/index.html?a=b&";
            url = TeaCore.ComposeUrl(teaRequest);
            Assert.Equal("http://fake.domain.com:8080/index.html?a=b&foo=bar", url);

            teaRequest.Query["fake"] = null;
            url = TeaCore.ComposeUrl(teaRequest);
            Assert.Equal("http://fake.domain.com:8080/index.html?a=b&foo=bar", url);

            teaRequest.Query["fake"] = "val*";
            url = TeaCore.ComposeUrl(teaRequest);
            Assert.Equal("http://fake.domain.com:8080/index.html?a=b&foo=bar&fake=val%2A", url);
        }

        [Fact]
        public void TestDoAction()
        {
            TeaRequest teaRequest = new TeaRequest();
            teaRequest.Protocol = "http";
            teaRequest.Method = "GET";
            teaRequest.Headers = new Dictionary<string, string>();
            teaRequest.Headers["host"] = "www.alibabacloud.com";
            teaRequest.Pathname = "/s/zh";
            teaRequest.Query = new Dictionary<string, string>();
            teaRequest.Query.Add("k", "ecs");
            Dictionary<string, object> runtime = new Dictionary<string, object>();
            runtime.Add("readTimeout", 7000);
            runtime.Add("connectTimeout", 7000);
            runtime.Add("httpsProxy", "http://www.alibabacloud.com/s/zh?k=ecs");
            runtime.Add("ignoreSSL", true);

            TeaResponse teaResponse = TeaCore.DoAction(teaRequest, runtime);
            Assert.NotNull(teaResponse);

            teaRequest.Protocol = "https";
            teaResponse = TeaCore.DoAction(teaRequest);
            Assert.NotNull(teaResponse);


            string bodyStr = TeaCore.GetResponseBody(teaResponse);
            Assert.NotNull(bodyStr);

            teaRequest.Method = "POST";
            teaRequest.Body = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            teaRequest.Headers["content-test"] = "test";
            teaResponse = TeaCore.DoAction(teaRequest, runtime);
            Assert.NotNull(teaResponse);

            TeaRequest teaRequest404 = new TeaRequest();
            teaRequest404.Protocol = "https";
            teaRequest404.Method = "GET";
            teaRequest404.Headers = new Dictionary<string, string>();
            teaRequest404.Headers["host"] = "www.alibabacloud404.com";
            teaRequest404.Pathname = "/s/zh";
            teaRequest404.Query = new Dictionary<string, string>();
            teaRequest404.Query.Add("k", "ecs");
            Dictionary<string, object> runtime404 = new Dictionary<string, object>();
            runtime404.Add("readTimeout", 7000);
            runtime404.Add("connectTimeout", 7000);
            Assert.Throws<AggregateException>(() => { TeaCore.DoAction(teaRequest404, runtime404); });

            TeaRequest teaRequestProxy = new TeaRequest();


            TeaRequest requestException = new TeaRequest
            {
                Protocol = "http",
                Method = "GET",
                Pathname = "/test"
            };
            Dictionary<string, object> runtimeException = new Dictionary<string, object>();
            requestException.Headers["host"] = "www.aliyun.com";
            TeaResponse responseException = TeaCore.DoAction(requestException, runtimeException);
            Assert.NotNull(responseException);
        }

        [Fact]
        public async Task TestDoActionAsync()
        {
            TeaRequest teaRequest = new TeaRequest();
            teaRequest.Protocol = "https";
            teaRequest.Method = "GET";
            teaRequest.Headers = new Dictionary<string, string>();
            teaRequest.Headers["host"] = "www.alibabacloud.com";
            teaRequest.Pathname = "/s/zh";
            teaRequest.Query = new Dictionary<string, string>();
            teaRequest.Query.Add("k", "ecs");

            TeaResponse teaResponse = await TeaCore.DoActionAsync(teaRequest);
            Assert.NotNull(teaResponse);

            Dictionary<string, object> runtime = new Dictionary<string, object>();
            runtime.Add("readTimeout", 4000);
            runtime.Add("connectTimeout", 0);

            teaResponse = await TeaCore.DoActionAsync(teaRequest, runtime);
            Assert.NotNull(teaResponse);

            string bodyStr = TeaCore.GetResponseBody(teaResponse);
            Assert.NotNull(bodyStr);

            teaRequest.Method = "POST";
            teaRequest.Body = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            teaResponse = await TeaCore.DoActionAsync(teaRequest, runtime);
            Assert.NotNull(teaResponse);

            TeaRequest teaRequest404 = new TeaRequest();
            teaRequest404.Protocol = "https";
            teaRequest404.Method = "GET";
            teaRequest404.Headers = new Dictionary<string, string>();
            teaRequest404.Headers["host"] = "www.alibabacloud404.com";
            teaRequest404.Pathname = "/s/zh";
            teaRequest404.Query = new Dictionary<string, string>();
            teaRequest404.Query.Add("k", "ecs");
            Dictionary<string, object> runtime404 = new Dictionary<string, object>();
            runtime404.Add("readTimeout", 7000);
            runtime404.Add("connectTimeout", 7000);
            await Assert.ThrowsAsync<HttpRequestException>(async() => { await TeaCore.DoActionAsync(teaRequest404, runtime); });

            TeaRequest requestException = new TeaRequest
            {
                Protocol = "http",
                Method = "GET",
                Pathname = "/test"
            };
            Dictionary<string, object> runtimeException = new Dictionary<string, object>();
            requestException.Headers["host"] = "www.aliyun.com";
            TeaResponse responseException = await TeaCore.DoActionAsync(requestException, runtimeException);
        }

        [Fact]
        public void TestConvertHeaders()
        {
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("testKey", "testValue");
            Dictionary<string, string> dic = TeaCore.ConvertHeaders(headers);
            Assert.NotNull(dic);
            Assert.True(dic.ContainsKey("testkey"));
            Assert.Equal("testValue", dic["testkey"]);
        }

        [Fact]
        public void TestAllowRetry()
        {
            long _now = System.DateTime.Now.Millisecond;

            Assert.True(TeaCore.AllowRetry(null, 0, _now));

            Assert.False(TeaCore.AllowRetry(null, 3, _now));

            Dictionary<string, object> dic = new Dictionary<string, object>();
            Assert.False(TeaCore.AllowRetry(dic, 3, _now));

            dic.Add("retryable", true);
            dic.Add("maxAttempts", null);
            Assert.False(TeaCore.AllowRetry(dic, 3, _now));

            dic["maxAttempts"] = 5;
            Assert.True(TeaCore.AllowRetry(dic, 3, _now));

            Dictionary<string, int?> dicInt = new Dictionary<string, int?>();
            Assert.False(TeaCore.AllowRetry(dicInt, 3, _now));
        }

        [Fact]
        public void TestGetBackoffTime()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            Assert.Equal(0, TeaCore.GetBackoffTime(dic, 1));

            dic.Add("policy", null);
            Assert.Equal(0, TeaCore.GetBackoffTime(dic, 1));

            dic["policy"] = string.Empty;
            Assert.Equal(0, TeaCore.GetBackoffTime(dic, 1));

            dic["policy"] = "no";
            Assert.Equal(0, TeaCore.GetBackoffTime(dic, 1));

            dic["policy"] = "yes";
            Assert.Equal(0, TeaCore.GetBackoffTime(dic, 1));

            dic.Add("period", null);
            Assert.Equal(0, TeaCore.GetBackoffTime(dic, 1));

            dic["period"] = -1;
            Assert.Equal(1, TeaCore.GetBackoffTime(dic, 1));

            dic["period"] = 1000;
            Assert.Equal(1000, TeaCore.GetBackoffTime(dic, 1));
        }

        [Fact]
        public void TestSleep()
        {
            TimeSpan tsBefore = new TimeSpan(DateTime.Now.Ticks);
            TeaCore.Sleep(1000);
            TimeSpan tsAfter = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan tsSubtract = tsBefore.Subtract(tsAfter).Duration();
            Assert.InRange(tsSubtract.TotalMilliseconds, 990, 1100);
        }

        [Fact]
        public async void TestSleepAsync()
        {
            TimeSpan tsBefore = new TimeSpan(DateTime.Now.Ticks);
            await TeaCore.SleepAsync(1000);
            TimeSpan tsAfter = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan tsSubtract = tsBefore.Subtract(tsAfter).Duration();
            Assert.InRange(tsSubtract.TotalMilliseconds, 990, 1000000);
        }

        [Fact]
        public void TestIsRetryable()
        {
            Exception ex = new Exception();
            Assert.False(TeaCore.IsRetryable(ex));

            TeaRetryableException webEx = new TeaRetryableException();
            Assert.True(TeaCore.IsRetryable(webEx));
        }

        [Fact]
        public void TestBytesReadable()
        {
            string str = "test";
            Stream stream = TeaCore.BytesReadable(str);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            string bytesStr = Encoding.UTF8.GetString(bytes);
            Assert.Equal("test", bytesStr);
        }

        [Fact]
        public void Test_PercentEncode()
        {
            Assert.Null(TeaCore.PercentEncode(null));

            Assert.Equal("test%3D", TeaCore.PercentEncode("test="));
        }
    }
}
