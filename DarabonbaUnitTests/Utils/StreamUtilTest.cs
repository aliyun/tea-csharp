using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Darabonba.Models;
using Darabonba.Utils;
using Xunit;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System;

namespace DaraUnitTests.Utils
{
    public class SseServer : IDisposable
    {
        private readonly HttpListener _httpListener;
        private CancellationTokenSource _cancellationTokenSource;

        public SseServer(string uriPrefix)
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(uriPrefix);
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _httpListener.Start();
            Task.Run(() => HandleIncomingConnections(_cancellationTokenSource.Token));
        }

        private async Task HandleIncomingConnections(CancellationToken cancellationToken)
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var context = await _httpListener.GetContextAsync().ConfigureAwait(false);

                    if (context.Request.Url?.AbsolutePath == "/sse")
                    {
                        HandleSseResponse(context.Response);
                    }
                    else if (context.Request.Url?.AbsolutePath == "/sse_with_no_spaces")
                    {
                        HandleSseWithNoSpacesResponse(context.Response);
                    }
                    else if (context.Request.Url?.AbsolutePath == "/sse_invalid_retry")
                    {
                        HandleSseWithInvalidRetryResponse(context.Response);
                    }
                    else if (context.Request.Url?.AbsolutePath == "/sse_with_data_divided")
                    {
                        HandleSseWithDataDividedResponse(context.Response);
                    }
                }
                catch (HttpListenerException) when (cancellationToken.IsCancellationRequested)
                {
                    throw new HttpListenerException();
                }
            }
        }

        private void HandleSseResponse(HttpListenerResponse response)
        {
            int count = 0;
            Timer timer = null;
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            timer = new Timer(_ =>
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                
                if (count >= 5)
                {
                    cts.Cancel();
                    timer.Dispose();
                    response.Close();
                    return;
                }

                try
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(string.Format(
                        "data: {0}\nevent: flow\nid: sse-test\nretry: 3\n:heartbeat\n\n",
                        JsonConvert.SerializeObject(new { count = count })));
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.OutputStream.Flush();
                    count++;
                }
                catch (ObjectDisposedException ex)
                {
                    Console.WriteLine($"ObjectDisposedException caught: {ex.Message}");
                }
            }, null, 0, 100);
        }

        private void HandleSseWithNoSpacesResponse(HttpListenerResponse response)
        {
            int count = 0;
            Timer timer = null;
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            timer = new Timer(_ =>
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                
                if (count >= 5)
                {
                    cts.Cancel();
                    timer.Dispose();
                    response.Close();
                    return;
                }

                try
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(string.Format("data: {0}\nevent:flow\nid:sse-test\nretry:3\n\n", JsonConvert.SerializeObject(new { count = count })));
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.OutputStream.Flush();
                    count++;
                }
                catch (ObjectDisposedException ex)
                {
                    Console.WriteLine($"ObjectDisposedException caught: {ex.Message}");
                }
            }, null, 0, 100);
        }

        private void HandleSseWithInvalidRetryResponse(HttpListenerResponse response)
        {
            int count = 0;
            Timer timer = null;
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            timer = new Timer(_ =>
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                
                if (count >= 5)
                {
                    cts.Cancel();
                    timer.Dispose();
                    response.Close();
                    return;
                }

                try
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(string.Format("data: {0}\nevent:flow\nid:sse-test\nretry: abc\n\n", JsonConvert.SerializeObject(new { count = count })));
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.OutputStream.Flush();
                    count++;
                }
                catch (ObjectDisposedException ex)
                {
                    Console.WriteLine($"ObjectDisposedException caught: {ex.Message}");
                }
            }, null, 0, 100);
        }

        private void HandleSseWithDataDividedResponse(HttpListenerResponse response)
        {
            int count = 0;
            Timer timer = null;
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            timer = new Timer(_ =>
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (count >= 5)
                {
                    cts.Cancel();
                    timer.Dispose();
                    response.Close();
                    return;
                }

                if (count == 1)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes("data:{\"count\":");
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.OutputStream.Flush();
                    count++;
                    return;
                }

                if (count == 2)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(string.Format("{0},\"tag\":\"divided\"}}\nevent:flow\nid:sse-test\nretry:3\n\n", count++));
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.OutputStream.Flush();
                    return;
                }

                try
                {
                    byte[] buffer1 = Encoding.UTF8.GetBytes(string.Format("data: {0}\nevent:flow\nid:sse-test\nretry:3\n\n", JsonConvert.SerializeObject(new { count = count++ })));
                    response.OutputStream.Write(buffer1, 0, buffer1.Length);
                    response.OutputStream.Flush();
                }
                catch (ObjectDisposedException ex)
                {
                    Console.WriteLine($"ObjectDisposedException caught: {ex.Message}");
                }
            }, null, 0, 100);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _httpListener.Stop();
            _httpListener.Close();
        }

        public void Dispose()
        {
            Stop();
            ((IDisposable)_httpListener)?.Dispose();
            _cancellationTokenSource?.Dispose();
        }
    }


    public class StreamUtilTest : IAsyncLifetime
    {
        private SseServer server = new SseServer("http://localhost:8384/");

        public async Task InitializeAsync()
        {
            server.Start();
            await Task.Delay(1000);
        }

        public Task DisposeAsync()
        {
            server.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public void Test_ReadAsString()
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("test")))
            {
                Assert.Equal("test", StreamUtils.ReadAsString(stream));
            }
        }

        [Fact]
        public async void Test_ReadAsStringAsync()
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("test")))
            {
                Assert.Equal("test", await StreamUtils.ReadAsStringAsync(stream));
            }
        }

        [Fact]
        public void Test_ReadAsJSON()
        {
            string jsonStr = "{\"arrayObj\":[[{\"itemName\":\"item\",\"itemInt\":1},{\"itemName\":\"item2\",\"itemInt\":2}],[{\"itemName\":\"item3\",\"itemInt\":3}]],\"arrayList\":[[[1,2],[3,4]],[[5,6],[7]],[]],\"listStr\":[1,2,3],\"items\":[{\"total_size\":18,\"partNumber\":1,\"tags\":[{\"aa\":\"11\"}]},{\"total_size\":20,\"partNumber\":2,\"tags\":[{\"aa\":\"22\"}]}],\"next_marker\":\"\",\"test\":{\"total_size\":19,\"partNumber\":1,\"tags\":[{\"aa\":\"11\"}]}}";
            byte[] array = Encoding.UTF8.GetBytes(jsonStr);
            using (MemoryStream stream = new MemoryStream(array))
            {
                Dictionary<string, object> dic = (Dictionary<string, object>)StreamUtils.ReadAsJSON(stream);
                Assert.NotNull(dic);
                List<object> listResult = (List<object>)dic["items"];
                Dictionary<string, object> item1 = (Dictionary<string, object>)listResult[0];
                Assert.Equal(18L, item1["total_size"]);
                Assert.Empty((string)dic["next_marker"]);
                Assert.Equal(2, ((List<object>)dic["arrayObj"]).Count);
            }

            jsonStr = "[{\"itemName\":\"item\",\"itemInt\":1},{\"itemName\":\"item2\",\"itemInt\":2}]";
            array = Encoding.UTF8.GetBytes(jsonStr);
            using (MemoryStream stream = new MemoryStream(array))
            {
                List<object> listResult = (List<object>)StreamUtils.ReadAsJSON(stream);
                Assert.NotNull(listResult);
                Dictionary<string, object> item1 = (Dictionary<string, object>)listResult[0];
                Assert.Equal("item", item1["itemName"]);
                Assert.Equal(1L, item1["itemInt"]);
            }
        }

        [Fact]
        public void Test_Read()
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("test")))
            {
                Assert.NotNull(StreamUtils.Read(stream, 3));
                Assert.Equal(3, StreamUtils.Read(stream, 3).Length);
            }
        }

        [Fact]
        public void Test_Pipe()
        {
            byte[] inputData = new byte[] {1, 2, 3, 4, 5};
            using (MemoryStream readStream = new MemoryStream(inputData))
            using (MemoryStream writeStream = new MemoryStream())
            {
                StreamUtils.Pipe(readStream, writeStream);
                byte[] outputData = writeStream.ToArray();
                Assert.Equal(inputData, outputData);
            }
            byte[] inputData1 = new byte[] { };
            using (MemoryStream readStream1 = new MemoryStream(inputData1))
            using (MemoryStream writeStream1 = new MemoryStream())
            {
                StreamUtils.Pipe(readStream1, writeStream1);
                byte[] outputData1 = writeStream1.ToArray();
                Assert.Empty(outputData1);
            }
        }

        [Fact]
        public void Test_StreamFor()
        {
            byte[] data = Encoding.UTF8.GetBytes("test");
            using (MemoryStream stream = new MemoryStream(data))
            {
                Stream copy = StreamUtils.StreamFor(stream);
                Assert.NotNull(copy);
                Assert.True(copy.CanRead);
                string str = new StreamReader(copy).ReadToEnd();
                Assert.Equal("test", str);

                string data1 = "test1";
                Stream copy1 = StreamUtils.StreamFor(data1);
                string str1 = new StreamReader(copy1).ReadToEnd();
                Assert.Equal("test1", str1);

                int data2 = 111;
                Exception ex = Assert.Throws<Exception>(() => StreamUtils.StreamFor(data2));
                Assert.Equal("data is not Stream or String", ex.Message);
            }
        }

        [Fact]
        public async void Test_ReadAsJSONAsync()
        {
            string jsonStr = "{\"arrayObj\":[[{\"itemName\":\"item\",\"itemInt\":1},{\"itemName\":\"item2\",\"itemInt\":2}],[{\"itemName\":\"item3\",\"itemInt\":3}]],\"arrayList\":[[[1,2],[3,4]],[[5,6],[7]],[]],\"listStr\":[1,2,3],\"items\":[{\"total_size\":18,\"partNumber\":1,\"tags\":[{\"aa\":\"11\"}]},{\"total_size\":20,\"partNumber\":2,\"tags\":[{\"aa\":\"22\"}]}],\"next_marker\":\"\",\"test\":{\"total_size\":19,\"partNumber\":1,\"tags\":[{\"aa\":\"11\"}]}}";
            byte[] array = Encoding.UTF8.GetBytes(jsonStr);
            using (MemoryStream stream = new MemoryStream(array))
            {
                Dictionary<string, object> dic = (Dictionary<string, object>)await StreamUtils.ReadAsJSONAsync(stream);
                Assert.NotNull(dic);
                List<object> listResult = (List<object>)dic["items"];
                Dictionary<string, object> item1 = (Dictionary<string, object>)listResult[0];
                Assert.Equal(18L, item1["total_size"]);
                Assert.Empty((string)dic["next_marker"]);
                Assert.Equal(2, ((List<object>)dic["arrayObj"]).Count);
            }

            jsonStr = "[{\"itemName\":\"item\",\"itemInt\":1},{\"itemName\":\"item2\",\"itemInt\":2}]";
            array = Encoding.UTF8.GetBytes(jsonStr);
            using (MemoryStream stream = new MemoryStream(array))
            {
                List<object> listResult = (List<object>)await StreamUtils.ReadAsJSONAsync(stream);
                Assert.NotNull(listResult);
                Dictionary<string, object> item1 = (Dictionary<string, object>)listResult[0];
                Assert.Equal("item", item1["itemName"]);
                Assert.Equal(1L, item1["itemInt"]);
            }
        }

        [Fact]
        public void Test_ReadAsBytes()
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("test")))
            {
                Assert.NotNull(StreamUtils.ReadAsBytes(stream));
            }
        }

        [Fact]
        public async void Test_ReadAsBytesAsync()
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("test")))
            {
                Assert.NotNull(await StreamUtils.ReadAsBytesAsync(stream));
            }
        }

        // [Fact]
        // public async Task Test_ReadAsSSEAsync()
        // {
        //     using (var client = new HttpClient())
        //     {
        //         var response = await client.GetStreamAsync("http://localhost:8384/sse");
        //
        //         var events = new List<SSEEvent>();
        //
        //         await foreach (var sseEvent in StreamUtil.ReadAsSSEAsync(response))
        //         {
        //             events.Add(sseEvent);
        //         }
        //
        //         for (int i = 0; i < 5; i++)
        //         {
        //             Assert.Equal(JsonConvert.SerializeObject(new { count = i }), events[i].Data);
        //             Assert.Equal("sse-test", events[i].Id);
        //             Assert.Equal("flow", events[i].Event);
        //             Assert.Equal(3, events[i].Retry);
        //         }
        //     }
        // }
        //
        // [Fact]
        // public async Task Test_ReadAsSSEAsync_WithNoSpaces()
        // {
        //     using (var client = new HttpClient())
        //     {
        //         var response = await client.GetStreamAsync("http://localhost:8384/sse_with_no_spaces");
        //
        //         var events = new List<SSEEvent>();
        //
        //         await foreach (var sseEvent in StreamUtil.ReadAsSSEAsync(response))
        //         {
        //             events.Add(sseEvent);
        //         }
        //
        //         Assert.Equal(5, events.Count);
        //
        //         for (int i = 0; i < 5; i++)
        //         {
        //             Assert.Equal(JsonConvert.SerializeObject(new { count = i }), events[i].Data);
        //             Assert.Equal("sse-test", events[i].Id);
        //             Assert.Equal("flow", events[i].Event);
        //             Assert.Equal(3, events[i].Retry);
        //         }
        //     }
        // }
        //
        // [Fact]
        // public async Task Test_ReadAsSSEAsync_WithInvalidRetry()
        // {
        //     using (var client = new HttpClient())
        //     {
        //         var response = await client.GetStreamAsync("http://localhost:8384/sse_invalid_retry");
        //
        //         var events = new List<SSEEvent>();
        //
        //         await foreach (var sseEvent in StreamUtil.ReadAsSSEAsync(response))
        //         {
        //             events.Add(sseEvent);
        //         }
        //
        //         Assert.Equal(5, events.Count);
        //
        //         for (int i = 0; i < 5; i++)
        //         {
        //             Assert.Equal(JsonConvert.SerializeObject(new { count = i }), events[i].Data);
        //             Assert.Equal("sse-test", events[i].Id);
        //             Assert.Equal("flow", events[i].Event);
        //             Assert.Null(events[i].Retry);
        //         }
        //     }
        // }
        //
        // [Fact]
        // public async Task Test_ReadAsSSEAsync_WithDividedData()
        // {
        //     using (var client = new HttpClient())
        //     {
        //         var response = await client.GetStreamAsync("http://localhost:8384/sse_with_data_divided");
        //
        //         var events = new List<SSEEvent>();
        //
        //         await foreach (var sseEvent in StreamUtil.ReadAsSSEAsync(response))
        //         {
        //             events.Add(sseEvent);
        //         }
        //         Assert.Equal(4, events.Count);
        //         Assert.Equal(JsonConvert.SerializeObject(new { count = 0 }), events[0].Data);
        //         Assert.Equal("sse-test", events[0].Id);
        //         Assert.Equal("flow", events[0].Event);
        //         Assert.Equal(3, events[0].Retry);
        //
        //         Assert.Equal(JsonConvert.SerializeObject(new { count = 2, tag = "divided" }), events[1].Data);
        //         Assert.Equal("sse-test", events[1].Id);
        //         Assert.Equal("flow", events[1].Event);
        //         Assert.Equal(3, events[1].Retry);
        //
        //         Assert.Equal(JsonConvert.SerializeObject(new { count = 3 }), events[2].Data);
        //         Assert.Equal("sse-test", events[2].Id);
        //         Assert.Equal("flow", events[2].Event);
        //         Assert.Equal(3, events[2].Retry);
        //
        //         Assert.Equal(JsonConvert.SerializeObject(new { count = 4 }), events[3].Data);
        //         Assert.Equal("sse-test", events[3].Id);
        //         Assert.Equal("flow", events[3].Event);
        //         Assert.Equal(3, events[3].Retry);
        //     }
        // }

        [Fact]
        public void Test_ReadAsSSE()
        {
            using (var client = new HttpClient())
            {
                var response = client.GetStreamAsync("http://localhost:8384/sse").Result;
                var events = new List<SSEEvent>();

                foreach (var sseEvent in StreamUtils.ReadAsSSE(response))
                {
                    events.Add(sseEvent);
                }

                for (int i = 0; i < 5; i++)
                {
                    Assert.Equal(JsonConvert.SerializeObject(new { count = i }), events[i].Data);
                    Assert.Equal("sse-test", events[i].Id);
                    Assert.Equal("flow", events[i].Event);
                    Assert.Equal(3, events[i].Retry);
                }
            }
        }

        [Fact]
        public async Task Test_ReadAsSSE_WithNoSpaces()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStreamAsync("http://localhost:8384/sse_with_no_spaces");

                var events = new List<SSEEvent>();

                foreach (var sseEvent in StreamUtils.ReadAsSSE(response))
                {
                    events.Add(sseEvent);
                }

                Assert.Equal(5, events.Count);

                for (int i = 0; i < 5; i++)
                {
                    Assert.Equal(JsonConvert.SerializeObject(new { count = i }), events[i].Data);
                    Assert.Equal("sse-test", events[i].Id);
                    Assert.Equal("flow", events[i].Event);
                    Assert.Equal(3, events[i].Retry);
                }
            }
        }

        [Fact]
        public async Task Test_ReadAsSSE_WithInvalidRetry()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStreamAsync("http://localhost:8384/sse_invalid_retry");

                var events = new List<SSEEvent>();

                foreach (var sseEvent in StreamUtils.ReadAsSSE(response))
                {
                    events.Add(sseEvent);
                }

                Assert.Equal(5, events.Count);

                for (int i = 0; i < 5; i++)
                {
                    Assert.Equal(JsonConvert.SerializeObject(new { count = i }), events[i].Data);
                    Assert.Equal("sse-test", events[i].Id);
                    Assert.Equal("flow", events[i].Event);
                    Assert.Null(events[i].Retry);
                }
            }
        }

        [Fact]
        public async Task Test_ReadAsSSE_WithDividedData()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStreamAsync("http://localhost:8384/sse_with_data_divided");

                var events = new List<SSEEvent>();

                foreach (var sseEvent in StreamUtils.ReadAsSSE(response))
                {
                    events.Add(sseEvent);
                }

                Assert.Equal(4, events.Count);
                Assert.Equal(JsonConvert.SerializeObject(new { count = 0 }), events[0].Data);
                Assert.Equal("sse-test", events[0].Id);
                Assert.Equal("flow", events[0].Event);
                Assert.Equal(3, events[0].Retry);

                Assert.Equal(JsonConvert.SerializeObject(new { count = 2, tag = "divided" }), events[1].Data);
                Assert.Equal("sse-test", events[1].Id);
                Assert.Equal("flow", events[1].Event);
                Assert.Equal(3, events[1].Retry);

                Assert.Equal(JsonConvert.SerializeObject(new { count = 3 }), events[2].Data);
                Assert.Equal("sse-test", events[2].Id);
                Assert.Equal("flow", events[2].Event);
                Assert.Equal(3, events[2].Retry);

                Assert.Equal(JsonConvert.SerializeObject(new { count = 4 }), events[3].Data);
                Assert.Equal("sse-test", events[3].Id);
                Assert.Equal("flow", events[3].Event);
                Assert.Equal(3, events[3].Retry);
            }
        }
    }
}

