using System;

namespace Darabonba.RetryPolicy
{
    public class ExponentialBackoffPolicy : BackoffPolicy
    {
        private readonly int? Period;
        public readonly long? Cap;

        public ExponentialBackoffPolicy(int? period, long? cap = 3L * 24 * 60 * 60 * 1000)
            : base(null)
        {
            Period = period;
            Cap = cap;
        }

        public override long? GetDelayTime(RetryPolicyContext ctx)
        {
            double potentialTime = Math.Pow(2.0, (double)ctx.RetriesAttempted) * (double)Period;
            return (long?)Math.Min((double)Cap, potentialTime);
        }
    }
}