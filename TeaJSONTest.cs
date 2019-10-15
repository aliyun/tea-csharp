using System.Collections.Generic;

using Xunit;

using Tea;

namespace TeaTest
{
    public class TestObject {
        public string t_string;
        public int t_int;
        public bool t_bool;
    }

    public class TeaJSONTests
    {
        [Fact]
        public void TestStringifyForEmptyObject()
        {
            TestObject obj = new TestObject();
            var output = TeaJSON.Stringify(obj);
            Assert.Equal("{\"t_int\":0,\"t_bool\":false}", output);
        }

        [Fact]
        public void TestStringifyFullSet()
        {
            TestObject obj = new TestObject();
            obj.t_string = "JacksonTian";
            obj.t_int = 1024;
            obj.t_bool = true;
            var output = TeaJSON.Stringify(obj);
            Assert.Equal("{\"t_string\":\"JacksonTian\",\"t_int\":1024,\"t_bool\":true}", output);
        }

        [Fact]
        public void TestStringifyForArray()
        {
            TestObject[] objects = new TestObject[1];
            objects[0] = new TestObject();
            var output = TeaJSON.Stringify(objects);
            Assert.Equal("[{\"t_int\":0,\"t_bool\":false}]", output);
        }

        [Fact]
        public void TestParseEmptyObject()
        {
            JSONItem item = TeaJSON.Parse("{}");
            Assert.True(item.IsObject());
            Dictionary<string, JSONItem> dict = item.GetAsDictionary();
            Assert.Empty(dict);
        }

        [Fact]
        public void TestParseObject()
        {
            var json = "{\"t_int\":10,\"t_bool\":true}";
            JSONItem result = TeaJSON.Parse(json);
            Assert.True(result.IsObject());
            Dictionary<string, JSONItem> dict = result.GetAsDictionary();
            Assert.True(dict.ContainsKey("t_int"));
            JSONItem tint = dict["t_int"];
            Assert.True(tint.IsNumber());
            int v = tint.GetAsNumber();
            Assert.Equal(10, v);
            Assert.True(dict.ContainsKey("t_bool"));
            JSONItem tbool = dict["t_bool"];
            Assert.True(tbool.IsBoolean());
            bool b = tbool.GetAsBoolean();
            Assert.True(b);
        }

        [Fact]
        public void TestParseHello()
        {
            var result = TeaJSON.Parse("{\"data\":{\"message\":\"Hello world\"}}");
            Assert.True(result.IsObject());
            var dict = result.GetAsDictionary();
            Assert.Single(dict);
            Assert.True(dict.ContainsKey("data"));
            var data = dict["data"];
            Assert.True(data.IsObject());
            var dataDict = data.GetAsDictionary();
            Assert.Single(dataDict);
            Assert.True(dataDict.ContainsKey("message"));
            var message = dataDict["message"];
            Assert.True(message.IsString());
            Assert.Equal("Hello world", message.GetAsString());
        }

        [Fact]
        public void TestGetValueForObject()
        {
            var result = TeaJSON.Parse("{\"data\":{\"message\":\"Hello world\"}}");
            Assert.True(result.IsObject());
            var value = result.GetValue();
            Assert.IsType<Dictionary<string, object>>(value);
            var returnValue = (Dictionary<string, object>)value;
            Assert.Single(returnValue);
            Assert.True(returnValue.ContainsKey("data"));
            Assert.IsType<Dictionary<string, object>>(returnValue["data"]);
            var dataDict = (Dictionary<string, object>)returnValue["data"];
            Assert.Single(dataDict);
            Assert.True(dataDict.ContainsKey("message"));
            Assert.IsType<string>(dataDict["message"]);
            var message = (string)dataDict["message"];
            Assert.Equal("Hello world", message);
        }

        [Fact]
        public void TestGetValueForArray()
        {
            var result = TeaJSON.Parse("[{\"data\":{\"message\":\"Hello world\"}}]");
            Assert.True(result.IsArray());
            var value = result.GetValue();
            Assert.IsType<List<object>>(value);
            var list = (List<object>)value;
            Assert.Single(list);
            var item = list[0];
            Assert.IsType<Dictionary<string, object>>(item);
            var returnValue = (Dictionary<string, object>)item;
            Assert.Single(returnValue);
            Assert.True(returnValue.ContainsKey("data"));
            Assert.IsType<Dictionary<string, object>>(returnValue["data"]);
            var dataDict = (Dictionary<string, object>)returnValue["data"];
            Assert.Single(dataDict);
            Assert.True(dataDict.ContainsKey("message"));
            Assert.IsType<string>(dataDict["message"]);
            var message = (string)dataDict["message"];
            Assert.Equal("Hello world", message);
        }

        [Fact]
        public void TestParseArray()
        {
            JSONItem item = TeaJSON.Parse("[]");
            Assert.True(item.IsArray());
            List<JSONItem> list = item.GetAsArray();
            Assert.Empty(list);
        }

        [Fact]
        public void TestLexerForEmptyArray()
        {
            var lexer = new JSONLexer("[]");
            List<Token> tokens = new List<Token>();
            Token token;
            do {
                token = lexer.scan();
                tokens.Add(token);
            }
            while (token.type != TokenType.EOF);
            Assert.Equal(3, tokens.Count);
            var token1 = tokens[0];
            Assert.Equal("[", token1.lexeme);
            Assert.Equal(TokenType.NORMARL, token1.type);
            var token2 = tokens[1];
            Assert.Equal("]", token2.lexeme);
            Assert.Equal(TokenType.NORMARL, token2.type);
            var token3 = tokens[2];
            Assert.Equal(TokenType.EOF, token3.type);
        }

        [Fact]
        public void TestLexerForEmptyObject()
        {
            var lexer = new JSONLexer("{}");
            List<Token> tokens = new List<Token>();
            Token token;
            do {
                token = lexer.scan();
                tokens.Add(token);
            }
            while (token.type != TokenType.EOF);
            Assert.Equal(3, tokens.Count);
            var token1 = tokens[0];
            Assert.Equal("{", token1.lexeme);
            Assert.Equal(TokenType.NORMARL, token1.type);
            var token2 = tokens[1];
            Assert.Equal("}", token2.lexeme);
            Assert.Equal(TokenType.NORMARL, token2.type);
            var token3 = tokens[2];
            Assert.Equal(TokenType.EOF, token3.type);
        }

        private void check(Token token, string lexeme, TokenType type) {
            Assert.Equal(lexeme, token.lexeme);
            Assert.Equal(type, token.type);
        }

        [Fact]
        public void TestLexerForObject()
        {
            var lexer = new JSONLexer("{\"t_int\":10,\"t_bool\":true}");
            List<Token> tokens = new List<Token>();
            Token token;
            do {
                token = lexer.scan();
                tokens.Add(token);
            }
            while (token.type != TokenType.EOF);
            Assert.Equal(10, tokens.Count);
            check(tokens[0], "{", TokenType.NORMARL);
            check(tokens[1], "t_int", TokenType.STRING);
            check(tokens[2], ":", TokenType.NORMARL);
            check(tokens[3], "10", TokenType.NUMBER);
            check(tokens[4], ",", TokenType.NORMARL);
            check(tokens[5], "t_bool", TokenType.STRING);
            check(tokens[6], ":", TokenType.NORMARL);
            check(tokens[7], "true", TokenType.BOOLEAN);
            check(tokens[8], "}", TokenType.NORMARL);
            check(tokens[9], null, TokenType.EOF);
        }

    }
}