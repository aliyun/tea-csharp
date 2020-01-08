using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Tea
{
    public static class TeaModelExtensions
    {
        public static Dictionary<string, object> ToMap(this TeaModel model)
        {
            if (model == null)
            {
                return new Dictionary<string, object>();
            }
            var result = new Dictionary<string, object>();
            Type type = model.GetType();
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
                    IList list = (IList) propertyInfo.GetValue(model);
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
                    TeaModel teaModel = (TeaModel) propertyInfo.GetValue(model);
                    if (teaModel != null)
                    {
                        result.Add(realName, ((TeaModel) propertyInfo.GetValue(model)).ToMap());
                    }
                    else
                    {
                        result.Add(realName, null);
                    }
                }
                else
                {
                    result.Add(realName, propertyInfo.GetValue(model));
                }
            }

            return result;
        }

        public static void Validate(this TeaModel model)
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
