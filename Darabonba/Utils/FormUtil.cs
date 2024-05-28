using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace Darabonba.Utils
{
    public class FormUtil
    {
        public static string ToFormString(Dictionary<string, object> map)
        {
            if (map == null || map.Count <= 0)
            {
                return "";
            }
            StringBuilder result = new StringBuilder();
            bool first = true;
            foreach (var entry in map)
            {
                if (entry.Value == null)
                {
                    continue;
                }
                if (first)
                {
                    first = false;
                }
                else
                {
                    result.Append("&");
                }
                result.Append(HttpUtility.UrlEncode(entry.Key, Encoding.UTF8));
                result.Append("=");
                result.Append(HttpUtility.UrlEncode(entry.Value.ToSafeString(""), Encoding.UTF8));
            }
            return result.ToString();
        }

        public static string GetBoundary()
        {
            long num = (long)Math.Floor((new Random()).NextDouble() * 100000000000000D); ;
            return num.ToString();
        }

        public static Stream ToFileForm(Dictionary<string, object> form, string boundary)
        {
            return new FileFormStream(form, boundary);
        }
    }
}