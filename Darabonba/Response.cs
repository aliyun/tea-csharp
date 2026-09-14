using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Darabonba
{
    public class Response
    {
        private HttpResponseMessage _responseAsync { get; set; }

        public int StatusCode { get; set; }

        public string StatusMessage { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public Stream Body
        {
            get
            {
                if (_responseAsync != null)
                {
                    return _responseAsync.Content.ReadAsStreamAsync().Result;
                }
                else
                {
                    return null;
                }
            }
        }


        public Response(HttpResponseMessage response)
        {
            if (response != null)
            {
                StatusCode = (int)response.StatusCode;
                StatusMessage = "";
                Headers = Core.ConvertHeaders(response.Headers);
                _responseAsync = response;
            }
        }

        public virtual Dictionary<string, object> ToMapCore(bool noStream = false)
        {
            var map = new Dictionary<string, object>();
            map["statusCode"] = this.StatusCode;
            map["statusMessage"] = this.StatusMessage;
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

        public static Response FromMap(IDictionary map)
        {
            if (map == null)
            {
                return null;
            }
            var model = new Response(new System.Net.Http.HttpResponseMessage());
            if (map.Contains("statusCode"))
            {
                model.StatusCode = System.Convert.ToInt32(map["statusCode"]);
            }
            if (map.Contains("statusMessage"))
            {
                model.StatusMessage = (string)map["statusMessage"];
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
                model._responseAsync.Content = map["body"] == null ? null : new System.Net.Http.StreamContent((Stream)map["body"]);
            }
            return model;
        }

        public static Response FromMap(Dictionary<string, object> map)
        {
            return FromMap((IDictionary)map);
        }
    }
}
