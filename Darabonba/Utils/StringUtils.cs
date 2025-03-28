using System;
using System.Text.RegularExpressions;
using System.Globalization;
using Darabonba.Exceptions;

namespace Darabonba.Utils
{
    public class StringUtils
    {
        public static string SubString(string str, int? start, int? end)
        {
            return str.Substring(start.Value, end.Value - start.Value);
        }

        public static byte[] ToBytes(string data, string type)
        {
            return BytesUtils.From(data, type);
        }

        private static string Replace(string data, string replacement, string pattern)
        {
            string regexPattern = @"\/(.*)\/([gi]*)$";
            Match match = Regex.Match(pattern, regexPattern);
            if (match.Success)
            {
                string patternStr = match.Groups[1].Value;
                string flags = match.Groups[2].Value;
                if (flags == "g")
                {
                    return Regex.Replace(data, patternStr, replacement, RegexOptions.None);
                }
                else if (flags == "gi")
                {
                    return Regex.Replace(data, patternStr, replacement, RegexOptions.IgnoreCase);
                }
                else if (flags == "i")
                {
                    Match matchFirst = Regex.Match(data, patternStr, RegexOptions.IgnoreCase);
                    if (matchFirst.Success)
                    {
                        return data.Remove(matchFirst.Index, matchFirst.Length).Insert(matchFirst.Index, replacement);
                    }
                    return data;
                }
                else if (flags == "")
                {
                    Match matchFirst = Regex.Match(data, patternStr);
                    if (matchFirst.Success)
                    {
                        return data.Remove(matchFirst.Index, matchFirst.Length).Insert(matchFirst.Index, replacement);
                    }
                    return data;
                }
            }
            try
            {
                return Regex.Replace(data, pattern, replacement, RegexOptions.None);
            }
            catch (Exception e)
            {
                throw new DaraException
                {
                    Message = "Replace error occured: " + e.Message
                };
            }
        }

        public static int ParseInt(string data)
        {
            return (int)double.Parse(data, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo);
        }

        public static long ParseLong(string data)
        {
            return (long)double.Parse(data, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo);
        }

        public static float ParseFloat(string data)
        {
            return (float)double.Parse(data, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo);
        }
    }
}