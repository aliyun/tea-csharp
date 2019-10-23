using Tea;

namespace TeaUnitTests.Models
{
    public class TestRegSubModel:TeaModel
    {
        [NameInMap(Name="requestId",Regexp="r")]
        public string RequestId { get; set; }
        
    }
}