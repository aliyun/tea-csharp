using System.Text;
using Darabonba;
using Xunit;
using System;
using System.IO;
using System.Threading.Tasks;
using File = Darabonba.File;

namespace DaraUnitTests
{
    public class FileTest : IAsyncLifetime
    {
        private File _file;
        private FileInfo _fileInfo;

        private string tempTestFile = Path.GetTempFileName();

        public async Task InitializeAsync()
        {
            System.IO.File.WriteAllText(tempTestFile, "Test For File");
            _file = new File(tempTestFile);
            _fileInfo = new FileInfo(tempTestFile);
        }

        public Task DisposeAsync()
        {
            _file.Close();
            System.IO.File.Delete(tempTestFile);
#if NET45
            return Task.FromResult(0);
#else
            return Task.CompletedTask;
#endif
        }

        [Fact]
        public async Task Test_All()
        {
            TestPath();
            await TestCreateTimeAsync();
            await TestModifyTimeAsync();
            await TestLengthAsync();
            TestExists();
            await TestExistsAsync();
            TestRead();
            await TestReadAsync();
            TestWrite();
            await TestWriteAsync();
            TestCreateWriteStream();
            TestCreateReadStream();
        }

        private void TestPath()
        {
            Assert.Equal(tempTestFile, _file.Path());
        }

        private async Task TestCreateTimeAsync()
        {
            var createTime = await _file.CreateTimeAsync();
            Assert.Equal(_fileInfo.CreationTimeUtc.ToString("yyyy-MM-dd HH:mm:ssZ"), createTime.DateTime.ToString("yyyy-MM-dd HH:mm:ssZ"));
        }

        private async Task TestModifyTimeAsync()
        {
            var modifyTime = await _file.ModifyTimeAsync();
            Assert.Equal(_fileInfo.LastWriteTimeUtc.ToString("yyyy-MM-dd HH:mm:ssZ"), modifyTime.DateTime.ToString("yyyy-MM-dd HH:mm:ssZ"));
        }

        private async Task TestLengthAsync()
        {
            var length = await _file.LengthAsync();
            Assert.Equal(_fileInfo.Length, length);
            string tempTestFile1 = Path.GetTempFileName();
            System.IO.File.WriteAllText(tempTestFile1, "Hello, World!");
            var newFile = new File(tempTestFile1);
            var newLength = await newFile.LengthAsync();
            Assert.Equal(_fileInfo.Length, newLength);
            await newFile.CloseAsync();
        }

        private void TestExists()
        {
            Assert.True(File.Exists(tempTestFile));
            Assert.False(File.Exists("../../../../TeaUnitTests/Fixtures/test1.txt"));
        }

        private async Task TestExistsAsync()
        {
            Assert.True(await File.ExistsAsync(tempTestFile));
            Assert.False(await File.ExistsAsync("../../../../TeaUnitTests/Fixtures/test1.txt"));
        }

        private void TestRead()
        {
            byte[] text1 = _file.Read(4);
            Assert.Equal("Test", Encoding.UTF8.GetString(text1));
            byte[] text2 = _file.Read(4);
            Assert.Equal(" For", Encoding.UTF8.GetString(text2));
            Assert.Equal(8, _file._position);
            string tempEmptyFile = Path.GetTempFileName();
            File emptyFile = new File(tempEmptyFile);
            byte[] empty = emptyFile.Read(10);
            Assert.Null(empty);
            emptyFile.Close();
        }

        private async Task TestReadAsync()
        {
            _file._position = 0;
            byte[] text1 = await _file.ReadAsync(4);
            Assert.Equal("Test", Encoding.UTF8.GetString(text1));
            byte[] text2 = await _file.ReadAsync(4);
            Assert.Equal(" For", Encoding.UTF8.GetString(text2));
            Assert.Equal(8, _file._position);
            string tempEmptyFile = Path.GetTempFileName();
            File emptyFile = new File(tempEmptyFile);
            byte[] empty = await emptyFile.ReadAsync(10);
            Assert.Null(empty);
            await emptyFile.CloseAsync();
        }

        private void TestWrite()
        {
            var expectedLen = _fileInfo.Length;
            _file.Write(Encoding.UTF8.GetBytes(" Test"));
            Date modifyTime = _file.ModifyTime();
            int length = _file.Length();
            Assert.Equal(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ssZ"), modifyTime.DateTime.ToString("yyyy-MM-dd HH:mm:ssZ"));
            Assert.Equal(expectedLen + 5, length);
            string tempNewFile = Path.GetTempFileName();
            File newFile = new File(tempNewFile);
            newFile.Write(Encoding.UTF8.GetBytes("Test"));
            byte[] text = newFile.Read(4);
            Assert.Equal("Test", Encoding.UTF8.GetString(text));
            newFile.Close();
        }

        private async Task TestWriteAsync()
        {
            var expectedLen = _fileInfo.Length;
            await _file.WriteAsync(Encoding.UTF8.GetBytes(" Test"));
            Date modifyTime = await _file.ModifyTimeAsync();
            int length = await _file.LengthAsync();
            Assert.Equal(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ssZ"), modifyTime.DateTime.ToString("yyyy-MM-dd HH:mm:ssZ"));
            Assert.Equal(expectedLen + 10, length);
            string tempNewFile = Path.GetTempFileName();
            File newFile = new File(tempNewFile);
            await newFile.WriteAsync(Encoding.UTF8.GetBytes("Test"));
            byte[] text = await newFile.ReadAsync(4);
            Assert.Equal("Test", Encoding.UTF8.GetString(text));
            await newFile.CloseAsync();
        }

        private void TestCreateWriteStream()
        {
            string tempWriteFile = Path.GetTempFileName();
            using (FileStream stream = File.CreateWriteStream(tempWriteFile))
            {
                Assert.NotNull(stream);
                Assert.True(stream.CanWrite);
                byte[] contentBytes = Encoding.UTF8.GetBytes("Test Write");
                stream.Write(contentBytes, 0, contentBytes.Length);
            }
            string finalContent = System.IO.File.ReadAllText(tempWriteFile);
            Assert.EndsWith("Test Write", finalContent);
        }

        private void TestCreateReadStream()
        {
            string tempReadFile = Path.GetTempFileName();
            System.IO.File.WriteAllText(tempReadFile, "Test For File");
            using (FileStream stream = File.CreateReadStream(tempReadFile))
            {
                Assert.NotNull(stream);
                Assert.True(stream.CanRead);
                byte[] buffer = new byte[13];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string actualContent = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Assert.Equal("Test For File", actualContent);
            }
        }
    }
}