using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Tea.Utils;

namespace Tea
{
    public class TeaException : Exception
    {
        private string code;
        private string message;
        private Dictionary<string, object> data;

        public string Code
        {
            get
            {
                return code;
            }
        }

        public override string Message
        {
            get
            {
                return message;
            }
        }

        public Dictionary<string, object> DataResult
        {
            get
            {
                return data;
            }
        }

        public TeaException(IDictionary dict)
        {
            Dictionary<string, object> dicObj = dict.Keys.Cast<string>().ToDictionary(key => key, key => dict[key]);
            code = DictUtils.GetDicValue(dicObj, "code").ToSafeString();
            message = DictUtils.GetDicValue(dicObj, "message").ToSafeString();
            object obj = DictUtils.GetDicValue(dicObj, "data");
            if (obj == null)
            {
                return;
            }
            if (typeof(IDictionary).IsAssignableFrom(obj.GetType()))
            {
                IDictionary dicData = (IDictionary) obj;
                data = dicData.Keys.Cast<string>().ToDictionary(key => key, key => dicData[key]);
                return;
            }

            Dictionary<string, object> filedsDict = new Dictionary<string, object>();
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo p = properties[i];
                filedsDict.Add(p.Name, p.GetValue(obj));
            }
            data = filedsDict;
        }
    }
}
