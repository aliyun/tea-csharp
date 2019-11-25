using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Tea
{
    public class TeaModel
    {
        public Dictionary<string, object> ToMap()
        {
            var result = new Dictionary<string, object>();
            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties();
            //PropertyInfo
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                Type property = propertyInfo.PropertyType;
                NameInMapAttribute attribute = propertyInfo.GetCustomAttribute(typeof(NameInMapAttribute)) as NameInMapAttribute;
                string realName = attribute == null ? propertyInfo.Name : attribute.Name;
                if (typeof(IList).IsAssignableFrom(property))
                {
                    IList list = (IList) propertyInfo.GetValue(this);
                    if (list != null)
                    {
                        Type listType = property.GetGenericArguments() [0];
                        if (typeof(TeaModel).IsAssignableFrom(listType))
                        {
                            IList dicList = new List<Dictionary<string, object>>();
                            for (int j = 0; j < list.Count; j++)
                            {
                                if (list[j] == null)
                                {
                                    dicList.Add(null);
                                }
                                else
                                {
                                    dicList.Add(((TeaModel) list[j]).ToMap());
                                }
                            }
                            result.Add(realName, dicList);
                        }
                        else
                        {
                            result.Add(realName, list);
                        }
                    }
                    else
                    {
                        result.Add(realName, null);
                    }
                }
                else if (typeof(TeaModel).IsAssignableFrom(property))
                {
                    TeaModel teaModel = (TeaModel) propertyInfo.GetValue(this);
                    if (teaModel != null)
                    {
                        result.Add(realName, ((TeaModel) propertyInfo.GetValue(this)).ToMap());
                    }
                    else
                    {
                        result.Add(realName, null);
                    }
                }
                else
                {
                    result.Add(realName, propertyInfo.GetValue(this));
                }
            }

            return result;
        }

        public static T ToObject<T>(Dictionary<string, object> dict) where T : class, new()
        {
            var result = new T();
            return ToObject(dict, result);
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
                    else if (value is Dictionary<string, object>)
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
                        p.SetValue(obj, value);
                    }
                }
            }
            return obj;
        }

        public void Validate()
        {
            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo p = properties[i];
                Type propertyType = p.PropertyType;
                object obj = p.GetValue(this);
                ValidationAttribute attribute = p.GetCustomAttribute(typeof(ValidationAttribute)) as ValidationAttribute;
                TeaValidator teaValidator = new TeaValidator(attribute, p.Name);
                teaValidator.ValidateRequired(obj);
                if (obj == null)
                {
                    continue;
                }
                if (typeof(IList).IsAssignableFrom(propertyType))
                {
                    IList list = (IList) obj;

                    Type listType = propertyType.GetGenericArguments() [0];
                    if (typeof(TeaModel).IsAssignableFrom(listType))
                    {
                        for (int j = 0; j < list.Count; j++)
                        {
                            ((TeaModel) list[j]).Validate();
                        }
                    }
                    else
                    {
                        for (int j = 0; j < list.Count; j++)
                        {
                            teaValidator.ValidateRegex(list[j]);
                        }
                    }
                }
                else if (typeof(TeaModel).IsAssignableFrom(propertyType))
                {
                    ((TeaModel) obj).Validate();
                }
                else
                {
                    teaValidator.ValidateRegex(obj);
                }
            }
        }
    }
}
