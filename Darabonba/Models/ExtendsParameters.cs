using System.Collections;
using System.Collections.Generic;

namespace Darabonba.Models
{
    public class ExtendsParameters : Model
    {
        public static implicit operator ExtendsParameters(AlibabaCloud.TeaUtil.Models.ExtendsParameters extendsParameters)
        {
            if (extendsParameters == null)
            {
                return null;
            }
            return new ExtendsParameters
            {
                Headers = extendsParameters.Headers,
                Queries = extendsParameters.Queries
            };
        }

        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, string> Queries { get; set; }

        public new void Validate()
        {
        }

        public new ExtendsParameters Copy()
        {
            ExtendsParameters copy = FromMap(ToMap());
            return copy;
        }

        public new ExtendsParameters CopyWithoutStream()
        {
            ExtendsParameters copy = FromMap(ToMap(true));
            return copy;
        }

        public override Dictionary<string, object> ToMapCore(bool noStream = false)
        {
            var map = new Dictionary<string, object>();

            if (Headers != null)
            {
                map["headers"] = Headers;
            }

            if (Queries != null)
            {
                map["queries"] = Queries;
            }

            return map;
        }

        public Dictionary<string, object> ToMap(bool noStream = false)
        {
            return ToMapCore(noStream);
        }

        public new static ExtendsParameters FromMap(IDictionary map)
        {
            if (map == null)
            {
                return null;
            }
            var model = new ExtendsParameters();
            if (map.Contains("headers"))
            {
                Dictionary<string, string> value0 = null;
                if (map["headers"] != null)
                {
                    value0 = new Dictionary<string, string>();
                    foreach (DictionaryEntry item0 in (IDictionary)map["headers"])
                    {
                        value0.Add((string)item0.Key, (string)item0.Value);
                    }
                }
                model.Headers = value0;
            }
            if (map.Contains("queries"))
            {
                Dictionary<string, string> value0 = null;
                if (map["queries"] != null)
                {
                    value0 = new Dictionary<string, string>();
                    foreach (DictionaryEntry item0 in (IDictionary)map["queries"])
                    {
                        value0.Add((string)item0.Key, (string)item0.Value);
                    }
                }
                model.Queries = value0;
            }
            return model;
        }

        public static ExtendsParameters FromMap(IDictionary<string, object> map)
        {
            return FromMap((IDictionary)(map == null ? null : new Dictionary<string, object>(map)));
        }

        public static ExtendsParameters FromMap(Dictionary<string, object> map)
        {
            return FromMap((IDictionary)map);
        }
    }
}
