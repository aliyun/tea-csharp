using System.Collections.Generic;

namespace Darabonba.Utils
{
    internal class DictUtils
    {
        internal static object GetDicValue(Dictionary<string, object> dic, string keyName)
        {
            if (dic.ContainsKey(keyName))
            {
                return dic[keyName];
            }
            return null;
        }

        internal static string GetDicValue(Dictionary<string, string> dic, string keyName)
        {
            if (dic != null && dic.ContainsKey(keyName))
            {
                return dic[keyName];
            }
            return null;
        }

        internal static bool Contains(List<string> list, string value)
        {
            if (list == null)
            {
                return false;
            }
            foreach (var item in list)
            {
                if (value != null && item != null && item == value)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
