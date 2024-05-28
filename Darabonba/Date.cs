using System;
using Darabonba.Exceptions;

namespace Darabonba
{
    public class Date
    {
        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public DateTime DateTime { get; private set; }

        public Date(DateTime date)
        {
            DateTime = date;
        }

        public Date(string dateStr)
        {
            long timestamp;
            DateTimeOffset dateTimeOffset;
            if (long.TryParse(dateStr, out timestamp))
            {
                dateTimeOffset = Jan1st1970.AddSeconds(timestamp);
                DateTime = dateTimeOffset.UtcDateTime;
            }
            // if no time zone, treat as local time as DateTimeOffset.Parse.
            else if (DateTimeOffset.TryParse(dateStr, out dateTimeOffset))
            {
                DateTime = dateTimeOffset.UtcDateTime;
            }
            else
            {
                throw new DaraException
                {
                    Message = dateStr + "is not a valid time string."
                };
            }
        }

        public string Format(string layout)
        {
            layout = layout.Replace('Y', 'y')
                           .Replace('D', 'd')
                           .Replace('h', 'H');
            return DateTime.ToUniversalTime().ToString(layout);
        }

        public long Unix()
        {
            return (long)(DateTime.ToUniversalTime() - Jan1st1970).TotalSeconds;
        }

        public string UTC()
        {
            return DateTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.ffffff '+0000 UTC'");
        }

        public Date Sub(string unit, int amount)
        {
            DateTime newDate;
            switch (unit.ToLowerInvariant())
            {
                case "millisecond":
                    newDate = DateTime.AddMilliseconds(-amount);
                    break;
                case "second":
                    newDate = DateTime.AddSeconds(-amount);
                    break;
                case "minute":
                    newDate = DateTime.AddMinutes(-amount);
                    break;
                case "hour":
                    newDate = DateTime.AddHours(-amount);
                    break;
                case "day":
                    newDate = DateTime.AddDays(-amount);
                    break;
                case "month":
                    newDate = DateTime.AddMonths(-amount);
                    break;
                case "year":
                    newDate = DateTime.AddYears(-amount);
                    break;
                default:
                    throw new ArgumentException("Unsupported unit.");
            }
            return new Date(newDate);
        }

        public Date Add(string unit, int amount)
        {
            DateTime newDate;
            switch (unit.ToLowerInvariant())
            {
                case "millisecond":
                    newDate = DateTime.AddMilliseconds(amount);
                    break;
                case "second":
                    newDate = DateTime.AddSeconds(amount);
                    break;
                case "minute":
                    newDate = DateTime.AddMinutes(amount);
                    break;
                case "hour":
                    newDate = DateTime.AddHours(amount);
                    break;
                case "day":
                    newDate = DateTime.AddDays(amount);
                    break;
                case "month":
                    newDate = DateTime.AddMonths(amount);
                    break;
                case "year":
                    newDate = DateTime.AddYears(amount);
                    break;
                default:
                    throw new ArgumentException("Unsupported unit.");
            }
            return new Date(newDate);
        }

        public int Diff(string unit, Date diffDate)
        {
            TimeSpan timeSpan = DateTime - diffDate.DateTime;
            switch (unit.ToLowerInvariant())
            {
                case "millisecond":
                    return timeSpan.Milliseconds;
                case "second":
                    return timeSpan.Seconds;
                case "minute":
                    return timeSpan.Minutes;
                case "hour":
                    return timeSpan.Hours;
                case "day":
                    return timeSpan.Days;
                case "month":
                    return timeSpan.Days / 30;
                case "year":
                    return timeSpan.Days / 365;
                default:
                    throw new ArgumentException("Unsupported unit.");
            }
        }

        public int Hour()
        {
            return DateTime.Hour;
        }

        public int Minute()
        {
            return DateTime.Minute;
        }

        public int Second()
        {
            return DateTime.Second;
        }

        public int Month()
        {
            return DateTime.Month;
        }

        public int Year()
        {
            return DateTime.Year;
        }

        public int DayOfMonth()
        {
            return DateTime.Day;
        }

        public int DayOfWeek()
        {
            if (DateTime.DayOfWeek == 0)
            {
                return 7;
            }
            return (int)DateTime.DayOfWeek;
        }
    }
}