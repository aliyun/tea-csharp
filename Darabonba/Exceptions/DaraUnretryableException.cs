using Darabonba.RetryPolicy;
using Tea;

namespace Darabonba.Exceptions
{
    public class DaraUnRetryableException : TeaUnretryableException
    {
        public DaraUnRetryableException()
        {
        }

        public DaraUnRetryableException(RetryPolicyContext retryPolicyContext) : base(retryPolicyContext.Request,
            retryPolicyContext.Exception)
        {
        }
    }
}