using System.Collections.Generic;
using Darabonba.RetryPolicy;

namespace Darabonba.Runtime
{
    public class RuntimeOptions : Model
    {
        /// <summary>
        /// 建议使用Darabonba.RuntimeOptions类
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        ///
        public static implicit operator RuntimeOptions(AlibabaCloud.TeaUtil.Models.RuntimeOptions options)
        {
            if (options == null)
            {
                return null;
            }
            return new RuntimeOptions
            {
                Autoretry = options.Autoretry,
                IgnoreSSL = options.IgnoreSSL,
                Key = options.Key,
                Cert = options.Cert,
                Ca = options.Ca,
                MaxAttempts = options.MaxAttempts,
                BackoffPeriod = options.BackoffPeriod,
                BackoffPolicy = options.BackoffPolicy,
                ReadTimeout = options.ReadTimeout,
                ConnectTimeout = options.ConnectTimeout,
                HttpProxy = options.HttpProxy,
                HttpsProxy = options.HttpsProxy,
                NoProxy = options.NoProxy,
                MaxIdleConns = options.MaxIdleConns,
                LocalAddr = options.LocalAddr,
                Socks5Proxy = options.Socks5Proxy,
                Socks5NetWork = options.Socks5NetWork,
                KeepAlive = options.KeepAlive,
                ExtendsParameters = options.ExtendsParameters,
                RetryOptions = null
            };
        }

        public RetryOptions RetryOptions { get; set; }
        public bool? Autoretry { get; set; }
        public bool? IgnoreSSL { get; set; }
        public string Key { get; set; }
        public string Cert { get; set; }
        public string Ca { get; set; }
        public int? MaxAttempts { get; set; }
        public string BackoffPolicy { get; set; }
        public int? BackoffPeriod { get; set; }
        public int? ReadTimeout { get; set; }
        public int? ConnectTimeout { get; set; }
        public string HttpProxy { get; set; }
        public string HttpsProxy { get; set; }
        public string NoProxy { get; set; }
        public int? MaxIdleConns { get; set; }
        public string LocalAddr { get; set; }
        public string Socks5Proxy { get; set; }
        public string Socks5NetWork { get; set; }
        public bool? KeepAlive { get; set; }
        public ExtendsParameters ExtendsParameters { get; set; }

        public new void Validate()
        {
        }

        public new RuntimeOptions Copy()
        {
            RuntimeOptions copy = FromMap(ToMap());
            return copy;
        }

        public new RuntimeOptions CopyWithoutStream()
        {
            RuntimeOptions copy = FromMap(ToMap(true));
            return copy;
        }

        public Dictionary<string, object> ToMap(bool noStream = false)
        {
            var map = new Dictionary<string, object>();

            if (RetryOptions != null)
            {
                map["retryOptions"] = RetryOptions;
            }
            
            if (Autoretry != null)
            {
                map["autoretry"] = Autoretry;
            }

            if (IgnoreSSL != null)
            {
                map["ignoreSSL"] = IgnoreSSL;
            }

            if (Key != null)
            {
                map["key"] = Key;
            }

            if (Cert != null)
            {
                map["cert"] = Cert;
            }

            if (Ca != null)
            {
                map["ca"] = Ca;
            }

            if (MaxAttempts != null)
            {
                map["max_attempts"] = MaxAttempts;
            }

            if (BackoffPolicy != null)
            {
                map["backoff_policy"] = BackoffPolicy;
            }

            if (BackoffPeriod != null)
            {
                map["backoff_period"] = BackoffPeriod;
            }

            if (ReadTimeout != null)
            {
                map["readTimeout"] = ReadTimeout;
            }

            if (ConnectTimeout != null)
            {
                map["connectTimeout"] = ConnectTimeout;
            }

            if (HttpProxy != null)
            {
                map["httpProxy"] = HttpProxy;
            }

            if (HttpsProxy != null)
            {
                map["httpsProxy"] = HttpsProxy;
            }

            if (NoProxy != null)
            {
                map["noProxy"] = NoProxy;
            }

            if (MaxIdleConns != null)
            {
                map["maxIdleConns"] = MaxIdleConns;
            }

            if (LocalAddr != null)
            {
                map["localAddr"] = LocalAddr;
            }

            if (Socks5Proxy != null)
            {
                map["socks5Proxy"] = Socks5Proxy;
            }

            if (Socks5NetWork != null)
            {
                map["socks5NetWork"] = Socks5NetWork;
            }

            if (KeepAlive != null)
            {
                map["keepAlive"] = KeepAlive;
            }

            if (ExtendsParameters != null)
            {
                map["extendsParameters"] = ExtendsParameters != null ? ExtendsParameters.ToMap(noStream) : null;
            }

            return map;
        }

        public static RuntimeOptions FromMap(Dictionary<string, object> map)
        {
            var model = new RuntimeOptions();

            if (map.ContainsKey("retryOptions"))
            {
                model.RetryOptions = (RetryOptions)map["retryOptions"];
            }
            
            if (map.ContainsKey("autoretry"))
            {
                model.Autoretry = (bool?)map["autoretry"];
            }

            if (map.ContainsKey("ignoreSSL"))
            {
                model.IgnoreSSL = (bool?)map["ignoreSSL"];
            }

            if (map.ContainsKey("key"))
            {
                model.Key = (string)map["key"];
            }

            if (map.ContainsKey("cert"))
            {
                model.Cert = (string)map["cert"];
            }

            if (map.ContainsKey("ca"))
            {
                model.Ca = (string)map["ca"];
            }

            if (map.ContainsKey("max_attempts"))
            {
                model.MaxAttempts = (int?)map["max_attempts"];
            }

            if (map.ContainsKey("backoff_policy"))
            {
                model.BackoffPolicy = (string)map["backoff_policy"];
            }

            if (map.ContainsKey("backoff_period"))
            {
                model.BackoffPeriod = (int?)map["backoff_period"];
            }

            if (map.ContainsKey("readTimeout"))
            {
                model.ReadTimeout = (int?)map["readTimeout"];
            }

            if (map.ContainsKey("connectTimeout"))
            {
                model.ConnectTimeout = (int?)map["connectTimeout"];
            }

            if (map.ContainsKey("httpProxy"))
            {
                model.HttpProxy = (string)map["httpProxy"];
            }

            if (map.ContainsKey("httpsProxy"))
            {
                model.HttpsProxy = (string)map["httpsProxy"];
            }

            if (map.ContainsKey("noProxy"))
            {
                model.NoProxy = (string)map["noProxy"];
            }

            if (map.ContainsKey("maxIdleConns"))
            {
                model.MaxIdleConns = (int?)map["maxIdleConns"];
            }

            if (map.ContainsKey("localAddr"))
            {
                model.LocalAddr = (string)map["localAddr"];
            }

            if (map.ContainsKey("socks5Proxy"))
            {
                model.Socks5Proxy = (string)map["socks5Proxy"];
            }

            if (map.ContainsKey("socks5NetWork"))
            {
                model.Socks5NetWork = (string)map["socks5NetWork"];
            }

            if (map.ContainsKey("keepAlive"))
            {
                model.KeepAlive = (bool?)map["keepAlive"];
            }

            if (map.ContainsKey("extendsParameters"))
            {
                var temp = (Dictionary<string, object>)map["extendsParameters"];
                model.ExtendsParameters = ExtendsParameters.FromMap(temp);
            }

            return model;
        }
    }
}