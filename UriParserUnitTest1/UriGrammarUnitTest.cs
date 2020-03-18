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
        #region subdelims tests

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

        #endregion

        #region Gendilms tests

        [TestMethod]
        public void Test_GenDelims()
        {
            // gen-delims -> : | / | ? | # | [ | ] | @
            // delimiter terminals
            var parsedResult = UriParser.UriGrammar.GenDelims.Parse(":/?");
            Assert.AreEqual(parsedResult, ':');


        }

        [TestMethod]
        public void Test_GenDelims1()
        {
            // gen-delims -> : | / | ? | # | [ | ] | @
            // delimiter terminals
            var parsedResult = UriParser.UriGrammar.GenDelims.Parse("[]@");
            Assert.AreEqual(parsedResult, '[');


        }

        [TestMethod]
        [ExpectedException((typeof(Sprache.ParseException)))]
        public void Test_GenDelimsError()
        {
            // gen-delims -> : | / | ? | # | [ | ] | @
            // delimiter terminals  
            var parsedResult = UriParser.UriGrammar.GenDelims.Parse("x");
        }

        [TestMethod]
        [ExpectedException((typeof(Sprache.ParseException)))]
        public void Test_GenDelimsError1()
        {
            // gen-delims -> : | / | ? | # | [ | ] | @
            // delimiter terminals
            var parsedResult = UriParser.UriGrammar.GenDelims.Parse("!$");
        }

        #endregion

        #region reserved tests

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

        #endregion

        #region Alpha tests

        [TestMethod]
        public void Test_Alpha()
        {
            // parse Chars as defined in ALPHA (rfc2234) 
            var parsedResult = UriParser.UriGrammar.Alpha.Parse("abc");
            Assert.AreEqual(parsedResult, 'a');
        }

        [TestMethod]
        public void Test_Alpha1()
        {
            // parse Chars as defined in ALPHA (rfc2234) 
            var parsedResult = UriParser.UriGrammar.Alpha.Parse("Abc");
            Assert.AreEqual(parsedResult, 'A');
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_AlphaError()
        {
            // parse Chars as defined in ALPHA (rfc2234) 
            var parsedResult = UriParser.UriGrammar.Alpha.Parse("?abc");

        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_AlphaError1()
        {
            // parse Chars as defined in ALPHA (rfc2234) 
            var parsedResult = UriParser.UriGrammar.Alpha.Parse("!Abc");

        }

        #endregion

        #region digit tests

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

        #endregion

        #region unreserved tests

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

        #endregion

        #region hexdigit tests
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


        #endregion

        #region pctchar tests
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


        #endregion

        #region pctencoded tests
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
        
        #endregion

        #region pchar tests
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
            // pchar -> unreserved | pct-encoded | sub-delim | ":" | "@"
            // var parsedResult = UriParser.UriGrammar.PChar.Parse(")");
            var parsedResult = UriParser.UriGrammar.Unreserved.Parse("?");
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


        #endregion

        #region fragment tests
        [TestMethod]
        public void Test_Fragment0()
        {
            var parsedResult = UriParser.UriGrammar.Fragment.Parse("%0A%0A//??");
            Assert.AreEqual(parsedResult, "%0A%0A//??");
        }

        [TestMethod]
        public void Test_Fragment1()
        {
            var parsedResult = UriParser.UriGrammar.Fragment.Parse("%0A%0A//");
            Assert.AreEqual(parsedResult, "%0A%0A//");
        }

        [TestMethod]
        public void Test_Fragment2()
        {
            var parsedResult = UriParser.UriGrammar.Fragment.Parse("%0A%0A??");
            Assert.AreEqual(parsedResult, "%0A%0A??");
        }

        [TestMethod]
        public void Test_Fragment3()
        {
            var parsedResult = UriParser.UriGrammar.Fragment.Parse("%0A%0A");
            Assert.AreEqual(parsedResult, "%0A%0A");
        }

        [TestMethod]
        public void Test_Fragment4()
        {
            var parsedResult = UriParser.UriGrammar.Fragment.Parse("//");
            Assert.AreEqual(parsedResult, "//");
        }

        [TestMethod]
        public void Test_Fragment5()
        {
            var parsedResult = UriParser.UriGrammar.Fragment.Parse("??");
            Assert.AreEqual(parsedResult, "??");
        }

        [TestMethod]
        public void Test_Fragment6()
        {
            var parsedResult = UriParser.UriGrammar.Fragment.Parse("//??");
            Assert.AreEqual(parsedResult, "//??");
        }


        #endregion

        #region query tests
        [TestMethod]
        public void Test_Query0()
        {
            var parsedResult = UriParser.UriGrammar.Fragment.Parse("%0A%0A//??");
            Assert.AreEqual(parsedResult, "%0A%0A//??");
        }

        [TestMethod]
        public void Test_Query1()
        {
            var parsedResult = UriParser.UriGrammar.Fragment.Parse("%0A%0A//");
            Assert.AreEqual(parsedResult, "%0A%0A//");
        }

        [TestMethod]
        public void Test_Query2()
        {
            var parsedResult = UriParser.UriGrammar.Fragment.Parse("%0A%0A??");
            Assert.AreEqual(parsedResult, "%0A%0A??");
        }

        [TestMethod]
        public void Test_Query3()
        {
            var parsedResult = UriParser.UriGrammar.Fragment.Parse("%0A%0A");
            Assert.AreEqual(parsedResult, "%0A%0A");
        }

        [TestMethod]
        public void Test_Query4()
        {
            var parsedResult = UriParser.UriGrammar.Fragment.Parse("//");
            Assert.AreEqual(parsedResult, "//");
        }

        [TestMethod]
        public void Test_Query5()
        {
            var parsedResult = UriParser.UriGrammar.Fragment.Parse("??");
            Assert.AreEqual(parsedResult, "??");
        }

        [TestMethod]
        public void Test_Query6()
        {
            var parsedResult = UriParser.UriGrammar.Fragment.Parse("//??");
            Assert.AreEqual(parsedResult, "//??");
        }


        #endregion

        #region segment tests
        [TestMethod]
        public void Test_SegmentNzNc0()
        {
            var parsedResult = UriParser.UriGrammar.SegmentNzNc.Parse("aB01-._~");
            Assert.AreEqual(parsedResult, "aB01-._~");
        }

        [TestMethod]
        public void Test_SegmentNzNc1()
        {
            var parsedResult = UriParser.UriGrammar.SegmentNzNc.Parse("%0E");
            Assert.AreEqual(parsedResult, "%0E");
        }

        [TestMethod]
        public void Test_SegmentNzNc2()
        {
            var parsedResult = UriParser.UriGrammar.SegmentNzNc.Parse("!$");
            Assert.AreEqual(parsedResult, "!$");
        }

        [TestMethod]
        public void Test_SegmentNz()
        {
            var parsedResult = UriParser.UriGrammar.SegmentNz.Parse("aB01-._~%2D!$&'()*+,;=:@");
            Assert.AreEqual(parsedResult, "aB01-._~%2D!$&'()*+,;=:@");
        }

        [TestMethod]
        public void Test_SegmentNz1()
        {
            var parsedResult = UriParser.UriGrammar.SegmentNz.Parse("aB_~%2D:@");
            Assert.AreEqual(parsedResult, "aB_~%2D:@");
        }

        [TestMethod]
        public void Test_SegmentNz2()
        {
            var parsedResult = UriParser.UriGrammar.SegmentNz.Parse("@");
            Assert.AreEqual(parsedResult, "@");
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_SegmentNz3()
        {
            var parsedResult = UriParser.UriGrammar.SegmentNz.Parse("");

        }

        [TestMethod]

        public void Test_Segment()
        {
            var parsedResult = UriParser.UriGrammar.Segment.Parse("");
            Assert.AreEqual(parsedResult, "");
        }

        [TestMethod]
        public void Test_Segment1()
        {
            var parsedResult = UriParser.UriGrammar.Segment.Parse("@");
            Assert.AreEqual(parsedResult, "@");
        }

        [TestMethod]
        public void Test_Segment2()
        {
            var parsedResult = UriParser.UriGrammar.Segment.Parse("aB_~%2D:@");
            Assert.AreEqual(parsedResult, "aB_~%2D:@");
        }

        [TestMethod]
        public void Test_Segment3()
        {
            var parsedResult = UriParser.UriGrammar.Segment.Parse("aB01-._~%2D!$&'()*+,;=:@");
            Assert.AreEqual(parsedResult, "aB01-._~%2D!$&'()*+,;=:@");
        }


        #endregion

        #region PathEmpty/PathRootless/PathNoScheme/PathAbsolute/PathAbEmpty/Path/RegName tests
        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_PathEmpty()
        {
            var parsedResult = UriParser.UriGrammar.PathEmpty.Parse("aB01-._~%2D!$&'()*+,;=:@");

        }

        [TestMethod]
        public void Test_PathEmpty1()
        {
            var parsedResult = UriParser.UriGrammar.PathEmpty.Parse("");
            Assert.AreEqual(parsedResult, "");
        }

        [TestMethod]
        public void Test_PathRootless()
        {
            var parsedResult = UriParser.UriGrammar.PathRootless.Parse("aB_~%2D:@/aB01/aB01");
            Assert.AreEqual(parsedResult, "aB_~%2D:@/aB01/aB01");
        }

        [TestMethod]
        public void Test_PathRootless1()
        {
            var parsedResult = UriParser.UriGrammar.PathRootless.Parse("aB_~%2D:@");
            Assert.AreEqual(parsedResult, "aB_~%2D:@");
        }

        [TestMethod]
        public void Test_PathRootless2()
        {
            var parsedResult = UriParser.UriGrammar.PathRootless.Parse("aB_~%2D:@///");
            Assert.AreEqual(parsedResult, "aB_~%2D:@///");
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_PathRootlessError()
        {
            var parsedResult = UriParser.UriGrammar.PathRootless.Parse("///");

        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_PathRootlessError1()
        {
            var parsedResult = UriParser.UriGrammar.PathRootless.Parse("?=");

        }

        [TestMethod]
        public void Test_PathNoScheme()
        {
            var parsedResult = UriParser.UriGrammar.PathNoScheme.Parse("aB_~/aB01/aB01");
            Assert.AreEqual(parsedResult, "aB_~/aB01/aB01");
        }

        [TestMethod]
        public void Test_PathNoScheme1()
        {
            var parsedResult = UriParser.UriGrammar.PathNoScheme.Parse("%0E/aB01/aB01");
            Assert.AreEqual(parsedResult, "%0E/aB01/aB01");
        }

        [TestMethod]
        public void Test_PathNoScheme2()
        {
            var parsedResult = UriParser.UriGrammar.PathNoScheme.Parse("!$/aB01/aB01");
            Assert.AreEqual(parsedResult, "!$/aB01/aB01");
        }

        [TestMethod]
        public void Test_PathNoScheme3()
        {
            var parsedResult = UriParser.UriGrammar.PathNoScheme.Parse("@/aB01/aB01");
            Assert.AreEqual(parsedResult, "@/aB01/aB01");
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_PathNoSchemeError()
        {
            var parsedResult = UriParser.UriGrammar.PathNoScheme.Parse("/aB01/aB01");

        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_PathNoSchemeError1()
        {
            var parsedResult = UriParser.UriGrammar.PathNoScheme.Parse(":/aB01/aB01");

        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_PathNoSchemeError2()
        {
            var parsedResult = UriParser.UriGrammar.PathNoScheme.Parse("///");

        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_PathNoSchemeError3()
        {
            var parsedResult = UriParser.UriGrammar.PathNoScheme.Parse(":?=");

        }

        [TestMethod]
        public void Test_PathAbsolute()
        {
            var parsedResult = UriParser.UriGrammar.PathAbsolute.Parse("/");
            Assert.AreEqual(parsedResult, "/");
        }

        [TestMethod]
        public void Test_PathAbsolute1()
        {
            var parsedResult = UriParser.UriGrammar.PathAbsolute.Parse("/aB_~%2D:@/aB01/aB01");
            Assert.AreEqual(parsedResult, "/aB_~%2D:@/aB01/aB01");
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_PathAbsoluteError()
        {
            var parsedResult = UriParser.UriGrammar.PathAbsolute.Parse("aB_~%2D:@/aB01/aB01");

        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_PathAbsoluteError1()
        {
            var parsedResult = UriParser.UriGrammar.PathAbsolute.Parse("");

        }

        [TestMethod]
        public void Test_PathAbEmpty()
        {
            var parsedResult = UriParser.UriGrammar.PathAbEmpty.Parse("/aB01/aB01");
            Assert.AreEqual(parsedResult, "/aB01/aB01");
        }

        [TestMethod]
        public void Test_PathAbEmpty1()
        {
            var parsedResult = UriParser.UriGrammar.PathAbEmpty.Parse("");
            Assert.AreEqual(parsedResult, "");
        }

        [TestMethod]
        public void Test_PathAbEmpty2()
        {
            var parsedResult = UriParser.UriGrammar.PathAbEmpty.Parse("?");
            Assert.AreEqual(parsedResult, "");
        }

        [TestMethod]
        public void Test_RegName()
        {
            var parsedResult = UriParser.UriGrammar.RegName.Parse("aB01-._~");
            Assert.AreEqual(parsedResult, "aB01-._~");
        }

        [TestMethod]
        public void Test_RegName1()
        {
            var parsedResult = UriParser.UriGrammar.RegName.Parse("%0E");
            Assert.AreEqual(parsedResult, "%0E");
        }

        [TestMethod]
        public void Test_RegName2()
        {
            var parsedResult = UriParser.UriGrammar.RegName.Parse("!$");
            Assert.AreEqual(parsedResult, "!$");
        }

        [TestMethod]
        public void Test_RegName3()
        {
            var parsedResult = UriParser.UriGrammar.RegName.Parse("aB01-._~!$");
            Assert.AreEqual(parsedResult, "aB01-._~!$");
        }

        [TestMethod]
        public void Test_RegName4()
        {
            var parsedResult = UriParser.UriGrammar.RegName.Parse("%0E!$");
            Assert.AreEqual(parsedResult, "%0E!$");
        }

        [TestMethod]
        public void Test_RegName5()
        {
            var parsedResult = UriParser.UriGrammar.RegName.Parse("aB01-._~%0E");
            Assert.AreEqual(parsedResult, "aB01-._~%0E");
        }

        [TestMethod]
        public void Test_RegName6()
        {
            var parsedResult = UriParser.UriGrammar.RegName.Parse("aB01-._~%0E!$");
            Assert.AreEqual(parsedResult, "aB01-._~%0E!$");
        }
        
        #endregion

        #region ipv4 tests

        [TestMethod]
        public void Test_IPv4Address()
        {
            var parsedResult = UriParser.UriGrammar.IPv4Address.Parse("192.168.255.255");
            Assert.AreEqual(parsedResult, "192.168.255.255");
        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_IPv4Address1()
        {   // TODO: dec-octet parser still allows poorly formed IPs!
            var parsedResult = UriParser.UriGrammar.IPv4Address.Parse("192.168.1.257");

        }

        [TestMethod]
        [ExpectedException(typeof(Sprache.ParseException))]
        public void Test_IPv4Address2()
        {   // TODO: dec-octet parser still allows poorly formed IPs!
            var parsedResult = UriParser.UriGrammar.IPv4Address.Parse("192.01.168.255");

        }

        #endregion

        #region ls32 tests

        [TestMethod]
        public void Test_ls32()
        {
            var parsedResult = UriParser.UriGrammar.ls32.Parse("01AB:23CD");
            Assert.AreEqual(parsedResult, "01AB:23CD");
        }

        [TestMethod]
        public void Test_ls32_2()
        {
            var parsedResult = UriParser.UriGrammar.ls32.Parse("192.168.255.255");
            Assert.AreEqual(parsedResult, "192.168.255.255");
        }

        [TestMethod]
        public void Test_ls32_1()
        {
            var parsedResult = UriParser.UriGrammar.ls32.Parse("192.168.1.1");
            Assert.AreEqual(parsedResult, "192.168.1.1");
        }

        [TestMethod]
        public void Test_ls32_3()
        {
            var parsedResult = UriParser.UriGrammar.ls32.Parse("01AB:23CD");
            Assert.AreEqual(parsedResult, "01AB:23CD");
        }


        #endregion

        #region IPv6 tests
        [TestMethod]
        public void Test_IPv6()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser1.Parse("01AB:23CD:45AB:67DE:89EF:12EF:89EF:12AB");
            Assert.AreEqual(parsedResult, "01AB:23CD:45AB:67DE:89EF:12EF:89EF:12AB");
        }

        [TestMethod]
        public void Test_IPv61()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser2.Parse("::45AB:67DE:89EF:12EF:89EF:12EF:12EF");
            Assert.AreEqual(parsedResult, "::45AB:67DE:89EF:12EF:89EF:12EF:12EF");
        }

        [TestMethod]
        public void Test_IPv61b()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser2.Parse("::45AB:67DE:89EF:12EF:89EF:192.168.255.255");
            Assert.AreEqual(parsedResult, "::45AB:67DE:89EF:12EF:89EF:192.168.255.255");
        }

        [TestMethod]
        public void Test_IPv62()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser3.Parse("01AB::45AB:67DE:89EF:12EF:89EF:12EF");
            Assert.AreEqual(parsedResult, "01AB::45AB:67DE:89EF:12EF:89EF:12EF");
        }

        [TestMethod]
        public void Test_IPv62b()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser3.Parse("::45AB:67DE:89EF:12EF:192.168.255.255");
            Assert.AreEqual(parsedResult, "::45AB:67DE:89EF:12EF:192.168.255.255");
        }

        [TestMethod]
        public void Test_IPv63()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser3.Parse("::45AB:67DE:89EF:12EF:89EF:12EF");
            Assert.AreEqual(parsedResult, "::45AB:67DE:89EF:12EF:89EF:12EF");
        }

        [TestMethod]
        public void Test_IPv64()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser4.Parse("45AB:45AB::45AB:67DE:89EF:12EF:89EF");
            Assert.AreEqual(parsedResult, "45AB:45AB::45AB:67DE:89EF:12EF:89EF");
        }

        [TestMethod]
        public void Test_IPv65()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser4.Parse("::45AB:67DE:89EF:12EF:89EF");
            Assert.AreEqual(parsedResult, "::45AB:67DE:89EF:12EF:89EF");
        }

        [TestMethod]
        public void Test_IPv64c()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser4.Parse("45AB:45AB::45AB:67DE:89EF:192.168.255.255");
            Assert.AreEqual(parsedResult, "45AB:45AB::45AB:67DE:89EF:192.168.255.255");
        }

        [TestMethod]
        public void Test_IPv65d()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser4.Parse("::45AB:67DE:89EF:192.168.255.255");
            Assert.AreEqual(parsedResult, "::45AB:67DE:89EF:192.168.255.255");
        }

        [TestMethod]
        public void Test_IPv66()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser5.Parse("45AB:45AB:89EF::12EF:89EF:12EF:89EF");
            Assert.AreEqual(parsedResult, "45AB:45AB:89EF::12EF:89EF:12EF:89EF");
        }

        [TestMethod]
        public void Test_IPv66b()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser5.Parse("::12EF:89EF:12EF:89EF");
            Assert.AreEqual(parsedResult, "::12EF:89EF:12EF:89EF");
        }

        [TestMethod]
        public void Test_IPv66c()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser5.Parse("45AB:45AB:89EF::12EF:89EF:198.168.255.255");
            Assert.AreEqual(parsedResult, "45AB:45AB:89EF::12EF:89EF:198.168.255.255");
        }

        [TestMethod]
        public void Test_IPv66d()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser5.Parse("::12EF:89EF:198.168.255.255");
            Assert.AreEqual(parsedResult, "::12EF:89EF:198.168.255.255");
        }

        [TestMethod]
        public void Test_IPv67()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser6.Parse("45AB:45AB:89EF:12EF::12EF:89EF:12EF");
            Assert.AreEqual(parsedResult, "45AB:45AB:89EF:12EF::12EF:89EF:12EF");
        }

        [TestMethod]
        public void Test_IPv68()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser6.Parse("::12EF:89EF:12EF");
            Assert.AreEqual(parsedResult, "::12EF:89EF:12EF");
        }

        [TestMethod]
        public void Test_IPv67b()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser6.Parse("45AB:45AB:89EF:12EF::12EF:192.168.255.255");
            Assert.AreEqual(parsedResult, "45AB:45AB:89EF:12EF::12EF:192.168.255.255");
        }

        [TestMethod]
        public void Test_IPv68b()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser6.Parse("::12EF:192.168.255.255");
            Assert.AreEqual(parsedResult, "::12EF:192.168.255.255");
        }

        [TestMethod]
        public void Test_IPv69()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser7.Parse("12EF:89EF:12EF:89EF:12EF::12EF:89EF");
            Assert.AreEqual(parsedResult, "12EF:89EF:12EF:89EF:12EF::12EF:89EF");
        }

        [TestMethod]
        public void Test_IPv610()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser7.Parse("::12EF:89EF");
            Assert.AreEqual(parsedResult, "::12EF:89EF");
        }

        [TestMethod]
        public void Test_IPv69b()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser7.Parse("12EF:89EF:12EF:89EF:12EF::192.168.255.255");
            Assert.AreEqual(parsedResult, "12EF:89EF:12EF:89EF:12EF::192.168.255.255");
        }

        [TestMethod]
        public void Test_IPv610b()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser7.Parse("::192.168.255.255");
            Assert.AreEqual(parsedResult, "::192.168.255.255");
        }

        [TestMethod]
        public void Test_IPv611()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser8.Parse("12EF:89EF:12EF:89EF:12EF:89EF::12EF");
            Assert.AreEqual(parsedResult, "12EF:89EF:12EF:89EF:12EF:89EF::12EF");
        }

        [TestMethod]
        public void Test_IPv612()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser8.Parse("::12EF");
            Assert.AreEqual(parsedResult, "::12EF");
        }

        [TestMethod]
        public void Test_IPv613()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser9.Parse("12EF:89EF:12EF:89EF:12EF:89EF:12EF::");
            Assert.AreEqual(parsedResult, "12EF:89EF:12EF:89EF:12EF:89EF:12EF::");
        }

        [TestMethod]
        public void Test_IPv614()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Parser9.Parse("::");
            Assert.AreEqual(parsedResult, "::");
        }

        [TestMethod]
        public void Test_IPv6Combinator()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Address.Parse("01AB:23CD:45AB:67DE:89EF:12EF:89EF:12EF");
            Assert.AreEqual(parsedResult, "01AB:23CD:45AB:67DE:89EF:12EF:89EF:12EF");
        }

        [TestMethod]
        public void Test_IPv6Combinator1()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Address.Parse("::45AB:67DE:89EF:12EF:89EF:12EF:12EF");
            Assert.AreEqual(parsedResult, "::45AB:67DE:89EF:12EF:89EF:12EF:12EF");
        }

        [TestMethod]
        public void Test_IPv6Combinator2()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Address.Parse("01AB::45AB:67DE:89EF:12EF:89EF:12EF");
            Assert.AreEqual(parsedResult, "01AB::45AB:67DE:89EF:12EF:89EF:12EF");
        }

        [TestMethod]
        public void Test_Combinator3()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Address.Parse("::45AB:67DE:89EF:12EF:89EF");
            Assert.AreEqual(parsedResult, "::45AB:67DE:89EF:12EF:89EF");
        }

        [TestMethod]
        public void Test_IPv6Combinator4()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Address.Parse("45AB:45AB::45AB:67DE:89EF:12EF:89EF");
            Assert.AreEqual(parsedResult, "45AB:45AB::45AB:67DE:89EF:12EF:89EF");
        }

        [TestMethod]
        public void Test_IPv6Combinator5()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Address.Parse("::45AB:67DE:89EF:12EF:89EF");
            Assert.AreEqual(parsedResult, "::45AB:67DE:89EF:12EF:89EF");
        }

        [TestMethod]
        public void Test_IPv6Combinator6()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Address.Parse("45AB:45AB:89EF::12EF:89EF:12EF:89EF");
            Assert.AreEqual(parsedResult, "45AB:45AB:89EF::12EF:89EF:12EF:89EF");
        }

        [TestMethod]
        public void Test_IPv6Combinator7()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Address.Parse("45AB:45AB:89EF:12EF::12EF:89EF:12EF");
            Assert.AreEqual(parsedResult, "45AB:45AB:89EF:12EF::12EF:89EF:12EF");
        }

        [TestMethod]
        public void Test_IPv6Combinator8()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Address.Parse("::12EF:89EF:12EF");
            Assert.AreEqual(parsedResult, "::12EF:89EF:12EF");
        }

        [TestMethod]
        public void Test_IPv6Combinator9()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Address.Parse("12EF:89EF:12EF:89EF:12EF::12EF:89EF");
            Assert.AreEqual(parsedResult, "12EF:89EF:12EF:89EF:12EF::12EF:89EF");
        }

        [TestMethod]
        public void Test_IPv6Combinator10()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Address.Parse("::12EF:89EF");
            Assert.AreEqual(parsedResult, "::12EF:89EF");
        }

        [TestMethod]
        public void Test_IPv6Combinator11()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Address.Parse("12EF:89EF:12EF:89EF:12EF:89EF::12EF");
            Assert.AreEqual(parsedResult, "12EF:89EF:12EF:89EF:12EF:89EF::12EF");
        }

        [TestMethod]
        public void Test_IPv6Combinator12()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Address.Parse("::12EF");
            Assert.AreEqual(parsedResult, "::12EF");
        }

        [TestMethod]
        public void Test_IPv6Combinator13()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Address.Parse("12EF:89EF:12EF:89EF:12EF:ABEF:DCEF::");
            Assert.AreEqual(parsedResult, "12EF:89EF:12EF:89EF:12EF:ABEF:DCEF::");
        }

        [TestMethod]
        public void Test_IPv6Combinator14()
        {
            var parsedResult = UriParser.UriGrammar.IPv6Address.Parse("::");
            Assert.AreEqual(parsedResult, "::");
        }


        #endregion

        #region IPvFuture tests

        [TestMethod]
        public void Test_IPvFuture1()
        {
            var parsedResult = UriParser.UriGrammar.IPvFuture.Parse("vB.~");
            Assert.AreEqual(parsedResult, "vB.~");
        }

        [TestMethod]
        public void Test_IPvFuture2()
        {
            var parsedResult = UriParser.UriGrammar.IPvFuture.Parse("vB.!");
            Assert.AreEqual(parsedResult, "vB.!");
        }

        [TestMethod]
        public void Test_IPvFuture3()
        {
            var parsedResult = UriParser.UriGrammar.IPvFuture.Parse("vB.:");
            Assert.AreEqual(parsedResult, "vB.:");
        }

        #endregion

        #region ipliteral tests
        [TestMethod]
        public void Test_IpLiteral()
        {
            var parsedresult = UriGrammar.IPLiteral.Parse("[vB.!]");
            Assert.AreEqual(parsedresult, "[vB.!]");
        }

        [TestMethod]
        public void Test_IpLiteral2()
        {
            var parsedresult = UriGrammar.IPLiteral.Parse("[01AB:23CD:45AB:67DE:89EF:12EF:89EF:12AB]");
            Assert.AreEqual(parsedresult, "[01AB:23CD:45AB:67DE:89EF:12EF:89EF:12AB]");
        }


        #endregion

        #region scheme/authority/userinfo/host/port tests

        [TestMethod]
        public void Test_Port()
        {
            var parsedResults = UriGrammar.port.Parse("1234");
            Assert.AreEqual(parsedResults, "1234");
        }

        [TestMethod]
        public void Test_Port1()
        {
            var parsedResults = UriGrammar.port.Parse("1");
            Assert.AreEqual(parsedResults, "1");
        }

        [TestMethod]
        public void Test_Port2()
        {
            var parsedResults = UriGrammar.port.Parse("");
            Assert.AreEqual(parsedResults, "");
        }

        [TestMethod]
        public void Test_host()
        {
            var parsedResults = UriGrammar.host.Parse("[vB.!]");
            Assert.AreEqual(parsedResults, "[vB.!]");
        }

        [TestMethod]
        public void Test_host1()
        {
            var parsedResults = UriGrammar.host.Parse("[01AB:23CD:45AB:67DE:89EF:12EF:89EF:12AB]");
            Assert.AreEqual(parsedResults, "[01AB:23CD:45AB:67DE:89EF:12EF:89EF:12AB]");
        }

        [TestMethod]
        public void Test_host2()
        {
            var parsedResults = UriGrammar.host.Parse("192.168.255.255");
            Assert.AreEqual(parsedResults, "192.168.255.255");
        }

        [TestMethod]
        public void Test_host3()
        {
            var parsedResults = UriGrammar.host.Parse("aB01-._~%0E!$");
            Assert.AreEqual(parsedResults, "aB01-._~%0E!$");
        }

        [TestMethod]
        public void Test_userinfo()
        {
            var parsedResults = UriGrammar.userinfo.Parse("~~~~~~");
            Assert.AreEqual(parsedResults, "~~~~~~");
        }

        [TestMethod]
        public void Test_userinfo1()
        {
            var parsedResults = UriGrammar.userinfo.Parse("%0F%0F%0F");
            Assert.AreEqual(parsedResults, "%0F%0F%0F");
        }

        [TestMethod]
        public void Test_userinfo2()
        {
            var parsedResults = UriGrammar.userinfo.Parse("%0F%0F%0F");
            Assert.AreEqual(parsedResults, "%0F%0F%0F");
        }

        [TestMethod]
        public void Test_authority()
        {
            var parsedResults = UriGrammar.authority.Parse("john.doe@www.example.com:123");
            Assert.AreEqual(parsedResults, "john.doe@www.example.com:123");
        }

        [TestMethod]
        public void Test_authority1()
        {
            var parsedResults = UriGrammar.authority.Parse("www.example.com");
            Assert.AreEqual(parsedResults, "www.example.com");
        }

        [TestMethod]
        public void Test_authority2()
        {
            var parsedResults = UriGrammar.authority.Parse("www.example.com:123");
            Assert.AreEqual(parsedResults, "www.example.com:123");
        }

        [TestMethod]
        public void Test_authority3()
        {
            var parsedResults = UriGrammar.authority.Parse("john.doe@www.example.com");
            Assert.AreEqual(parsedResults, "john.doe@www.example.com");
        }

        [TestMethod]
        public void Test_scheme()
        {
            var parsedResults = UriGrammar.scheme.Parse("https");
            Assert.AreEqual(parsedResults, "https");
        }

        #endregion

        #region absoluteURI/relativeRef/relativePart tests

        [TestMethod]
        public void relativePart_Test()
        {
            var parsedResult = UriGrammar.relativePart.Parse("//john.doe@www.example.com:123/aB01/aB01");
            Assert.AreEqual(parsedResult, "//john.doe@www.example.com:123/aB01/aB01");
        }

        [TestMethod]
        public void relativePart_Test1()
        {
            var parsedResult = UriGrammar.relativePart.Parse("//www.example.com");
            Assert.AreEqual(parsedResult, "//www.example.com");
        }

        [TestMethod]
        public void relativePart_Test3()
        {
            var parsedResult = UriGrammar.relativePart.Parse("/aB_~%2D:@/aB01/aB01");
            Assert.AreEqual(parsedResult, "/aB_~%2D:@/aB01/aB01");
        }

        [TestMethod]
        public void relativePart_Test4()
        {
            var parsedResult = UriGrammar.relativePart.Parse("%0E/aB01/aB01");
            Assert.AreEqual(parsedResult, "%0E/aB01/aB01");
        }

        [TestMethod]
        public void relativePart_Test5()
        {
            var parsedResult = UriGrammar.relativePart.Parse("aB01-._~%2D!$&'()*+,;=:@");
            Assert.AreEqual(parsedResult, "aB01-._~%2D!$&'()*+,;=:@");
        }

        [TestMethod]
        public void relativeRef_Test()
        {
            var parsedResult = UriGrammar.relativeRef.Parse("//john.doe@www.example.com:123/aB01/aB01");
            Assert.AreEqual(parsedResult, "//john.doe@www.example.com:123/aB01/aB01");
        }

        [TestMethod]
        public void relativeRef_Test1()
        {
            var parsedResult = UriGrammar.relativeRef.Parse("//john.doe@www.example.com:123/aB01/aB01?%0A%0A//??");
            Assert.AreEqual(parsedResult, "//john.doe@www.example.com:123/aB01/aB01?%0A%0A//??");
        }

        [TestMethod]
        public void relativeRef_Test2()
        {
            var parsedResult = UriGrammar.relativeRef.Parse("//john.doe@www.example.com:123/aB01/aB01?%0A%0A//??#%0A%0A??");
            Assert.AreEqual(parsedResult, "//john.doe@www.example.com:123/aB01/aB01?%0A%0A//??#%0A%0A??");
        }

        [TestMethod]
        public void relativeRef_Test3()
        {
            var parsedResult = UriGrammar.relativeRef.Parse("//john.doe@www.example.com:123/aB01/aB01#%0A%0A??");
            Assert.AreEqual(parsedResult, "//john.doe@www.example.com:123/aB01/aB01#%0A%0A??");
        }

        [TestMethod]
        public void absoluteURI_Test()
        {
            var parsedResult = UriGrammar.absoluteURI.Parse("https://john.doe@www.example.com:123/aB01/aB01?%0A%0A//??");
            Assert.AreEqual(parsedResult, "https://john.doe@www.example.com:123/aB01/aB01?%0A%0A//??");
        }

        [TestMethod]
        public void absoluteURI_Test1()
        {
            var parsedResult = UriGrammar.absoluteURI.Parse("https://john.doe@www.example.com:123/aB01/aB01");
            Assert.AreEqual(parsedResult, "https://john.doe@www.example.com:123/aB01/aB01");
        }

        [TestMethod]
        public void absoluteURI_Test2()
        {
            var parsedResult = UriGrammar.absoluteURI.Parse("https:aB_~%2D:@/aB01/aB01");
            Assert.AreEqual(parsedResult, "https:aB_~%2D:@/aB01/aB01");
        }

        #endregion

        #region URI/hier-part/URI-refernece tests

        [TestMethod]
        public void hierPart_Test()
        {
            var parsedResult = UriGrammar.hierPart.Parse("//john.doe@www.example.com:123/aB01/aB01");
            Assert.AreEqual(parsedResult, "//john.doe@www.example.com:123/aB01/aB01");
        }

        [TestMethod]
        public void hierPart_Test1()
        {
            var parsedResult = UriGrammar.hierPart.Parse("aB_~%2D:@/aB01/aB01");
            Assert.AreEqual(parsedResult, "aB_~%2D:@/aB01/aB01");
        }

        [TestMethod]
        public void hierPart_Test2()
        {
            var parsedResult = UriGrammar.hierPart.Parse("/aB_~%2D:@/aB01/aB01");
            Assert.AreEqual(parsedResult, "/aB_~%2D:@/aB01/aB01");
        }

        [TestMethod]
        public void hierPart_Test3()
        {
            var parsedResult = UriGrammar.hierPart.Parse("aB01-._~%2D!$&'()*+,;=:@");
            Assert.AreEqual(parsedResult, "aB01-._~%2D!$&'()*+,;=:@");
        }

        [TestMethod]
        public void URI_Test()
        {
            var parsedResult = UriGrammar.URI.Parse("https://john.doe@www.example.com:123/aB01/aB01?%0A%0A//??");
            Assert.AreEqual(parsedResult, "https://john.doe@www.example.com:123/aB01/aB01?%0A%0A//??");
        }

        [TestMethod]
        public void URI_Test1()
        {
            var parsedResult = UriGrammar.URI.Parse("https://john.doe@www.example.com:123/aB01/aB01?%0A%0A//??#%0A%0A??");
            Assert.AreEqual(parsedResult, "https://john.doe@www.example.com:123/aB01/aB01?%0A%0A//??#%0A%0A??");
        }

        [TestMethod]
        public void URI_Test2()
        {
            var parsedResult = UriGrammar.URI.Parse("https://john.doe@www.example.com:123/aB01/aB01#%0A%0A??");
            Assert.AreEqual(parsedResult, "https://john.doe@www.example.com:123/aB01/aB01#%0A%0A??");
        }

        [TestMethod]
        public void URI_Test3()
        {
            var parsedResult = UriGrammar.URI.Parse("https://john.doe@www.example.com:123/aB01/aB01");
            Assert.AreEqual(parsedResult, "https://john.doe@www.example.com:123/aB01/aB01");
        }

        [TestMethod]
        public void URIreference_Test()
        {
            var parsedResult = UriGrammar.URIreference.Parse("https://john.doe@www.example.com:123/aB01/aB01");
            Assert.AreEqual(parsedResult, "https://john.doe@www.example.com:123/aB01/aB01");
        }

        [TestMethod]
        public void URIreference_Test1()
        {
            var parsedResult = UriGrammar.URIreference.Parse("//john.doe@www.example.com:123/aB01/aB01?%0A%0A//??#%0A%0A??");
            Assert.AreEqual(parsedResult, "//john.doe@www.example.com:123/aB01/aB01?%0A%0A//??#%0A%0A??");
        }

        #endregion

    }
}
