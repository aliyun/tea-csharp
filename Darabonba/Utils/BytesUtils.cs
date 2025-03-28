using System.Text;

namespace Darabonba.Utils
{
    public class BytesUtils
    {
        public static byte[] From(string data, string type)
        {
            string lowerEncoding = type.ToLower();
            switch (lowerEncoding.ToLowerInvariant())
            {
                case "ascii":
                    return Encoding.ASCII.GetBytes(data);
                case "bigendianunicode":
                    return Encoding.BigEndianUnicode.GetBytes(data);
                case "unicode":
                    return Encoding.Unicode.GetBytes(data);
                case "utf32":
                case "utf-32":
                    return Encoding.UTF32.GetBytes(data);
                case "utf8":
                case "utf-8":
                    return Encoding.UTF8.GetBytes(data);
                default:
                    return Encoding.UTF8.GetBytes(data);
            }
        }

        public static string ToHex(byte[] raw)
        {
            if (raw == null)
            {
                return string.Empty;
            }

            StringBuilder result = new StringBuilder(raw.Length * 2);
            for (int i = 0; i < raw.Length; i++)
                result.Append(raw[i].ToString("x2"));
            return result.ToString();
        }
    }
}