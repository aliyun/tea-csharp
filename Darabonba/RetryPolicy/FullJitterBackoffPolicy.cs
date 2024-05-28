using System;

namespace Darabonba.RetryPolicy
{
    public class FullJitterBackoffPolicy : BackoffPolicy
    {
        private readonly int? Period;
        private readonly long? Cap;

        public FullJitterBackoffPolicy(int? period, long? cap = 3L * 24 * 60 * 60 * 1000)
            : base(null)
        {
            Period = period;
            Cap = cap;
        }

        public override long? GetDelayTime(RetryPolicyContext ctx)
        {
            double ceil = Math.Min((double)Cap, (double)(Math.Pow(2.0, (double)ctx.RetriesAttempted) * Period));
            Random random = new Random();
            double delay = random.NextDouble() * ceil;
            return (long?)delay;
        }
    }
}