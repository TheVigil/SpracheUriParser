using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Sprache;


namespace UriParser
{
    class Parser : UriGrammar

    {
        public static Tuple<string, string> ParseTestUri(string uri)
        {
            var testUri = uri;
            Console.WriteLine("TestUri: " + uri);
            var parserResult = Fragment.Parse(testUri);

            return new Tuple<string, string>("Parser Result: ", parserResult);
        }

        public static void Main()
        {
            //ParseTestUri("!$&'()*+,;=");
            //ParseTestUri(":/?#[]@");
            //ParseTestUri("defg");
            //ParseTestUri("789");
            //ParseTestUri("789");
            //ParseTestUri("-._~");
            ParseTestUri("%0A%0A//");
        }
    }
}
