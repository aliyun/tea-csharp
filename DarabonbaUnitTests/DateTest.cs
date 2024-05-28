using Darabonba;
using Xunit;
using System;

namespace DaraUnitTests
{
    public class DateTest
    {
        Date dateLocal = new Date("2023-12-31 00:00:00.916000");
        Date dateUTC = new Date("2023-12-31 00:00:00.916000 +0000");

        [Fact]
        public void Test_TimestampStr()
        {
            Date date = new Date("1723081751");
            Assert.Equal("2024-08-08 01:49:11.000000 +0000 UTC", date.DateTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff '+0000 UTC'"));
        }

        [Fact]
        public void Test_Init_NoTimeZone()
        {
            Assert.Equal("2023-12-31 00:00:00.916000", dateLocal.DateTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
        }

        [Fact]
        public void Test_Init_WithTimeZone()
        {
            DateTime expectedDate = DateTimeOffset.Parse("2023-12-31 00:00:00.916000 +0000").UtcDateTime;
            Assert.Equal(expectedDate, dateUTC.DateTime);
        }

        [Fact]
        public void Test_Format()
        {
            Assert.Equal("2023-12-31 00:00:00.916", dateUTC.Format("yyyy-MM-dd HH:mm:ss.fff"));
            Assert.Equal("2023-12-31 00:00:00", dateUTC.Format("yyyy-MM-dd HH:mm:ss"));
            Assert.Equal("2023-12-31T00:00:00Z", dateUTC.Format("yyyy-MM-ddTHH:mm:ssZ"));
        }

        [Fact]
        public void Test_Unix()
        {
            Assert.Equal(1703980800, dateUTC.Unix());
            Assert.Equal(1703980800, dateLocal.Unix());
        }

        [Fact]
        public void Test_UTC()
        {
            Assert.Equal("2023-12-31 00:00:00.916000 +0000 UTC", dateUTC.UTC());
            // Local time
            Assert.Equal("2023-12-31 00:00:00.916000 +0000 UTC", dateLocal.UTC());
        }

        [Fact]
        public void Test_Methods()
        {
            Date yesterday = dateUTC.Sub("day", 1);
            Assert.Equal("2023-12-30 00:00:00.916", yesterday.Format("yyyy-MM-dd HH:mm:ss.fff"));
            Assert.Equal(1, dateUTC.Diff("day", yesterday));
            Date tomorrow = dateUTC.Add("day", 1);
            Assert.Equal(-1, dateUTC.Diff("day", tomorrow));
            Assert.Equal(2023, dateUTC.Year());
            Assert.Equal(2024, tomorrow.Year());
            Assert.Equal(1, tomorrow.Month());
            Assert.Equal(12, dateUTC.Month());
            Assert.Equal(0, dateUTC.Hour());
            Assert.Equal(0, dateUTC.Minute());
            Assert.Equal(0, dateUTC.Second());
            Assert.Equal(31, dateUTC.DayOfMonth());
            Assert.Equal(7, dateUTC.DayOfWeek());
        }
    }
}