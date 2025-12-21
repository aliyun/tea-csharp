using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Darabonba.Models;

namespace Darabonba.Utils
{
    public class StreamUtils
    {
        private const string DATA_PREFIX = "data:";
        private const string EVENT_PREFIX = "event:";
        private const string ID_PREFIX = "id:";
        private const string RETRY_PREFIX = "retry:";

        public static Stream BytesReadable(string str)
        {
            return BytesReadable(Encoding.UTF8.GetBytes(str));
        }

        public static Stream BytesReadable(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public static string ToString(byte[] val)
        {
            return Encoding.UTF8.GetString(val);
        }

        public static object ParseJSON(string val)
        {
            return JsonConvert.DeserializeObject(val);
        }

        public static byte[] Read(Stream stream, int length)
        {
            byte[] data = new byte[length];
            stream.Read(data, 0, length);
            return data;
        }

        public static void Pipe(Stream readStream, Stream writeStream)
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = readStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
            }
        }

        public static Stream StreamFor(object data)
        {
            if (data is Stream)
            {
                Stream stream = data as Stream;
                if (stream.CanRead)
                {
                    Stream copy = new MemoryStream();
                    stream.Position = 0;
                    stream.CopyTo(copy);
                    copy.Position = 0;
                    return copy;
                }
                throw new Exception("stream is not readable");
            }
            if (data is string)
            {
                string str = data as string;
                Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(str));
                return stream;
            }
            throw new Exception("data is not Stream or String");
        }

        public static byte[] ReadAsBytes(Stream stream)
        {
            int bufferLength = 4096;
            using (var ms = new MemoryStream())
            {
                var buffer = new byte[bufferLength];

                while (true)
                {
                    var length = stream.Read(buffer, 0, bufferLength);
                    if (length == 0)
                    {
                        break;
                    }

                    ms.Write(buffer, 0, length);
                }

                ms.Seek(0, SeekOrigin.Begin);
                var bytes = new byte[ms.Length];
                ms.Read(bytes, 0, bytes.Length);

                stream.Close();
                stream.Dispose();

                return bytes;
            }
        }

        public async static Task<byte[]> ReadAsBytesAsync(Stream stream)
        {
            int bufferLength = 4096;
            using (var ms = new MemoryStream())
            {
                var buffer = new byte[bufferLength];

                while (true)
                {
                    var length = await stream.ReadAsync(buffer, 0, bufferLength);
                    if (length == 0)
                    {
                        break;
                    }

                    await ms.WriteAsync(buffer, 0, length);
                }

                ms.Seek(0, SeekOrigin.Begin);
                var bytes = new byte[ms.Length];
                await ms.ReadAsync(bytes, 0, bytes.Length);

                stream.Close();
                stream.Dispose();

                return bytes;
            }
        }

        public static string ReadAsString(Stream stream)
        {
            return ToString(ReadAsBytes(stream));
        }

        public static async Task<string> ReadAsStringAsync(Stream stream)
        {
            return ToString(await ReadAsBytesAsync(stream));
        }

        public static object ReadAsJSON(Stream stream)
        {
            object jResult = ParseJSON(ReadAsString(stream));
            object result = JSONUtils.Deserialize(jResult);
            return result;
        }

        public async static Task<object> ReadAsJSONAsync(Stream stream)
        {
            object jResult = ParseJSON(await ReadAsStringAsync(stream));
            object result = JSONUtils.Deserialize(jResult);
            return result;
        }

        public class EventResult
        {
            public List<SSEEvent> Events { get; set; }
            public string Remain { get; set; }

            public EventResult(List<SSEEvent> events, string remain)
            {
                Events = events;
                Remain = remain;
            }
        }

        private static EventResult TryGetEvents(string head, string chunk)
        {
            string all = head + chunk;
            var events = new List<SSEEvent>();
            var start = 0;
            for (var i = 0; i < all.Length - 1; i++)
            {
                // message separated by \n\n
                if (all[i] == '\n' && i + 1 < all.Length && all[i + 1] == '\n')
                {
                    var rawEvent = all.Substring(start, i - start).Trim();
                    var sseEvent = ParseEvent(rawEvent);
                    events.Add(sseEvent);
                    start = i + 2;
                    i++;
                }
            }
            string remain = all.Substring(start);
            return new EventResult(events, remain);
        }

        private static SSEEvent ParseEvent(string rawEvent)
        {
            var sseEvent = new SSEEvent();
            var lines = rawEvent.Split('\n');

                foreach (var line in lines)
                {
                    if (line.StartsWith(DATA_PREFIX))
                    {
                        sseEvent.Data = line.Substring(DATA_PREFIX.Length).Trim();
                    }
                    else if (line.StartsWith(EVENT_PREFIX))
                    {
                        sseEvent.Event = line.Substring(EVENT_PREFIX.Length).Trim();
                    }
                    else if (line.StartsWith(ID_PREFIX))
                    {
                        sseEvent.Id = line.Substring(ID_PREFIX.Length).Trim();
                    }
                    else if (line.StartsWith(RETRY_PREFIX))
                    {
                        var retryData = line.Substring(RETRY_PREFIX.Length).Trim();
                        int retryValue;
                        if (int.TryParse(retryData, out retryValue))
                        {
                            sseEvent.Retry = retryValue;
                        }
                    }
                    else if (line.StartsWith(":"))
                    {
                        // ignore the line
                    }
                }

            return sseEvent;
        }


        public static IEnumerable<SSEEvent> ReadAsSSE(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var buffer = new char[4096];
                var rest = string.Empty;
                int count;

                while ((count = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    var chunk = new string(buffer, 0, count);

                    var eventResult = TryGetEvents(rest, chunk);
                    rest = eventResult.Remain;

                    if (eventResult.Events != null && eventResult.Events.Count > 0)
                    {
                        foreach (var @event in eventResult.Events)
                        {
                            yield return @event;
                        }
                    }
                }
            }
        }
    }
}