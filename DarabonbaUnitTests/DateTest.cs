using Darabonba;
using Darabonba.Exceptions;
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
            // No timezone => DateTimeOffset.Parse treats as local, Date stores UTC.
            DateTime expected = DateTimeOffset.Parse("2023-12-31 00:00:00.916000").UtcDateTime;
            Assert.Equal(expected, dateLocal.DateTime);
        }

        [Fact]
        public void Test_Init_WithTimeZone()
        {
            DateTime expectedDate = DateTimeOffset.Parse("2023-12-31 00:00:00.916000 +0000").UtcDateTime;
            Assert.Equal(expectedDate, dateUTC.DateTime);
        }

        [Fact]
        public void Test_Init_Invalid()
        {
            var ex = Assert.Throws<DaraException>(() => new Date("not-a-date"));
            Assert.Contains("is not a valid time string.", ex.Message);
        }

        [Fact]
        public void Test_Init_DateTime()
        {
            var dt = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            var date = new Date(dt);
            Assert.Equal(dt, date.DateTime);
        }

        [Fact]
        public void Test_Format()
        {
            Assert.Equal("2023-12-31 00:00:00.916", dateUTC.Format("yyyy-MM-dd HH:mm:ss.fff"));
            Assert.Equal("2023-12-31 00:00:00", dateUTC.Format("yyyy-MM-dd HH:mm:ss"));
            Assert.Equal("2023-12-31T00:00:00Z", dateUTC.Format("yyyy-MM-ddTHH:mm:ssZ"));
            Assert.Equal("2023-12-31", dateUTC.Format("YYYY-MM-DD"));
        }

        [Fact]
        public void Test_Unix()
        {
            Assert.Equal(1703980800, dateUTC.Unix());
            long expectedLocalUnix = (long)(DateTimeOffset.Parse("2023-12-31 00:00:00.916000").UtcDateTime
                - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            Assert.Equal(expectedLocalUnix, dateLocal.Unix());
        }

        [Fact]
        public void Test_UTC()
        {
            Assert.Equal("2023-12-31 00:00:00.916000 +0000 UTC", dateUTC.UTC());
            string expectedLocalUtc = DateTimeOffset.Parse("2023-12-31 00:00:00.916000").UtcDateTime
                .ToString("yyyy-MM-dd HH:mm:ss.ffffff '+0000 UTC'");
            Assert.Equal(expectedLocalUtc, dateLocal.UTC());
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

        [Fact]
        public void Test_Add_Sub_Diff_AllUnits()
        {
            Assert.Equal("2023-12-31 00:00:00.916", dateUTC.Add("millisecond", 0).Format("yyyy-MM-dd HH:mm:ss.fff"));
            Assert.Equal("2023-12-31 00:00:01.916", dateUTC.Add("second", 1).Format("yyyy-MM-dd HH:mm:ss.fff"));
            Assert.Equal("2023-12-31 00:01:00.916", dateUTC.Add("minute", 1).Format("yyyy-MM-dd HH:mm:ss.fff"));
            Assert.Equal("2023-12-31 01:00:00.916", dateUTC.Add("hour", 1).Format("yyyy-MM-dd HH:mm:ss.fff"));
            Assert.Equal("2024-01-31 00:00:00.916", dateUTC.Add("month", 1).Format("yyyy-MM-dd HH:mm:ss.fff"));
            Assert.Equal("2024-12-31 00:00:00.916", dateUTC.Add("year", 1).Format("yyyy-MM-dd HH:mm:ss.fff"));

            Assert.Equal("2023-12-31 00:00:00.916", dateUTC.Sub("millisecond", 0).Format("yyyy-MM-dd HH:mm:ss.fff"));
            Assert.Equal("2023-12-30 23:59:59.916", dateUTC.Sub("second", 1).Format("yyyy-MM-dd HH:mm:ss.fff"));
            Assert.Equal("2023-12-30 23:59:00.916", dateUTC.Sub("minute", 1).Format("yyyy-MM-dd HH:mm:ss.fff"));
            Assert.Equal("2023-12-30 23:00:00.916", dateUTC.Sub("hour", 1).Format("yyyy-MM-dd HH:mm:ss.fff"));
            Assert.Equal("2023-11-30 00:00:00.916", dateUTC.Sub("month", 1).Format("yyyy-MM-dd HH:mm:ss.fff"));
            Assert.Equal("2022-12-31 00:00:00.916", dateUTC.Sub("year", 1).Format("yyyy-MM-dd HH:mm:ss.fff"));

            Date other = dateUTC.Add("day", 2);
            Assert.Equal(0, dateUTC.Diff("millisecond", dateUTC));
            Assert.Equal(0, dateUTC.Diff("second", dateUTC));
            Assert.Equal(0, dateUTC.Diff("minute", dateUTC));
            Assert.Equal(0, dateUTC.Diff("hour", dateUTC));
            Assert.Equal(-2, dateUTC.Diff("day", other));
            Assert.Equal(0, dateUTC.Diff("month", dateUTC));
            Assert.Equal(0, dateUTC.Diff("year", dateUTC));

            Assert.Throws<ArgumentException>(() => dateUTC.Add("week", 1));
            Assert.Throws<ArgumentException>(() => dateUTC.Sub("week", 1));
            Assert.Throws<ArgumentException>(() => dateUTC.Diff("week", other));
        }

        [Fact]
        public void Test_DayOfWeek_NonSunday()
        {
            // 2024-01-01 is Monday
            var monday = new Date("2024-01-01 00:00:00 +0000");
            Assert.Equal(1, monday.DayOfWeek());
        }
    }
}
