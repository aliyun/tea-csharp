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
using Darabonba.RetryPolicy;
using Darabonba.Exceptions;
using Darabonba.Utils;

namespace Darabonba
{
    public class Core
    {
        private static readonly int bufferLength = 1024;
        private static readonly int DefaultMaxAttempts = 3;
        private static List<string> bodyMethod = new List<string> { "POST", "PUT", "PATCH" };
        private static readonly TimeSpan DefaultMinDelay = TimeSpan.FromMilliseconds(100);
        private static readonly TimeSpan DefaultMaxDelay = TimeSpan.FromSeconds(120);


        internal static string ComposeUrl(Request request)
        {
            var urlBuilder = new StringBuilder("");

            urlBuilder.Append(ConverterUtil.StrToLower(request.Protocol)).Append("://");
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
                    if (val == null)
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

        public static Response DoAction(Request request)
        {
            return DoAction(request, new Dictionary<string, object>());
        }

        public static Response DoAction(Request request, Dictionary<string, object> runtimeOptions)
        {
            int timeout;
            var url = ComposeUrl(request);
            Uri uri = new Uri(url);
            HttpRequestMessage req = GetRequestMessage(request, runtimeOptions, out timeout);

            try
            {
                HttpClient httpClient = HttpClientUtils.GetOrAddHttpClient(request.Protocol, uri.Host, uri.Port, runtimeOptions);
                HttpResponseMessage response = httpClient.SendAsync(req, new CancellationTokenSource(timeout).Token).Result;
                return new Response(response);
            }
            catch (TaskCanceledException)
            {
                throw new WebException("operation is timeout");
            }
        }

        public static async Task<Response> DoActionAsync(Request request)
        {
            return await DoActionAsync(request, new Dictionary<string, object>());
        }

        public static async Task<Response> DoActionAsync(Request request, Dictionary<string, object> runtimeOptions)
        {
            int timeout;
            var url = ComposeUrl(request);
            Uri uri = new Uri(url);
            HttpRequestMessage req = GetRequestMessage(request, runtimeOptions, out timeout);

            try
            {
                HttpClient httpClient = HttpClientUtils.GetOrAddHttpClient(request.Protocol, uri.Host, uri.Port, runtimeOptions);
                HttpResponseMessage response = await httpClient.SendAsync(req, new CancellationTokenSource(timeout).Token);
                return new Response(response);
            }
            catch (TaskCanceledException)
            {
                throw new WebException("operation is timeout");
            }
        }

        internal static string GetResponseBody(Response response)
        {
            using (var ms = new MemoryStream())
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

        internal static Dictionary<string, string> ConvertHeaders(WebHeaderCollection headers)
        {
            var result = new Dictionary<string, string>();
            for (int i = 0; i < headers.Count; i++)
            {
                result.Add(ConverterUtil.StrToLower(headers.GetKey(i)), headers.Get(i));
            }
            return result;
        }

        internal static Dictionary<string, string> ConvertHeaders(HttpResponseHeaders headers)
        {
            var result = new Dictionary<string, string>();
            var enumerator = headers.GetEnumerator();
            foreach (var item in headers)
            {
                result.Add(ConverterUtil.StrToLower(item.Key), item.Value.First());
            }
            return result;
        }

        internal static bool AllowRetry(IDictionary dict, int retryTimes, long now)
        {
            if (retryTimes == 0)
            {
                return true;
            }

            if (!dict.Get("retryable").ToSafeBool(false))
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

        public static bool ShouldRetry(RetryOptions options, RetryPolicyContext ctx)
        {
            if (ctx.RetriesAttempted == 0)
            {
                return true;
            }
            if (options == null || options.Retryable == null || options.Retryable == false)
            {
                return false;
            }

            if (ctx.Exception is DaraException)
            {
                var daraException = (DaraException)ctx.Exception;
                List<RetryCondition> noRetryConditions = options.NoRetryCondition;
                List<RetryCondition> retryConditions = options.RetryCondition;

                if (noRetryConditions != null)
                {
                    foreach (var noRetryCondition in noRetryConditions)
                    {
                        if (DictUtils.Contains(noRetryCondition.Exception, daraException.Message) ||
                            DictUtils.Contains(noRetryCondition.ErrorCode, daraException.Code))
                        {
                            return false;
                        }
                    }
                }
                if (retryConditions != null)
                {
                    foreach (var retryCondition in retryConditions)
                    {
                        if (!DictUtils.Contains(retryCondition.Exception, daraException.Message) &&
                            !DictUtils.Contains(retryCondition.ErrorCode, daraException.Code))
                        {
                            continue;
                        }

                        if (ctx.RetriesAttempted > retryCondition.MaxAttempts)
                        {
                            return false;
                        }

                        return true;
                    }
                }
            }
            return false;
        }

        internal static int GetBackoffTime(IDictionary Idict, int retryTimes)
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

        public static int GetBackoffDelay(RetryOptions options, RetryPolicyContext ctx)
        {
            if (ctx.RetriesAttempted == null || ctx.RetriesAttempted == 0)
            {
                return 0;
            }

            if (ctx.Exception is DaraException)
            {
                if (options != null)
                {
                    var daraException = (DaraException)ctx.Exception;
                    var retryConditions = options.RetryCondition;
                    if (retryConditions != null)
                    {
                        foreach (var retryCondition in retryConditions)
                        {
                            if (!DictUtils.Contains(retryCondition.Exception, daraException.Message) && !DictUtils.Contains(retryCondition.ErrorCode, daraException.Code))
                            {
                                continue;
                            }
                            long maxDelay = retryCondition.MaxDelayTimeMillis ?? (long)DefaultMaxDelay.TotalMilliseconds;
                            if (daraException is DaraResponseException)
                            {
                                var responseException = (DaraResponseException)daraException;
                                if (responseException.RetryAfter != null)
                                {
                                    return (int)Math.Min(responseException.RetryAfter.Value, maxDelay);
                                }
                            }
                            if (retryCondition.Backoff == null)
                            {
                                return (int)DefaultMinDelay.TotalMilliseconds;
                            }
                            var delayTimeMillis = retryCondition.Backoff.GetDelayTime(ctx);
                            long delayTime = delayTimeMillis != null ? (long)delayTimeMillis : (long)DefaultMinDelay.TotalMilliseconds;
                            return (int)Math.Min(delayTime, maxDelay);
                        }
                    }
                }
            }
            return  (int)DefaultMinDelay.TotalMilliseconds;
        }

        public static void Sleep(int backoffTime)
        {
            Thread.Sleep(backoffTime);
        }

        public static async Task SleepAsync(int backoffTime)
        {
            await Task.Delay(backoffTime);
        }

        internal static bool IsRetryable(Exception e)
        {
            return e is DaraRetryableException;
        }

        public static Dictionary<string, object> ToObject(IDictionary dictionary)
        {
            var result = new Dictionary<string, object>();

            foreach (DictionaryEntry entry in dictionary)
            {
                result[entry.Key.ToString()] = entry.Value; // 将值存储为 object
            }

            return result;
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
                    stringBuilder.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)c));
                }
            }

            return stringBuilder.ToString();
        }

        internal static HttpRequestMessage GetRequestMessage(Request request, Dictionary<string, object> runtimeOptions, out int timeout)
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

            if (request.Body != null)
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

        public static Exception ThrowException(RetryPolicyContext context)
        {
            if (context.Exception is IOException)
            {
                return new DaraUnRetryableException(context);
            }

            return context.Exception;
        }
    }
}
