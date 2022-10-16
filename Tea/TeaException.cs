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
        private int statusCode;
        private string description;
        private Dictionary<string, object> accessDeniedDetail;

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

        public int StatusCode
        {
            get
            {
                return statusCode;
            }
        }


        public string Description
        {
            get
            {
                return description;
            }
        }

        public Dictionary<string, object> AccessDeniedDetail
        {
            get
            {
                return accessDeniedDetail;
            }
        }

        public TeaException(IDictionary dict)
        {
            Dictionary<string, object> dicObj = dict.Keys.Cast<string>().ToDictionary(key => key, key => dict[key]);
            code = DictUtils.GetDicValue(dicObj, "code").ToSafeString();
            message = DictUtils.GetDicValue(dicObj, "message").ToSafeString();
            description = DictUtils.GetDicValue(dicObj, "description").ToSafeString();
            object obj = DictUtils.GetDicValue(dicObj, "accessDeniedDetail");
            if (obj != null)
            {
                if (typeof(IDictionary).IsAssignableFrom(obj.GetType()))
                {
                    IDictionary dicDetail = (IDictionary) obj;
                    accessDeniedDetail = dicDetail.Keys.Cast<string>().ToDictionary(key => key, key => dicDetail[key]);
                }
            }
            obj = DictUtils.GetDicValue(dicObj, "data");
            if (obj == null)
            {
                return;
            }
            if (typeof(IDictionary).IsAssignableFrom(obj.GetType()))
            {
                IDictionary dicData = (IDictionary) obj;
                data = dicData.Keys.Cast<string>().ToDictionary(key => key, key => dicData[key]);
                if (DictUtils.GetDicValue(data, "statusCode") != null)
                {
                    statusCode = int.Parse(DictUtils.GetDicValue(data, "statusCode").ToSafeString());
                }
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
