using System;

namespace Darabonba.RetryPolicy
{
     public class RandomBackoffPolicy : BackoffPolicy
    {
        private readonly int? Period;
        private readonly long? Cap;

        public RandomBackoffPolicy(int? period, long? cap = 2000)
            : base(null)
        {
            Period = period;
            Cap = cap;
        }

        public override long? GetDelayTime(RetryPolicyContext ctx)
        {
            Random random = new Random();
            double randomTime = random.NextDouble() * Math.Min((double)Cap, (double)(Period * ctx.RetriesAttempted));
            return (long?)randomTime;
        }
    }
}