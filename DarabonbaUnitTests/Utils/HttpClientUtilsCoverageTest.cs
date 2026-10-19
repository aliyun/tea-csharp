using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Darabonba.Utils;
using Xunit;

namespace DaraUnitTests.Utils
{
    public class HttpClientUtilsCoverageTest
    {
        [Fact]
        public void TestProxyCacheUsesProxyEndpointAndSeparatesProtocols()
        {
            var proxyHost = "coverage-" + Guid.NewGuid().ToString("N") + ".invalid";
            var proxy = "http://" + proxyHost + ":8080";
            var options = new Dictionary<string, object>
            {
                { "httpProxy", proxy },
                { "httpsProxy", "http://ignored.invalid:9090" }
            };
            try
            {
                var client = HttpClientUtils.GetOrAddHttpClient("https", "first.invalid", 443, options);
                var sameProxy = HttpClientUtils.GetOrAddHttpClient("https", "second.invalid", 8443,
                    new Dictionary<string, object> { { "httpsProxy", proxy } });
                var otherProtocol = HttpClientUtils.GetOrAddHttpClient("http", "first.invalid", 80, options);

                Assert.Same(client, sameProxy);
                Assert.NotSame(client, otherProtocol);
            }
            finally
            {
                foreach (var protocol in new[] { "http", "https" })
                {
                    HttpClient client;
                    if (HttpClientUtils.httpClients.TryRemove(protocol + ":" + proxyHost + ":8080", out client))
                    {
                        client.Dispose();
                    }
                }
            }
        }

        [Theory]
        [InlineData(SslPolicyErrors.RemoteCertificateNameMismatch)]
        [InlineData(SslPolicyErrors.RemoteCertificateNotAvailable)]
        public void TestCertificateValidationRejectsNonChainErrors(SslPolicyErrors errors)
        {
            Assert.False(HttpClientUtils.CertificateValidationCallBack(null,
                new X509Certificate2Collection(), null, null, errors));
        }

#if !NET45
        [Fact]
        public void TestCertificateValidationRequiresTheConfiguredRoot()
        {
            using (var key = RSA.Create(2048))
            using (var certificate = CreateCertificate("CN=Darabonba coverage root", key))
            using (var otherCertificate = CreateCertificate("CN=Darabonba other root", key))
            using (var chain = new X509Chain())
            {
                Assert.False(HttpClientUtils.CertificateValidationCallBack(null,
                    new X509Certificate2Collection(), certificate, chain,
                    SslPolicyErrors.RemoteCertificateChainErrors));
                Assert.False(HttpClientUtils.CertificateValidationCallBack(null,
                    new X509Certificate2Collection(otherCertificate), certificate, chain,
                    SslPolicyErrors.RemoteCertificateChainErrors));
                Assert.True(HttpClientUtils.CertificateValidationCallBack(null,
                    new X509Certificate2Collection(certificate), certificate, chain,
                    SslPolicyErrors.RemoteCertificateChainErrors));
            }
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async Task TestLocalTlsUsesConfiguredCertificateValidation(bool customValidation, bool ignoreSsl)
        {
            using (var certificate = CreateTlsCertificate())
            using (var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
            {
                var listener = new TcpListener(IPAddress.Loopback, 0);
                var previousCallback = HttpClientUtils.ServerCertificateCustomValidationCallback;
                var callbackInvoked = false;
                Task server = null;
                listener.Start();
                try
                {
                    HttpClientUtils.ServerCertificateCustomValidationCallback = customValidation
                        ? (sender, authorities, peer, chain, errors) =>
                        {
                            callbackInvoked = true;
                            Assert.Single(authorities);
                            Assert.Equal(certificate.RawData, authorities[0].RawData);
                            Assert.Equal(certificate.RawData, peer.GetRawCertData());
                            return true;
                        }
                        : (Func<object, X509Certificate2Collection, X509Certificate, X509Chain, SslPolicyErrors, bool>)null;
                    server = Task.Run(async () =>
                    {
                        using (timeout.Token.Register(listener.Stop))
                        using (var connection = await listener.AcceptTcpClientAsync())
                        using (var stream = new SslStream(connection.GetStream(), false))
                        {
                            await stream.AuthenticateAsServerAsync(new SslServerAuthenticationOptions
                            {
                                ServerCertificate = certificate,
                                EnabledSslProtocols = SslProtocols.Tls12
                            }, timeout.Token);
                            var request = new byte[4096];
                            var received = 0;
                            while (!Encoding.ASCII.GetString(request, 0, received).EndsWith("\r\n\r\n",
                                StringComparison.Ordinal))
                            {
                                var count = await stream.ReadAsync(request, received, request.Length - received, timeout.Token);
                                Assert.True(count > 0);
                                received += count;
                            }
                            var response = Encoding.ASCII.GetBytes(
                                "HTTP/1.1 200 OK\r\nContent-Length: 2\r\nConnection: close\r\n\r\nok");
                            await stream.WriteAsync(response, 0, response.Length, timeout.Token);
                        }
                    });
                    var options = ignoreSsl
                        ? new Dictionary<string, object> { { "ignoreSSL", true } }
                        : new Dictionary<string, object>
                        {
                            { "ca", "-----BEGIN CERTIFICATE-----\n" + Convert.ToBase64String(certificate.RawData)
                                + "\n-----END CERTIFICATE-----" }
                        };
                    using (var client = HttpClientUtils.CreatHttpClient(options))
                    {
                        client.Timeout = TimeSpan.FromSeconds(10);
                        var endpoint = (IPEndPoint)listener.LocalEndpoint;
                        Assert.Equal("ok", await client.GetStringAsync("https://127.0.0.1:" + endpoint.Port));
                        Assert.Equal(customValidation, callbackInvoked);
                    }
                    await server;
                }
                finally
                {
                    HttpClientUtils.ServerCertificateCustomValidationCallback = previousCallback;
                    listener.Stop();
                    timeout.Cancel();
                    if (server != null)
                    {
                        await server;
                    }
                }
            }
        }

        private static X509Certificate2 CreateTlsCertificate()
        {
            using (var key = RSA.Create(2048))
            {
                var request = new CertificateRequest("CN=localhost", key,
                    HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                var names = new SubjectAlternativeNameBuilder();
                names.AddDnsName("localhost");
                names.AddIpAddress(IPAddress.Loopback);
                request.CertificateExtensions.Add(names.Build());
                request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
                using (var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-5),
                    DateTimeOffset.UtcNow.AddDays(1)))
                {
                    // Schannel needs a provider-backed private key instead of an ephemeral key.
                    return new X509Certificate2(certificate.Export(X509ContentType.Pfx));
                }
            }
        }

        private static X509Certificate2 CreateCertificate(string subject, RSA key)
        {
            var request = new CertificateRequest(subject, key, HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
            request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
            return request.Create(request.SubjectName, X509SignatureGenerator.CreateForRSA(key,
                RSASignaturePadding.Pkcs1), DateTimeOffset.UtcNow.AddDays(-1),
                DateTimeOffset.UtcNow.AddDays(1), new byte[] { 1 });
        }
#endif
    }
}
