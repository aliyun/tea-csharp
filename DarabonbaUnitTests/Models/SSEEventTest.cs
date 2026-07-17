using System.Collections.Generic;
using Darabonba.Models;
using Xunit;

namespace DaraUnitTests.Models
{
    public class SSEEventTest
    {
        [Fact]
        public void Test_ToMap_FromMap_Copy()
        {
            var empty = new SSEEvent();
            Assert.Empty(empty.ToMap());
            Assert.Empty(empty.ToMap(true));

            var evt = new SSEEvent
            {
                Data = "hello",
                Id = "1",
                Event = "message",
                Retry = 3
            };
            var map = evt.ToMap();
            Assert.Equal("hello", map["data"]);
            Assert.Equal("1", map["id"]);
            Assert.Equal("message", map["event"]);
            Assert.Equal(3, map["retry"]);

            var fromMap = SSEEvent.FromMap(map);
            Assert.Equal("hello", fromMap.Data);
            Assert.Equal("1", fromMap.Id);
            Assert.Equal("message", fromMap.Event);
            Assert.Equal(3, fromMap.Retry);

            var copy = evt.Copy();
            Assert.Equal(evt.Data, copy.Data);
            Assert.Equal(evt.Id, copy.Id);
            Assert.Equal(evt.Event, copy.Event);
            Assert.Equal(evt.Retry, copy.Retry);

            var copyNoStream = evt.CopyWithoutStream();
            Assert.Equal(evt.Data, copyNoStream.Data);

            var partial = SSEEvent.FromMap(new Dictionary<string, object>());
            Assert.Null(partial.Data);
            Assert.Null(partial.Id);
            Assert.Null(partial.Event);
            Assert.Null(partial.Retry);
        }
    }
}
