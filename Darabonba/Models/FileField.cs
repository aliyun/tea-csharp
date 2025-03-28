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
    }
}