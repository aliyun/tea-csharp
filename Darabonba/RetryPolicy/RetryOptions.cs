using Darabonba;
using System;
using System.Collections.Generic;

namespace Darabonba.RetryPolicy
{
    public class RetryOptions
    {
        public bool? Retryable { get; set; }
        public List<RetryCondition> RetryCondition { get; set; }
        public List<RetryCondition> NoRetryCondition { get; set; }

        public Dictionary<string, object> ToMap(bool noStream = false)
        {
            return new Dictionary<string, object>();
        }

        public static RetryOptions FromMap(Dictionary<string, object> map)
        {
            return new RetryOptions();
        }
    }
}