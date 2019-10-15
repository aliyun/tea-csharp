using System;
using System.Reflection;
using System.Text;

namespace Tea
{
    public class TeaJSON
    {
        public static string Stringify(object obj)
        {
            var sb = new StringBuilder();
            Type type = obj.GetType();
            if (type.IsArray)
            {
                sb.Append("[");
                Array arr = (Array) obj;
                for (int i = 0; i < arr.Length; i++)
                {
                    sb.Append(Stringify(arr.GetValue(i)));
                    if (i < arr.Length - 1)
                    {
                        sb.Append(",");
                    }
                }
                sb.Append("]");
            }
            else
            {
                sb.Append("{");
                FieldInfo[] fields = type.GetFields();
                FieldInfo[] filtered = Array.FindAll(fields, f => f.GetValue(obj) != null);
                for (int i = 0; i < filtered.Length; i++)
                {
                    FieldInfo f = filtered[i];
                    var value = f.GetValue(obj);
                    sb.AppendFormat("\"{0}\"", f.Name).Append(":");
                    if (value is string)
                    {
                        sb.Append("\"").Append(value).Append("\"");
                    }
                    else if (value is int)
                    {
                        sb.Append(value);
                    }
                    else if (value is bool)
                    {
                        sb.Append((bool) value ? "true" : "false");
                    }
                    else
                    {
                        sb.Append(Stringify(value));
                    }
                    if (i < filtered.Length - 1)
                    {
                        sb.Append(",");
                    }
                }
                sb.Append("}");
            }

            return sb.ToString();
        }

        public static JSONItem Parse(string json)
        {
            var lexer = new JSONLexer(json);
            var parser = new JSONParser(lexer);
            return parser.parse();
        }

    }
}
