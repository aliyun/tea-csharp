using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tea
{
    public class TeaConverter
    {
        public static Dictionary<string, object> toMap(object obj)
        {
            var result = new Dictionary<string, object>();
            Type type = obj.GetType();
            FieldInfo[] fields = type.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo f = fields[i];
                result.Add(f.Name, f.GetValue(obj));
            }
            PropertyInfo[] properties = type.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property=properties[i];
                result.Add(property.Name,property.GetValue(obj));
            }

            return result;
        }

        public static T toObject<T>(Dictionary<string, object> dict) where T : new()
        {
            var result = new T();
            return toObject(dict, result);
        }

        public static T toObject<T>(Dictionary<string, object> dict, T obj)
        {
            Type type = obj.GetType();
            FieldInfo[] fields = type.GetFields();
            PropertyInfo[] properties = type.GetProperties();
            //Fields Map
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo f = fields[i];
                if (dict.ContainsKey(f.Name))
                {
                    var value = dict[f.Name];
                    if (value is List<Dictionary<string, object>>)
                    {
                        var fieldType = f.FieldType;
                        var list = Activator.CreateInstance(fieldType);
                        Type innerFieldType = fieldType.GetGenericArguments() [0];
                        var v = Activator.CreateInstance(innerFieldType);
                        MethodInfo mAddList = fieldType.GetMethod("Add", new Type[] { innerFieldType });
                        foreach (Dictionary<string, object> dic in (List<Dictionary<string, object>>) value)
                        {
                            var item = toObject(dic, v);
                            mAddList.Invoke(list, new object[] { item });
                        }
                        f.SetValue(obj, list);
                    }
                    else if (value is Dictionary<string, object>)
                    {
                        var fieldType = f.FieldType;
                        var v = Activator.CreateInstance(fieldType);
                        f.SetValue(obj, toObject((Dictionary<string, object>) value, v));
                    }
                    else
                    {
                        f.SetValue(obj, value);
                    }
                }
            }
            //Properties Map
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo p = properties[i];
                var propertyType = p.PropertyType;
                NameInMapAttribute attribute = p.GetCustomAttribute(typeof(NameInMapAttribute)) as NameInMapAttribute;
                string realName = attribute == null?p.Name : attribute.Name;
                if (dict.ContainsKey(realName))
                {
                    var value = dict[realName];
                    if (value is List<Dictionary<string, object>>)
                    {
                        var list = Activator.CreateInstance(propertyType);
                        Type innerPropertyType = propertyType.GetGenericArguments() [0];
                        var v = Activator.CreateInstance(innerPropertyType);
                        MethodInfo mAddList = propertyType.GetMethod("Add", new Type[] { innerPropertyType });
                        foreach (Dictionary<string, object> dic in (List<Dictionary<string, object>>) value)
                        {
                            var item = toObject(dic, v);
                            mAddList.Invoke(list, new object[] { item });
                        }
                        p.SetValue(obj, list);
                    }
                    else if (value is Dictionary<string, object>)
                    {
                        var v = Activator.CreateInstance(propertyType);
                        p.SetValue(obj, toObject((Dictionary<string, object>) value, v));
                    }
                    else if (propertyType.Equals(typeof(Int32)) && value is Int64)
                    {
                        p.SetValue(obj, Convert.ToInt32((Int64) value));
                    }
                    else
                    {
                        p.SetValue(obj, value);
                    }
                }
            }
            return obj;
        }

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
