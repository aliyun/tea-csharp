using Darabonba.Utils;
using Xunit;
using System.Collections.Generic;
using Darabonba.Exceptions;

namespace DaraUnitTests.Utils
{
    public class ListUtilsTest
    {

        [Fact]
        public void TestShift()
        {
            List<string> array = new List<string> { "a", "b", "c" };
            string first = ListUtils.Shift(array);
            Assert.Equal(2, array.Count);
            Assert.Equal("a", first);
            Assert.Equal("b", array[0]);

            var ex = Assert.Throws<DaraException>(() => ListUtils.Shift(new List<string>()));
            Assert.Equal("array is empty", ex.Message);
            Assert.Throws<DaraException>(() => ListUtils.Shift<string>(null));
        }

        [Fact]
        public void TestUnshift()
        {
            List<string> array = new List<string> { "a", "b", "c" };
            int count = ListUtils.Unshift(array, "x");
            Assert.Equal(4, count);
            Assert.Equal(4, array.Count);
            Assert.Equal("x", array[0]);
        }

        [Fact]
        public void TestPush()
        {
            List<string> array = new List<string> { "a", "b", "c" };
            int count = ListUtils.Push(array, "x");
            Assert.Equal(4, count);
            Assert.Equal(4, array.Count);
            Assert.Equal("x", array[3]);
        }

        [Fact]
        public void TestPop()
        {
            List<string> array = new List<string> { "a", "b", "c" };
            string last = ListUtils.Pop(array);
            Assert.Equal(2, array.Count);
            Assert.Equal("c", last);
            Assert.Equal("b", array[1]);

            var ex = Assert.Throws<DaraException>(() => ListUtils.Pop(new List<string>()));
            Assert.Equal("array is empty", ex.Message);
            Assert.Throws<DaraException>(() => ListUtils.Pop<string>(null));
        }

        [Fact]
        public void TestConcat()
        {
            List<string> array1 = new List<string> { "a", "b", "c" };
            List<string> array2 = new List<string> { "d", "e", "f" };
            ListUtils.Concat(array1, array2);
            Assert.Equal(6, array1.Count);
            Assert.Equal(new List<string> { "a", "b", "c", "d", "e", "f" }, array1);
        }

        [Fact]
        public void TestSort()
        {
            List<int> asc = new List<int> { 3, 1, 2 };
            Assert.Equal(new List<int> { 1, 2, 3 }, ListUtils.Sort(asc, "asc"));

            List<int> desc = new List<int> { 3, 1, 2 };
            Assert.Equal(new List<int> { 3, 2, 1 }, ListUtils.Sort(desc, "desc"));

            List<int> other = new List<int> { 2, 1 };
            Assert.Equal(new List<int> { 2, 1 }, ListUtils.Sort(other, "none"));
        }
    }
}
