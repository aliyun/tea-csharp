using System.Collections.Generic;
using Darabonba.Exceptions;
using Darabonba.RetryPolicy;
using Xunit;

namespace DaraUnitTests.RetryPolicy
{
    public class BackoffPolicyTest
    {
        [Fact]
        public void Test_BackoffPolicy()
        {
            var exception = Assert.Throws<DaraException>(() =>
            {
                BackoffPolicy backoffPolicy = BackoffPolicy.NewBackOffPolicy(new Dictionary<string, object>
                {
                    { "policy", "Any" }
                });
            });
            Assert.Equal("Invalid backoff policy", exception.Message);

            BackoffPolicy backoffPolicy = BackoffPolicy.NewBackOffPolicy(new Dictionary<string, object>
            {
                { "policy", "Fixed" },
                { "period", 1000 }
            });
            Assert.Equal("FixedBackoffPolicy", backoffPolicy.GetType().Name);

            backoffPolicy = BackoffPolicy.NewBackOffPolicy(new Dictionary<string, object>
            {
                { "policy", "Random" },
                { "period", 2 },
                { "cap", 60000L }
            });
            Assert.Equal("RandomBackoffPolicy", backoffPolicy.GetType().Name);

            backoffPolicy = BackoffPolicy.NewBackOffPolicy(new Dictionary<string, object>
            {
                { "policy", "Exponential" },
                { "period", 2 },
                { "cap", 60000L }
            });
            Assert.Equal("ExponentialBackoffPolicy", backoffPolicy.GetType().Name);

            backoffPolicy = BackoffPolicy.NewBackOffPolicy(new Dictionary<string, object>
            {
                { "policy", "EqualJitter" },
                { "period", 2 },
                { "cap", 60000L }
            });
            Assert.Equal("EqualJitterBackoffPolicy", backoffPolicy.GetType().Name);

            backoffPolicy = BackoffPolicy.NewBackOffPolicy(new Dictionary<string, object>
            {
                { "policy", "ExponentialWithEqualJitter" },
                { "period", 2 },
                { "cap", 60000L }
            });
            Assert.Equal("EqualJitterBackoffPolicy", backoffPolicy.GetType().Name);

            backoffPolicy = BackoffPolicy.NewBackOffPolicy(new Dictionary<string, object>
            {
                { "policy", "FullJitter" },
                { "period", 2 },
                { "cap", 60000L }
            });
            Assert.Equal("FullJitterBackoffPolicy", backoffPolicy.GetType().Name);

            backoffPolicy = BackoffPolicy.NewBackOffPolicy(new Dictionary<string, object>
            {
                { "policy", "ExponentialWithFullJitter" },
                { "period", 2 },
                { "cap", 60000L }
            });
            Assert.Equal("FullJitterBackoffPolicy", backoffPolicy.GetType().Name);
        }
    }
}