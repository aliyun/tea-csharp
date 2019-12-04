using System;
using System.Collections.Generic;
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

        public new string Message
        {
            get
            {
                return message;
            }
        }

        public new Dictionary<string, object> Data
        {
            get
            {
                return data;
            }
        }

        public TeaException(Dictionary<string, object> dict)
        {
            code = DictUtils.GetDicValue(dict, "code").ToSafeString();
            message = DictUtils.GetDicValue(dict, "message").ToSafeString();
            object obj = DictUtils.GetDicValue(dict, "data");
            if (obj == null)
            {
                return;
            }
            if (obj is Dictionary<string, object>)
            {
                data = (Dictionary<string, object>) obj;
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
