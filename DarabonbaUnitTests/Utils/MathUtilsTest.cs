using Darabonba.Utils;
using Darabonba.Exceptions;
using Xunit;

namespace DaraUnitTests.Utils
{
    public class MathUtilsTest
    {
        [Fact]
        public void TestFloor()
        {
            float funm = 2.13f;
            Assert.Equal(2, MathUtils.Floor(funm));
            double dunm = 2.13d;
            Assert.Equal(2, MathUtils.Floor(dunm));
        }

        [Fact]
        public void TestRound()
        {
            float funm = 2.49f;
            Assert.Equal(2, MathUtils.Round(funm));
            double dunm = 2.51d;
            Assert.Equal(3, MathUtils.Round(dunm));
        }
        
        [Fact]
        public void TestParseInt()
        {
            float funm = 2.13f;
            Assert.Equal(2, MathUtils.ParseInt(funm));
            double dunm = 2.13d;
            Assert.Equal(2, MathUtils.ParseInt(dunm));
            var ex = Assert.Throws<DaraException>(() => MathUtils.ParseInt<string>(null));
            Assert.Equal("Data is null.", ex.Message);
        }

        [Fact]
        public void TestParseLong()
        {
            float funm = 2.13f;
            Assert.Equal(2L, MathUtils.ParseLong(funm));
            double dunm = 2.13d;
            Assert.Equal(2L, MathUtils.ParseLong(dunm));
            var ex = Assert.Throws<DaraException>(() => MathUtils.ParseLong<string>(null));
            Assert.Equal("Data is null.", ex.Message);
        }

        [Fact]
        public void TestParseFloat()
        {
            int iunm = 2;
            Assert.Equal(2f, MathUtils.ParseFloat(iunm));
            float funm = 2.13f;
            Assert.Equal(2.13f, MathUtils.ParseFloat(funm));
            double dunm = 2.13d;
            Assert.Equal(2.13f, MathUtils.ParseFloat(dunm));
            var ex = Assert.Throws<DaraException>(() => MathUtils.ParseFloat<string>(null));
            Assert.Equal("Data is null.", ex.Message);
        }

        [Fact]
        public void TestMin()
        {
            int inum = 2;
            float fnum = 2.01f;
            double dnum = 2.001d;
            Assert.Equal(2, MathUtils.Min(inum, fnum));
            Assert.Equal(2.001d, MathUtils.Min(dnum, fnum));
        }

        [Fact]
        public void TestMax()
        {
            int inum = 2;
            float fnum = 2.01f;
            double dnum = 2.02d;
            Assert.Equal(2.01f, MathUtils.Max(inum, fnum));
            Assert.Equal(2.02d, MathUtils.Max(dnum, fnum));
        }
    }

}
