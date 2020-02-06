using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;
using Sprache;

namespace UriParser
{
    public class UriGrammar
    {
        /*
         * A class to implement the grammar of URIs in their BNF form given in rfc3986.
         * 
         * (ABNF description the URI grammar is here : https://tools.ietf.org/html/rfc3986#page-49)
         *
         */

        public static readonly Parser<char> SubDelims =
            // sub-delims -> ! | $ | & | ' | ( | ) | * | + | , | ; | =
            // delimiter terminals
            Parse.Chars("!$&'()*+,;=");

        public static readonly Parser<char> GenDelims =
            //gen-delims -> : | / | ? | # | [ | ] | @
            // delimiter terminals
            Parse.Chars(":/?#[]@");

        public static readonly Parser<char> Reserved =
            // reserved -> sub-delim | gen-delim
            // parse reserved chars
            SubDelims.Or(GenDelims);

        public static readonly Parser<char> Alpha =
            // parse Chars as defined in ALPHA (rfc2234) 
            Parse.Chars("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");

        public static readonly Parser<char> Digit =
            // parse Digits as defined in DIGIT (rfc2234)
            Parse.Chars("0123456789");

        public static readonly Parser<char> Unreserved =
            // unreserved -> ALPHA | DIGIT | - | . | _ | ~
            // parse the unreserved characters
            Alpha
                .Or(Digit)
                .Or(Parse.Chars("-._~"));

        public static readonly Parser<char> HexDigit =
            //parse Hexadecimal digits as defined in HEXDIG (rfc2234)
            // TODO: Find out if the '0x' leading characters are needed!
            // TODO: the .IgnoreCase() call keeps this parser from consuming subsequent characters 
            /*Parse.IgnoreCase("0x")
                .Then(x => Parse.Chars("0123456789abcdefABCDEF"));*/
            Parse.Chars("0123456789abcdefABCDEF");


        public static readonly Parser<char> PctChar =
            // helper parser for grabbing leading '%' in pct-encoded.
            Parse.Char('%');

        public static readonly Parser<string> PctEncoded =
            // pct-encoded -> "%"HEXDIGHEXDIG
            from pct in PctChar.Once()
            from hexDigits in HexDigit.Once()
            from hexDigits2 in HexDigit.Once()
            select new string(pct.Concat(hexDigits).Concat(hexDigits2).ToArray());

        public static readonly Parser<string> PChar =
            // pchar -> unreserved | pct-encoded | ":" | "@"
           Unreserved.AtLeastOnce().Text().Or(PctEncoded)
                .Or(SubDelims.AtLeastOnce().Text())
                .Or(Parse.String(":").Text())
                .Or(Parse.String("@").Text());

        public static readonly Parser<string> Fragment =
            // TODO: does this parse zero or many pchars?
            // TODO: enlist Dejan's help!
            // fragment -> *(pchar | "/" | "?" )
            PChar
                .Or(Parse.Char('/').Many().Text())
                .Or(Parse.Char('?').Many().Text());

        public static readonly Parser<string> Query =
            // query -> *(pchar | "/" | "?" )
            PChar
                .Or(Parse.Char('/').Many().Text())
                .Or(Parse.Char('?').Many().Text());

        public static readonly Parser<string> SegmentNzNc =
            // segment-nz-nc -> 1*( unreserved | pct-encoded | sub-delims | "@"
            Unreserved.AtLeastOnce().Text()
                .Or(PctEncoded)
                .Or(SubDelims.AtLeastOnce().Text())
                .Or(Parse.Char('@').AtLeastOnce().Text());

        public static readonly Parser<string> SegmentNz =
            // segment-nz -> 1*(pchar)
            from pchar in PChar.AtLeastOnce()
            select new string(pchar.ToString());

        public static readonly Parser<string> Segment =
            // segement -> *pchar
            from pchar in PChar.Many()
            select new string(pchar.ToString());

        public static readonly Parser<string> PathEmpty =
            // path-empty -> 0<pchar>
            // this is always successful??
            Parse.String(String.Empty).Text();

        /* public static readonly Parser<string> PathRootless =
            // path-rootless -> segment-nz*("/"segment)
            from segnz in SegmentNz.Once()
            from path in (
                from slash in Parse.Char('/').Many().Text()
                from seg in Segment.Many().Text()
                select slash.Concat(seg))
            select new string() */
    }
    
    
}

