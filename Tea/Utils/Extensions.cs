using System;

namespace Tea.Utils
{
    internal static class Extensions
    {
        internal static string ToSafeString(this object obj, string defaultStr = null)
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

        internal static int? ToSafeInt(this object obj)
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

        internal static int ToSafeInt(this object obj, int defaultStr)
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
    }
}
