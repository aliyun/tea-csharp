using System.Collections.Generic;
using System.IO;

namespace Tea
{
    public class TeaRequest
    {
        public TeaRequest()
        {
            Query = new Dictionary<string, string>();
            Headers = new Dictionary<string, string>();
        }
        public string Protocol;

        public int Port;

        public string Method;

        public string Pathname;

        public Dictionary<string, string> Query;

        public Dictionary<string, string> Headers;

        public Stream Body;
    }
}
