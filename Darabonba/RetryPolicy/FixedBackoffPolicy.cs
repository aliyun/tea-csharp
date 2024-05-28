namespace Darabonba.RetryPolicy
{
    public class FixedBackoffPolicy : BackoffPolicy
    {
        private readonly int? Period;

        public FixedBackoffPolicy(int? period)
            : base(null)
        {
            Period = period;
        }

        public override long? GetDelayTime(RetryPolicyContext ctx)
        {
            return Period;
        }
    }
}