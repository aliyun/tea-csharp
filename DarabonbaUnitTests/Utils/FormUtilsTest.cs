using System.Collections.Generic;
using System.IO;
using System.Text;
using Darabonba.Models;
using Darabonba.Utils;
using Xunit;

namespace DaraUnitTests.Utils
{
    public class FormUtilsTest
    {
        [Fact]
        public void Test_ToFormString()
        {
            Assert.Empty(FormUtils.ToFormString(null));
            Assert.Empty(FormUtils.ToFormString(new Dictionary<string, object>()));

            Dictionary<string, object> dict = new Dictionary<string, object>
            {
                { "form", "test" },
                { "param", "test" },
                { "testNull", null }
            };
            Assert.Equal("form=test&param=test", FormUtils.ToFormString(dict));
        }

        [Fact]
        public void Test_GetBoundary()
        {
            Assert.Equal(14, FormUtils.GetBoundary().Length);
        }

        [Fact]
        public void Test_ToFileForm()
        {
            Stream fileFormStream = FormUtils.ToFileForm(new Dictionary<string, object>(), "boundary");
            Assert.NotNull(fileFormStream);

            string formStr = GetFormStr(fileFormStream);
            Assert.Equal("--boundary--\r\n", formStr);

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("stringkey", "string");
            fileFormStream = FormUtils.ToFileForm(dict, "boundary");
            formStr = GetFormStr(fileFormStream);
            Assert.Equal("--boundary\r\n" +
                "Content-Disposition: form-data; name=\"stringkey\"\r\n\r\n" +
                "string\r\n" +
                "--boundary--\r\n", formStr);

            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            FileStream file = File.OpenRead("../../../../DarabonbaUnitTests/Fixtures/test.json");
            FileField fileField = new FileField
            {
                Filename = "fakefilename",
                ContentType = "application/json",
                Content = file
            };
            dict = new Dictionary<string, object>
            {
                { "stringkey", "string" },
                { "filefield", fileField }
            };
            fileFormStream = FormUtils.ToFileForm(dict, "boundary");
            formStr = GetFormStr(fileFormStream);
            Assert.Equal("--boundary\r\n" +
                "Content-Disposition: form-data; name=\"stringkey\"\r\n\r\n" +
                "string\r\n" +
                "--boundary\r\n" +
                "Content-Disposition: form-data; name=\"filefield\"; filename=\"fakefilename\"\r\n" +
                "Content-Type: application/json\r\n" +
                "\r\n" +
                "{\"key\":\"value\"}" +
                "\r\n" +
                "--boundary--\r\n", formStr);
        }

        private string GetFormStr(Stream stream)
        {
            string formStr = string.Empty;
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                formStr += Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }

            return formStr;
        }
    }
}