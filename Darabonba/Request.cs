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

        public Dictionary<string, object> ToMap(bool noStream = false)
        {
            var map = new Dictionary<string, object>();
            return map;
        }

        public static Request FromMap(Dictionary<string, object> map)
        {
            var model = new Request();
            return model;
        }
    }
}
