using System;
using System.Collections.Generic;
using System.Text;

namespace Tea
{
    public class TeaUnretryableException : Exception
    {
        public TeaUnretryableException() : base()
        {

        }

        public TeaUnretryableException(TeaRequest lastRequest)
        {
            // TODO
        }
    }
}
