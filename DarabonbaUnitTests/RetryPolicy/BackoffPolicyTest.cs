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
                BackoffPolicy bp = BackoffPolicy.NewBackOffPolicy(new Dictionary<string, object>
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

            var ctx = new RetryPolicyContext { RetriesAttempted = 1 };
            Assert.NotNull(new FixedBackoffPolicy(100).GetDelayTime(ctx));
            Assert.NotNull(new RandomBackoffPolicy(2, 60000L).GetDelayTime(ctx));
            Assert.NotNull(new ExponentialBackoffPolicy(2, 60000L).GetDelayTime(ctx));
            Assert.NotNull(new EqualJitterBackoffPolicy(2, 60000L).GetDelayTime(ctx));
            Assert.NotNull(new FullJitterBackoffPolicy(2, 60000L).GetDelayTime(ctx));

            // default cap paths
            Assert.Equal("RandomBackoffPolicy", BackoffPolicy.NewBackOffPolicy(new Dictionary<string, object>
            {
                { "policy", "Random" },
                { "period", 2 }
            }).GetType().Name);
            Assert.Equal("ExponentialBackoffPolicy", BackoffPolicy.NewBackOffPolicy(new Dictionary<string, object>
            {
                { "policy", "Exponential" },
                { "period", 2 }
            }).GetType().Name);
            Assert.Equal("EqualJitterBackoffPolicy", BackoffPolicy.NewBackOffPolicy(new Dictionary<string, object>
            {
                { "policy", "EqualJitter" },
                { "period", 2 }
            }).GetType().Name);
            Assert.Equal("FullJitterBackoffPolicy", BackoffPolicy.NewBackOffPolicy(new Dictionary<string, object>
            {
                { "policy", "FullJitter" },
                { "period", 2 }
            }).GetType().Name);

            Assert.Throws<DaraException>(() => BackoffPolicy.NewBackOffPolicy(new Dictionary<string, object>
            {
                { "policy", "Fixed" }
            }));
            Assert.Throws<DaraException>(() => BackoffPolicy.NewBackOffPolicy(new Dictionary<string, object>()));
            Assert.Throws<DaraException>(() => BackoffPolicy.NewBackOffPolicy(new Dictionary<string, object>
            {
                { "policy", null }
            }));
        }

        [Fact]
        public void Test_RetryOptions_ToMap_FromMap()
        {
            var options = new RetryOptions
            {
                Retryable = true,
                RetryCondition = new List<RetryCondition>(),
                NoRetryCondition = new List<RetryCondition>()
            };
            Assert.Empty(options.ToMap());
            Assert.Empty(options.ToMap(true));
            Assert.NotNull(RetryOptions.FromMap(new Dictionary<string, object>()));
        }
    }
}