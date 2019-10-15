using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

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
            this.StatusCode = (int) response.StatusCode;
            this.StatusMessage = response.StatusDescription;
            this.Headers = TeaCore.ConvertHeaders(response.Headers);
            this._Response = response;
        }
    }
}
