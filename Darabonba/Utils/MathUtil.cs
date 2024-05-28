using System;
using Darabonba.Exceptions;

namespace Darabonba.Utils
{
    public class MathUtil
    {
        public static int Floor(double? num)
        {
            return (int)Math.Floor(num.Value);
        }

        public static int Round(double? num)
        {
            return (int)Math.Round(num.Value, MidpointRounding.AwayFromZero);
        }

        public static int ParseInt<T>(T data)
        {
            if (data == null)
            {
                throw new DaraException
                {
                    Message = "Data is null."
                };
            }
            return (int)double.Parse(data.ToString());
        }

        public static long ParseLong<T>(T data)
        {
            if (data == null)
            {
                throw new DaraException
                {
                    Message = "Data is null."
                };
            }
            return (long)double.Parse(data.ToString());
        }

        public static float ParseFloat<T>(T data)
        {
            if (data == null)
            {
                throw new DaraException
                {
                    Message = "Data is null."
                };
            }
            return (float)double.Parse(data.ToString());
        }

        public static T Min<T>(T num1, T num2) where T : IComparable<T>
        {
            return num1.CompareTo(num2) <= 0 ? num1 : num2;
        }

        public static T Max<T>(T num1, T num2) where T : IComparable<T>
        {
            return num1.CompareTo(num2) <= 0 ? num2 : num1;
        }
    }
}