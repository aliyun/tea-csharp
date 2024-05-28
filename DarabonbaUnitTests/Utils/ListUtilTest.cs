using Darabonba.Utils;
using Xunit;
using System.Collections.Generic;

namespace DaraUnitTests.Utils
{
    public class ListUtilTest
    {

        [Fact]
        public void TestShift()
        {
            List<string> array = new List<string> { "a", "b", "c" };
            string first = ListUtil.Shift(array);
            Assert.Equal(2, array.Count);
            Assert.Equal("a", first);
            Assert.Equal("b", array[0]);
        }

        [Fact]
        public void TestUnshift()
        {
            List<string> array = new List<string> { "a", "b", "c" };
            ListUtil.Unshift(array, "x");
            Assert.Equal(4, array.Count);
            Assert.Equal("x", array[0]);
        }

        [Fact]
        public void TestPush()
        {
            List<string> array = new List<string> { "a", "b", "c" };
            ListUtil.Push(array, "x");
            Assert.Equal(4, array.Count);
            Assert.Equal("x", array[3]);
        }

        [Fact]
        public void TestPop()
        {
            List<string> array = new List<string> { "a", "b", "c" };
            string last = ListUtil.Pop(array);
            Assert.Equal(2, array.Count);
            Assert.Equal("c", last);
            Assert.Equal("b", array[1]);
        }

        [Fact]
        public void TestConcat()
        {
            List<string> array1 = new List<string> { "a", "b", "c" };
            List<string> array2 = new List<string> { "d", "e", "f" };
            ListUtil.Concat(array1, array2);
            Assert.Equal(6, array1.Count);
            Assert.Equal(new List<string> { "a", "b", "c", "d", "e", "f" }, array1);
        }
    }
}