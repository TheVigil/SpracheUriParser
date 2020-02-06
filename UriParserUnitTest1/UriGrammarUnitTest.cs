using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;
using UriParser;
using Sprache;

namespace UriParserUnitTest1
{
    [TestClass]
    public class UriGrammarUnitTest
    {
        [TestMethod]
        public void Test_SubDelims()
        {
            // sub-delims -> ! | $ | & | ' | ( | ) | * | + | , | ; | =
            // delimiter terminals
            var parsedResult = UriParser.UriGrammar.SubDelims.Parse("!");
            Assert.AreEqual(parsedResult, '!');
        }

        [TestMethod]
        public void Test_SubDelims1()
        {
            // sub-delims -> ! | $ | & | ' | ( | ) | * | + | , | ; | =
            // delimiter terminals
            var parsedResult = UriParser.UriGrammar.SubDelims.Parse("$+");
            Assert.AreEqual(parsedResult, '$');
        }

        [TestMethod]
        [ExpectedException((typeof(Sprache.ParseException)))]
        public void Test_SubDelimsError()
        {
            // sub-delims -> ! | $ | & | ' | ( | ) | * | + | , | ; | =
            // delimiter terminals
            var parsedResult = UriParser.UriGrammar.SubDelims.Parse("x");


        }

        [TestMethod]
        [ExpectedException((typeof(Sprache.ParseException)))]
        public void Test_SubDelimsError2()
        {
            // sub-delims -> ! | $ | & | ' | ( | ) | * | + | , | ; | =
            // delimiter terminals
            var parsedResult = UriParser.UriGrammar.SubDelims.Parse(":/?");


        }

        [TestMethod]
        public void Test_GenDelims()
        {   // gen-delims -> : | / | ? | # | [ | ] | @
            // delimiter terminals
            var parsedResult = UriParser.UriGrammar.GenDelims.Parse(":/?");
            Assert.AreEqual(parsedResult, ':');


        }

        [TestMethod]
        public void Test_GenDelims1()
        {   // gen-delims -> : | / | ? | # | [ | ] | @
            // delimiter terminals
            var parsedResult = UriParser.UriGrammar.GenDelims.Parse("[]@");
            Assert.AreEqual(parsedResult, '[');


        }

        [TestMethod]
        [ExpectedException((typeof(Sprache.ParseException)))]
        public void Test_GenDelimsError()
        {   // gen-delims -> : | / | ? | # | [ | ] | @
            // delimiter terminals  
            var parsedResult = UriParser.UriGrammar.GenDelims.Parse("x");
        }

        [TestMethod]
        [ExpectedException((typeof(Sprache.ParseException)))]
        public void Test_GenDelimsError1()
        {   // gen-delims -> : | / | ? | # | [ | ] | @
            // delimiter terminals
            var parsedResult = UriParser.UriGrammar.GenDelims.Parse("!$");
        }

        [TestMethod]
        public void Test_Reserved()
        {
            // reserved -> sub-delim | gen-delim
            // parse reserved chars
            var parsedResult = UriParser.UriGrammar.Reserved.Parse("!$");
            Assert.AreEqual(parsedResult, '!');
        }

        [TestMethod]
        public void Test_Reserved1()
        {
            // reserved -> sub-delim | gen-delim
            // parse reserved chars
            var parsedResult = UriParser.UriGrammar.Reserved.Parse("[$");
            Assert.AreEqual(parsedResult, '[');
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_ReservedError()
        {
            // reserved -> sub-delim | gen-delim
            // parse reserved chars
            var parsedResult = UriParser.UriGrammar.Reserved.Parse("xxx");
            
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_ReservedError1()
        {
            // reserved -> sub-delim | gen-delim
            // parse reserved chars
            var parsedResult = UriParser.UriGrammar.Reserved.Parse("d$!");

        }

        [TestMethod]
        public void Test_Alpha()
        {   // parse Chars as defined in ALPHA (rfc2234) 
            var parsedResult = UriParser.UriGrammar.Alpha.Parse("abc");
            Assert.AreEqual(parsedResult, 'a');
        }

        [TestMethod]
        public void Test_Alpha1()
        {   // parse Chars as defined in ALPHA (rfc2234) 
            var parsedResult = UriParser.UriGrammar.Alpha.Parse("Abc");
            Assert.AreEqual(parsedResult, 'A');
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_AlphaError()
        {   // parse Chars as defined in ALPHA (rfc2234) 
            var parsedResult = UriParser.UriGrammar.Alpha.Parse("?abc");

        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_AlphaError1()
        {   // parse Chars as defined in ALPHA (rfc2234) 
            var parsedResult = UriParser.UriGrammar.Alpha.Parse("!Abc");

        }

        [TestMethod]
        public void Test_Digit()
        {
            // parse Digits as defined in DIGIT (rfc2234)
            var parsedResult = UriParser.UriGrammar.Digit.Parse("1234");
            Assert.AreEqual(parsedResult, '1');
        }

        [TestMethod]
        public void Test_Digit2()
        {
            // parse Digits as defined in Digit (rfc2234)
            var parsedResult = UriParser.UriGrammar.Digit.Parse("4321");
            Assert.AreEqual(parsedResult, '4');
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_DigitError()
        {
            // parse Digits as defined in Digit (rfc2234)
            var parsedResult = UriParser.UriGrammar.Digit.Parse("a43");
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_DigitError1()
        {
            // parse Digits as defined in Digit (rfc2234)
            var parsedResult = UriParser.UriGrammar.Digit.Parse("B12");
        }

        [TestMethod]
        public void Test_Unreserved()
        {
            // unreserved -> ALPHA | DIGIT | - | . | _ | ~
            // parse the unreserved characters
            var parsedResult = UriParser.UriGrammar.Unreserved.Parse("~");
            Assert.AreEqual(parsedResult, '~');
        }

        [TestMethod]
        public void Test_Unreserved1()
        {
            // unreserved -> ALPHA | DIGIT | - | . | _ | ~
            // parse the unreserved characters
            var parsedResult = UriParser.UriGrammar.Unreserved.Parse("_");
            Assert.AreEqual(parsedResult, '_');
        }

        [TestMethod]
        public void Test_Unreserved2()
        {
            // unreserved -> ALPHA | DIGIT | - | . | _ | ~
            // parse the unreserved characters
            var parsedResult = UriParser.UriGrammar.Unreserved.Parse("C");
            Assert.AreEqual(parsedResult, 'C');
        }

        [TestMethod]
        public void Test_Unreserved3()
        {
            // unreserved -> ALPHA | DIGIT | - | . | _ | ~
            // parse the unreserved characters
            var parsedResult = UriParser.UriGrammar.Unreserved.Parse("4");
            Assert.AreEqual(parsedResult, '4');
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_UnreservedError()
        {
            // unreserved -> ALPHA | DIGIT | - | . | _ | ~
            // parse the unreserved characters
            var parsedResult = UriParser.UriGrammar.Unreserved.Parse("?");

        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_UnreservedError1()
        {
            // unreserved -> ALPHA | DIGIT | - | . | _ | ~
            // parse the unreserved characters
            var parsedResult = UriParser.UriGrammar.Unreserved.Parse("@");

        }

        [TestMethod]
        public void Test_HexDigit()
        {
            //parse Hexadecimal digits as defined in HEXDIG (rfc2234)
            var parsedResult = UriParser.UriGrammar.HexDigit.Parse("A");
            Assert.AreEqual(parsedResult, 'A');
        }

        [TestMethod]
        public void Test_HexDigit1()
        {
            //parse Hexadecimal digits as defined in HEXDIG (rfc2234)
            var parsedResult = UriParser.UriGrammar.HexDigit.Parse("0");
            Assert.AreEqual(parsedResult, '0');
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_HexDigitError()
        {
            //parse Hexadecimal digits as defined in HEXDIG (rfc2234)
            var parsedResult = UriParser.UriGrammar.HexDigit.Parse("J");
            
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_HexDigitError1()
        {
            //parse Hexadecimal digits as defined in HEXDIG (rfc2234)
            var parsedResult = UriParser.UriGrammar.HexDigit.Parse("?0");

        }

        [TestMethod]
        public void Test_PctChar()
        {
            var parsedResult = UriParser.UriGrammar.PctChar.Parse("%");
            Assert.AreEqual(parsedResult, '%');
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_PctCharError()
        {
            var parsedResult = UriParser.UriGrammar.PctChar.Parse("!");
        }

        [TestMethod]
         public void Test_PctEncoded()
        {
            var parsedResult = UriParser.UriGrammar.PctEncoded.Parse("%0F");
            Assert.AreEqual(parsedResult, "%0F");
        }

        [TestMethod]
        public void Test_PctEncoded1()
        {
            var parsedResult = UriParser.UriGrammar.PctEncoded.Parse("%9E");
            Assert.AreEqual(parsedResult, "%9E");
        }

        [TestMethod]
        public void Test_PctEncoded2()
        {
            var parsedResult = UriParser.UriGrammar.PctEncoded.Parse("%00");
            Assert.AreEqual(parsedResult, "%00");
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_PctEncodedError()
        {
            var parsedResult = UriParser.UriGrammar.PctEncoded.Parse("%Xa");
            
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_PctEncodedError1()
        {
            var parsedResult = UriParser.UriGrammar.PctEncoded.Parse("%GG");

        }

        [TestMethod]
        public void Test_PChar()
        {
            // pchar -> unreserved | pct-encoded | ":" | "@"
            var parsedResult = UriParser.UriGrammar.PChar.Parse("%0F");
            Assert.AreEqual(parsedResult, "%0F");
        }

        [TestMethod]
        public void Test_PChar1()
        {
            // pchar -> unreserved | pct-encoded | ":" | "@"
            var parsedResult = UriParser.UriGrammar.PChar.Parse("_");
            Assert.AreEqual(parsedResult, "_");
        }

        [TestMethod]
        public void Test_PChar2()
        {
            // pchar -> unreserved | pct-encoded | ":" | "@"
            var parsedResult = UriParser.UriGrammar.PChar.Parse("A");
            Assert.AreEqual(parsedResult, "A");
        }

        [TestMethod]
        public void Test_PChar3()
        {
            // pchar -> unreserved | pct-encoded | ":" | "@"
            var parsedResult = UriParser.UriGrammar.PChar.Parse("1");
            Assert.AreEqual(parsedResult, "1");
        }

        [TestMethod]
        public void Test_PChar4()
        {
            // pchar -> unreserved | pct-encoded | ":" | "@"
            var parsedResult = UriParser.UriGrammar.PChar.Parse("@");
            Assert.AreEqual(parsedResult, "@");
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_PCharError()
        {   
            // TODO: this test fails when it should be pasing, manual debugging shows a Sprache.ParseException being thrown.
            // pchar -> unreserved | pct-encoded | ":" | "@"
            var parsedResult = UriParser.UriGrammar.PChar.Parse(")");
            
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_PCharError1()
        {
            // pchar -> unreserved | pct-encoded | ":" | "@"
            var parsedResult = UriParser.UriGrammar.PChar.Parse("[");

        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_PCharError2()
        {
            // pchar -> unreserved | pct-encoded | ":" | "@"
            var parsedResult = UriParser.UriGrammar.PChar.Parse("%%0F");

        }

        [TestMethod]
        public void Test_Fragment()
        {
            var parsedResult = UriParser.UriGrammar.Fragment.Parse("%0A%0A//");
            Assert.AreEqual(parsedResult, "%0A%0A//");
        }
    }
}
