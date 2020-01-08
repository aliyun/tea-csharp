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
                    if (value is List<Dictionary<string, object>>)
                    {
                        var list = Activator.CreateInstance(propertyType);
                        Type innerPropertyType = propertyType.GetGenericArguments() [0];

                        foreach (Dictionary<string, object> dic in (List<Dictionary<string, object>>) value)
                        {
                            var v = Activator.CreateInstance(innerPropertyType);
                            MethodInfo mAddList = propertyType.GetMethod("Add", new Type[] { innerPropertyType });
                            if (mAddList != null)
                            {
                                var item = ToObject(dic, v);
                                mAddList.Invoke(list, new object[] { item });
                            }
                        }

                        p.SetValue(obj, list);
                    }
                    else if (typeof(TeaModel).IsAssignableFrom(propertyType))
                    {
                        var v = Activator.CreateInstance(propertyType);
                        p.SetValue(obj, ToObject((Dictionary<string, object>) value, v));
                    }
                    else if (propertyType.Equals(typeof(Int32)) && value is Int64)
                    {
                        p.SetValue(obj, Convert.ToInt32((Int64) value));
                    }
                    else
                    {
                        p.SetValue(obj, Convert.ChangeType(value, propertyType));
                    }
                }
            }
            return obj;
        }
    }
}
