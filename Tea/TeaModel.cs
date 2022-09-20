using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tea
{
    public class TeaModel
    {
        public static T ToObject<T>(IDictionary dict) where T : class, new()
        {
            var result = new T();
            if (dict == null)
            {
                return null;
            }
            Dictionary<string, object> dicObj = dict.Keys.Cast<string>().ToDictionary(key => key, key => dict[key]);
            return ToObject(dicObj, result);
        }

        public static T ToObject<T>(Dictionary<string, object> dict, T obj) where T : class
        {
            if (dict == null)
            {
                return null;
            }
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();
            //Properties Map
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo p = properties[i];
                var propertyType = p.PropertyType;
                NameInMapAttribute attribute = p.GetCustomAttribute(typeof(NameInMapAttribute)) as NameInMapAttribute;
                string realName = attribute == null ? p.Name : attribute.Name;
                if (dict.ContainsKey(realName))
                {
                    var value = dict[realName];
                    if (value == null)
                    {
                        p.SetValue(obj, value);
                        continue;
                    }
                    p.SetValue(obj, MapObj(propertyType, value));
                }
            }
            return obj;
        }

        private static object MapObj(Type propertyType, object value)
        {
            if (value == null)
            {
                return null;
            }
            else if (typeof(IList).IsAssignableFrom(value.GetType()) && !typeof(Array).IsAssignableFrom(value.GetType()))
            {
                var list = Activator.CreateInstance(propertyType);
                Type[] types = propertyType.GetGenericArguments();
                if (types.Length == 0 || types == null)
                {
                    return value;
                }
                Type innerPropertyType = types[0];
                foreach (var temp in (IList) value)
                {
                    MethodInfo mAddList = propertyType.GetMethod("Add", new Type[] { innerPropertyType });
                    if (mAddList != null)
                    {
                        if (temp == null)
                        {
                            mAddList.Invoke(list, new object[] { null });
                            continue;
                        }
                        var item = MapObj(innerPropertyType, temp);
                        mAddList.Invoke(list, new object[] { item });
                    }
                }
                return list;
            }
            else if (typeof(TeaModel).IsAssignableFrom(propertyType))
            {
                var v = Activator.CreateInstance(propertyType);
                Dictionary<string, object> dicObj = ((IDictionary) value).Keys.Cast<string>().ToDictionary(key => key, key => ((IDictionary) value) [key]);
                return ToObject(dicObj, v);
            }
            else if (typeof(IDictionary).IsAssignableFrom(propertyType) || typeof(IDictionary).IsAssignableFrom(value.GetType()))
            {
                var dic = (IDictionary) value;
                if (dic.Count == 0)
                {
                    return dic;
                }
                IDictionary resultDic;
                if (propertyType.Equals(typeof(IDictionary)))
                {
                    resultDic = dic;
                }
                else
                {
                    resultDic = (IDictionary) System.Activator.CreateInstance(propertyType);
                    var innerType = propertyType.GetGenericArguments() [1];
                    foreach (DictionaryEntry keypair in dic)
                    {
                        if (keypair.Value == null)
                        {
                            resultDic.Add(keypair.Key, null);
                        }
                        else
                        {
                            Type valueType = keypair.Value.GetType();
                            resultDic.Add(keypair.Key, MapObj(innerType, keypair.Value));
                        }
                    }
                }

                return resultDic;
            }
            else if (propertyType.Equals(typeof(Int32)) && value is Int64)
            {
                return Convert.ToInt32((Int64) value);
            }
            else if (propertyType == typeof(int?))
            {
                return Convert.ToInt32(value);
            }
            else if (propertyType == typeof(long?))
            {
                return Convert.ToInt64(value);
            }
            else if (propertyType == typeof(float?))
            {
                return Convert.ToSingle(value);
            }
            else if (propertyType == typeof(double?))
            {
                return Convert.ToDouble(value);
            }
            else if (propertyType == typeof(bool?))
            {
                return Convert.ToBoolean(value);
            }
            else if (propertyType == typeof(short?))
            {
                return Convert.ToInt16(value);
            }
            else if (propertyType == typeof(ushort?))
            {
                return Convert.ToUInt16(value);
            }
            else if (propertyType == typeof(uint?))
            {
                return Convert.ToUInt32(value);
            }
            else if (propertyType == typeof(ulong?))
            {
                return Convert.ToUInt64(value);
            }
            else
            {
                return Convert.ChangeType(value, propertyType);
            }
        }
    }
}
