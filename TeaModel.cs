using System;
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
            FieldInfo[] fields = type.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo f = fields[i];
                result.Add(f.Name, f.GetValue(this));
            }
            PropertyInfo[] properties = type.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property=properties[i];
                result.Add(property.Name,property.GetValue(this));
            }

            return result;
        }
    }
}