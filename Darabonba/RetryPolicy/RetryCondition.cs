using System.Collections.Generic;

namespace Darabonba.RetryPolicy
{
    public class RetryCondition
    {
        public int? MaxAttempts { get; set; }
        public long? MaxDelayTimeMillis { get; set; }
        public BackoffPolicy Backoff { get; set; }
        public List<string> Exception { get; set; }
        public List<string> ErrorCode { get; set; }
    }
}