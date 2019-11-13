using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

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
            teaRequest.Protocol = "http";
            teaRequest.Headers = new Dictionary<string, string>();
            teaRequest.Headers["host"] = "host";
            teaRequest.Port = 8080;
            teaRequest.Pathname = "/Pathname";
            teaRequest.Query = new Dictionary<string, string>();
            teaRequest.Query.Add("query", "query");
            teaRequest.Query.Add("queryNull", null);
            string url = TeaCore.ComposeUrl(teaRequest);
            Assert.NotNull(url);
            Assert.Equal("http://host:8080/Pathname?query=query&queryNull", url);
        }

        [Fact]
        public void TestDoAction()
        {
            TeaRequest teaRequest = new TeaRequest();
            teaRequest.Protocol = "https";
            teaRequest.Method = "GET";
            teaRequest.Headers = new Dictionary<string, string>();
            teaRequest.Headers["host"] = "www.alibabacloud.com";
            teaRequest.Pathname = "/s/zh";
            teaRequest.Query = new Dictionary<string, string>();
            teaRequest.Query.Add("k", "ecs");

            Dictionary<string, object> runtime = new Dictionary<string, object>();
            runtime.Add("readTimeout", 7000);
            runtime.Add("connectTimeout", 7000);

            TeaResponse teaResponse = TeaCore.DoAction(teaRequest, runtime);
            Assert.NotNull(teaResponse);

            string bodyStr = TeaCore.GetResponseBody(teaResponse);
            Assert.NotNull(bodyStr);
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
            Assert.False(TeaCore.AllowRetry(null, 3, _now));

            Dictionary<string, object> dic = new Dictionary<string, object>();
            Assert.False(TeaCore.AllowRetry(dic, 3, _now));

            dic.Add("maxAttempts", null);
            Assert.False(TeaCore.AllowRetry(dic, 3, _now));

            dic["maxAttempts"] = 5;
            Assert.True(TeaCore.AllowRetry(dic, 3, _now));
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
            Assert.InRange(tsSubtract.TotalMilliseconds, 1000, 1100);
        }

        [Fact]
        public void TestIsRetryable()
        {
            Exception ex = new Exception();
            Assert.False(TeaCore.IsRetryable(ex));

            WebException webEx = new WebException();
            Assert.True(TeaCore.IsRetryable(webEx));

            OperationCanceledException opEx = new OperationCanceledException();
            Assert.True(TeaCore.IsRetryable(opEx));
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
    }
}
