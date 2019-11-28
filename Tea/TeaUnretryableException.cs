using System;

namespace Tea
{
    public class TeaUnretryableException : Exception
    {
        private readonly TeaRequest _lastRequest;

        private readonly Exception _innerException;

        public TeaRequest LastRequest { get { return _lastRequest; } }

        public new Exception InnerException { get { return _innerException; } }

        public TeaUnretryableException() : base()
        {

        }

        public TeaUnretryableException(TeaRequest lastRequest, Exception innerException) : base(" Retry failed : " + (innerException == null ? "" : innerException.Message), innerException)
        {
            _lastRequest = lastRequest;
            _innerException = innerException;
        }
    }
}
