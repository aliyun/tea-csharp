using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Tea
{
    public class TeaRequest
    {
        public TeaRequest() {

        }
        public string Protocol;

        public int Port;

        public string Method;

        public string Pathname;

        public Dictionary<string, string> Query;

        public Dictionary<string, string> Headers;

        public string Body;
    }
}