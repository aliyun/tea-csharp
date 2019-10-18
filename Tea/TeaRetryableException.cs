using System;
using System.Collections.Generic;
using System.Text;

namespace Tea
{
    public class TeaRetryableException : Exception
    {
        private Exception innerWebException;

        public TeaRetryableException() : base()
        {

        }

        public TeaRetryableException(string msg) : base(msg)
        {

        }

        public TeaRetryableException(Exception innerWebException)
        {
            this.innerWebException = innerWebException;
        }

        public TeaRetryableException(string msg, Exception innerWebException) : base(msg)
        {
            this.innerWebException = innerWebException;
        }

        public Exception GetInnerWebException()
        {
            return innerWebException;
        }
    }
}
