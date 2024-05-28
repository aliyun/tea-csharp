using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Darabonba
{
    public static class ModelExtensions
    {
        public static Dictionary<string, object> ToMap(this Model model)
        {
            if (model == null)
            {
                return null;
            }
            var result = new Dictionary<string, object>();
            Type type = model.GetType();
            PropertyInfo[] properties = type.GetProperties();
            //PropertyInfo
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                Type property = propertyInfo.PropertyType;
                if (typeof(Stream).IsAssignableFrom(property))
                {
                    continue;
                }
                NameInMapAttribute attribute = propertyInfo.GetCustomAttribute(typeof(NameInMapAttribute)) as NameInMapAttribute;
                string realName = attribute == null ? propertyInfo.Name : attribute.Name;
                result.Add(realName, ToMapFactory(property, propertyInfo.GetValue(model)));
            }

            return result;
        }

        public static object ToMapFactory(Type type, object value)
        {
            if (value == null)
            {
                return null;
            }
            if (typeof(IList).IsAssignableFrom(type) && !typeof(Array).IsAssignableFrom(type))
            {
                IList list = (IList)value;
                Type listType = type.GetGenericArguments()[0];
                List<object> resultList = new List<object>();
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j] == null)
                    {
                        resultList.Add(null);
                    }
                    else
                    {
                        resultList.Add(ToMapFactory(listType, list[j]));
                    }
                }
                return resultList;
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                IDictionary dic = (IDictionary)value;
                IDictionary resultDic = new Dictionary<string, object>();
                foreach (DictionaryEntry keypair in dic)
                {
                    if (keypair.Value == null)
                    {
                        resultDic.Add(keypair.Key, null);
                    }
                    else
                    {
                        Type valueType = keypair.Value.GetType();
                        resultDic.Add(keypair.Key, ToMapFactory(valueType, keypair.Value));
                    }
                }
                return resultDic;
            }
            else if (typeof(Model).IsAssignableFrom(type))
            {
                Model model = (Model)value;
                return model.ToMap();
            }

            return value;
        }

        public static void Validate(this Model model)
        {
            if (model == null)
            {
                throw new ArgumentException("instance is not allowed as null.");
            }
            Type type = model.GetType();
            PropertyInfo[] properties = type.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo p = properties[i];
                Type propertyType = p.PropertyType;
                object obj = p.GetValue(model);
                ValidationAttribute attribute = p.GetCustomAttribute(typeof(ValidationAttribute)) as ValidationAttribute;
                Validator validator = new Validator(attribute, p.Name);
                validator.ValidateRequired(obj);
                if (obj == null)
                {
                    continue;
                }
                if (typeof(IList).IsAssignableFrom(propertyType) && !typeof(Array).IsAssignableFrom(propertyType))
                {
                    IList list = (IList)obj;

                    //validate list count
                    validator.ValidateMaxLength(list);
                    validator.ValidateMinLength(list);

                    Type listType = propertyType.GetGenericArguments()[0];
                    if (typeof(Model).IsAssignableFrom(listType))
                    {
                        for (int j = 0; j < list.Count; j++)
                        {
                            ((Model)list[j]).Validate();
                        }
                    }
                    else
                    {
                        for (int j = 0; j < list.Count; j++)
                        {
                            //validate pattern
                            validator.ValidateRegex(list[j]);
                        }
                    }
                }
                else if (typeof(Model).IsAssignableFrom(propertyType))
                {
                    ((Model)obj).Validate();
                }
                else
                {
                    //validate pattern
                    validator.ValidateRegex(obj);
                    //validate count
                    validator.ValidateMaxLength(obj);
                    validator.ValidateMinLength(obj);
                    //validate num
                    validator.ValidateMaximum(obj);
                    validator.ValidateMinimum(obj);
                }
            }
        }
    }
}
