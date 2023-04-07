using System;
using Tea;
namespace TeaUnitTests.Models
{
	public class TestShrinkRequest : TeaModel
    {
        [NameInMap("Str1")]
        [Validation(Required = false)]
        public string Str1 { get; set; }

        [NameInMap("Str2")]
        [Validation(Required = false)]
        public string Str2 { get; set; }

        [NameInMap("Strs")]
        [Validation(Required = false)]
        public string StrShrink { get; set; }
    }
}

