using Tea;

namespace TeaUnitTests.Models
{
    public class TestRegSubModel : TeaModel
    {
        [NameInMap("requestId")]
        [Validation(Pattern = "r", MaxLength = 0, Required = true)]
        public string RequestId { get; set; }
    }
}
