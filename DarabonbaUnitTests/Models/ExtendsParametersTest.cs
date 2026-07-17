using System.Collections.Generic;
using Darabonba.Models;
using Xunit;

namespace DaraUnitTests.Models
{
    public class ExtendsParametersTest
    {
        [Fact]
        public void Test_ToMap_FromMap_Copy_Validate()
        {
            var empty = new ExtendsParameters();
            empty.Validate();
            Assert.Empty(empty.ToMap());
            Assert.Empty(empty.ToMap(true));

            ExtendsParameters nullConvert = (AlibabaCloud.TeaUtil.Models.ExtendsParameters)null;
            Assert.Null(nullConvert);

            var teaExt = new AlibabaCloud.TeaUtil.Models.ExtendsParameters
            {
                Headers = new Dictionary<string, string> { { "h", "v" } },
                Queries = new Dictionary<string, string> { { "q", "1" } }
            };
            ExtendsParameters converted = teaExt;
            Assert.Equal("v", converted.Headers["h"]);
            Assert.Equal("1", converted.Queries["q"]);

            var model = new ExtendsParameters
            {
                Headers = new Dictionary<string, string> { { "a", "b" } },
                Queries = new Dictionary<string, string> { { "c", "d" } }
            };
            var map = model.ToMap();
            Assert.Equal(model.Headers, map["headers"]);
            Assert.Equal(model.Queries, map["queries"]);

            var fromMap = ExtendsParameters.FromMap(map);
            Assert.Equal("b", fromMap.Headers["a"]);
            Assert.Equal("d", fromMap.Queries["c"]);

            var copy = model.Copy();
            Assert.Equal("b", copy.Headers["a"]);
            Assert.Equal("d", copy.Queries["c"]);

            var copyNoStream = model.CopyWithoutStream();
            Assert.Equal("b", copyNoStream.Headers["a"]);

            var partial = ExtendsParameters.FromMap(new Dictionary<string, object>());
            Assert.Null(partial.Headers);
            Assert.Null(partial.Queries);
        }
    }
}
