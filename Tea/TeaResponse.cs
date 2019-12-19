using System.Collections.Generic;
using System.Net;

namespace Tea
{
    public class TeaResponse
    {
        public int StatusCode { get; set; }

        public string StatusMessage { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public HttpWebResponse _Response { get; set; }

        public TeaResponse(HttpWebResponse response)
        {
            if (response != null)
            {
                StatusCode = (int) response.StatusCode;
                StatusMessage = response.StatusDescription;
                Headers = TeaCore.ConvertHeaders(response.Headers);
                _Response = response;
            }
        }
    }
}
