using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using LanguageExt;
using LanguageExt.SomeHelp;
using LanguageExt.Parsec;
using Sprache;
using SpracheExtensionsLibrary;
using Char = System.Char;

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

        #region Subdelims/GenDelims/Reserved/ALPHA/DIGIT/Unreserved

        public static readonly Sprache.Parser<char> SubDelims =
            // sub-delims -> ! | $ | & | ' | ( | ) | * | + | , | ; | =
            // delimiter terminals
            Parse.Chars("!$&'()*+,;=");


        public static readonly Sprache.Parser<char> GenDelims =
            //gen-delims -> : | / | ? | # | [ | ] | @
            // delimiter terminals
            Parse.Chars(":/?#[]@");

        public static readonly Sprache.Parser<char> Reserved =
            // reserved -> sub-delim | gen-delim
            // parse reserved chars
            SubDelims.Or(GenDelims);

        public static readonly Sprache.Parser<char> Alpha =
            // parse Chars as defined in ALPHA (rfc2234) 
            Parse.Chars("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");

        public static readonly Sprache.Parser<char> Digit =
            // parse Digits as defined in DIGIT (rfc2234)
            Parse.Chars("0123456789");

        public static readonly Sprache.Parser<char> Unreserved =
            // unreserved -> ALPHA | DIGIT | - | . | _ | ~
            // parse the unreserved characters
            Alpha
                .Or(Digit)
                .Or(Parse.Chars("-._~"));

        #endregion

        #region HexDigit/PctChar/PctEncoded/PChar

        public static readonly Sprache.Parser<char> HexDigit =
            //parse Hexadecimal digits as defined in HEXDIG (rfc2234)
            // TODO: Rewrite to correctly parse with 0x leading hex nums.
            /*Parse.IgnoreCase("0x")
                .Then(x => Parse.Chars("0123456789abcdefABCDEF"));*/
            Parse.Chars("0123456789abcdefABCDEF");


        public static readonly Sprache.Parser<char> PctChar =
            // helper Sprache.Parser for grabbing leading '%' in pct-encoded.
            Parse.Char('%');

        public static readonly Sprache.Parser<string> PctEncoded =
            // pct-encoded -> "%"HEXDIGHEXDIG
            from pct in PctChar.Once()
            from hexDigits in HexDigit.Once()
            from hexDigits2 in HexDigit.Once()
            select new string(pct.Concat(hexDigits).Concat(hexDigits2).ToArray());

        public static readonly Sprache.Parser<string> PChar =
            // pchar -> unreserved | pct-encoded | sub-delim | ":" | "@"
            Unreserved.AtLeastOnce().Text().Or(PctEncoded)
                .Or(SubDelims.AtLeastOnce().Text())
                .Or(Parse.String(":").Text())
                .Or(Parse.String("@").Text());

        #endregion

        #region Fragment/Query/Segment

        public static readonly Sprache.Parser<string> Fragment =
            // TODO: Covers too much?
            // fragment -> *(pchar | "/" | "?" )
            from pchars in PChar.Many()
            from slash in Parse.Char('/').Many().Text()
            from qmark in Parse.Char('?').Many().Text()
            select new string(pchars.Fold("", (x, y) => x + y) + slash + qmark);

        public static readonly Sprache.Parser<string> Query =
            // query -> *(pchar | "/" | "?" )
            // fragment -> *(pchar | "/" | "?" )
            from pchars in PChar.Many()
            from slash in Parse.Char('/').Many().Text()
            from qmark in Parse.Char('?').Many().Text()
            select new string(pchars.Fold("", (x, y) => x + y) + slash + qmark);

        public static readonly Sprache.Parser<string> SegmentNzNc1 =
            // segment-nz-nc -> 1*( unreserved | pct-encoded | sub-delims | "@")
            Unreserved.AtLeastOnce().Text();

        public static readonly Sprache.Parser<string> SegmentNzNc2 =
            // segment-nz-nc -> 1*( unreserved | pct-encoded | sub-delims | "@")
            from pct in PctEncoded.AtLeastOnce()
            select new string(pct.Fold("", (x, y) => x + y));

        public static readonly Sprache.Parser<string> SegmentNzNc3 =
            // segment-nz-nc -> 1*( unreserved | pct-encoded | sub-delims | "@")
            from subdel in SubDelims.AtLeastOnce()
            select new string(subdel.Fold("", (x, y) => x + y));

        public static readonly Sprache.Parser<string> SegmentNzNc4 =
            // segment-nz-nc -> 1*( unreserved | pct-encoded | sub-delims | "@")
            from at in Parse.Char('@').AtLeastOnce()
            select new string(at.Fold("", (x, y) => x + y));

        public static readonly Sprache.Parser<string> SegmentNzNc =
            // segment-nz-nc -> 1*( unreserved | pct-encoded | sub-delims | "@")
            SegmentNzNc1
                .Or(SegmentNzNc2)
                .Or(SegmentNzNc3)
                .Or(SegmentNzNc4);

        public static readonly Sprache.Parser<string> SegmentNz =
            // segment-nz -> 1*(pchar)
            from pchar in PChar.AtLeastOnce()
            select new string(pchar.Fold("", (x, y) => x + y));

        public static readonly Sprache.Parser<string> Segment =
            // segement -> *pchar
            from pchar in PChar.Many()
            select new string(pchar.Fold("", (x, y) => x + y));

        #endregion

        #region PathEmpty/PathRootless/PathNoScheme/PathAbsolute/PathAbEmpty/Path/RegName

        public static readonly Sprache.Parser<string> PathEmpty =
            // TODO: This may not be correct.
            // path-empty -> 0<pchar>
            from not in PChar.Not()
            select new string("");

        public static readonly Sprache.Parser<string> PathRootless =
            // path-rootless -> segment-nz*("/"segment)
            from segnz in SegmentNz
            from path in Parse.Char('/').Then(x => Segment).Many()
            select new string(segnz.Fold("", (x, y) => x + y) +
                              path.Collect(x => x.Prepend('/'))
                                  .Fold("", (x, y) => x + y));

        public static readonly Sprache.Parser<string> PathNoScheme =
            // path-noscheme -> segment-nz-nc *("/"segment)
            from segnznc in SegmentNzNc
            from path in Parse.Char('/').Then(x => Segment).Many()
            select new string(segnznc.Fold("", (x, y) => x + y) +
                              path.Collect(x => x.Prepend('/'))
                                  .Fold("", (x, y) => x + y));

        public static readonly Sprache.Parser<string> PathAbsolute =
            // path-absolute -> "/"[segment-nz*("/"segment)]
            from slash in Parse.Char('/')
            from optseg in PathRootless.Optional()
            select new string(slash + optseg.GetOrElse(""));

        public static readonly Sprache.Parser<string> PathAbEmpty =
            // path-abempty -> *("/"segment)
            from path in Parse.Char('/').Then(x => Segment).Many()
            select new string(path.Collect(x => x.Prepend('/'))
                .Fold("", (x, y) => x + y));

        public static readonly Sprache.Parser<string> Path =
            // path -> path-abempty | path-absolute | path-noscheme | path-rootless | path-empty
            PathAbEmpty
                .Or(PathAbsolute)
                .Or(PathNoScheme)
                .Or(PathRootless)
                .Or(PathEmpty);

        public static readonly Sprache.Parser<string> RegName =
            // reg-name -> *(unreserved | pct-encoded | sub-delims)
            from unrev in Unreserved.Many()
            from pct in PctEncoded.Many()
            from subdel in SubDelims.Many()
            select new string(unrev.Fold("", (x, y) => x + y)
                              + pct.Fold("", (x, y) => x + y)
                              + subdel.Fold("", (x, y) => x + y));

        #endregion

        #region DecOctets

        public static readonly Sprache.Parser<string> DecOctet1 =
            from dig in Digit
            select new string(dig.ToString());

        public static readonly Sprache.Parser<string> DecOctet2 =
            from digits in Parse.AnyChar.ParseIPV4()
            select new string(digits.Fold("", (x, y) => x + y));

        public static readonly Sprache.Parser<string> DecOctet3 =
            from hundreds in Parse.Char('1')
            from tens in Digit
            from ones in Digit
            select new string(hundreds.ToString() + tens.ToString() + ones.ToString());

        public static readonly Sprache.Parser<string> DecOctet4 =
            from hundreds2 in Parse.Char('2')
            from tens in Parse.Chars("01234")
            from ones in Digit
            select new string(hundreds2.ToString() + tens + ones);

        public static readonly Sprache.Parser<string> DecOctet5 =
            from hund25 in Parse.String("25")
            from ones in Parse.Chars("012345")
            select new string(hund25.Fold("", (x, y) => x + y) + ones.ToString());

        public static readonly Sprache.Parser<string> DecOctet =
            //dec-octect -> DIGIT | %x31-39 DIGIT | "1" 2DIGIT | "2" %x30-34 DIGIT | "25" %x30-35
            DecOctet5
                .Or(DecOctet4)
                .Or(DecOctet3)
                .Or(DecOctet2)
                .Or(DecOctet1);


        #endregion

        #region IPv4Address

        public static readonly Sprache.Parser<string> IPv4Address =
            // IPv4Address -> decoctet "." decoctet "." decoctect "." decoctet
            from ipv4Address in Parse.AnyChar.ParseIPV4()
            select (ipv4Address.Fold("", (x, y) => x + y));

        #endregion

        #region IPv6Parsers (partials)

        /*
         * A series of partial parsers for IPv6 rules followed by the Parser Combinator IPv6Address
         */

        public static readonly Sprache.Parser<string> h16 =
            //h16 -> 1*4HEXDIG
            from hex1 in HexDigit
            from hex2 in HexDigit
            from hex3 in HexDigit
            from hex4 in HexDigit
            select new string(hex1.ToString() + hex2.ToString() + hex3.ToString() + hex4.ToString());

        public static readonly Sprache.Parser<string> ls32 =
            // ls32 -> (h16 ":" h16) | IPv4Address
            (from h1 in h16
                from colon in Parse.Char(':')
                from h2 in h16
                select new string(h1 + colon + h2)).Or(IPv4Address);

        public static Sprache.Parser<string> H16ColonParser =
            (from h16 in h16
                from colon in Parse.Char(':')
                select new string(h16.Fold("", (x, y) => (x + y)) + colon));

        public static Sprache.Parser<string> IPv6Parser1 =
            from h16 in Parse.AnyChar.ParseIPV6Rule1()
            from ls32 in Parse.AnyChar.ParseIPV4().Optional()
            select new string(h16.Fold("", (x, y) => (x + y))
                              + ls32.GetOrElse("").Fold("", (x, y) => (x + y)));

        public static Sprache.Parser<string> IPv6Parser2 =
            from h16 in Parse.AnyChar.ParseIPV6Rule2()
            from ls32 in Parse.AnyChar.ParseIPV4().Optional()
            select new string(h16.Fold("", (x, y) => (x + y))
                              + ls32.GetOrElse("").Fold("", (x, y) => (x + y)));

        public static Sprache.Parser<string> IPv6Parser3 =
            from h16 in Parse.AnyChar.ParseIPV6Rule3()
            from ls32 in Parse.AnyChar.ParseIPV4().Optional()
            select new string(h16.Fold("", (x, y) => (x + y))
                              + ls32.GetOrElse("").Fold("", (x, y) => (x + y)));

        public static Sprache.Parser<string> IPv6Parser4 =
            from h16 in Parse.AnyChar.ParseIPV6Rule4()
            from ls32 in Parse.AnyChar.ParseIPV4().Optional()
            select new string(h16.Fold("", (x, y) => (x + y))
                              + ls32.GetOrElse("").Fold("", (x, y) => (x + y)));

        public static Sprache.Parser<string> IPv6Parser5 =
            from h16 in Parse.AnyChar.ParseIPV6Rule5()
            from ls32 in Parse.AnyChar.ParseIPV4().Optional()
            select new string(h16.Fold("", (x, y) => (x + y))
                              + ls32.GetOrElse("").Fold("", (x, y) => (x + y)));

        public static Sprache.Parser<string> IPv6Parser6 =
            from h16 in Parse.AnyChar.ParseIPV6Rule6()
            from ls32 in Parse.AnyChar.ParseIPV4().Optional()
            select new string(h16.Fold("", (x, y) => (x + y))
                              + ls32.GetOrElse("").Fold("", (x, y) => (x + y)));

        public static Sprache.Parser<string> IPv6Parser7 =
            from h16 in Parse.AnyChar.ParseIPV6Rule7()
            from ls32 in Parse.AnyChar.ParseIPV4().Optional()
            select new string(h16.Fold("", (x, y) => (x + y))
                              + ls32.GetOrElse("").Fold("", (x, y) => (x + y)));

        public static Sprache.Parser<string> IPv6Parser8 =
            from h16 in Parse.AnyChar.ParseIPV6Rule8()
            select new string(h16.Fold("", (x, y) => (x + y)));

        public static Sprache.Parser<string> IPv6Parser9 =
            from h16 in Parse.AnyChar.ParseIPV6Rule9()
            select new string(h16.Fold("", (x, y) => (x + y)));


        #endregion

        #region IPv6/IPvFuture/IPLiteral

        public static Sprache.Parser<string> IPv6Address =

            /* IPv6Address -> 6(h16 ":" )ls32
             *             | "::" 5(h16 ":")ls32
             *             | [ h16] "::" 4(h16 ":")ls32
             *             | [ *1( h16 ":" ) h16 ] "::" 3( h16 ":" ) ls32
             *             | [ *2( h16 ":" ) h16 ] "::" 2( h16 ":" ) ls32
             *             | [ *3( h16 ":" ) h16 ] "::"    h16 ":"   ls32
             *             | [ *4( h16 ":" ) h16 ] "::"              ls32
             *             | [ *5( h16 ":" ) h16 ] "::"              h16
             *             | [ *6( h16 ":" ) h16 ] "::"
             */

            IPv6Parser1
                .Or(IPv6Parser2)
                .Or(IPv6Parser3)
                .Or(IPv6Parser4)
                .Or(IPv6Parser5)
                .Or(IPv6Parser6)
                .Or(IPv6Parser7)
                .Or(IPv6Parser8)
                .Or(IPv6Parser9);

        public static Sprache.Parser<string> IPvFuture =
            // IPvFuture -> "v" 1*HEXDIG "." 1*( unreserved / sub-delims / ":" )

            from vee in Parse.Char('v')
            from hex in HexDigit.AtLeastOnce()
            from dot in Parse.Char('.')
            from symbol in (Unreserved.AtLeastOnce().Or(SubDelims.AtLeastOnce()).Or(Parse.Char(':').AtLeastOnce()))
            select new string(vee + hex.Fold("", (x, y) => x + y) + dot 
                              + symbol.Fold("", (x, y) => x + y));

        public static Sprache.Parser<string> IPLiteral =

            // IP-literal -> "[" ( IPv6address / IPvFuture  ) "]"
            from lBracket in Parse.Char('[')
            from ip in IPv6Address.Or(IPvFuture)
            from rBracket in Parse.Char(']')
            select new string(lBracket + ip.Fold("", (x, y) => x + y) + rBracket);

        #endregion

        #region scheme/authority/userinfo/host/port

        public static Sprache.Parser<string> port =

            // port -> *DIGIT

            from digit in Digit.Many()
            select new string(digit.Fold("", (x, y) => x + y));


        public static Sprache.Parser<string> host =

            // host -> IPLiteral | IPv4Address | reg-name

            IPLiteral
                .Or(IPv4Address)
                .Or(RegName);

        public static Sprache.Parser<string> userinfo =

            // userinfo -> *( unreserved | pct-encoded | sub-delims | ":" )

            from unreserved in Unreserved.Many()
            from pctenc in PctEncoded.Many()
            from subdel in SubDelims.Many()
            from colon in Parse.Char(':').Many()
            select new string(unreserved.Fold("", (x, y) => x + y) 
                              + pctenc.Fold("", (x, y) => x + y)
                              + subdel.Fold("", (x, y) => x + y) 
                              + colon.Fold("", (x, y) => x + y));

        public static Sprache.Parser<string> authority =

            // authority -> [ userinfo "@" ] host [ ":" port ]

            from user in userinfo.Optional()
            from at in Parse.String("@").Optional()
            from hst in host
            from colon in Parse.String(":").Optional()
            from prt in port.Optional()
            select new string(user.GetOrElse("").Fold("", (x, y) => x + y) 
                              + at.GetOrElse("").Fold("", (x, y) => x + y)
                              + hst.Fold("", (x, y) => x + y)
                              + colon.GetOrElse("").Fold("", (x, y) => x + y)
                              + prt.GetOrElse("").Fold("", (x, y) => x + y));

        public static Sprache.Parser<string> scheme =

            // scheme -> ALPHA *( ALPHA | DIGIT | "+" | "-" | "." )

            from alpha in Alpha.Once()
            from symbol in (Alpha.Many().Or(Digit.Many()).Or(Parse.Char('+').Many())
                .Or(Parse.Char('-').Many()).Or(Parse.Char('.').Many()))
            select new string(alpha.Fold("", (x, y) => x + y) + symbol.Fold("", (x, y) => x + y));

        #endregion

        #region absolute-URI/relative-ref/relative-part

        public static Sprache.Parser<string> relativePartRule1 =

            // relative-part -> "//" authority path-abempt 

            from forwardSlash in Parse.Char('/').Twice()
            from auth in authority
            from pathabempt in PathAbEmpty
            select new string(forwardSlash.Fold("", (x, y) => x + y) + auth + pathabempt);

        public static Sprache.Parser<string> relativePart =

            /* relative-part -> "//" authority path-abempt
             *
             *                  | path - absolute
             *                  | path - noscheme
             *                  | path - empty
             * */

            relativePartRule1
                .Or(PathAbsolute)
                .Or(PathNoScheme)
                .Or(PathEmpty);

        public static Sprache.Parser<string> relativeRef =

            // relative-ref -> relative-part [ "?" query ] [ "#" fragment ]

            from relPart in relativePart
            from qmark in Parse.String("?").Optional()
            from query in Query.Optional()
            from hash in Parse.String("#").Optional()
            from frag in Fragment.Optional()
            select new string(relPart
                              + qmark.GetOrElse("").Fold("", (x, y) => x + y)
                              + query.GetOrElse("").Fold("", (x, y) => x + y)
                              + hash.GetOrElse("").Fold("", (x, y) => x + y)
                              + frag.GetOrElse("").Fold("", (x, y) => x + y));

        public static Sprache.Parser<string> absoluteURI =

            // absolute-URI -> scheme ":" hier-part [ "?" query ]

            from scheme in scheme
            from colon in Parse.Char(':')
            from hierp in hierPart
            from qmark in Parse.String("?").Optional()
            from query in Query.Optional()
            select new string(scheme
                              + colon
                              + hierp
                              + qmark.GetOrElse("").Fold("", (x, y) => x + y)
                              + query.GetOrElse("").Fold("", (x, y) => x + y));

        #endregion

        #region URI/hier-part/URI-reference

        public static Sprache.Parser<string> URI =

            // URI -> scheme ":" hier-part [ "?" query ] [ "#" fragment ]

            from scheme in scheme
            from colon in Parse.Char(':')
            from hierp in hierPart
            from qmark in Parse.String("?").Optional()
            from query in Query.Optional()
            from hash in Parse.String("#").Optional()
            from frag in Fragment.Optional()
            select new string(scheme 
                              + colon
                              + hierp 
                              + qmark.GetOrElse("").Fold("", (x, y) => x + y)
                              + query.GetOrElse("").Fold("", (x, y) => x + y)
                              + hash.GetOrElse("").Fold("", (x, y) => x + y)
                              + frag.GetOrElse("").Fold("", (x, y) => x + y));

        public static Sprache.Parser<string> hierPartRule1 =
            // hier-part -> "//" authority path-abempty

            from forwardSlash in Parse.Char('/').Twice()
            from auth in authority
            from pathabempt in PathAbEmpty
            select new string(forwardSlash.Fold("", (x, y) => x + y) + auth + pathabempt);

        public static Sprache.Parser<string> hierPart =
            /*
             * hier-part -> "//" authority path-abempty
             *
             *   | path-absolute
             *   | path-rootless
             *   | path-empty
             *
             * */

            hierPartRule1
                .Or(PathAbsolute)
                .Or(PathRootless)
                .Or(PathEmpty);

        public static Sprache.Parser<string> URIreference =
            // URI-reference -> URI | relative-ref

            URI
                .Or(relativeRef);

        #endregion








    }


}

