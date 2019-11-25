using System.Collections.Generic;
using System.IO;

namespace Tea
{
    public class TeaRequest
    {
        private string _protocol;
        private Dictionary<string, string> _query;
        private Dictionary<string, string> _headers;
        public TeaRequest()
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

        public string Method { get; set; }

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
    }
}
