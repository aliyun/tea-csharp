using System;
using System.Collections.Generic;

namespace Tea
{
    public class Position
    {
        public int line;
        public int column;

        public Position(int line, int column)
        {
            this.line = line;
            this.column = column;
        }
    }

    public enum TokenType
    {
        NORMARL, // {}[],
        STRING, // string
        BOOLEAN, // true/false
        NUMBER, // number
        EOF, // EOF
    }

    public class Token
    {
        public string lexeme;
        public Position begin;
        public Position end;
        public TokenType type;

        public Token(string lexeme, TokenType type, Position begin, Position end)
        {
            this.lexeme = lexeme;
            this.type = type;
            this.begin = begin;
            this.end = end;
        }
    }

    public class JSONLexer
    {
        public JSONLexer(string source)
        {
            this.source = source;
            this.line = 0;
            this.column = 0;
            this.index = 0;
        }

        public Position getPosition()
        {
            return new Position(this.line, this.column);
        }

        private int index;
        private string source;
        private int line;
        private int column;

        private Exception error(string message)
        {
            string[] lines = this.source.Split('\n');
            string line = lines[this.line];
            Console.WriteLine(line);
            Console.WriteLine("^".PadLeft(this.column));
            return new Exception(message);
        }

        public void print(Token token)
        {
            string[] lines = this.source.Split('\n');
            string line = lines[token.begin.line];
            Console.WriteLine(line);
            Console.WriteLine("^".PadLeft(token.begin.column));
        }

        public Token scan()
        {
            if (this.get() == -1)
            {
                return new Token(null, TokenType.EOF, null, null);
            }

            this.skipWhitespaces();
            var begin = this.getPosition();
            var current = this.get();
            switch (current)
            {
                case '{':
                case '}':
                case '[':
                case ']':
                case ':':
                case ',':
                    this.next();
                    return new Token(((char) current).ToString(), TokenType.NORMARL, begin, this.getPosition());
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    var num = "" + (char) current;
                    this.next();
                    while (this.get() >= '0' && this.get() <= '9')
                    {
                        num += (char) this.get();
                        this.next();
                    }
                    return new Token(num, TokenType.NUMBER, begin, this.getPosition());
                case '"':
                    var str = "";
                    this.next();
                    while (this.get() != '"')
                    {
                        str += (char) this.get();
                        this.next();
                    }
                    this.next();
                    return new Token(str, TokenType.STRING, begin, this.getPosition());
                case 't':
                    // true
                    if (this.get(1) == 'r' && this.get(2) == 'u' && this.get(3) == 'e')
                    {
                        this.next(3);
                        var end = this.getPosition();
                        this.next();
                        return new Token("true", TokenType.BOOLEAN, begin, end);
                    }
                    else
                    {
                        throw this.error("unexpected token: ");
                    }
                case 'f':
                    // false
                    if (this.get(1) == 'a' && this.get(2) == 'l' && this.get(3) == 's' && this.get(4) == 'e')
                    {
                        this.next(4);
                        return new Token("false", TokenType.BOOLEAN, begin, this.getPosition());
                    }
                    else
                    {
                        throw this.error("unexpected token: ");
                    }
                default:
                    throw this.error("unexpected token: ");
            }
        }

        private void next()
        {
            this.index++;
            this.column++;
        }

        private void next(int count)
        {
            this.index = this.index + count;
        }

        public int get()
        {
            if (this.index >= this.source.Length)
            {
                return -1;
            }
            return this.source[this.index];
        }

        public char get(int offset)
        {
            return this.source[this.index + offset];
        }

        public void skipWhitespaces()
        {
            while (this.get() == ' ' || this.get() == '\n' || this.get() == '\r')
            {
                if (this.get() == '\n')
                {
                    this.line++;
                    this.column = 0;
                }
                this.index++;
            }
        }
    }

    public class JSONItem
    {
        private bool isObject;
        private bool isArray;
        private bool isBoolean;
        private bool isString;
        private object item;
        private bool isNumber;

        public JSONItem(Dictionary<string, JSONItem> dict)
        {
            this.isObject = true;
            this.item = dict;
        }

        public JSONItem(bool value)
        {
            this.isBoolean = true;
            this.item = value;
        }

        public JSONItem(string value)
        {
            this.isString = true;
            this.item = value;
        }

        public JSONItem(List<JSONItem> list)
        {
            this.isArray = true;
            this.item = list;
        }

        public JSONItem(int value)
        {
            this.isNumber = true;
            this.item = value;
        }

        public Dictionary<string, JSONItem> GetAsDictionary()
        {
            return (Dictionary<string, JSONItem>) this.item;
        }

        public List<JSONItem> GetAsArray()
        {
            return (List<JSONItem>) this.item;
        }

        public bool IsObject()
        {
            return isObject;
        }

        public bool IsArray()
        {
            return isArray;
        }

        public bool IsString()
        {
            return isString;
        }

        public bool IsBoolean()
        {
            return isBoolean;
        }

        public bool GetAsBoolean()
        {
            return (bool) this.item;
        }

        public bool IsNumber()
        {
            return isNumber;
        }

        public int GetAsNumber()
        {
            return (int) this.item;
        }

        public object GetValue()
        {
            if (IsObject())
            {
                var dict = new Dictionary<string, object>();
                var value = this.GetAsDictionary();
                foreach (var item in value)
                {
                    dict.Add(item.Key, item.Value.GetValue());
                }
                return dict;
            }

            if (IsArray())
            {
                var list = new List<object>();
                var value = this.GetAsArray();
                foreach (var item in value)
                {
                    list.Add(item.GetValue());
                }
                return list;
            }

            return this.item;
        }

        public string GetAsString()
        {
            return (string) this.item;
        }
    }

    public class JSONParser
    {
        private JSONLexer lexer;
        private Token look;

        public JSONParser(JSONLexer lexer)
        {
            this.lexer = lexer;
            this.next();
        }

        private void next()
        {
            this.look = this.lexer.scan();
        }

        private void match(string expected)
        {
            if (this.look.lexeme == expected)
            {
                this.next();
            }
            else
            {
                throw this.error(String.Format("unexpected token {0}, expected {1}", this.look.lexeme, expected), this.look);
            }
        }

        public JSONItem parse()
        {
            if (this.look.type == TokenType.NORMARL)
            {
                if (this.look.lexeme == "{")
                {
                    return parseObject();
                }
                if (this.look.lexeme == "[")
                {
                    return parseArray();
                }
                throw this.error("expected { or [", this.look);
                // throw new Exception("unexpected token");
            }
            else if (this.look.type == TokenType.BOOLEAN)
            {
                var look = this.look;
                this.next();
                return new JSONItem(look.lexeme == "true");
            }
            else if (this.look.type == TokenType.NUMBER)
            {
                var look = this.look;
                this.next();
                return new JSONItem(int.Parse(look.lexeme));
            }
            else if (this.look.type == TokenType.STRING)
            {
                var look = this.look;
                this.next();
                return new JSONItem(look.lexeme);
            }
            else
            {
                throw this.error("unexpected token: ", this.look);
            }
        }

        private Exception error(string message, Token token)
        {
            this.lexer.print(token);
            return new Exception("unexpected token: ");
        }

        private JSONItem parseObject()
        {
            this.match("{");
            var dict = new Dictionary<string, JSONItem>();
            while (this.look.lexeme != "}")
            {
                var key = this.look.lexeme;
                this.next();
                this.match(":");
                var value = this.parse();
                dict.Add(key, value);
                if (this.look.lexeme == ",")
                {
                    this.next();
                }
            }
            this.match("}");
            return new JSONItem(dict);
        }

        private JSONItem parseArray()
        {
            this.match("[");
            var list = new List<JSONItem>();
            while (this.look.lexeme != "]")
            {
                var value = this.parse();
                list.Add(value);
                if (this.look.lexeme == ",")
                {
                    this.next();
                }
            }
            this.match("]");
            return new JSONItem(list);
        }

        private JSONItem parseString()
        {
            return new JSONItem("");
        }
    }
}
