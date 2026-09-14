using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tea;

namespace Darabonba
{
    public class Request : TeaRequest
    {
        private string _protocol;
        private string _method;
        private Dictionary<string, string> _query;
        private Dictionary<string, string> _headers;
        public Request()
        {
            Query = new Dictionary<string, string>();
            Headers = new Dictionary<string, string>();
        }
        public string Protocol
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_protocol))
                {
                    return "http";
                }
                else
                {
                    return _protocol;
                }
            }
            set
            {
                _protocol = value;
            }
        }

        public int Port { get; set; }

        public string Method
        {
            get
            {
                if(_method == null)
                {
                    return "GET";
                }
                return _method;
            }
            set
            {
                _method = value;
            }
        }

        public string Pathname { get; set; }

        public Dictionary<string, string> Query
        {
            get
            {
                if (_query == null)
                {
                    return new Dictionary<string, string>();
                }
                else
                {
                    return _query;
                }
            }
            set
            {
                _query = value;
            }
        }

        public Dictionary<string, string> Headers
        {
            get
            {
                if (_headers == null)
                {
                    return new Dictionary<string, string>();
                }
                else
                {
                    return _headers;
                }
            }
            set
            {
                _headers = value;
            }
        }

        public Stream Body;

        public virtual Dictionary<string, object> ToMapCore(bool noStream = false)
        {
            var map = new Dictionary<string, object>();
            map["protocol"] = this.Protocol;
            map["port"] = this.Port;
            map["method"] = this.Method;
            map["pathname"] = this.Pathname;
            {
                Dictionary<string, object> value0 = null;
                if (this.Query != null)
                {
                    value0 = new Dictionary<string, object>();
                    foreach (var item0 in this.Query)
                    {
                        value0.Add(item0.Key, item0.Value);
                    }
                }
                map["query"] = value0;
            }
            {
                Dictionary<string, object> value0 = null;
                if (this.Headers != null)
                {
                    value0 = new Dictionary<string, object>();
                    foreach (var item0 in this.Headers)
                    {
                        value0.Add(item0.Key, item0.Value);
                    }
                }
                map["headers"] = value0;
            }
            return map;
        }

        public Dictionary<string, object> ToMap(bool noStream = false)
        {
            return ToMapCore(noStream);
        }

        public static Request FromMap(IDictionary map)
        {
            if (map == null)
            {
                return null;
            }
            var model = new Request();
            if (map.Contains("protocol"))
            {
                model.Protocol = (string)map["protocol"];
            }
            if (map.Contains("port"))
            {
                model.Port = System.Convert.ToInt32(map["port"]);
            }
            if (map.Contains("method"))
            {
                model.Method = (string)map["method"];
            }
            if (map.Contains("pathname"))
            {
                model.Pathname = (string)map["pathname"];
            }
            if (map.Contains("query"))
            {
                Dictionary<string, string> value0 = null;
                if (map["query"] != null)
                {
                    value0 = new Dictionary<string, string>();
                    foreach (DictionaryEntry item0 in (IDictionary)map["query"])
                    {
                        value0.Add((string)item0.Key, (string)item0.Value);
                    }
                }
                model.Query = value0;
            }
            if (map.Contains("headers"))
            {
                Dictionary<string, string> value0 = null;
                if (map["headers"] != null)
                {
                    value0 = new Dictionary<string, string>();
                    foreach (DictionaryEntry item0 in (IDictionary)map["headers"])
                    {
                        value0.Add((string)item0.Key, (string)item0.Value);
                    }
                }
                model.Headers = value0;
            }
            if (map.Contains("body"))
            {
                model.Body = (Stream)map["body"];
            }
            return model;
        }

        public static Request FromMap(Dictionary<string, object> map)
        {
            return FromMap((IDictionary)map);
        }
    }
}
