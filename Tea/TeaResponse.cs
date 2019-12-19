using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Tea
{
    public class TeaResponse
    {

        private HttpWebResponse _response { get; set; }

        public int StatusCode { get; set; }

        public string StatusMessage { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public Stream Body
        {
            get
            {
                return _response.GetResponseStream();
            }
        }

        public TeaResponse(HttpWebResponse response)
        {
            if (response != null)
            {
                StatusCode = (int) response.StatusCode;
                StatusMessage = response.StatusDescription;
                Headers = TeaCore.ConvertHeaders(response.Headers);
                _response = response;
            }
        }
    }
}
