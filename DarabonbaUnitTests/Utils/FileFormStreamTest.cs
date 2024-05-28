using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Darabonba.Utils;
using Xunit;

namespace DaraUnitTests.Utils
{
    public class FileFormStreamTest
    {
        [Fact]
        public void Test_FileField()
        {
            FileField fileField = new FileField();
            fileField.Content = new MemoryStream();
            fileField.ContentType = "contentType";
            fileField.Filename = "fileName";
            Assert.NotNull(fileField);
            Assert.Equal("contentType", fileField.ContentType);
            Assert.Equal("fileName", fileField.Filename);
            Assert.NotNull(fileField.Content);
        }
        
        [Fact]
        public void Test_FileFormStream()
        {
            FileFormStream fileFormStream = new FileFormStream(new Dictionary<string, object>(), "");
            Assert.True(fileFormStream.CanRead);
            Assert.False(fileFormStream.CanSeek);
            Assert.False(fileFormStream.CanWrite);
            fileFormStream.Position = 1;
            Assert.Equal(1, fileFormStream.Position);
            fileFormStream.SetLength(2);
            Assert.Equal(2, fileFormStream.Length);
            Assert.Throws<NotImplementedException>(() => { fileFormStream.Flush(); });
            Assert.Throws<NotImplementedException>(() => { fileFormStream.Seek(0, System.IO.SeekOrigin.Begin); });
            Assert.Throws<NotImplementedException>(() => { fileFormStream.Write(new byte[1024], 0, 1024); });
        }

        [Fact]
        public void Test_Read()
        {
            FileFormStream fileFormStream = new FileFormStream(new Dictionary<string, object>(), "");
            Assert.Equal(6, fileFormStream.Read(new byte[1024], 0, 1024));

            FileField fileFieldNoContent = new FileField()
            {
                Filename = "noContent",
                Content = null,
                ContentType = "contentType"
            };
            MemoryStream content = new MemoryStream();
            byte[] contentBytes = Encoding.UTF8.GetBytes("This is file test. This sentence must be long");
            content.Write(contentBytes, 0, contentBytes.Length);
            content.Seek(0, SeekOrigin.Begin);
            FileField fileField = new FileField()
            {
                Filename = "haveContent",
                Content = content,
                ContentType = "contentType"
            };

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("key", "value");
            dict.Add("testKey", "testValue");
            dict.Add("haveFile", fileField);
            dict.Add("noFile", fileFieldNoContent);
            MemoryStream StreamResult = new MemoryStream();
            byte[] bytes = new byte[1024];
            fileFormStream = new FileFormStream(dict, "testBoundary");
            int readNoStreamLength = 0;
            while ((readNoStreamLength = fileFormStream.Read(bytes, 0, 1024)) != 0)
            {
                StreamResult.Write(bytes, 0, readNoStreamLength);
            }
            StreamResult.Seek(0, SeekOrigin.Begin);
            byte[] bytesResult = new byte[StreamResult.Length];
            StreamResult.Read(bytesResult, 0, (int) StreamResult.Length);
            string result = Encoding.UTF8.GetString(bytesResult);
            Assert.Equal("--testBoundary\r\nContent-Disposition: form-data; name=\"key\"\r\n\r\nvalue\r\n--testBoundary\r\nContent-Disposition: form-data; name=\"testKey\"\r\n\r\ntestValue\r\n--testBoundary\r\nContent-Disposition: form-data; name=\"haveFile\"; filename=\"haveContent\"\r\nContent-Type: contentType\r\n\r\nThis is file test. This sentence must be long\r\n--testBoundary\r\nContent-Disposition: form-data; name=\"noFile\"; filename=\"noContent\"\r\nContent-Type: contentType\r\n\r\n\r\n--testBoundary--\r\n", result);
        }

        [Fact]
        public async Task Test_ReadAsync()
        {
            FileFormStream fileFormStream = new FileFormStream(new Dictionary<string, object>(), "");
            Assert.Equal(6, await fileFormStream.ReadAsync(new byte[1024], 0, 1024));

            FileField fileFieldNoContent = new FileField()
            {
                Filename = "noContent",
                Content = null,
                ContentType = "contentType"
            };
            MemoryStream content = new MemoryStream();
            byte[] contentBytes = Encoding.UTF8.GetBytes("This is file test. This sentence must be long");
            content.Write(contentBytes, 0, contentBytes.Length);
            content.Seek(0, SeekOrigin.Begin);
            FileField fileField = new FileField()
            {
                Filename = "haveContent",
                Content = content,
                ContentType = "contentType"
            };

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("key", "value");
            dict.Add("testKey", "testValue");
            dict.Add("haveFile", fileField);
            dict.Add("noFile", fileFieldNoContent);
            MemoryStream StreamResult = new MemoryStream();
            byte[] bytes = new byte[1024];
            fileFormStream = new FileFormStream(dict, "testBoundary");
            int readNoStreamLength = 0;
            while ((readNoStreamLength = await fileFormStream.ReadAsync(bytes, 0, 1024)) != 0)
            {
                StreamResult.Write(bytes, 0, readNoStreamLength);
            }
            StreamResult.Seek(0, SeekOrigin.Begin);
            byte[] bytesResult = new byte[StreamResult.Length];
            StreamResult.Read(bytesResult, 0, (int) StreamResult.Length);
            string result = Encoding.UTF8.GetString(bytesResult);
            Assert.Equal("--testBoundary\r\nContent-Disposition: form-data; name=\"key\"\r\n\r\nvalue\r\n--testBoundary\r\nContent-Disposition: form-data; name=\"testKey\"\r\n\r\ntestValue\r\n--testBoundary\r\nContent-Disposition: form-data; name=\"haveFile\"; filename=\"haveContent\"\r\nContent-Type: contentType\r\n\r\nThis is file test. This sentence must be long\r\n--testBoundary\r\nContent-Disposition: form-data; name=\"noFile\"; filename=\"noContent\"\r\nContent-Type: contentType\r\n\r\n\r\n--testBoundary--\r\n", result);
        }

        [Fact]
        public void Test_PercentEncode()
        {
            Assert.Null(FileFormStream.PercentEncode(null));
            Assert.Equal("ab%3Dcd", FileFormStream.PercentEncode("ab=cd"));
        }
    }
}