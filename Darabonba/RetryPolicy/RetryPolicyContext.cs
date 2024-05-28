using System;

namespace Darabonba.RetryPolicy
{
    public class RetryPolicyContext
    {
        public int? RetriesAttempted { get; set; }
        public Request Request { get; set; }
        public Response Response { get; set; }
        public Exception Exception { get; set; }
    }
}