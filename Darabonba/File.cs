using System;
using System.IO;
using System.Threading.Tasks;

namespace Darabonba
{
    public class File : IDisposable
    {
        public readonly string _path;
        public FileInfo _fileInfo;
        public FileStream _fileStream;
        public long _position;


        public File(string path)
        {
            _path = path;
            _fileInfo = new FileInfo(path);
            _position = 0;
            _fileStream = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }

        public void Dispose()
        {
            if (_fileStream != null)
            {
                _fileStream.Dispose();
            }
        }

        public string Path()
        {
            return _path;
        }

        private void EnsureFileInfoLoaded()
        {
            if (!_fileInfo.Exists)
            {
                _fileInfo = new FileInfo(_path);
            }
        }

        public Date CreateTime()
        {
            EnsureFileInfoLoaded();
            return new Date(_fileInfo.CreationTimeUtc);
        }

        public async Task<Date> CreateTimeAsync()
        {
            return await Task.Run(() =>
            {
                EnsureFileInfoLoaded();
                return new Date(_fileInfo.CreationTimeUtc);
            });
        }

        public Date ModifyTime()
        {
            EnsureFileInfoLoaded();
            return new Date(_fileInfo.LastWriteTimeUtc);
        }

        public async Task<Date> ModifyTimeAsync()
        {
            EnsureFileInfoLoaded();
            return new Date(_fileInfo.LastWriteTimeUtc);
        }

        public int Length()
        {
            EnsureFileInfoLoaded();
            return (int)_fileInfo.Length;
        }

        public async Task<int> LengthAsync()
        {
            EnsureFileInfoLoaded();
            return (int)_fileInfo.Length;
        }

        public byte[] Read(int size)
        {
            _fileStream.Seek(_position, SeekOrigin.Begin);
            byte[] buffer = new byte[size];
            int bytesRead = _fileStream.Read(buffer, 0, size);
            if (bytesRead == 0)
            {
                return null;
            }
            _position += bytesRead;
            return buffer;
        }

        public async Task<byte[]> ReadAsync(int size)
        {
            _fileStream.Seek(_position, SeekOrigin.Begin);
            byte[] buffer = new byte[size];
            int bytesRead = await _fileStream.ReadAsync(buffer, 0, size);
            if (bytesRead == 0)
            {
                return null;
            }

            _position += bytesRead;
            return buffer;
        }

        public void Write(byte[] data)
        {
            _fileStream.Seek(0, SeekOrigin.End);
            _fileStream.Write(data, 0, data.Length);
            _fileStream.Flush();
            _fileInfo.Refresh();
        }

        public async Task WriteAsync(byte[] data)
        {
            _fileStream.Seek(0, SeekOrigin.End);
            await _fileStream.WriteAsync(data, 0, data.Length);
            await _fileStream.FlushAsync();
            _fileInfo.Refresh();
        }


        public void Close()
        {
            if (_fileStream != null)
            {
                _fileStream.Flush();
                _fileStream.Close();
                _fileStream = null;
            }
        }
        public async Task CloseAsync()
        {
            if (_fileStream != null)
            {
                await _fileStream.FlushAsync();
                _fileStream.Close();
                _fileStream = null;
            }
        }

        public static bool Exists(string path)
        {
            return System.IO.File.Exists(path);
        }

        public static async Task<bool> ExistsAsync(string path)
        {
            return System.IO.File.Exists(path);
        }

        public static FileStream CreateReadStream(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
        }

        public static FileStream CreateWriteStream(string path)
        {
            return new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.None);
        }
    }
}