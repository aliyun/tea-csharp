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
        private readonly string code;
        private readonly string message;
        private readonly Dictionary<string, object> data;
        private readonly string httpCode;
        private readonly string hostId;
        private readonly string requestId;

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
                string res = message;
                if (!string.IsNullOrWhiteSpace(requestId))
                {
                    res += string.Format(";requestId:{0}", requestId);
                }
                if (!string.IsNullOrWhiteSpace(httpCode))
                {
                    res += string.Format(";httpCode:{0}", httpCode);
                }
                if (!string.IsNullOrWhiteSpace(hostId))
                {
                    res += string.Format(";hostId:{0}", hostId);
                }
                return res;
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
            }
            else
            {
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

            var dataKeyLower = data.Keys.Cast<string>().ToDictionary(key => key.ToLower(), key => data[key]);
            httpCode = dataKeyLower.Get("httpcode").ToSafeString();
            hostId = dataKeyLower.Get("hostid").ToSafeString();
            requestId = dataKeyLower.Get("requestid").ToSafeString();
        }
    }
}
