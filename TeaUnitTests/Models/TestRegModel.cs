using System.Collections.Generic;
using Tea;

namespace TeaUnitTests.Models
{
    public class TestRegModel:TeaModel
    {
        [NameInMap(Name="requestId",Regexp="re")]
        public string RequestId { get; set; }

        [NameInMap(Name="items")]
        public List<TestRegSubModel> Items { get; set; }

        [NameInMap(Name="next_marker",Regexp="next")]
        public string NextMarker { get; set; }

        public string testNoAttr { get; set; }
    }
}