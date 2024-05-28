using Darabonba;

namespace DaraUnitTests.Models
{
    public class TestRegSubModel : Model
    {
        [NameInMap("requestId")]
        [Validation(Pattern = "r", MaxLength = 0, Required = true)]
        public string RequestId { get; set; }

        public int? testInt { get; set; }
    }
}
