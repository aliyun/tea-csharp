using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace Tea.Utils
{
    public class HttpClientUtils
    {
        internal static Dictionary<string, HttpClient> httpClients = new Dictionary<string, HttpClient>();

        internal static HttpClient GetOrAddHttpClient(string protocol,string host, int port, Dictionary<string, object> options)
        {
            string key;
            string proxyUrl = options.Get("httpProxy") != null ? options.Get("httpProxy").ToSafeString() : options.Get("httpsProxy").ToSafeString();
            
            if(!string.IsNullOrWhiteSpace(proxyUrl))
            {
                Uri uri = new Uri(proxyUrl);
                key = string.Format("{0}:{1}:{2}", protocol, uri.Host, uri.Port);
            }
            else
            {
                key = string.Format("{0}:{1}:{2}", protocol, host, port);
            }
            HttpClient httpClient = (HttpClient)httpClients.Get(key);

            if(httpClient == null)
            {
                httpClient = CreatHttpClient(options);
                httpClients.Add(key, httpClient);
            }

            return httpClient;
        }

        internal static HttpClient CreatHttpClient(Dictionary<string, object> options)
        {
            HttpClient httpClient;
            string proxyUrl = options.Get("httpProxy") != null ? options.Get("httpProxy").ToSafeString() : options.Get("httpsProxy").ToSafeString();
            bool ignoreSSL = options.Get("ignoreSSL").ToSafeBool(false);
            string certStr = options.Get("cert").ToSafeString();
            string keyStr = options.Get("key").ToSafeString();
            string caStr = options.Get("ca").ToSafeString();
            X509Certificate2Collection CaCerts = new X509Certificate2Collection();
            Org.BouncyCastle.X509.X509Certificate pemCert;
            AsymmetricKeyParameter pemKey;
            X509Certificate2 x509Cert = new X509Certificate2(new byte[0]);

            if (!string.IsNullOrWhiteSpace(certStr) && !string.IsNullOrWhiteSpace(keyStr))
            {
                byte[] certOption = Encoding.UTF8.GetBytes(certStr);
                byte[] keyOption = Encoding.UTF8.GetBytes(keyStr);

                if(!string.IsNullOrWhiteSpace(caStr))
                {
                    byte[] caOption = Encoding.UTF8.GetBytes(caStr);
                    X509Certificate2 rooCert = new X509Certificate2(caOption);
                    CaCerts.Add(rooCert);
                }
                

                pemCert = new X509CertificateParser().ReadCertificate(new MemoryStream(certOption));

                object obj;
                using (var reader = new StreamReader(new MemoryStream(keyOption)))
                {
                    obj = new PemReader(reader).ReadObject();
                    var keypair = obj as AsymmetricCipherKeyPair;
                    if (keypair != null)
                    {
                        var cipherKey = keypair;
                        obj = cipherKey.Private;
                    }
                }

                pemKey = (AsymmetricKeyParameter)obj;
                var builder = new Pkcs12StoreBuilder();
                builder.SetUseDerEncoding(true);
                var store = builder.Build();
                var certEntry = new X509CertificateEntry(pemCert);
                store.SetCertificateEntry("", certEntry);
                store.SetKeyEntry("", new AsymmetricKeyEntry(pemKey), new[] { certEntry });
                byte[] data;
                using (var ms = new MemoryStream())
                {
                    store.Save(ms, new char[0], new SecureRandom());
                    data = ms.ToArray();
                }

                x509Cert = new X509Certificate2(data);
            }

#if NET45
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
                                       | SecurityProtocolType.Tls
                                       | SecurityProtocolType.Tls11
                                       | SecurityProtocolType.Tls12;
            var handler = new WebRequestHandler();
            if(!string.IsNullOrWhiteSpace(proxyUrl))
            {
                handler.Proxy = new WebProxy(new Uri(proxyUrl));
            }
            else
            {
                handler.UseProxy = false;
            }

            if(!string.IsNullOrWhiteSpace(certStr) && !string.IsNullOrWhiteSpace(keyStr))
            {
                handler.ClientCertificates.Add(x509Cert);
                if(!ignoreSSL)
                {
                    handler.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                    {
                        return CertificateValidationCallBack(sender, CaCerts, certificate, chain,
                            sslPolicyErrors);
                    };
                }
            }

            if(ignoreSSL)
            {
                handler.ServerCertificateValidationCallback = delegate { return true; };
            }
            httpClient = new HttpClient(handler);
#else
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            if (!string.IsNullOrWhiteSpace(proxyUrl))
            {
                httpClientHandler.Proxy = new WebProxy(new Uri(proxyUrl));
            }
            else
            {
                httpClientHandler.UseProxy = false;
            }

            if(!string.IsNullOrWhiteSpace(certStr) && !string.IsNullOrWhiteSpace(keyStr))
            {
                httpClientHandler.ClientCertificates.Add(x509Cert);
                if(!ignoreSSL)
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                    {
                        return CertificateValidationCallBack(sender, CaCerts, certificate, chain,
                            sslPolicyErrors);
                    };
                }
            }

            if (ignoreSSL)
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;
            }
            httpClient = new HttpClient(httpClientHandler);
#endif
            return httpClient;

        }


        public static bool CertificateValidationCallBack(
            object sender,
            X509Certificate2Collection caCerts,
            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                // Added our trusted certificates to the chain
                //
                chain.ChainPolicy.ExtraStore.AddRange(caCerts);

                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                var isValid = chain.Build((X509Certificate2)certificate);

                var isTrusted = false;

                var rootCert = chain.ChainElements[chain.ChainElements.Count - 1].Certificate;

                // Make sure that one of our trusted certs exists in the chain provided by the server.
                //
                foreach (var cert in caCerts)
                {
                    if (rootCert.RawData.SequenceEqual(cert.RawData))
                    {
                        isTrusted = true;
                        break;
                    }
                }

                return isValid && isTrusted;
            }

            // In all other cases, return false.
            return false;
        }
    }
}
