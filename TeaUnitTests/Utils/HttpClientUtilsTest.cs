using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Tea.Utils;
using Xunit;

namespace TeaUnitTests.Utils
{
    public class HttpClientUtilsTest
    {
        [Fact]
        public void Test_CreatHttpClient()
        {
            var runtime = new Dictionary<string, object>();
            runtime.Add("cert", @"-----BEGIN CERTIFICATE-----
MIIBvDCCAWYCCQDKjNYQxar0mjANBgkqhkiG9w0BAQsFADBlMQswCQYDVQQGEwJh
czEMMAoGA1UECAwDYXNmMQwwCgYDVQQHDANzYWQxCzAJBgNVBAoMAnNkMQ0wCwYD
VQQLDARxd2VyMQswCQYDVQQDDAJzZjERMA8GCSqGSIb3DQEJARYCd2UwHhcNMjAx
MDE5MDI0MDMwWhcNMzAxMDE3MDI0MDMwWjBlMQswCQYDVQQGEwJhczEMMAoGA1UE
CAwDYXNmMQwwCgYDVQQHDANzYWQxCzAJBgNVBAoMAnNkMQ0wCwYDVQQLDARxd2Vy
MQswCQYDVQQDDAJzZjERMA8GCSqGSIb3DQEJARYCd2UwXDANBgkqhkiG9w0BAQEF
AANLADBIAkEA3kjVUItivYiVMusWnBQZyfCXzKLpXwx3D6bLF+6w2ARaIf8VDhMB
mwi96Is7P0ZxzybaZlLGy3aoYgjTSxymIQIDAQABMA0GCSqGSIb3DQEBCwUAA0EA
ZjePopbFugNK0US1MM48V1S2petIsEcxbZBEk/wGqIzrY4RCFKMtbtPSgTDUl3D9
XePemktG22a54ItVJ5FpcQ==
-----END CERTIFICATE-----");
            runtime.Add("key", @"-----BEGIN RSA PRIVATE KEY-----
MIIBPAIBAAJBAN5I1VCLYr2IlTLrFpwUGcnwl8yi6V8Mdw+myxfusNgEWiH/FQ4T
AZsIveiLOz9Gcc8m2mZSxst2qGII00scpiECAwEAAQJBAJZEhnA8yjN28eXKJy68
J/LsQrKEL1+h/ZsHFqTHJ6XfiA0CXjbjPsa4jEbpyilMTSgUyoKdJ512ioeco2n6
xUECIQD/JUHaKSuxz55t3efKdppqfopb92mJ2NuPJgrJI70OCwIhAN8HZ0bzr/4a
DLvYCDUKvOj3GzsV1dtBwWuHBaZEafQDAiEAtTnrel//7z5/U55ow4BW0gmrkQM9
bXIhEZ59zryZzl0CIQDFmBqRCu9eshecCP7kd3n88IjopSTOV4iUypBfyXcRnwIg
eXNxUx+BCu2We36+c0deE2+vizL1s6f5XhE6l4bqtiM=
-----END RSA PRIVATE KEY-----");
            runtime.Add("ca", @"-----BEGIN CERTIFICATE-----
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
-----END CERTIFICATE-----");
            runtime.Add("ignoreSSL", true);

            var httpClient = HttpClientUtils.CreatHttpClient(runtime);
            Assert.NotNull(httpClient); 
        }


        [Fact]
        public void Test_CertificateValidationCallBack()
        {
            Assert.True(HttpClientUtils.CertificateValidationCallBack(null, new X509Certificate2Collection(), null, null, System.Net.Security.SslPolicyErrors.None));

            string cert = @"-----BEGIN CERTIFICATE-----
MIICVDCCAb0CFAexGL8aJihganALdhlA3+2PuEE4MA0GCSqGSIb3DQEBCwUAMGkx
CzAJBgNVBAYTAmFhMQswCQYDVQQIDAJiYjELMAkGA1UEBwwCY2MxCzAJBgNVBAoM
AmRkMQswCQYDVQQLDAJlZTELMAkGA1UEAwwCZmYxGTAXBgkqhkiG9w0BCQEWCmFs
aUBxcS5jb20wHhcNMjAxMDIxMDE1NzA0WhcNMjExMDIxMDE1NzA0WjBpMQswCQYD
VQQGEwJhYTELMAkGA1UECAwCYmIxCzAJBgNVBAcMAmNjMQswCQYDVQQKDAJkZDEL
MAkGA1UECwwCZWUxCzAJBgNVBAMMAmZmMRkwFwYJKoZIhvcNAQkBFgphbGlAcXEu
Y29tMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDDmTJt4TNH5FcpJ0Lj6cIX
IDrkjqveRjg62TT4vBV1UzV/W3hIaMYWTrW3LyqQMq3kIPNbjyJoZlfXosRjhW/Z
B1Xxl0rtCvp6YSKy2N+wSvyM5pyWhHXg0xoSQYg9aIqOel67ZRqF/XcADjXCAFNq
4nxY1jb2YiaP1q24QpKWPQIDAQABMA0GCSqGSIb3DQEBCwUAA4GBACRTgas7Q0KV
TcUDwSdWCCoTHVDpsk7kFZF+tVFsssCnA5RKqf6WsCncnHP+T7hSnX3ZfNwg6DFv
PoE+j53QJ5tkE0TEmffzXaywsKh9rwK4GRaOUTc43cWQU4G6Yv0uWzmgO/Xx9XJ7
juPBOeucQtbgVjyRswrYbqGR6vdBgA+a
-----END CERTIFICATE-----";

            string certErr = @"-----BEGIN CERTIFICATE-----
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
            byte[] certBytes = Encoding.UTF8.GetBytes(cert);
            byte[] certErrBytes = Encoding.UTF8.GetBytes(certErr);

            Assert.False(HttpClientUtils.CertificateValidationCallBack(null, 
                new X509Certificate2Collection(), new X509Certificate2(certBytes),
                new X509Chain(), 
                System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors));

            var collection = new X509Certificate2Collection();
            collection.Add(new X509Certificate2(certErrBytes));
            Assert.False(HttpClientUtils.CertificateValidationCallBack(null,
                collection, new X509Certificate2(certBytes),
                new X509Chain(),
                System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors));

            collection = new X509Certificate2Collection();
            collection.Add(new X509Certificate2(certBytes));
            Assert.True(HttpClientUtils.CertificateValidationCallBack(null,
                collection, new X509Certificate2(certBytes),
                new X509Chain(),
                System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors));
        }
    }
}
