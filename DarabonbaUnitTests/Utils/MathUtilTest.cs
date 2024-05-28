using Darabonba.Utils;
using Xunit;

namespace DaraUnitTests.Utils
{
    public class MathUtilTest
    {
        [Fact]
        public void TestFloor()
        {
            float funm = 2.13f;
            Assert.Equal(2, MathUtil.Floor(funm));
            double dunm = 2.13d;
            Assert.Equal(2, MathUtil.Floor(dunm));
        }

        [Fact]
        public void TestRound()
        {
            float funm = 2.49f;
            Assert.Equal(2, MathUtil.Round(funm));
            double dunm = 2.51d;
            Assert.Equal(3, MathUtil.Round(dunm));
        }
        
        [Fact]
        public void TestParseInt()
        {
            float funm = 2.13f;
            Assert.Equal(2, MathUtil.ParseInt(funm));
            double dunm = 2.13d;
            Assert.Equal(2, MathUtil.ParseInt(dunm));
        }

        [Fact]
        public void TestParseLong()
        {
            float funm = 2.13f;
            Assert.Equal(2L, MathUtil.ParseLong(funm));
            double dunm = 2.13d;
            Assert.Equal(2L, MathUtil.ParseLong(dunm));
        }

        [Fact]
        public void TestParseFloat()
        {
            int iunm = 2;
            Assert.Equal(2f, MathUtil.ParseFloat(iunm));
            float funm = 2.13f;
            Assert.Equal(2.13f, MathUtil.ParseFloat(funm));
            double dunm = 2.13d;
            Assert.Equal(2.13f, MathUtil.ParseFloat(dunm));
        }

        [Fact]
        public void TestMin()
        {
            int inum = 2;
            float fnum = 2.01f;
            double dnum = 2.001d;
            Assert.Equal(2, MathUtil.Min(inum, fnum));
            Assert.Equal(2.001d, MathUtil.Min(dnum, fnum));
        }

        [Fact]
        public void TestMax()
        {
            int inum = 2;
            float fnum = 2.01f;
            double dnum = 2.02d;
            Assert.Equal(2.01f, MathUtil.Max(inum, fnum));
            Assert.Equal(2.02d, MathUtil.Max(dnum, fnum));
        }
    }

}