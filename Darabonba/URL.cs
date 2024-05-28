using System;
using System.Globalization;
using System.Net;
using System.Text;

namespace Darabonba
{
    public class URL
    {
        private Uri _uri;

        public URL(string str)
        {
            _uri = new Uri(str);
        }

        public string Path()
        {
            return _uri.AbsolutePath + _uri.Query;
        }

        public string Pathname()
        {
            return _uri.AbsolutePath;
        }

        public string Protocol()
        {
            return _uri.Scheme;
        }

        public string Hostname()
        {
            return _uri.Host;
        }

        public string Host()
        {
            return _uri.Authority;
        }

        public string Port()
        {
            return _uri.Port == -1 ? "" : _uri.Port.ToString();
        }

        public string Hash()
        {
            return string.IsNullOrWhiteSpace(_uri.Fragment) ? "" : _uri.Fragment.TrimStart('#');
        }

        public string Search()
        {
            return string.IsNullOrWhiteSpace(_uri.Query) ? "" : _uri.Query.TrimStart('?');
        }

        public string Href()
        {
            return _uri.ToString();
        }

        public string Auth()
        {
            return _uri.UserInfo;
        }


        public static URL Parse(string url)
        {
            return new URL(url);
        }

        public static string UrlEncode(string url)
        {
            return url != null ? WebUtility.UrlEncode(url) : string.Empty;
        }

        public static string PercentEncode(string value)
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

        public static string PathEncode(string path)
        {
            if (string.IsNullOrEmpty(path) || path == "/")
            {
                return path;
            }
            var paths = path.Split('/');
            var stringBuilder = new StringBuilder();

            foreach (var s in paths)
            {
                stringBuilder.Append('/').Append(PercentEncode(s));
            }

            return "/" + stringBuilder.ToString().TrimStart('/');
        }
    }
}