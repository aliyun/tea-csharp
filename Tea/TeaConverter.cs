using System;
using System.Collections.Generic;

namespace Tea
{
    public class TeaConverter
    {
        public static Dictionary<string, T> merge<T>(params object[] objs)
        {
            Dictionary<string, T> dicResult = new Dictionary<string, T>();
            if (objs == null)
            {
                return dicResult;
            }

            foreach (object obj in objs)
            {
                if (obj == null)
                {
                    continue;
                }
                Dictionary<string, object> dicObj = new Dictionary<string, object>();
                Type typeObj = obj.GetType();
                if (typeof(TeaModel).IsAssignableFrom(typeObj))
                {
                    dicObj = ((TeaModel) obj).ToMap();
                }
                else if (obj is Dictionary<string, object>)
                {
                    dicObj = (Dictionary<string, object>) obj;
                }
                else if (obj is Dictionary<string, string>)
                {
                    Dictionary<string, string> dicString = (Dictionary<string, string>) obj;
                    foreach (var keypair in dicString)
                    {
                        dicObj.Add(keypair.Key, keypair.Value);
                    }
                }
                else
                {
                    throw new ArgumentException(" inparams only support Dictionary or TeaModel. ");
                }

                foreach (var keypair in dicObj)
                {
                    T dicValue = (T) keypair.Value;
                    if (dicResult.ContainsKey(keypair.Key))
                    {
                        dicResult[keypair.Key] = dicValue;
                    }
                    else
                    {
                        dicResult.Add(keypair.Key, dicValue);
                    }
                }
            }
            return dicResult;
        }

        public static string StrToLower(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }
            else
            {
                return str.ToLower();
            }
        }
    }
}
