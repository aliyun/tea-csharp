using System;
using System.Collections.Generic;
using Tea;
namespace TeaUnitTests.Models
{
	public class TestRequest : TeaModel
    {
        public class TestRequestStrs : TeaModel
        {
            [NameInMap("ImageURL")]
            [Validation(Required = false)]
            public string ImageURL { get; set; }
        }

        [NameInMap("Str1")]
        [Validation(Required = false)]
        public string Str1 { get; set; }

        [NameInMap("Str2")]
        [Validation(Required = false)]
        public string Str2 { get; set; }

        [NameInMap("Strs")]
        [Validation(Required = false)]
        public List<TestRequestStrs> Strs { get; set; }


    }
}

