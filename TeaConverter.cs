using System.Collections.Generic;

namespace Tea
{
    public class TeaConverter
    {
        public static Dictionary<string, object> merge(params Dictionary<string, object>[] dics)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (dics == null)
            {
                return dic;
            }

            foreach (Dictionary<string, object> dicItem in dics)
            {
                if (dicItem == null) { continue; }

                foreach (string key in dicItem.Keys)
                {
                    if (dic.ContainsKey(key))
                    {
                        dic[key] = dicItem[key];
                    }
                    else
                    {
                        dic.Add(key, dicItem[key]);
                    }
                }
            }
            return dic;
        }
    }
}
