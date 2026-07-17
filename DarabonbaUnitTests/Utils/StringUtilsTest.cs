using System;
using System.Reflection;
using Darabonba.Exceptions;
using Darabonba.Utils;
using Xunit;

namespace DaraUnitTests.Utils
{
    public class StringUtilsTest
    {
        [Fact]
        public void TestSubString()
        {
            int? start = null;
            int? end = null;
            Assert.Throws<InvalidOperationException>(() => StringUtils.SubString("test", start, end));
            Assert.Equal("te", StringUtils.SubString("test", 0, 2));
        }

        [Fact]
        public void TestToBytes()
        {
            string data = "test";
            Assert.Equal(new byte[] { 116, 101, 115, 116 }, StringUtils.ToBytes(data, "utf8"));
            Assert.Equal(new byte[] { 116, 101, 115, 116 }, BytesUtils.From(data, "utf8"));
            Assert.Equal(new byte[] { 116, 101, 115, 116 }, BytesUtils.From(data, "ASCII"));
            Assert.Equal(new byte[] { 0, 116, 0, 101, 0, 115, 0, 116 }, BytesUtils.From(data, "bigendianunicode"));
            Assert.Equal(new byte[] { 116, 0, 101, 0, 115, 0, 116, 0 }, BytesUtils.From(data, "unicode"));
            Assert.Equal(new byte[] { 116, 0, 0, 0, 101, 0, 0, 0, 115, 0, 0, 0, 116, 0, 0, 0 }, BytesUtils.From(data, "utf32"));
        }

        [Fact]
        public void TestReplace()
        {
            MethodInfo replace = typeof(StringUtils).GetMethod("Replace", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(replace);

            string data = "Beijing city, hangzhou city, beijing city, another city, beijing city";
            string replacement = "chengdu";

            Assert.Equal(
                "Beijing city, hangzhou city, chengdu city, another city, beijing city",
                (string)replace.Invoke(null, new object[] { data, replacement, "/beijing/" }));
            Assert.Equal(
                "Beijing city, hangzhou city, chengdu city, another city, chengdu city",
                (string)replace.Invoke(null, new object[] { data, replacement, "/beijing/g" }));
            Assert.Equal(
                "chengdu city, hangzhou city, chengdu city, another city, chengdu city",
                (string)replace.Invoke(null, new object[] { data, replacement, "/beijing/gi" }));
            Assert.Equal(
                "chengdu city, hangzhou city, beijing city, another city, beijing city",
                (string)replace.Invoke(null, new object[] { data, replacement, "/beijing/i" }));
            Assert.Equal(
                "no match",
                (string)replace.Invoke(null, new object[] { "no match", replacement, "/beijing/i" }));
            Assert.Equal(
                "no match",
                (string)replace.Invoke(null, new object[] { "no match", replacement, "/beijing/" }));
            Assert.Equal(
                "chengdu city, chengdu city, chengdu city, chengdu city, chengdu city",
                (string)replace.Invoke(null, new object[] { data, replacement, @"\S+(?=\s*city)" }));

            var ex = Assert.Throws<TargetInvocationException>(() =>
                replace.Invoke(null, new object[] { data, replacement, "(" }));
            Assert.IsType<DaraException>(ex.InnerException);
            Assert.Contains("Replace error occured:", ex.InnerException.Message);
        }

        [Fact]
        public void TestParse()
        {
            string numberStr = "1.3433";
            int intRes = StringUtils.ParseInt(numberStr);
            Assert.Equal(1, intRes);
            numberStr = "2";
            intRes = StringUtils.ParseInt(numberStr);
            Assert.Equal(2, intRes);
            numberStr = "1,682.80";
            intRes = StringUtils.ParseInt(numberStr);
            Assert.Equal(1682, intRes);
            numberStr = "1.682.80";
            Assert.Throws<FormatException>(() => StringUtils.ParseInt(numberStr));
            numberStr = "3.1415926";
            float floatRes = StringUtils.ParseFloat(numberStr);
            Assert.Equal(3.1415926f, floatRes);
            numberStr = "3.1415926";
            long longRes = StringUtils.ParseLong(numberStr);
            Assert.Equal(3, longRes);
        }
        
    }
}
