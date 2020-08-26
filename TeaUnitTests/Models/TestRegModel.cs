using System.Collections;
using System.Collections.Generic;

using Tea;

namespace TeaUnitTests.Models
{
    public class TestRegModel : TeaModel
    {
        [NameInMap("requestId")]
        [Validation(Pattern = "re", MaxLength = 0, Required = true)]
        public string RequestId { get; set; }

        [NameInMap("items")]
        [Validation(Required = true)]
        public List<TestRegSubModel> Items { get; set; }

        [NameInMap("next_marker")]
        [Validation(Pattern = "next", MaxLength = 0, Required = true)]
        public string NextMarker { get; set; }

        public string testNoAttr { get; set; }

        [NameInMap("testListStr")]
        [Validation(Pattern = "listStr", MaxLength = 0)]
        public List<string> testListStr { get; set; }

        public TestRegSubModel subModel { get; set; }

        public Dictionary<string, object> dict { get; set; }

        [Validation(Maximun = 10)]
        public int? testInt32 { get; set; }

        public long? testLong { get; set; }

        [Validation(Minimum = -1)]
        public float? testFloat { get; set; }

        public double? testDouble { get; set; }

        public bool? testBool { get; set; }

        public short? testShort { get; set; }

        public ushort? testUShort { get; set; }

        public uint? testUInt { get; set; }

        public ulong? testULong { get; set; }

        public string testNull { get; set; }

        public Dictionary<string, Dictionary<string, TestRegSubModel>> dicNestDic { get; set; }

        public List<List<IDictionary>> listIDic { get; set; }

        public byte[] bytes { get; set; }

    }
}
