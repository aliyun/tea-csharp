using System.Collections;
using System.Collections.Generic;
using Darabonba.RetryPolicy;

namespace Darabonba.Models
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

        public override Dictionary<string, object> ToMapCore(bool noStream = false)
        {
            var map = new Dictionary<string, object>();

            if (RetryOptions != null)
            {
                map["retryOptions"] = RetryOptions.ToMap();
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

        public Dictionary<string, object> ToMap(bool noStream = false)
        {
            return ToMapCore(noStream);
        }

        public new static RuntimeOptions FromMap(IDictionary map)
        {
            if (map == null)
            {
                return null;
            }
            var model = new RuntimeOptions();
            if (!map.Contains("maxAttempts") && map.Contains("max_attempts"))
            {
                model.MaxAttempts = map["max_attempts"] == null ? (int?)null : System.Convert.ToInt32(map["max_attempts"]);
            }
            if (!map.Contains("backoffPolicy") && map.Contains("backoff_policy"))
            {
                model.BackoffPolicy = System.Convert.ToString(map["backoff_policy"]);
            }
            if (!map.Contains("backoffPeriod") && map.Contains("backoff_period"))
            {
                model.BackoffPeriod = map["backoff_period"] == null ? (int?)null : System.Convert.ToInt32(map["backoff_period"]);
            }
            if (map.Contains("retryOptions"))
            {
                model.RetryOptions = map["retryOptions"] as RetryOptions ?? (map["retryOptions"] == null ? null : Darabonba.RetryPolicy.RetryOptions.FromMap((IDictionary)map["retryOptions"]));
            }
            if (map.Contains("autoretry"))
            {
                model.Autoretry = map["autoretry"] == null ? (bool?)null : System.Convert.ToBoolean(map["autoretry"]);
            }
            if (map.Contains("ignoreSSL"))
            {
                model.IgnoreSSL = map["ignoreSSL"] == null ? (bool?)null : System.Convert.ToBoolean(map["ignoreSSL"]);
            }
            if (map.Contains("key"))
            {
                model.Key = (string)map["key"];
            }
            if (map.Contains("cert"))
            {
                model.Cert = (string)map["cert"];
            }
            if (map.Contains("ca"))
            {
                model.Ca = (string)map["ca"];
            }
            if (map.Contains("maxAttempts"))
            {
                model.MaxAttempts = map["maxAttempts"] == null ? (int?)null : System.Convert.ToInt32(map["maxAttempts"]);
            }
            if (map.Contains("backoffPolicy"))
            {
                model.BackoffPolicy = (string)map["backoffPolicy"];
            }
            if (map.Contains("backoffPeriod"))
            {
                model.BackoffPeriod = map["backoffPeriod"] == null ? (int?)null : System.Convert.ToInt32(map["backoffPeriod"]);
            }
            if (map.Contains("readTimeout"))
            {
                model.ReadTimeout = map["readTimeout"] == null ? (int?)null : System.Convert.ToInt32(map["readTimeout"]);
            }
            if (map.Contains("connectTimeout"))
            {
                model.ConnectTimeout = map["connectTimeout"] == null ? (int?)null : System.Convert.ToInt32(map["connectTimeout"]);
            }
            if (map.Contains("httpProxy"))
            {
                model.HttpProxy = (string)map["httpProxy"];
            }
            if (map.Contains("httpsProxy"))
            {
                model.HttpsProxy = (string)map["httpsProxy"];
            }
            if (map.Contains("noProxy"))
            {
                model.NoProxy = (string)map["noProxy"];
            }
            if (map.Contains("maxIdleConns"))
            {
                model.MaxIdleConns = map["maxIdleConns"] == null ? (int?)null : System.Convert.ToInt32(map["maxIdleConns"]);
            }
            if (map.Contains("localAddr"))
            {
                model.LocalAddr = (string)map["localAddr"];
            }
            if (map.Contains("socks5Proxy"))
            {
                model.Socks5Proxy = (string)map["socks5Proxy"];
            }
            if (map.Contains("socks5NetWork"))
            {
                model.Socks5NetWork = (string)map["socks5NetWork"];
            }
            if (map.Contains("keepAlive"))
            {
                model.KeepAlive = map["keepAlive"] == null ? (bool?)null : System.Convert.ToBoolean(map["keepAlive"]);
            }
            if (map.Contains("extendsParameters"))
            {
                model.ExtendsParameters = map["extendsParameters"] == null ? null : Darabonba.Models.ExtendsParameters.FromMap((IDictionary)map["extendsParameters"]);
            }
            return model;
        }

        public static RuntimeOptions FromMap(Dictionary<string, object> map)
        {
            return FromMap((IDictionary)map);
        }
    }
}
