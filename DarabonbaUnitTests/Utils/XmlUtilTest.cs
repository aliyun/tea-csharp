using System.Collections.Generic;
using Darabonba;
using Darabonba.Utils;
using Xunit;
using DaraUnitTests.Models;
using static DaraUnitTests.Models.ListAllMyBucketsResult;
using static DaraUnitTests.Models.ListAllMyBucketsResult.Buckets;

namespace DaraUnitTests.Utils
{
    public class XmlUtilTest
    {
        ToBodyModel model;
        public XmlUtilTest()
        {
            model = new ToBodyModel();
            ListAllMyBucketsResult result = new ListAllMyBucketsResult();
            Buckets buckets = new Buckets();
            buckets.bucket = new List<Bucket>();
            buckets.bucket.Add(new Bucket { CreationDate = "2015-12-17T18:12:43.000Z", ExtranetEndpoint = "oss-cn-shanghai.aliyuncs.com", IntranetEndpoint = "oss-cn-shanghai-internal.aliyuncs.com", Location = "oss-cn-shanghai", Name = "app-base-oss", StorageClass = "Standard" });
            buckets.bucket.Add(new Bucket { CreationDate = "2014-12-25T11:21:04.000Z", ExtranetEndpoint = "oss-cn-hangzhou.aliyuncs.com", IntranetEndpoint = "oss-cn-hangzhou-internal.aliyuncs.com", Location = "oss-cn-hangzhou", Name = "atestleo23", StorageClass = "IA" });
            buckets.bucket.Add(null);
            result.buckets = buckets;
            Owner owner = new Owner { ID = 512, DisplayName = "51264" };
            result.owner = owner;
            model.listAllMyBucketsResult = result;
            model.listAllMyBucketsResult.testStrList = new List<string> { "1", "2" };
            model.listAllMyBucketsResult.owners = new List<Owner>();
            model.listAllMyBucketsResult.owners.Add(owner);
            model.listAllMyBucketsResult.TestDouble = 1;
            model.listAllMyBucketsResult.TestFloat = 2;
            model.listAllMyBucketsResult.TestLong = 3;
            model.listAllMyBucketsResult.TestShort = 4;
            model.listAllMyBucketsResult.TestUInt = 5;
            model.listAllMyBucketsResult.TestULong = 6;
            model.listAllMyBucketsResult.TestUShort = 7;
            model.listAllMyBucketsResult.TestBool = true;
            model.listAllMyBucketsResult.TestNull = null;
            model.listAllMyBucketsResult.TestString = "string";
            model.listAllMyBucketsResult.TestListNull = null;
            model.listAllMyBucketsResult.dict = new Dictionary<string, string> { { "key", "value" } };
        }

        [Fact]
        public void Test_ToXml()
        {
            Model modelNull = new Model();
            Assert.Empty(XmlUtil.ToXML(modelNull.ToMap()));

            ToBodyModel model = new ToBodyModel();
            ListAllMyBucketsResult result = new ListAllMyBucketsResult();
            Buckets buckets = new Buckets
            {
                bucket = new List<Bucket>
            {
                new Bucket { CreationDate = "2015-12-17T18:12:43.000Z", ExtranetEndpoint = "oss-cn-shanghai.aliyuncs.com", IntranetEndpoint = "oss-cn-shanghai-internal.aliyuncs.com", Location = "oss-cn-shanghai", Name = "app-base-oss", StorageClass = "Standard" },
                new Bucket { CreationDate = "2014-12-25T11:21:04.000Z", ExtranetEndpoint = "oss-cn-hangzhou.aliyuncs.com", IntranetEndpoint = "oss-cn-hangzhou-internal.aliyuncs.com", Location = "oss-cn-hangzhou", Name = "atestleo23", StorageClass = "IA" },
                null
            }
            };
            result.buckets = buckets;
            Owner owner = new Owner { ID = 512, DisplayName = "51264" };
            result.owner = owner;
            model.listAllMyBucketsResult = result;
            model.listAllMyBucketsResult.testStrList = new List<string> { "1", "2" };
            model.listAllMyBucketsResult.owners = new List<Owner>
            {
                owner
            };
            model.listAllMyBucketsResult.TestDouble = 1;
            model.listAllMyBucketsResult.TestFloat = 2;
            model.listAllMyBucketsResult.TestLong = 3;
            model.listAllMyBucketsResult.TestShort = 4;
            model.listAllMyBucketsResult.TestUInt = 5;
            model.listAllMyBucketsResult.TestULong = 6;
            model.listAllMyBucketsResult.TestUShort = 7;
            model.listAllMyBucketsResult.TestBool = true;
            model.listAllMyBucketsResult.TestNull = null;
            model.listAllMyBucketsResult.TestListNull = null;
            string xmlStr = XmlUtil.ToXML(model.ToMap());
            Assert.NotNull(xmlStr);

            Dictionary<string, object> xmlBody = (Dictionary<string, object>)XmlUtil.ParseXml(xmlStr, typeof(ToBodyModel));
            ToBodyModel teaModel = Model.ToObject<ToBodyModel>(xmlBody);
            Assert.NotNull(teaModel);
            Assert.Equal(1, teaModel.listAllMyBucketsResult.TestDouble);

            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                "<body><Contents><Owner><DisplayName>disName</DisplayName></Owner><Key>key</Key></Contents><Contents/></body>";
            Dictionary<string, object> map = XmlUtil.ParseXml(xml, null);
            Assert.False(map.ContainsKey("xml"));
            Assert.Single(map);
            Assert.True(((Dictionary<string, object>)map["body"]).ContainsKey("Contents"));
            List<object> list = (List<object>)((Dictionary<string, object>)map["body"])["Contents"];
            Assert.Equal(2, list.Count);
            Assert.Equal("key", ((Dictionary<string, object>)list[0])["Key"]);
            Assert.Equal("disName", ((Dictionary<string, object>)((Dictionary<string, object>)list[0])["Owner"])["DisplayName"]);
            Assert.Null((Dictionary<string, object>)list[1]);
        }

        [Fact]
        public void TestXml()
        {
            string xmlStr = XmlUtil.SerializeXml(model);
            Assert.NotNull(xmlStr);
            Assert.NotEmpty(xmlStr);

            Assert.Equal(xmlStr, XmlUtil.SerializeXml(model.ToMap()));

            model.listAllMyBucketsResult.dict = null;
            xmlStr = XmlUtil.SerializeXml(model);
            Dictionary<string, object> dict = XmlUtil.DeserializeXml(xmlStr, model.GetType());
            Assert.NotNull(dict);
        }
    }
}
