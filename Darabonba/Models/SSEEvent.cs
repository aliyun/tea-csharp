using System.Collections;
using System.Collections.Generic;
using Darabonba;

namespace Darabonba.Models
{
    public class SSEEvent : Model
    {
        public string Data { get; set; }
        public string Id { get; set; }
        public string Event { get; set; }
        public int? Retry { get; set; }

        public SSEEvent Copy()
        {
            SSEEvent copy = FromMap(ToMap());
            return copy;
        }

        public SSEEvent CopyWithoutStream()
        {
            SSEEvent copy = FromMap(ToMap(true));
            return copy;
        }

        public override Dictionary<string, object> ToMapCore(bool noStream = false)
        {
            var map = new Dictionary<string, object>();
            if (Data != null)
            {
                map["data"] = Data;
            }
            if (Id != null)
            {
                map["id"] = Id;
            }
            if (Event != null)
            {
                map["event"] = Event;
            }
            if (Retry != null)
            {
                map["retry"] = Retry;
            }
            return map;
        }

        public Dictionary<string, object> ToMap(bool noStream = false)
        {
            return ToMapCore(noStream);
        }

        public new static SSEEvent FromMap(IDictionary map)
        {
            if (map == null)
            {
                return null;
            }
            var model = new SSEEvent();
            if (map.Contains("data"))
            {
                model.Data = (string)map["data"];
            }
            if (map.Contains("id"))
            {
                model.Id = (string)map["id"];
            }
            if (map.Contains("event"))
            {
                model.Event = (string)map["event"];
            }
            if (map.Contains("retry"))
            {
                model.Retry = map["retry"] == null ? (int?)null : System.Convert.ToInt32(map["retry"]);
            }
            return model;
        }

        public static SSEEvent FromMap(Dictionary<string, object> map)
        {
            return FromMap((IDictionary)map);
        }
    }
}
