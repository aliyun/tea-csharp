using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Tea.Utils
{
    public static class HttpClientUtils
    {
        public static Func<object , X509Certificate2Collection, System.Security.Cryptography.X509Certificates.X509Certificate, X509Chain, SslPolicyErrors, bool> ServerCertificateCustomValidationCallback { get; set; }
        internal static ConcurrentDictionary<string, HttpClient> httpClients = new ConcurrentDictionary<string, HttpClient>();

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
                httpClients[key] = httpClient;
            }

            return httpClient;
        }

        internal static HttpClient CreatHttpClient(Dictionary<string, object> options)
        {
            HttpClient httpClient;
            string proxyUrl = options.Get("httpProxy") != null ? options.Get("httpProxy").ToSafeString() : options.Get("httpsProxy").ToSafeString();
            bool ignoreSSL = options.Get("ignoreSSL").ToSafeBool(false);

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

            if(ignoreSSL)
            {
                handler.ServerCertificateValidationCallback = delegate { return true; };
            }
            else
            {
                string ca = options.Get("ca") != null ? options.Get("ca").ToSafeString() : options.Get("ca").ToSafeString();
                if (!string.IsNullOrWhiteSpace(ca))
                {

                    handler.ServerCertificateValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                    {
                        var collection = new X509Certificate2Collection();
                        byte[] certBytes = Encoding.UTF8.GetBytes(ca);
                        X509Certificate2 cacert = new X509Certificate2(certBytes);
                        collection.Add(cacert);
                        if (ServerCertificateCustomValidationCallback != null)
                        {
                            return ServerCertificateCustomValidationCallback(message, collection, cert, chain, sslPolicyErrors);
                        }
                        return CertificateValidationCallBack(message, collection, cert, chain, sslPolicyErrors);
                    };

                }

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
            
            if (ignoreSSL)
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;
            }
            else
            {
                string ca = options.Get("ca") != null ? options.Get("ca").ToSafeString() : options.Get("ca").ToSafeString();
                if(!string.IsNullOrWhiteSpace(ca))
                {

                    httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                    {
                   
                        var collection = new X509Certificate2Collection();
                        byte[] certBytes = Encoding.UTF8.GetBytes(ca);
                        X509Certificate2 cacert = new X509Certificate2(certBytes);
                        collection.Add(cacert);
                        if (ServerCertificateCustomValidationCallback != null)
                        {
                            return ServerCertificateCustomValidationCallback(message, collection, cert, chain, sslPolicyErrors);
                        }
                        return CertificateValidationCallBack(message, collection, cert, chain, sslPolicyErrors);
                    };

                }

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
