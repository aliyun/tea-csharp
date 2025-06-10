using Darabonba.Http;
using Xunit;

namespace DaraUnitTests
{
    public class URLTest
    {
        [Fact]
        public void Test_Parse()
        {
            string url = "https://sdk:test@ecs.aliyuncs.com:443/sdk/?api&ok=test#sddd";
            URL ret = URL.Parse(url);
            Assert.Equal("/sdk/?api&ok=test", ret.Path());
            Assert.Equal("/sdk/", ret.Pathname());
            Assert.Equal("https", ret.Protocol());
            Assert.Equal("ecs.aliyuncs.com", ret.Hostname());
            Assert.Equal("ecs.aliyuncs.com", ret.Host());
            Assert.Equal("443", ret.Port());
            Assert.Equal("sddd", ret.Hash());
            Assert.Equal("api&ok=test", ret.Search());
            Assert.Equal("https://sdk:test@ecs.aliyuncs.com/sdk/?api&ok=test#sddd", ret.Href());
            Assert.Equal("sdk:test", ret.Auth());
        }

        [Fact]
        public void Test_UrlEncode()
        {
            string result = URL.UrlEncode("https://www.baidu.com/");
            Assert.Equal("https%3A%2F%2Fwww.baidu.com%2F", result);
        }
        
        [Fact]
        public void Test_PercentEncode()
        {
            Assert.Null(URL.PercentEncode(null));
            Assert.Equal("test%3D", URL.PercentEncode("test="));

            string result = URL.PercentEncode("https://www.bai+*~du.com/");
            Assert.Equal("https%3A%2F%2Fwww.bai%2B%2A~du.com%2F", result);
        }

        [Fact]
        public void Test_PathEncode()
        {
            string result = URL.PathEncode("/work_space/DARABONBA/GIT/darabonba-util/ts");
            Assert.Equal("/work_space/DARABONBA/GIT/darabonba-util/ts", result);
        }
    }
}