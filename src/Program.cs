using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var json = File.ReadAllText(@"sample.json");

            var map = JsonParser.Parse(json);

            Console.WriteLine(JsonConvert.SerializeObject(map));
        }
    }

    public static class JsonParser
    {
        public static Dictionary<string, object> Parse(string json)
        {
            var stack = new Stack<object>();
            var reader = new TokenReader(json);
            bool loop = true;
            bool needRead = true;
            int i = 0;

            while (loop)
            {
                if (needRead == true)
                {
                    i = reader.Read();
                }
                else
                {
                    needRead = false;
                }

                switch (i)
                {
                    case '{':
                        stack.Push(new Dictionary<string, object>());
                        break;
                    case '"':
                        var token = reader.ReadToken(out i);

                        if (i == ',')
                        {
                            needRead = false;
                        }

                        stack.Push(token);
                        break;
                    case ',':
                        var value = stack.Pop();
                        var key = stack.Pop() as string;
                        var dic = stack.Peek() as Dictionary<string, object>;
                        dic.Add(key, value);
                        break;
                    case '}':
                        var value1 = stack.Pop();
                        var key1 = stack.Pop() as string;
                        var dic1 = stack.Peek() as Dictionary<string, object>;
                        dic1.Add(key1, value1);
                        break;
                    case -1:
                        loop = false;
                        break;
                }
            }

            var map = (Dictionary<string, object>)stack.Pop();

            return map;
        }
    }

    public class TokenReader
    {
        private StringReader _reader;

        public TokenReader(string s)
        {
            _reader = new StringReader(s);
        }

        public string ReadToken(out int i)
        {
            int c;
            string str = "";

            while ((c = _reader.Read()) != '"')
            {
                str += (char)c;
            }

            i = c;

            return str;
        }

        public int Read()
        {
            return _reader.Read();
        }
    }
}