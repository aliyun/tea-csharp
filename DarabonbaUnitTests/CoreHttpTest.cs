using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Darabonba;
using Darabonba.Utils;
using Xunit;

namespace DaraUnitTests
{
    public class CoreHttpTest
    {
        private static readonly string RequestBody = new string('x', 4097) + "测试";
        private static readonly string ResponseBody = new string('界', 500);

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestDoAction(bool useRuntime)
        {
            using (var client = new CachedClient(AssertRequestAndRespond, useRuntime ? "http" : "https"))
            {
                SetRequestBody(client.Request);
                var response = useRuntime
                    ? Core.DoAction(client.Request, RuntimeOptions())
                    : Core.DoAction(client.Request);

                AssertResponse(response);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestDoActionAsync(bool useRuntime)
        {
            using (var client = new CachedClient(AssertRequestAndRespond, useRuntime ? "http" : "https"))
            {
                SetRequestBody(client.Request);
                var response = useRuntime
                    ? await Core.DoActionAsync(client.Request, RuntimeOptions())
                    : await Core.DoActionAsync(client.Request);

                AssertResponse(response);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestSseDoesNotBufferResponseContent(bool asynchronous)
        {
            var content = new UnbufferedContent("data: hello\n\n");
            using (var client = new CachedClient((request, token) => Task.FromResult(
                new HttpResponseMessage(HttpStatusCode.OK) { Content = content })))
            {
                var response = asynchronous
                    ? await Core.DoSSEActionAsync(client.Request, RuntimeOptions())
                    : Core.DoSSEAction(client.Request, RuntimeOptions());

                Assert.Equal(200, response.StatusCode);
                Assert.False(content.Serialized);
                Assert.Equal("data: hello\n\n", Core.GetResponseBody(response));
                Assert.True(content.Serialized);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestSyncTransportPreservesRequestFailure(bool sse)
        {
            using (var client = new CachedClient((request, token) => FailedResponse(new HttpRequestException("connection failed"))))
            {
                var exception = Assert.Throws<AggregateException>(() =>
                {
                    if (sse)
                        Core.DoSSEAction(client.Request, RuntimeOptions());
                    else
                        Core.DoAction(client.Request, RuntimeOptions());
                });

                Assert.Equal("connection failed", Assert.IsType<HttpRequestException>(exception.InnerException).Message);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestAsyncTransportPreservesRequestFailure(bool sse)
        {
            using (var client = new CachedClient((request, token) => FailedResponse(new HttpRequestException("connection failed"))))
            {
                var exception = await Assert.ThrowsAsync<HttpRequestException>(async () =>
                {
                    if (sse)
                        await Core.DoSSEActionAsync(client.Request, RuntimeOptions());
                    else
                        await Core.DoActionAsync(client.Request, RuntimeOptions());
                });

                Assert.Equal("connection failed", exception.Message);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestSyncTransportWrapsCancellation(bool sse)
        {
            using (var client = new CachedClient((request, token) => CanceledResponse()))
            {
                var exception = Assert.Throws<AggregateException>(() =>
                {
                    if (sse)
                        Core.DoSSEAction(client.Request, RuntimeOptions());
                    else
                        Core.DoAction(client.Request, RuntimeOptions());
                });

                Assert.IsType<TaskCanceledException>(exception.InnerException);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestAsyncTransportConvertsCancellationToTimeout(bool sse)
        {
            using (var client = new CachedClient((request, token) => CanceledResponse()))
            {
                var exception = await Assert.ThrowsAsync<WebException>(async () =>
                {
                    if (sse)
                        await Core.DoSSEActionAsync(client.Request, RuntimeOptions());
                    else
                        await Core.DoActionAsync(client.Request, RuntimeOptions());
                });

                Assert.Equal("operation is timeout", exception.Message);
            }
        }

        [Fact]
        public async Task TestRuntimeTimeoutCancelsTheRequest()
        {
            using (var client = new CachedClient(async (request, token) =>
            {
                await Task.Delay(1000, token).ConfigureAwait(false);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }))
            {
                var exception = await Assert.ThrowsAsync<WebException>(() => Core.DoActionAsync(client.Request,
                    new Dictionary<string, object> { { "readTimeout", 20 }, { "connectTimeout", 20 } }));

                Assert.Equal("operation is timeout", exception.Message);
            }
        }

        [Theory]
        [InlineData(0, 0, 100000)]
        [InlineData(25, 0, 25)]
        [InlineData(0, 15, 15)]
        [InlineData(25, 15, 40)]
        public void TestRequestTimeoutIncludesReadAndConnectTimeouts(int readTimeout, int connectTimeout, int expected)
        {
            var request = new Request();
            request.Headers["host"] = "request.invalid";
            int timeout;
            using (var message = Core.GetRequestMessage(request,
                new Dictionary<string, object> { { "readTimeout", readTimeout }, { "connectTimeout", connectTimeout } }, out timeout))
            {
                Assert.Equal(expected, timeout);
                Assert.Null(message.Content);
                Assert.Equal(HttpMethod.Get, message.Method);
            }
        }

        private static Dictionary<string, object> RuntimeOptions()
        {
            return new Dictionary<string, object> { { "readTimeout", 1000 }, { "connectTimeout", 2000 } };
        }

        private static void SetRequestBody(Request request)
        {
            request.Method = "POST";
            request.Pathname = "/messages";
            request.Query["search"] = "a b*";
            request.Headers["X-Trace"] = "trace";
            request.Headers["Content-Type"] = "application/json";
            request.Body = new MemoryStream(Encoding.UTF8.GetBytes(RequestBody));
            request.Body.Position = 10;
        }

        private static async Task<HttpResponseMessage> AssertRequestAndRespond(HttpRequestMessage request, CancellationToken token)
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("/messages?search=a%20b%2A", request.RequestUri.PathAndQuery);
            Assert.Equal(request.RequestUri.Host, request.Headers.Host);
            Assert.Contains("trace", request.Headers.GetValues("X-Trace"));
            Assert.Equal("application/json", request.Content.Headers.ContentType.MediaType);
            Assert.Equal(RequestBody, await request.Content.ReadAsStringAsync().ConfigureAwait(false));
            Assert.True(token.CanBeCanceled);

            var response = new HttpResponseMessage(HttpStatusCode.Accepted)
            {
                Content = new StringContent(ResponseBody)
            };
            response.Headers.Add("X-Result", new[] { "first", "second" });
            return response;
        }

        private static void AssertResponse(Response response)
        {
            Assert.Equal(202, response.StatusCode);
            Assert.Equal("first", response.Headers["x-result"]);
            Assert.Equal(ResponseBody, Core.GetResponseBody(response));
        }

        private static Task<HttpResponseMessage> FailedResponse(Exception exception)
        {
            var source = new TaskCompletionSource<HttpResponseMessage>();
            source.SetException(exception);
            return source.Task;
        }

        private static Task<HttpResponseMessage> CanceledResponse()
        {
            var source = new TaskCompletionSource<HttpResponseMessage>();
            source.SetCanceled();
            return source.Task;
        }

        private sealed class CachedClient : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send;
            private readonly HttpClient client;
            private readonly string key;

            public Request Request { get; private set; }

            public CachedClient(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send, string protocol = "http")
            {
                this.send = send;
                Request = new Request { Protocol = protocol };
                Request.Headers["host"] = Guid.NewGuid().ToString("N") + ".invalid";
                key = string.Format("{0}:{1}:{2}", protocol, Request.Headers["host"], protocol == "https" ? 443 : 80);
                client = new HttpClient(this, false) { Timeout = TimeSpan.FromSeconds(5) };
                Assert.True(HttpClientUtils.httpClients.TryAdd(key, client));
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return send(request, cancellationToken);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    HttpClient ignored;
                    HttpClientUtils.httpClients.TryRemove(key, out ignored);
                    client.Dispose();
                    if (Request.Body != null)
                        Request.Body.Dispose();
                }
                base.Dispose(disposing);
            }
        }

        private sealed class UnbufferedContent : HttpContent
        {
            private readonly byte[] bytes;

            public bool Serialized { get; private set; }

            public UnbufferedContent(string content)
            {
                bytes = Encoding.UTF8.GetBytes(content);
            }

            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                Serialized = true;
                return stream.WriteAsync(bytes, 0, bytes.Length);
            }

            protected override bool TryComputeLength(out long length)
            {
                length = bytes.Length;
                return true;
            }
        }
    }
}
