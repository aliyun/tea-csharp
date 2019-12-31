using System;

namespace Tea
{
    public class TeaRetryableException : Exception
    {
        public TeaRequest Request { get; set; }

        public TeaResponse Response { get; set; }

        public TeaRetryableException(TeaRequest request, TeaResponse response)
        {
            Request = request;
            Response = response;
        }
    }
}
