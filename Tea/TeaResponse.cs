using System.Collections.Generic;
using System.Net;

namespace Tea
{
    public class TeaResponse
    {
        public int StatusCode;

        public string StatusMessage;

        public Dictionary<string, string> Headers;

        public HttpWebResponse _Response;

        public TeaResponse(HttpWebResponse response)
        {
            if (response != null)
            {
                this.StatusCode = (int) response.StatusCode;
                this.StatusMessage = response.StatusDescription;
                this.Headers = TeaCore.ConvertHeaders(response.Headers);
                this._Response = response;
            }
        }
    }
}
