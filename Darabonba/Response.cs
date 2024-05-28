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

        public Dictionary<string, object> ToMap(bool noStream = false)
        {
            var map = new Dictionary<string, object>();
            return map;
        }

        public static Response FromMap(Dictionary<string, object> map)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            var model = new Response(httpResponseMessage);
            return model;
        }
    }
}
