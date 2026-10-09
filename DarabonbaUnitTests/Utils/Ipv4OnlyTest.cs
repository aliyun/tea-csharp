using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Darabonba;
using Darabonba.Utils;
using Xunit;

namespace DaraUnitTests.Utils
{
    public class Ipv4OnlyTest
    {
        private const string Ca = @"-----BEGIN CERTIFICATE-----
MIIBuDCCAWICCQCLw4OWpjlJCDANBgkqhkiG9w0BAQsFADBjMQswCQYDVQQGEwJm
ZDEMMAoGA1UECAwDYXNkMQswCQYDVQQHDAJxcjEKMAgGA1UECgwBZjEMMAoGA1UE
CwwDc2RhMQswCQYDVQQDDAJmZDESMBAGCSqGSIb3DQEJARYDYXNkMB4XDTIwMTAx
OTAyNDQwNFoXDTIzMDgwOTAyNDQwNFowYzELMAkGA1UEBhMCZmQxDDAKBgNVBAgM
A2FzZDELMAkGA1UEBwwCcXIxCjAIBgNVBAoMAWYxDDAKBgNVBAsMA3NkYTELMAkG
A1UEAwwCZmQxEjAQBgkqhkiG9w0BCQEWA2FzZDBcMA0GCSqGSIb3DQEBAQUAA0sA
MEgCQQCxXZTl5IO61Lqd0fBBOSy7ER1gsdA0LkvflP5HEaQygjecLGfrAtD/DWu0
/sxCcBVnQRoP9Yp0ijHJwgXvBnrNAgMBAAEwDQYJKoZIhvcNAQELBQADQQBJF+/4
DEMilhlFY+o9mqCygFVxuvHtQVhpPS938H2h7/P6pXN65jK2Y5hHefZEELq9ulQe
91iBwaQ4e9racCgP
-----END CERTIFICATE-----";

        [Fact]
        public void Test_IsIpv4Only_DefaultFalse()
        {
            Assert.False(HttpClientUtils.IsIpv4Only(new Dictionary<string, object>()));
            Assert.False(HttpClientUtils.IsIpv4Only(new Dictionary<string, object> { { "ipv4Only", null } }));
            Assert.False(HttpClientUtils.IsIpv4Only(new Dictionary<string, object> { { "ipv4Only", false } }));
            Assert.True(HttpClientUtils.IsIpv4Only(new Dictionary<string, object> { { "ipv4Only", true } }));
        }

        [Fact]
        public void Test_CacheKey_SeparatesIpv4OnlyFromDualStack()
        {
            var dualStack = new Dictionary<string, object>();
            HttpClient defaultClient = HttpClientUtils.GetOrAddHttpClient("http", "ipv4only-cache.test", 8081, dualStack);
            Assert.True(HttpClientUtils.httpClients.ContainsKey("http:ipv4only-cache.test:8081"));
            Assert.Same(defaultClient, HttpClientUtils.GetOrAddHttpClient("http", "ipv4only-cache.test", 8081,
                new Dictionary<string, object> { { "ipv4Only", false } }));

            if (!IsConnectCallbackSupported())
            {
                Assert.Throws<NotSupportedException>(() => HttpClientUtils.GetOrAddHttpClient("http",
                    "ipv4only-cache.test", 8081, new Dictionary<string, object> { { "ipv4Only", true } }));
                return;
            }

            var ipv4Only = new Dictionary<string, object> { { "ipv4Only", true } };
            HttpClient ipv4Client = HttpClientUtils.GetOrAddHttpClient("http", "ipv4only-cache.test", 8081, ipv4Only);
            Assert.True(HttpClientUtils.httpClients.ContainsKey("http:ipv4only-cache.test:8081:ipv4Only"));
            Assert.NotSame(defaultClient, ipv4Client);
            Assert.Same(ipv4Client, HttpClientUtils.GetOrAddHttpClient("http", "ipv4only-cache.test", 8081, ipv4Only));

            var proxied = new Dictionary<string, object> { { "ipv4Only", true }, { "httpProxy", "http://ipv4only-proxy.test:3128" } };
            HttpClientUtils.GetOrAddHttpClient("https", "ipv4only-cache.test", 443, proxied);
            Assert.True(HttpClientUtils.httpClients.ContainsKey("https:ipv4only-proxy.test:3128:ipv4Only"));
        }

#if NET45
        [Fact]
        public void Test_FailClosed_OnNetFramework()
        {
            var ex = Assert.Throws<NotSupportedException>(() => HttpClientUtils.CreatHttpClient(
                new Dictionary<string, object> { { "ipv4Only", true } }));
            Assert.Contains("ipv4Only", ex.Message);
        }
#else
        private class NotAHandler
        {
            public Func<int, CancellationToken, int> ConnectCallback { get; set; }
        }

        [Fact]
        public void Test_FailClosed_WhenConnectCallbackUnavailable()
        {
            // Simulates .NET Framework (no SocketsHttpHandler) and netcoreapp < 5 (no ConnectCallback).
            var ex = Assert.Throws<NotSupportedException>(() => Ipv4OnlyHandlerFactory.Create(null, null, false, null));
            Assert.Equal(Ipv4OnlyHandlerFactory.NotSupportedMessage, ex.Message);
            Assert.Throws<NotSupportedException>(() => Ipv4OnlyHandlerFactory.Create(typeof(HttpClientHandler), null, false, null));
            Assert.Throws<NotSupportedException>(() => Ipv4OnlyHandlerFactory.Create(typeof(NotAHandler), null, false, null));

            Assert.Throws<NotSupportedException>(() => Ipv4OnlyHandlerFactory.BuildConnectCallback(typeof(Action)));
            Assert.Throws<NotSupportedException>(() => Ipv4OnlyHandlerFactory.BuildConnectCallback(typeof(Func<int, int, int>)));
            Assert.Throws<NotSupportedException>(() => Ipv4OnlyHandlerFactory.BuildConnectCallback(typeof(Func<int, CancellationToken, int>)));
        }

        [Fact]
        public void Test_FailClosed_ThroughCore_WhenRuntimeUnsupported()
        {
            if (IsConnectCallbackSupported())
            {
                return;
            }
            var runtime = new Dictionary<string, object> { { "ipv4Only", true } };
            Assert.Throws<NotSupportedException>(() => Core.DoAction(BuildRequest("localhost", 1), runtime));
        }

        [Fact]
        public void Test_Create_ConfiguresSocketsHttpHandler()
        {
            if (!IsConnectCallbackSupported())
            {
                return;
            }
            Type handlerType = Ipv4OnlyHandlerFactory.ResolveSocketsHttpHandlerType();

            HttpMessageHandler plain = Ipv4OnlyHandlerFactory.Create(null, false, null);
            Assert.IsType(handlerType, plain);
            Assert.NotNull(handlerType.GetProperty("ConnectCallback").GetValue(plain));
            Assert.Equal(false, handlerType.GetProperty("UseProxy").GetValue(plain));

            HttpMessageHandler proxied = Ipv4OnlyHandlerFactory.Create("http://127.0.0.1:3128", true, null);
            var proxy = (WebProxy)handlerType.GetProperty("Proxy").GetValue(proxied);
            Assert.Equal(new Uri("http://127.0.0.1:3128"), proxy.Address);
            object sslOptions = handlerType.GetProperty("SslOptions").GetValue(proxied);
            Assert.NotNull(sslOptions.GetType().GetProperty("RemoteCertificateValidationCallback").GetValue(sslOptions));

            HttpMessageHandler withCa = Ipv4OnlyHandlerFactory.Create(null, false, Ca);
            sslOptions = handlerType.GetProperty("SslOptions").GetValue(withCa);
            Assert.NotNull(sslOptions.GetType().GetProperty("RemoteCertificateValidationCallback").GetValue(sslOptions));

            var client = HttpClientUtils.CreatHttpClient(new Dictionary<string, object> { { "ipv4Only", true }, { "ca", Ca } });
            Assert.NotNull(client);
        }

        [Fact]
        public void Test_BuildCertificateValidation()
        {
            Assert.Null(Ipv4OnlyHandlerFactory.BuildCertificateValidation(false, null));
            Assert.Null(Ipv4OnlyHandlerFactory.BuildCertificateValidation(false, " "));
            Assert.True(Ipv4OnlyHandlerFactory.BuildCertificateValidation(true, null)(null, null, null, SslPolicyErrors.RemoteCertificateChainErrors));

            RemoteCertificateValidationCallback withCa = Ipv4OnlyHandlerFactory.BuildCertificateValidation(false, Ca);
            Assert.True(withCa(null, null, null, SslPolicyErrors.None));

            var original = HttpClientUtils.ServerCertificateCustomValidationCallback;
            try
            {
                X509Certificate2Collection seen = null;
                HttpClientUtils.ServerCertificateCustomValidationCallback = (sender, cas, cert, chain, errors) =>
                {
                    seen = cas;
                    return false;
                };
                Assert.False(withCa(null, null, null, SslPolicyErrors.None));
                Assert.Single(seen.Cast<X509Certificate2>());
            }
            finally
            {
                HttpClientUtils.ServerCertificateCustomValidationCallback = original;
            }
        }

        [Fact]
        public async Task Test_ConnectAsync_RejectsIpv6AndFailures()
        {
            var noIpv4 = await Assert.ThrowsAsync<IOException>(() => Ipv4OnlyHandlerFactory.ConnectAsync("::1", 80, CancellationToken.None));
            Assert.Contains("no IPv4 address", noIpv4.Message);

            int closedPort = GetUnusedPort(IPAddress.Loopback);
            var refused = await Assert.ThrowsAsync<IOException>(() => Ipv4OnlyHandlerFactory.ConnectAsync("127.0.0.1", closedPort, CancellationToken.None));
            Assert.NotNull(refused.InnerException);

            var cancelled = new CancellationTokenSource();
            cancelled.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Ipv4OnlyHandlerFactory.ConnectAsync("127.0.0.1", closedPort, cancelled.Token));

            var resolver = Ipv4OnlyHandlerFactory.CreateIpv4Resolver();
            IPAddress[] addresses = await resolver("localhost", CancellationToken.None);
            Assert.Contains(addresses, a => a.AddressFamily == AddressFamily.InterNetwork);
        }

        [Fact]
        public async Task Test_ConnectAsync_CancelledWhileConnecting()
        {
            var source = new CancellationTokenSource(TimeSpan.FromMilliseconds(200));
            // Non-routable address: connect either hangs until cancelled or fails fast; both must not succeed.
            var ex = await Record.ExceptionAsync(() => Ipv4OnlyHandlerFactory.ConnectAsync(new DnsEndPoint("10.255.255.1", 81), source.Token));
            Assert.True(ex is OperationCanceledException || ex is IOException, ex == null ? "no exception" : ex.ToString());
        }

        [Fact]
        public async Task Test_RealConnection_Ipv6OnlyListener()
        {
            if (!IsConnectCallbackSupported() || !IsLocalhostDualStack())
            {
                return;
            }

            using (var server = new LoopbackHttpServer(IPAddress.IPv6Loopback))
            {
                Request request = BuildRequest("localhost", server.Port);
                var dualStack = new Dictionary<string, object>();
                var ipv4Only = new Dictionary<string, object> { { "ipv4Only", true } };

                Assert.Equal(200, Core.DoAction(request, dualStack).StatusCode);

                var ex = Assert.ThrowsAny<Exception>(() => Core.DoAction(request, ipv4Only));
                Assert.Contains("over IPv4 failed", Flatten(ex));
                await Assert.ThrowsAnyAsync<Exception>(() => Core.DoActionAsync(request, ipv4Only));

                var literal = Assert.ThrowsAny<Exception>(() => Core.DoAction(BuildRequest("[::1]", server.Port), ipv4Only));
                Assert.Contains("no IPv4 address", Flatten(literal));

                // No process-global effect: cached and freshly created dual-stack clients still reach the IPv6 listener.
                Assert.Equal(200, Core.DoAction(request, dualStack).StatusCode);
                using (HttpClient fresh = HttpClientUtils.CreatHttpClient(new Dictionary<string, object>()))
                {
                    HttpResponseMessage response = await fresh.GetAsync("http://localhost:" + server.Port + "/");
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
            }
        }

        [Fact]
        public async Task Test_RealConnection_Ipv4ListenerWithIpv4Only()
        {
            if (!IsConnectCallbackSupported())
            {
                return;
            }

            using (var server = new LoopbackHttpServer(IPAddress.Loopback))
            {
                var ipv4Only = new Dictionary<string, object> { { "ipv4Only", true } };
                Assert.Equal(200, Core.DoAction(BuildRequest("localhost", server.Port), ipv4Only).StatusCode);
                Response response = await Core.DoActionAsync(BuildRequest("127.0.0.1", server.Port), ipv4Only);
                Assert.Equal(200, response.StatusCode);
                Assert.Equal("ok", Core.GetResponseBody(response));
            }
        }
#endif

        private static bool IsConnectCallbackSupported()
        {
#if NET45
            return false;
#else
            Type handlerType = Ipv4OnlyHandlerFactory.ResolveSocketsHttpHandlerType();
            return handlerType != null && handlerType.GetProperty("ConnectCallback") != null;
#endif
        }

        private static bool IsLocalhostDualStack()
        {
            if (!Socket.OSSupportsIPv6)
            {
                return false;
            }
            IPAddress[] addresses = Dns.GetHostAddresses("localhost");
            return addresses.Any(a => a.AddressFamily == AddressFamily.InterNetwork)
                   && addresses.Any(a => a.AddressFamily == AddressFamily.InterNetworkV6);
        }

        private static Request BuildRequest(string host, int port)
        {
            var request = new Request
            {
                Protocol = "http",
                Method = "GET",
                Headers = new Dictionary<string, string>(),
                Port = port,
                Pathname = "/"
            };
            request.Headers["host"] = host;
            return request;
        }

        private static int GetUnusedPort(IPAddress address)
        {
            var listener = new TcpListener(address, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private static string Flatten(Exception ex)
        {
            var builder = new StringBuilder();
            for (Exception current = ex; current != null; current = current.InnerException)
            {
                builder.AppendLine(current.Message);
                var aggregate = current as AggregateException;
                if (aggregate != null)
                {
                    foreach (Exception inner in aggregate.InnerExceptions)
                    {
                        builder.AppendLine(Flatten(inner));
                    }
                }
            }
            return builder.ToString();
        }

        private sealed class LoopbackHttpServer : IDisposable
        {
            private readonly TcpListener listener;

            public int Port { get; private set; }

            public LoopbackHttpServer(IPAddress address)
            {
                listener = new TcpListener(address, 0);
                listener.Start();
                Port = ((IPEndPoint)listener.LocalEndpoint).Port;
                Task.Run(AcceptLoop);
            }

            private async Task AcceptLoop()
            {
                while (true)
                {
                    TcpClient client;
                    try
                    {
                        client = await listener.AcceptTcpClientAsync();
                    }
                    catch
                    {
                        return;
                    }
                    var ignored = Task.Run(() => Serve(client));
                }
            }

            private static async Task Serve(TcpClient client)
            {
                using (client)
                using (NetworkStream stream = client.GetStream())
                {
                    var buffer = new byte[4096];
                    var received = new StringBuilder();
                    while (!received.ToString().Contains("\r\n\r\n"))
                    {
                        int read = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (read == 0)
                        {
                            return;
                        }
                        received.Append(Encoding.ASCII.GetString(buffer, 0, read));
                    }
                    byte[] response = Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\nContent-Length: 2\r\nConnection: close\r\n\r\nok");
                    await stream.WriteAsync(response, 0, response.Length);
                }
            }

            public void Dispose()
            {
                listener.Stop();
            }
        }
    }
}
