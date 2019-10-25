using System;

namespace Tea
{
    public class TeaUnretryableException : Exception
    {
        private readonly TeaRequest _lastRequest;

        public TeaRequest LastRequest { get { return _lastRequest; } }

        public TeaUnretryableException() : base()
        {

        }

        public TeaUnretryableException(TeaRequest lastRequest, Exception innerException) : base(" Retry failed : " + (innerException == null ? "" : innerException.Message), innerException)
        {
            _lastRequest = lastRequest;
        }
    }
}
