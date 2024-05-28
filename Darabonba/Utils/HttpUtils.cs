using System.Net.Http;

namespace Darabonba.Utils
{
    internal static class HttpUtils
    {
        internal static HttpMethod GetHttpMethod(string method)
        {
            switch (method.ToSafeString(string.Empty).ToLower())
            {
                case "get":
                    return HttpMethod.Get;
                case "post":
                    return HttpMethod.Post;
                case "delete":
                    return HttpMethod.Delete;
                case "put":
                    return HttpMethod.Put;
                case "head":
                    return HttpMethod.Head;
                case "options":
                    return HttpMethod.Options;
                case "trace":
                    return HttpMethod.Trace;
                default:
                    return HttpMethod.Get;
            }
        }
    }
}
