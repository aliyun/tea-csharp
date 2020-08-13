using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Tea.Utils;

namespace Tea
{
    public class TeaCore
    {
        private static readonly int bufferLength = 1024;
        private static List<string> bodyMethod = new List<string> { "POST", "PUT", "PATCH" };

        public static string ComposeUrl(TeaRequest request)
        {
            var urlBuilder = new StringBuilder("");

            urlBuilder.Append(TeaConverter.StrToLower(request.Protocol)).Append("://");
            urlBuilder.Append(DictUtils.GetDicValue(request.Headers, "host"));
            if (request.Port > 0)
            {
                urlBuilder.Append(":");
                urlBuilder.Append(request.Port);
            }
            urlBuilder.Append(request.Pathname);

            if (request.Query != null && request.Query.Count > 0)
            {
                if (urlBuilder.ToString().Contains("?"))
                {
                    if (!urlBuilder.ToString().EndsWith("&"))
                    {
                        urlBuilder.Append("&");
                    }
                }
                else
                {
                    urlBuilder.Append("?");
                }
                foreach (var entry in request.Query)
                {
                    var key = entry.Key;
                    var val = entry.Value;
                    if (string.IsNullOrWhiteSpace(val))
                    {
                        continue;
                    }
                    urlBuilder.Append(PercentEncode(key));
                    urlBuilder.Append("=");
                    urlBuilder.Append(PercentEncode(val));
                    urlBuilder.Append("&");
                }
            }
            return urlBuilder.ToString().TrimEnd('?').TrimEnd('&');
        }

        public static TeaResponse DoAction(TeaRequest request)
        {
            return DoAction(request, new Dictionary<string, object>());
        }

        public static TeaResponse DoAction(TeaRequest request, Dictionary<string, object> runtimeOptions)
        {
            int timeout;
            var url = ComposeUrl(request);
            Uri uri = new Uri(url);
            HttpRequestMessage req = GetRequestMessage(request, runtimeOptions, out timeout);

            try
            {
                HttpClient httpClient = HttpClientUtils.GetOrAddHttpClient(request.Protocol,uri.Host, uri.Port, runtimeOptions);
                HttpResponseMessage response = httpClient.SendAsync(req, new CancellationTokenSource(timeout).Token).Result;
                return new TeaResponse(response);
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                throw new WebException("operation is timeout");
            }
        }

        public static async Task<TeaResponse> DoActionAsync(TeaRequest request)
        {
            return await DoActionAsync(request, new Dictionary<string, object>());
        }

        public static async Task<TeaResponse> DoActionAsync(TeaRequest request, Dictionary<string, object> runtimeOptions)
        {
            int timeout;
            var url = ComposeUrl(request);
            Uri uri = new Uri(url);
            HttpRequestMessage req = GetRequestMessage(request, runtimeOptions, out timeout);

            try
            {
                HttpClient httpClient = HttpClientUtils.GetOrAddHttpClient(request.Protocol, uri.Host, uri.Port, runtimeOptions);
                HttpResponseMessage response = await httpClient.SendAsync(req, new CancellationTokenSource(timeout).Token);
                return new TeaResponse(response);
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                throw new WebException("operation is timeout");
            }
        }

        public static string GetResponseBody(TeaResponse response)
        {
            using(var ms = new MemoryStream())
            {
                var buffer = new byte[bufferLength];
                var stream = response.Body;

                while (true)
                {
                    var length = stream.Read(buffer, 0, bufferLength);
                    if (length == 0)
                    {
                        break;
                    }

                    ms.Write(buffer, 0, length);
                }

                ms.Seek(0, SeekOrigin.Begin);
                var bytes = new byte[ms.Length];
                ms.Read(bytes, 0, bytes.Length);

                stream.Close();
                stream.Dispose();

                return Encoding.UTF8.GetString(bytes);
            }
        }

        public static Dictionary<string, string> ConvertHeaders(WebHeaderCollection headers)
        {
            var result = new Dictionary<string, string>();
            for (int i = 0; i < headers.Count; i++)
            {
                result.Add(TeaConverter.StrToLower(headers.GetKey(i)), headers.Get(i));
            }
            return result;
        }

        public static Dictionary<string, string> ConvertHeaders(HttpResponseHeaders headers)
        {
            var result = new Dictionary<string, string>();
            var enumerator = headers.GetEnumerator();
            foreach (var item in headers)
            {
                result.Add(TeaConverter.StrToLower(item.Key), item.Value.First());
            }
            return result;
        }

        public static bool AllowRetry(IDictionary dict, int retryTimes, long now)
        {
            if(retryTimes == 0)
            {
                return true;
            }

            if(!dict.Get("retryable").ToSafeBool(false))
            {
                return false;
            }
            
            int retry;
            if (dict == null)
            {
                return false;
            }
            Dictionary<string, object> dictObj = dict.Keys.Cast<string>().ToDictionary(key => key, key => dict[key]);
            if (!dictObj.ContainsKey("maxAttempts"))
            {
                return false;
            }
            else
            {
                retry = dictObj["maxAttempts"] == null ? 0 : Convert.ToInt32(dictObj["maxAttempts"]);
            }

            return retry >= retryTimes;
        }

        public static int GetBackoffTime(IDictionary Idict, int retryTimes)
        {
            int backOffTime = 0;
            Dictionary<string, object> dict = Idict.Keys.Cast<string>().ToDictionary(key => key, key => Idict[key]);
            if (!dict.ContainsKey("policy") || dict["policy"] == null ||
                string.IsNullOrWhiteSpace(dict["policy"].ToString()) || dict["policy"].ToString() == "no")
            {
                return backOffTime;
            }

            if (dict.ContainsKey("period") && dict["period"] != null)
            {
                int.TryParse(dict["period"].ToString(), out backOffTime);
                if (backOffTime <= 0)
                {
                    return retryTimes;
                }
            }
            return backOffTime;
        }

        public static void Sleep(int backoffTime)
        {
            Thread.Sleep(backoffTime);
        }

        public static async Task SleepAsync(int backoffTime)
        {
            await Task.Run(() =>
            {
                Thread.Sleep(1000);
            });
        }

        public static bool IsRetryable(Exception e)
        {
            return e is TeaRetryableException;
        }

        public static Stream BytesReadable(string str)
        {
            return BytesReadable(Encoding.UTF8.GetBytes(str));
        }

        public static Stream BytesReadable(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        internal static string PercentEncode(string value)
        {
            if (value == null)
            {
                return null;
            }
            var stringBuilder = new StringBuilder();
            var text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            var bytes = Encoding.UTF8.GetBytes(value);
            foreach (char c in bytes)
            {
                if (text.IndexOf(c) >= 0)
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int) c));
                }
            }

            return stringBuilder.ToString();
        }

        internal static HttpRequestMessage GetRequestMessage(TeaRequest request, Dictionary<string, object> runtimeOptions, out int timeout)
        {
            var url = ComposeUrl(request);
            HttpRequestMessage req = new HttpRequestMessage();
            req.Method = HttpUtils.GetHttpMethod(request.Method);
            req.RequestUri = new Uri(url);
            timeout = 100000;
            int readTimeout = DictUtils.GetDicValue(runtimeOptions, "readTimeout").ToSafeInt(0);
            int connectTimeout = DictUtils.GetDicValue(runtimeOptions, "connectTimeout").ToSafeInt(0);

            if (readTimeout != 0 || connectTimeout != 0)
            {
                timeout = readTimeout + connectTimeout;
            }

            if (bodyMethod.Contains(request.Method) && request.Body != null)
            {
                Stream requestStream = new MemoryStream();
                request.Body.Position = 0;

                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = request.Body.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }

                requestStream.Position = 0;
                StreamContent streamContent = new StreamContent(requestStream);
                req.Content = streamContent;
            }

            foreach (var header in request.Headers)
            {
                req.Headers.TryAddWithoutValidation(header.Key, header.Value);
                if (header.Key.ToLower().StartsWith("content-") && req.Content != null)
                {
                    req.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return req;
        }

    }
}
