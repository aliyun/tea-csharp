using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Tea
{
    public class TeaResponse
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

        public TeaResponse(HttpResponseMessage response)
        {
            if (response != null)
            {
                StatusCode = (int) response.StatusCode;
                StatusMessage = "";
                Headers = TeaCore.ConvertHeaders(response.Headers);
                _responseAsync = response;
            }
        }
    }
}
