using System.Collections;
using Darabonba;
using System;
using System.Collections.Generic;

namespace Darabonba.RetryPolicy
{
    public class RetryOptions
    {
        public bool? Retryable { get; set; }
        public List<RetryCondition> RetryCondition { get; set; }
        public List<RetryCondition> NoRetryCondition { get; set; }

        public virtual Dictionary<string, object> ToMapCore(bool noStream = false)
        {
            var map = new Dictionary<string, object>();
            map["retryable"] = this.Retryable;
            {
                List<object> value0 = null;
                if (this.RetryCondition != null)
                {
                    value0 = new List<object>();
                    foreach (var item0 in this.RetryCondition)
                    {
                        value0.Add(item0);
                    }
                }
                map["retryCondition"] = value0;
            }
            {
                List<object> value0 = null;
                if (this.NoRetryCondition != null)
                {
                    value0 = new List<object>();
                    foreach (var item0 in this.NoRetryCondition)
                    {
                        value0.Add(item0);
                    }
                }
                map["noRetryCondition"] = value0;
            }
            return map;
        }

        public Dictionary<string, object> ToMap(bool noStream = false)
        {
            return ToMapCore(noStream);
        }

        public static RetryOptions FromMap(IDictionary map)
        {
            if (map == null)
            {
                return null;
            }
            var model = new RetryOptions();
            if (map.Contains("retryable"))
            {
                model.Retryable = map["retryable"] == null ? (bool?)null : System.Convert.ToBoolean(map["retryable"]);
            }
            if (map.Contains("retryCondition"))
            {
                List<RetryCondition> value0 = null;
                if (map["retryCondition"] != null)
                {
                    value0 = new List<RetryCondition>();
                    foreach (var item0 in (IList)map["retryCondition"])
                    {
                        value0.Add((RetryCondition)item0);
                    }
                }
                model.RetryCondition = value0;
            }
            if (map.Contains("noRetryCondition"))
            {
                List<RetryCondition> value0 = null;
                if (map["noRetryCondition"] != null)
                {
                    value0 = new List<RetryCondition>();
                    foreach (var item0 in (IList)map["noRetryCondition"])
                    {
                        value0.Add((RetryCondition)item0);
                    }
                }
                model.NoRetryCondition = value0;
            }
            return model;
        }

        public static RetryOptions FromMap(Dictionary<string, object> map)
        {
            return FromMap((IDictionary)map);
        }
    }
}
