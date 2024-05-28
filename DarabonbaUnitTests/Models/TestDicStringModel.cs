using Darabonba;

namespace DaraUnitTests.Models
{
    public class TestDicStringModel
    {
        [NameInMap("requestId")]
        public string RequestId { get; set; }

        [NameInMap("count")]
        public int? Count { get; set; }
    }
}
