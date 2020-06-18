using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

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
#if NET45
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
            httpClient = new HttpClient(httpClientHandler);
#endif
            return httpClient;

        }
    }
}
