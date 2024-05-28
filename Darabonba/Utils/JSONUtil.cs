using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Darabonba.Utils
{
    public static class JSONUtil
    {
        public static string SerializeObject(object data)
        {
            if (data is string)
            {
                return data.ToString();
            }
            return JsonConvert.SerializeObject(data);
        }

        public static object Deserialize(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj is JArray)
            {
                return DeserializeJArray((JArray) obj);
            }
            else if (obj is JObject)
            {
                return DeserializeJObject((JObject) obj);
            }
            else
            {
                return obj;
            }
        }
        
        public static Dictionary<string, object> ParseToMap(object input)
        {
            if (input == null)
            {
                return null;
            }

            var type = input.GetType();
            var map = (Dictionary<string, object>)ModelExtensions.ToMapFactory(type, input);

            return map;
        }

        private static Dictionary<string, object> DeserializeJObject(JObject obj)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            Dictionary<string, object> dicJObj = obj.ToObject<Dictionary<string, object>>();
            foreach (var keypair in dicJObj)
            {
                dic.Add(keypair.Key, Deserialize(keypair.Value));
            }
            return dic;
        }

        private static List<object> DeserializeJArray(JArray obj)
        {
            if (obj.Count == 0)
            {
                return new List<object>();
            }

            if (obj[0].Type == JTokenType.Object)
            {
                List<object> dicList = new List<object>();
                List<Dictionary<string, object>> dicObjList = obj.ToObject<List<Dictionary<string, object>>>();
                foreach (Dictionary<string, object> objItem in dicObjList)
                {
                    Dictionary<string, object> objDict = new Dictionary<string, object>();
                    foreach (var keypair in objItem)
                    {
                        objDict.Add(keypair.Key, Deserialize(keypair.Value));
                    }
                    dicList.Add(objDict);
                }
                return dicList;
            }
            else if (obj[0].Type == JTokenType.Array)
            {
                List<object> dicObjList = obj.ToObject<List<object>>();
                List<object> dicList = new List<object>();
                foreach (var item in dicObjList)
                {
                    dicList.Add(Deserialize((JArray) item));
                }

                return dicList;
            }
            else
            {
                List<object> dicObjList = obj.ToObject<List<object>>();
                return dicObjList;
            }
        }

        public static object ReadPath(object obj, string path)
        {
            object result;
            string jsonStr;
            if (obj is JObject)
            {
                jsonStr = JsonConvert.SerializeObject(obj);
                result = JObject.Parse(jsonStr).SelectToken(path);
            }
            else
            {
                jsonStr = SerializeObject(ParseToMap(obj));
                result = JObject.Parse(jsonStr).SelectToken(path);
            }
            return ConvertNumber(result);
        }
        
        private static object ConvertNumber(object input)
        {
            if (input == null) return null;

            var token = input as JToken;
            if (token != null)
            {
                if (token.Type == JTokenType.Integer)
                {
                    return token.ToObject<long>();
                }
                if (token.Type == JTokenType.Float)
                {
                    return token.ToObject<double>();
                }
                if (token.Type == JTokenType.String)
                {
                    return token.ToString();
                }
                if (token.Type == JTokenType.Array)
                {
                    return HandleList(token.Children());
                }
                if (token.Type == JTokenType.Object)
                {
                    return HandleMap(token.ToObject<Dictionary<string, object>>());
                }
                if (token.Type == JTokenType.Boolean)
                {
                    return token.ToObject<bool>();
                }
            }

            return input; 
        }
        
        private static object HandleList(IEnumerable<JToken> list)
        {
            var convertedList = new List<object>();
            foreach (var item in list)
            {
                convertedList.Add(ConvertNumber(item));
            }
            return convertedList;
        }

        private static object HandleMap(IDictionary<string, object> map)
        {
            var convertedMap = new Dictionary<string, object>();
            foreach (var entry in map)
            {
                convertedMap[entry.Key] = ConvertNumber(entry.Value);
            }
            return convertedMap;
        }
    }
}