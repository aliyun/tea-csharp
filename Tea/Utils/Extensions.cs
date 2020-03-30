using System;
using System.Collections;
using System.Collections.Generic;

namespace Tea.Utils
{
    public static class Extensions
    {
        public static string ToSafeString(this object obj, string defaultStr = null)
        {
            try
            {
                return obj.ToString();
            }
            catch
            {
                return defaultStr;
            }
        }

        public static int? ToSafeInt(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            try
            {
                return Convert.ToInt32(obj);
            }
            catch
            {
                return null;
            }
        }

        public static int ToSafeInt(this object obj, int defaultStr)
        {
            try
            {
                return Convert.ToInt32(obj);
            }
            catch
            {
                return defaultStr;
            }
        }

        public static bool ToSafeBool(this object obj, bool defaultBool = false)
        {
            try
            {
                return Convert.ToBoolean(obj);
            }
            catch
            {
                return defaultBool;
            }
        }

        public static double? ToSafeDouble(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            try
            {
                return Convert.ToDouble(obj);
            }
            catch
            {
                return null;
            }
        }

        public static double ToSafeDouble(this object obj, double defaultDouble)
        {
            try
            {
                return Convert.ToDouble(obj);
            }
            catch
            {
                return defaultDouble;
            }
        }

        public static float? ToSafeFloat(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            try
            {
                return Convert.ToSingle(obj);
            }
            catch
            {
                return null;
            }
        }

        public static float ToSafeFloat(this object obj, float defaultFloat)
        {
            try
            {
                return Convert.ToSingle(obj);
            }
            catch
            {
                return defaultFloat;
            }
        }

        public static string Get(this Dictionary<string,string> dic, string key, string defaultVal = null)
        {
            if(dic == null)
            {
                return null;
            }

            string value = string.Empty;
            if(dic.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                return defaultVal;
            }
        }

        public static object Get(this IDictionary dic, string key, object defaultVal = null)
        {
            if (dic == null)
            {
                return null;
            }
            if(dic.Contains(key))
            {
                return dic[key];
            }
            else
            {
                return null;
            }
        }

    }
}
