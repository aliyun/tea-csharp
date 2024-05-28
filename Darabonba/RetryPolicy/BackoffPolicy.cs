using System;
using System.Collections.Generic;
using Darabonba.Exceptions;

namespace Darabonba.RetryPolicy
{
    public interface IBackoffPolicy
    {
        long? GetDelayTime(RetryPolicyContext ctx);
    }

    public abstract class BackoffPolicy : IBackoffPolicy
    {
        protected string Policy { get; set; }

        public BackoffPolicy(string policy)
        {
            Policy = policy;
        }

        public abstract long? GetDelayTime(RetryPolicyContext ctx);

        public static BackoffPolicy NewBackOffPolicy(Dictionary<string, object> option)
        {
            var validPolicy = new List<string> { "Fixed", "Random", "Exponential", "EqualJitter", "ExponentialWithEqualJitter", "FullJitter", "ExponentialWithFullJitter" };
            if (!option.ContainsKey("policy") || option["policy"] == null || !(option["policy"] is string))
            {
                throw new DaraException
                {
                    Message = "Invalid backoff policy"
                };
            }
            else
            {
                string policy = (string)option["policy"];
                if (!validPolicy.Contains(policy))
                {
                    throw new DaraException
                    {
                        Message = "Invalid backoff policy"
                    };
                }
                if (!option.ContainsKey("period") || option["period"] == null || !(option["period"] is int))
                {
                    throw new DaraException { Message = "Period must be specified." };
                }
                int period = (int)option["period"];
                switch (policy)
                {
                    case "Fixed":
                        {
                            return new FixedBackoffPolicy(period);
                        }
                    case "Random":
                        {
                            var cap = option.ContainsKey("cap") && option["cap"] != null && option["cap"] is long ? (long)option["cap"] : 20000;
                            return new RandomBackoffPolicy(period, cap);
                        }
                    case "Exponential":
                        {
                            var cap = option.ContainsKey("cap") && option["cap"] != null && option["cap"] is long ? (long)option["cap"] : 3L * 24 * 60 * 60 * 1000;
                            return new ExponentialBackoffPolicy(period, cap);
                        }
                    case "EqualJitter":
                    case "ExponentialWithEqualJitter":
                        {
                            var cap = option.ContainsKey("cap") && option["cap"] != null && option["cap"] is long ? (long)option["cap"] : 3L * 24 * 60 * 60 * 1000;
                            return new EqualJitterBackoffPolicy(period, cap);
                        }
                    case "FullJitter":
                    case "ExponentialWithFullJitter":
                        {
                            var cap = option.ContainsKey("cap") && option["cap"] != null && option["cap"] is long ? (long)option["cap"] : 3L * 24 * 60 * 60 * 1000;
                            return new FullJitterBackoffPolicy(period, cap);
                        }
                    default:
                        throw new DaraException
                        {
                            Message = "Invalid backoff policy"
                        };
                }
            }
        }
    }
}