using System.Collections.Generic;

namespace Darabonba.Http
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

        public Dictionary<string, object> ToMap(bool noStream = false)
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

        public static SSEEvent FromMap(Dictionary<string, object> map)
        {
            var model = new SSEEvent();
            if (map.ContainsKey("data"))
            {
                model.Data = (string)map["data"];
            }
            if (map.ContainsKey("id"))
            {
                model.Id = (string)map["id"];
            }
            if (map.ContainsKey("event"))
            {
                model.Event = (string)map["event"];
            }
            if (map.ContainsKey("retry"))
            {
                model.Retry = (int?)map["retry"];
            }
            return model;
        }
    }
}