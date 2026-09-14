using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Darabonba.Models
{
    public class FileField
    {
        [NameInMap("filename")]
        public string Filename { get; set; }

        [NameInMap("contentType")]
        public string ContentType { get; set; }

        [NameInMap("content")]
        public Stream Content { get; set; }
        public Dictionary<string, object> ToMap()
        {
            return ToMapCore();
        }

        public virtual Dictionary<string, object> ToMapCore(bool noStream = false)
        {
            var map = new Dictionary<string, object>();
            map["filename"] = this.Filename;
            map["contentType"] = this.ContentType;
            return map;
        }

        public static FileField FromMap(IDictionary map)
        {
            if (map == null)
            {
                return null;
            }
            var model = new FileField();
            if (map.Contains("filename"))
            {
                model.Filename = (string)map["filename"];
            }
            if (map.Contains("contentType"))
            {
                model.ContentType = (string)map["contentType"];
            }
            if (map.Contains("content"))
            {
                model.Content = (Stream)map["content"];
            }
            return model;
        }
    }
}
