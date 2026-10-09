using System.Collections.Generic;
using Darabonba.Models;
using Darabonba.RetryPolicy;
using Tea.Utils;
using Xunit;

namespace DaraUnitTests.Models
{
    public class RunTimeOptionsTest
    {
        [Fact]
        public void Test_RunTimeOptions()
        {
            var run = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 1000,
                ExtendsParameters = new AlibabaCloud.TeaUtil.Models.ExtendsParameters
                {
                    Headers = new Dictionary<string, string> { { "test", "test" } }
                }
            };
            var runtimeOptions = Test(run);
            Assert.Equal("test", runtimeOptions.ExtendsParameters.Headers.Get("test"));
            Assert.Equal(1000, runtimeOptions.ReadTimeout);
            Assert.Null(runtimeOptions.RetryOptions);

            RuntimeOptions nullConvert = (AlibabaCloud.TeaUtil.Models.RuntimeOptions)null;
            Assert.Null(nullConvert);
        }

        [Fact]
        public void Test_ToMap_FromMap_Copy_Validate()
        {
            var empty = new RuntimeOptions();
            empty.Validate();
            Assert.Empty(empty.ToMap());
            Assert.Empty(empty.ToMap(true));

            var options = new RuntimeOptions
            {
                RetryOptions = new RetryOptions { Retryable = true },
                Autoretry = true,
                IgnoreSSL = false,
                Key = "key",
                Cert = "cert",
                Ca = "ca",
                MaxAttempts = 3,
                BackoffPolicy = "Fixed",
                BackoffPeriod = 100,
                ReadTimeout = 1000,
                ConnectTimeout = 2000,
                HttpProxy = "http://proxy",
                HttpsProxy = "https://proxy",
                NoProxy = "localhost",
                MaxIdleConns = 10,
                LocalAddr = "127.0.0.1",
                Socks5Proxy = "socks5://proxy",
                Socks5NetWork = "tcp",
                KeepAlive = true,
                ExtendsParameters = new ExtendsParameters
                {
                    Headers = new Dictionary<string, string> { { "h", "v" } },
                    Queries = new Dictionary<string, string> { { "q", "1" } }
                }
            };

            var map = options.ToMap();
            Assert.NotNull(map["retryOptions"]);
            Assert.Equal(true, map["autoretry"]);
            Assert.Equal(false, map["ignoreSSL"]);
            Assert.Equal("key", map["key"]);
            Assert.Equal("cert", map["cert"]);
            Assert.Equal("ca", map["ca"]);
            Assert.Equal(3, map["max_attempts"]);
            Assert.Equal("Fixed", map["backoff_policy"]);
            Assert.Equal(100, map["backoff_period"]);
            Assert.Equal(1000, map["readTimeout"]);
            Assert.Equal(2000, map["connectTimeout"]);
            Assert.Equal("http://proxy", map["httpProxy"]);
            Assert.Equal("https://proxy", map["httpsProxy"]);
            Assert.Equal("localhost", map["noProxy"]);
            Assert.Equal(10, map["maxIdleConns"]);
            Assert.Equal("127.0.0.1", map["localAddr"]);
            Assert.Equal("socks5://proxy", map["socks5Proxy"]);
            Assert.Equal("tcp", map["socks5NetWork"]);
            Assert.Equal(true, map["keepAlive"]);
            Assert.NotNull(map["extendsParameters"]);

            var fromMap = RuntimeOptions.FromMap(map);
            Assert.True(fromMap.Autoretry);
            Assert.False(fromMap.IgnoreSSL);
            Assert.Equal("key", fromMap.Key);
            Assert.Equal("cert", fromMap.Cert);
            Assert.Equal("ca", fromMap.Ca);
            Assert.Equal(3, fromMap.MaxAttempts);
            Assert.Equal("Fixed", fromMap.BackoffPolicy);
            Assert.Equal(100, fromMap.BackoffPeriod);
            Assert.Equal(1000, fromMap.ReadTimeout);
            Assert.Equal(2000, fromMap.ConnectTimeout);
            Assert.Equal("http://proxy", fromMap.HttpProxy);
            Assert.Equal("https://proxy", fromMap.HttpsProxy);
            Assert.Equal("localhost", fromMap.NoProxy);
            Assert.Equal(10, fromMap.MaxIdleConns);
            Assert.Equal("127.0.0.1", fromMap.LocalAddr);
            Assert.Equal("socks5://proxy", fromMap.Socks5Proxy);
            Assert.Equal("tcp", fromMap.Socks5NetWork);
            Assert.True(fromMap.KeepAlive);
            Assert.Equal("v", fromMap.ExtendsParameters.Headers["h"]);
            Assert.NotNull(fromMap.RetryOptions);

            var copy = options.Copy();
            Assert.Equal(options.Key, copy.Key);
            Assert.Equal(options.ReadTimeout, copy.ReadTimeout);
            Assert.Equal("v", copy.ExtendsParameters.Headers["h"]);

            var copyNoStream = options.CopyWithoutStream();
            Assert.Equal(options.Cert, copyNoStream.Cert);

            var partial = RuntimeOptions.FromMap(new Dictionary<string, object>());
            Assert.Null(partial.Key);
            Assert.Null(partial.ExtendsParameters);
        }

        [Fact]
        public void Test_Ipv4Only_ToMap_FromMap_Copy()
        {
            var defaults = new RuntimeOptions();
            Assert.Null(defaults.Ipv4Only);
            Assert.False(defaults.ToMap().ContainsKey("ipv4Only"));
            Assert.Null(RuntimeOptions.FromMap(new Dictionary<string, object>()).Ipv4Only);

            var enabled = new RuntimeOptions { Ipv4Only = true, KeepAlive = false };
            var map = enabled.ToMap();
            Assert.Equal(true, map["ipv4Only"]);
            Assert.True(RuntimeOptions.FromMap(map).Ipv4Only);
            Assert.True(enabled.Copy().Ipv4Only);
            Assert.True(enabled.CopyWithoutStream().Ipv4Only);

            var disabled = new RuntimeOptions { Ipv4Only = false };
            Assert.Equal(false, disabled.ToMap()["ipv4Only"]);
            Assert.False(RuntimeOptions.FromMap(disabled.ToMap()).Ipv4Only);
            Assert.Null(RuntimeOptions.FromMap(new Dictionary<string, object> { { "ipv4Only", null } }).Ipv4Only);

            RuntimeOptions converted = new AlibabaCloud.TeaUtil.Models.RuntimeOptions { KeepAlive = true };
            Assert.Null(converted.Ipv4Only);
        }

        private static RuntimeOptions Test(RuntimeOptions runtimeOptions)
        {
            return runtimeOptions;
        }
    }
}