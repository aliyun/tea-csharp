using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Darabonba.Models;
using Darabonba.Utils;
using Xunit;

namespace DaraUnitTests.Utils
{
    public class StreamUtilsCoverageTest
    {
        [Fact]
        public void StreamForRejectsUnreadableStream()
        {
            var stream = new MemoryStream();
            stream.Dispose();

            var exception = Assert.Throws<Exception>(() => StreamUtils.StreamFor(stream));

            Assert.Equal("stream is not readable", exception.Message);
        }

        [Fact]
        public void ReadAsSSEHandlesSplitDelimiterAndIgnoresIncompleteTail()
        {
            var data = new string('x', 4090);
            var payload = "data:" + data + "\n\n" +
                          ": heartbeat\nunknown: ignored\nid: next\nevent: update\nretry: 0\ndata: second\n\n" +
                          "data: incomplete";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)))
            {
                var events = new List<SSEEvent>(StreamUtils.ReadAsSSE(stream));

                Assert.Equal(2, events.Count);
                Assert.Equal(data, events[0].Data);
                Assert.Null(events[0].Id);
                Assert.Null(events[0].Event);
                Assert.Null(events[0].Retry);
                Assert.Equal("second", events[1].Data);
                Assert.Equal("next", events[1].Id);
                Assert.Equal("update", events[1].Event);
                Assert.Equal(0, events[1].Retry);
            }
        }

        [Fact]
        public void ReadAsSSEReturnsNoEventsForEmptyStream()
        {
            var stream = new MemoryStream();

            Assert.Empty(StreamUtils.ReadAsSSE(stream));
            Assert.False(stream.CanRead);
        }

#if NETCOREAPP3_1_OR_GREATER || NETSTANDARD2_1 || NET5_0_OR_GREATER
        [Fact]
        public async Task ReadAsSSEAsyncHandlesSplitDelimiterAndInvalidRetry()
        {
            var data = new string('x', 4090);
            var payload = "data:" + data + "\n\n" +
                          ": heartbeat\nunknown: ignored\nid: next\nevent: update\nretry: invalid\ndata: second\n\n" +
                          "data: incomplete";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            var events = new List<SSEEvent>();

            await foreach (var item in StreamUtils.ReadAsSSEAsync(stream))
            {
                events.Add(item);
            }

            Assert.Equal(2, events.Count);
            Assert.Equal(data, events[0].Data);
            Assert.Equal("second", events[1].Data);
            Assert.Equal("next", events[1].Id);
            Assert.Equal("update", events[1].Event);
            Assert.Null(events[1].Retry);
            Assert.False(stream.CanRead);
        }

        [Fact]
        public async Task ReadAsSSEAsyncReturnsNoEventsForEmptyStream()
        {
            var stream = new MemoryStream();
            var events = new List<SSEEvent>();

            await foreach (var item in StreamUtils.ReadAsSSEAsync(stream))
            {
                events.Add(item);
            }

            Assert.Empty(events);
            Assert.False(stream.CanRead);
        }

        [Fact]
        public async Task ReadAsSSEAsyncHonorsCanceledTokenAndDisposesStream()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("data: ignored\n\n"));
            using (var cancellation = new CancellationTokenSource())
            {
                cancellation.Cancel();
                var events = new List<SSEEvent>();

                var exception = await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                {
                    await foreach (var item in StreamUtils.ReadAsSSEAsync(stream, cancellation.Token))
                    {
                        events.Add(item);
                    }
                });

                Assert.Equal(cancellation.Token, exception.CancellationToken);
                Assert.Empty(events);
                Assert.False(stream.CanRead);
            }
        }

        [Fact]
        public async Task ReadAsSSEAsyncCanBeCanceledBetweenChunks()
        {
            var data = new string('x', 4089);
            var payload = "data:" + data + "\n\ndata: ignored\n\n";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            using (var cancellation = new CancellationTokenSource())
            {
                var events = new List<SSEEvent>();

                var exception = await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                {
                    await foreach (var item in StreamUtils.ReadAsSSEAsync(stream, cancellation.Token))
                    {
                        events.Add(item);
                        cancellation.Cancel();
                    }
                });

                Assert.Equal(cancellation.Token, exception.CancellationToken);
                Assert.Single(events);
                Assert.Equal(data, events[0].Data);
                Assert.False(stream.CanRead);
            }
        }
#endif
    }
}
