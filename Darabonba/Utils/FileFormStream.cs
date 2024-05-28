using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darabonba;

namespace Darabonba.Utils
{
    public class FileField
    {
        [NameInMap("filename")]
        public string Filename { get; set; }

        [NameInMap("contentType")]
        public string ContentType { get; set; }

        [NameInMap("content")]
        public Stream Content { get; set; }
    }
    
    public class FileFormStream : Stream
    {

        private Stream streamingStream { get; set; }

        private Dictionary<string, object> form { get; set; }

        private string boundary;

        private List<string> keys;

        private int index;

        private bool streaming;

        private long position;

        private long length;

        public FileFormStream(Dictionary<string, object> form, string boundary)
        {
            this.form = form;
            this.boundary = boundary;
            index = 0;
            keys = form.Keys.ToList();
            streaming = false;
        }

        public override bool CanRead { get { return true; } }

        public override bool CanSeek { get { return false; } }

        public override bool CanWrite { get { return false; } }

        public override long Length { get { return length; } }

        public override long Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (streaming)
            {
                int bytesRLength;
                if (streamingStream != null && (bytesRLength = streamingStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    return bytesRLength;
                }
                else
                {
                    streaming = false;
                    if (streamingStream != null && streamingStream.CanSeek)
                    {
                        streamingStream.Seek(0, SeekOrigin.Begin);
                    }
                    streamingStream = null;
                    byte[] bytesFileEnd = Encoding.UTF8.GetBytes("\r\n");
                    for (int i = 0; i < bytesFileEnd.Length; i++)
                    {
                        buffer[i] = bytesFileEnd[i];
                    }
                    index++;
                    return bytesFileEnd.Length;
                }
            }

            if (index < keys.Count)
            {
                string name = this.keys[this.index];
                object fieldValue = form[name];
                if (fieldValue is FileField)
                {
                    FileField fileField = (FileField) fieldValue;

                    streaming = true;
                    streamingStream = fileField.Content;
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("--").Append(boundary).Append("\r\n");
                    stringBuilder.Append("Content-Disposition: form-data; name=\"").Append(name).Append("\"; filename=\"").Append(fileField.Filename).Append("\"\r\n");
                    stringBuilder.Append("Content-Type: ").Append(fileField.ContentType).Append("\r\n\r\n");
                    byte[] startBytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    for (int i = 0; i < startBytes.Length; i++)
                    {
                        buffer[i] = startBytes[i];
                    }
                    return startBytes.Length;
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("--").Append(boundary).Append("\r\n");
                    stringBuilder.Append("Content-Disposition: form-data; name=\"").Append(name).Append("\"\r\n\r\n");
                    stringBuilder.Append(fieldValue.ToString()).Append("\r\n");
                    byte[] formBytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    for (int i = 0; i < formBytes.Length; i++)
                    {
                        buffer[i] = formBytes[i];
                    }
                    index++;
                    return formBytes.Length;
                }
            }
            else if (index == keys.Count)
            {
                string endStr = string.Format("--{0}--\r\n", boundary);
                byte[] endBytes = Encoding.UTF8.GetBytes(endStr);
                for (int i = 0; i < endBytes.Length; i++)
                {
                    buffer[i] = endBytes[i];
                }
                index++;
                return endBytes.Length;
            }
            else
            {
                return 0;
            }
        }

        public new async Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            if (streaming)
            {
                int bytesRLength;
                if (streamingStream != null && (bytesRLength = await streamingStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    return bytesRLength;
                }
                else
                {
                    streaming = false;
                    if (streamingStream != null)
                    {
                        streamingStream.Flush();
                        streamingStream.Close();
                    }
                    streamingStream = null;
                    byte[] bytesFileEnd = Encoding.UTF8.GetBytes("\r\n");
                    for (int i = 0; i < bytesFileEnd.Length; i++)
                    {
                        buffer[i] = bytesFileEnd[i];
                    }
                    index++;
                    return bytesFileEnd.Length;
                }
            }

            if (index < keys.Count)
            {
                string name = this.keys[this.index];
                object fieldValue = form[name];
                if (fieldValue is FileField)
                {
                    FileField fileField = (FileField) fieldValue;

                    streaming = true;
                    streamingStream = fileField.Content;
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("--").Append(boundary).Append("\r\n");
                    stringBuilder.Append("Content-Disposition: form-data; name=\"").Append(name).Append("\"; filename=\"").Append(fileField.Filename).Append("\"\r\n");
                    stringBuilder.Append("Content-Type: ").Append(fileField.ContentType).Append("\r\n\r\n");
                    byte[] startBytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    for (int i = 0; i < startBytes.Length; i++)
                    {
                        buffer[i] = startBytes[i];
                    }
                    return startBytes.Length;
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("--").Append(boundary).Append("\r\n");
                    stringBuilder.Append("Content-Disposition: form-data; name=\"").Append(name).Append("\"\r\n\r\n");
                    stringBuilder.Append(fieldValue.ToString()).Append("\r\n");
                    byte[] formBytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    for (int i = 0; i < formBytes.Length; i++)
                    {
                        buffer[i] = formBytes[i];
                    }
                    index++;
                    return formBytes.Length;
                }
            }
            else if (index == keys.Count)
            {
                string endStr = string.Format("--{0}--\r\n", boundary);
                byte[] endBytes = Encoding.UTF8.GetBytes(endStr);
                for (int i = 0; i < endBytes.Length; i++)
                {
                    buffer[i] = endBytes[i];
                }
                index++;
                return endBytes.Length;
            }
            else
            {
                return 0;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            length = value;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        internal static string PercentEncode(string value)
        {
            if (value == null)
            {
                return null;
            }
            var stringBuilder = new StringBuilder();
            var text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            var bytes = Encoding.UTF8.GetBytes(value);
            foreach (char c in bytes)
            {
                if (text.IndexOf(c) >= 0)
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int) c));
                }
            }

            return stringBuilder.ToString().Replace("+", "%20")
                .Replace("*", "%2A").Replace("%7E", "~");
        }
    }
}