using System.Collections.Generic;

namespace Darabonba.Runtime
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

        public Dictionary<string, object> ToMap(bool noStream = false)
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

        public static ExtendsParameters FromMap(IDictionary<string, object> map)
        {
            var model = new ExtendsParameters();

            if (map.ContainsKey("headers"))
            {
                model.Headers = map["headers"] as Dictionary<string, string>;
            }

            if (map.ContainsKey("queries"))
            {
                model.Queries = map["queries"] as Dictionary<string, string>;
            }

            return model;
        }
    }
}