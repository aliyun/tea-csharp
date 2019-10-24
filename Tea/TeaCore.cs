using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Tea
{
    public class TeaCore
    {
        private static readonly int bufferLength = 1024;
        public static string ComposeUrl(TeaRequest request)
        {
            var urlBuilder = new StringBuilder("");

            urlBuilder.Append(request.Protocol.ToLower()).Append("://");
            urlBuilder.Append(request.Headers["host"]);
            if (request.Port > 0)
            {
                urlBuilder.Append(":");
                urlBuilder.Append(request.Port);
            }
            urlBuilder.Append(request.Pathname);

            if (request.Query != null && request.Query.Count > 0)
            {
                urlBuilder.Append("?");
                var i = 0;
                foreach (var entry in request.Query)
                {
                    var key = entry.Key;
                    var val = entry.Value;

                    urlBuilder.Append(key);
                    if (!string.IsNullOrEmpty(val))
                    {
                        urlBuilder.Append("=").Append(val);
                    }

                    if (i < request.Query.Count - 1)
                    {
                        urlBuilder.Append("&");
                    }
                    i = i + 1;
                }
            }

            return urlBuilder.ToString();
        }

        public static TeaResponse DoAction(TeaRequest request, Dictionary<string, object> runtimeOptions)
        {
            var url = TeaCore.ComposeUrl(request);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = request.Method;
            httpWebRequest.KeepAlive = true;

            foreach (var header in request.Headers)
            {
                httpWebRequest.Headers.Add(header.Key, header.Value);
            }

            if (runtimeOptions.ContainsKey("readTimeout") && !string.IsNullOrWhiteSpace(runtimeOptions["readTimeout"].ToString()))
            {
                httpWebRequest.ReadWriteTimeout = Convert.ToInt32(runtimeOptions["readTimeout"]);
            }

            if (runtimeOptions.ContainsKey("connectTimeout") && !string.IsNullOrWhiteSpace(runtimeOptions["connectTimeout"].ToString()))
            {
                httpWebRequest.Timeout = Convert.ToInt32(runtimeOptions["connectTimeout"]);
            }

            if (request.Method == "POST" && request.Body != null)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(request.Body);
                httpWebRequest.ContentLength = bytes.Length;
                httpWebRequest.GetRequestStream().Write(bytes, 0, bytes.Length);
            }

            HttpWebResponse httpWebResponse;

            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            return new TeaResponse(httpWebResponse);

        }

        public static string GetResponseBody(TeaResponse response)
        {
            using (var ms = new MemoryStream())
            {
                var buffer = new byte[bufferLength];
                var stream = response._Response.GetResponseStream();

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
                result.Add(headers.GetKey(i).ToLower(), headers.Get(i));
            }
            return result;
        }

        public static bool AllowRetry(Dictionary<string, object> dict, int retryTimes, long now)
        {
            int retry;
            if (dict == null || !dict.ContainsKey("maxAttempts"))
            {
                return false;
            }
            else
            {
                retry = dict["maxAttempts"] == null ? 0 : Convert.ToInt32(dict["maxAttempts"]);
            }

            return retry >= retryTimes;
        }

        public static int GetBackoffTime(Dictionary<string, object> dict, int retryTimes)
        {
            int backOffTime = 0;
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

        public static bool IsRetryable(Exception e)
        {
            return e is WebException || e is OperationCanceledException;
        }
    }
}
