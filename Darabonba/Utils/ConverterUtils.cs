using System;
using System.Collections.Generic;
using Darabonba.Exceptions;

namespace Darabonba.Utils
{
    public class ConverterUtils
    {
        public static Dictionary<string, object> Merge(Dictionary<string, object> dic1, Dictionary<string, object> dic2)
        {
            object[] objs = new object[] { dic1, dic2 };
            return Merge<object>(objs);
        }

        public static Dictionary<string, T> Merge<T>(params object[] objs)
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
                if (typeof(Model).IsAssignableFrom(typeObj))
                {
                    dicObj = ((Model)obj).ToMap();
                }
                else if (obj is Dictionary<string, object>)
                {
                    dicObj = (Dictionary<string, object>)obj;
                }
                else if (obj is Dictionary<string, string>)
                {
                    Dictionary<string, string> dicString = (Dictionary<string, string>)obj;
                    foreach (var keypair in dicString)
                    {
                        dicObj.Add(keypair.Key, keypair.Value);
                    }
                }
                else
                {
                    throw new ArgumentException(" inparams only support Dictionary or Darabonba.Model. ");
                }

                foreach (var keypair in dicObj)
                {
                    T dicValue = (T)keypair.Value;
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

        public static int ParseInt<T>(T data)
        {
            if (data == null)
            {
                throw new DaraException
                {
                    Message = "Data is null."
                };
            }

            return (int)double.Parse(data.ToString());
        }

        public static long ParseLong<T>(T data)
        {
            if (data == null)
            {
                throw new DaraException
                {
                    Message = "Data is null."
                };
            }

            var str = new List<string>();
            
            
            return (long)double.Parse(data.ToString());
        }

        public static float ParseFloat<T>(T data)
        {
            if (data == null)
            {
                throw new DaraException
                {
                    Message = "Data is null.."
                };
            }
            return (float)double.Parse(data.ToString());
        }

        public static bool ParseBool<T>(T data)
        {
            if (data == null)
            {
                throw new DaraException
                {
                    Message = "Data is null.."
                };
            }

            var stringValue = data.ToString();

            if (stringValue.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                stringValue.Equals("1"))
            {
                return true;
            }
            if (stringValue.Equals("false", StringComparison.OrdinalIgnoreCase) ||
                     stringValue.Equals("0"))
            {
                return false;
            }
            throw new DaraException
            {
                Message = "Cannot convert data to bool."
            };
        }
    }
}
