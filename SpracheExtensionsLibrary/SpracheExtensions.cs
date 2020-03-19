using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Sprache;
using LanguageExt;



namespace SpracheExtensionsLibrary
{
    public static class SpracheExtensions
    {
        public static Parser<IEnumerable<T>> Twice<T>(
            this Parser<T> parser)
        {
            return ParseN(parser, 2);
        }
        public static Parser<IEnumerable<T>> Thrice<T>(
            this Parser<T> parser)
        {
            return ParseN(parser, 3);
        }

        public static Parser<IEnumerable<T>> ParseN<T>(this Parser<T> parser, int n)
        {
            if (parser == null)
            {
                throw new ArgumentNullException(nameof(parser));
            }

            return i =>
            {
                var remainder = i; // remaining input stream
                var result = new List<T>(); // list of parsers
                var r = parser(i);
                var c = 0;
                while (c < n && r.WasSuccessful)
                {
                    if (remainder.Equals(r.Remainder))
                        break;

                    result.Add(r.Value);
                    remainder = r.Remainder;
                    r = parser(remainder);
                    c++;
                }

                /*if (!(r.WasSuccessful))
                {
                    return Result.Failure<IEnumerable<T>>(remainder, $"Parser failed on iteration {c}.", r.Expectations);
                }*/
          
                    return Result.Success<IEnumerable<T>>(result, remainder);
                

            };
        }

        public static Parser<IEnumerable<T>> ParseIPV4<T>(this Parser<T> parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException(nameof(parser));
            }

            return i =>
            {
                var remainder = i;
                var length = i.Source.Length;
                var result = new List<T>();
                var r = parser(i);
                var dots = 0;
                var consumed = string.Empty;
                var pos = i.Position;
                var illegal = new[] {'6', '7', '8', '9' };
                
                while (dots < 3 || pos <= length) // TODO: This may not break correctly if there are characters left in the input stream!
                {
                    try
                    {
                        if (pos == length || i.Source[pos] == '.')
                        {
                            // validate current octet
                            // TODO: This condition will never trigger if I don't clear the consumed characters after every octet
                            if (consumed.Length.Equals(3) && consumed.Substring(0, 2).Equals("25")
                                    && illegal.Contains(consumed[2]))
                            {
                                throw new Sprache.ParseException("Invalid IPv4 (illegal trailing digit in three digit octet)!");
                            }

                            if (consumed.Length.Equals(2) && consumed[0] == '0')
                            {
                                throw new Sprache.ParseException("Invalid IPv4 (leading zero in two digit octet)");
                            }

                            consumed = string.Empty;
                            dots++;
                            pos++;
                        }
                        else
                        {
                            if (dots != 3 && i.Source[pos + 1] == '.')
                            {   // look ahead to handle dots in input stream
                                result.Add(r.Value);
                                consumed += i.Source[pos]; // add parsed char to current octet
                                remainder = r.Remainder;
                                r = parser(remainder);
                                result.Add(r.Value);
                                r = parser(remainder.Advance());
                                pos++;

                            }
                            else
                            {
                                result.Add(r.Value);
                                consumed += i.Source[pos]; // add parsed char to current octet
                                remainder = r.Remainder;
                                r = parser(remainder); // parse the remaining input stream
                                pos++;

                            }

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return Result.Failure<IEnumerable<T>>(remainder,"Ipv4Failed", r.Expectations);
                    }
                   
                }

                return Result.Success<IEnumerable<T>>(result, remainder);
            };
        }

       public static Parser<IEnumerable<T>> ParseIPV6Rule1<T>(this Parser<T> parser)
         {
             if (parser == null)
             {
                 throw new ArgumentNullException();
             }

             return i =>
             {
                 var remainder = i;
                 var result = new List<T>();
                 var parsed = parser(i);
                 var consumed = string.Empty;
                 var pos = i.Position;
                 var length = i.Source.Length;
                 var legalDigits = new[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
                 var legalLetters = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F', ':'};


                 if (length.Equals(39) || length.Equals(41))
                 {
                     //IPv6Address -> 6(h16 ":" )ls32
                     // extra length accounts for l and r brackets in Ip literals

                     while (pos < length)
                     {

                         try
                         {
                             if (i.Source[0].Equals(':'))
                             {
                                 return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 1 Failed", 
                                     parsed.Expectations);
                             }

                             if (legalDigits.Contains(i.Source[pos]) || legalLetters.Contains(i.Source[pos]))
                             {
                                 consumed += i.Source[pos];
                                 result.Add(parsed.Value);
                                 remainder = parsed.Remainder;
                                 parsed = parser(remainder);
                                 pos++;
                             }
                             else
                             {
                                 // skip the bracket in ip literals
                                 pos++;
                             } 


                         }
                         catch (Exception e)
                         {
                             Console.WriteLine(e);

                             return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 1 Failed",
                                 parsed.Expectations); 
                         }


                     }

                 }
                 else if (length.Equals(45) || length.Equals(47))
                 { // parse the ls32 if it isn't an h16
                     // extra length accounts for l and r brackets in Ip literals

                     while (pos < 30 || pos < 32)
                     {

                         try
                         {
                             if (i.Source[0].Equals(':'))
                             {
                                 return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 1 Failed",
                                     parsed.Expectations);
                             }

                             if (legalDigits.Contains(i.Source[pos]) || legalLetters.Contains(i.Source[pos]))
                             {
                                 if (i.Source[pos].Equals(i.Source[pos + 1]) || i.Source.EndsWith(':'))
                                 {
                                     return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 1 Failed",
                                         parsed.Expectations);
                                 }
                                 consumed += i.Source[pos];
                                 result.Add(parsed.Value);
                                 remainder = parsed.Remainder;
                                 parsed = parser(remainder);
                                 pos++;
                             }
                             else
                             {
                                 // skip the bracket in ip literals
                                 pos++;
                             }


                         }
                         catch (Exception e)
                         {
                             Console.WriteLine(e);

                             return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 1 Failed",
                                 parsed.Expectations);
                         }


                     }

                 }
                 else
                 {
                     return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 1 Failed",
                         parsed.Expectations);
                 }

                 return Result.Success<IEnumerable<T>>(result, remainder);
             };

         }

        public static Parser<IEnumerable<T>> ParseIPV6Rule2<T>(this Parser<T> parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException();
            }

            return i =>
            {
                var remainder = i;
                var result = new List<T>();
                var parsed = parser(i);
                var consumed = string.Empty; //for debugging purposes
                var pos = i.Position;
                var length = i.Source.Length;
                var legalDigits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                var legalLetters = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F', ':' };

                if (length.Equals(36) || length.Equals(38))
                {
                    //IPv6Address -> "::" 5(h16 ":")ls32
                    // extra length to account for l and r brackets in ip literals

                    while (pos < length)
                    {

                        try
                        {
                            if (!(i.Source.Substring(0,2).Equals("::")))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 2 Failed",
                                    parsed.Expectations);
                            }

                            if (legalDigits.Contains(i.Source[pos]) || legalLetters.Contains(i.Source[pos]))
                            {
                                consumed += i.Source[pos];
                                result.Add(parsed.Value);
                                remainder = parsed.Remainder;
                                parsed = parser(remainder);
                                pos++;
                            }
                            else
                            {
                                // skip brackets in ip literals
                                pos++;
                            }


                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);

                            return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 2 Failed",
                                parsed.Expectations);
                        }


                    }

                }
                else if (length.Equals(42) || length.Equals(44))
                { // parse the ls32 if it isn't an h16

                    while (pos < 27 || pos < 29)
                    {

                        try
                        {
                            if (!(i.Source.Substring(0,2).Equals("::")) || i.Source.EndsWith(':'))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 2 Failed",
                                    parsed.Expectations);
                            }

                            if (legalDigits.Contains(i.Source[pos]) || legalLetters.Contains(i.Source[pos]))
                            {
                                consumed += i.Source[pos];
                                result.Add(parsed.Value);
                                remainder = parsed.Remainder;
                                parsed = parser(remainder);
                                pos++;
                            }
                            else
                            {
                                // skip brackets in ip literals
                                pos++;
                            }

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);

                            return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 2 Failed",
                                parsed.Expectations);
                        }


                    }

                }
                else
                {
                    return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 2 Failed",
                        parsed.Expectations);
                }

                return Result.Success<IEnumerable<T>>(result, remainder);
            };

        }

        public static Parser<IEnumerable<T>> ParseIPV6Rule3<T>(this Parser<T> parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException();
            }

            return i =>
            {
                var remainder = i;
                var result = new List<T>();
                var parsed = parser(i);
                var consumed = string.Empty;
                var pos = i.Position;
                var length = i.Source.Length;
                var legalDigits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                var legalLetters = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F', ':' };

                if (length.Equals(31) || length.Equals(35) || length.Equals(33) || 
                    (length.Equals(37) && !(i.Source.Contains('.'))))
                {
                    //IPv6Address -> [ h16] "::" 4(h16 ":")ls32

                    while (pos < length)
                    {

                        try
                        {
                            if (length.Equals(31) && !(i.Source.Substring(0, 2).Equals("::")))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 3 Failed",
                                    parsed.Expectations);
                            }

                            if (length.Equals(35) && !(i.Source.Substring(4, 2).Equals("::")))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 3 Failed",
                                    parsed.Expectations);
                            }

                            if (legalDigits.Contains(i.Source[pos]) || legalLetters.Contains(i.Source[pos]))
                            {
                                consumed += i.Source[pos];
                                result.Add(parsed.Value);
                                remainder = parsed.Remainder;
                                parsed = parser(remainder);
                                pos++;
                            }
                            else
                            {
                                pos++;
                            }


                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);

                            return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 3 Failed",
                                parsed.Expectations);
                        }


                    }

                }

                else if (length.Equals(37) || length.Equals(41) || length.Equals(39) || length.Equals(43))
                { // parse the ls32 if it isn't an h16

                    while (pos < length - 15)
                    {

                        try
                        {
                            if ((length.Equals(37) && !(i.Source.Substring(0, 2).Equals("::"))) || i.Source.EndsWith(':'))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 3 Failed",
                                    parsed.Expectations);
                            }

                            if ((length.Equals(41) && !(i.Source.Substring(4, 2).Equals("::"))) || i.Source.EndsWith(':'))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 3 Failed",
                                    parsed.Expectations);
                            }

                            if (legalDigits.Contains(i.Source[pos]) || legalLetters.Contains(i.Source[pos]))
                            {
                                consumed += i.Source[pos];
                                result.Add(parsed.Value);
                                remainder = parsed.Remainder;
                                parsed = parser(remainder);
                                pos++;
                            }
                            else
                            {
                                pos++;
                            }

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);

                            return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 3 Failed",
                                parsed.Expectations);
                        }


                    }

                }
                else
                {
                    return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 3 Failed",
                        parsed.Expectations);
                }

                return Result.Success<IEnumerable<T>>(result, remainder);
            };

        }

        public static Parser<IEnumerable<T>> ParseIPV6Rule4<T>(this Parser<T> parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException();
            }

            return i =>
            {
                var remainder = i;
                var result = new List<T>();
                var parsed = parser(i);
                var consumed = string.Empty;
                var pos = i.Position;
                var length = i.Source.Length;
                var legalDigits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                var legalLetters = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F', ':' };

                if (length.Equals(26) || length.Equals(35) || length.Equals(28) || length.Equals(37))
                {
                    //IPv6Address -> [ *1( h16 ":" ) h16 ] "::" 3( h16 ":" ) ls32

                    while (pos < length)
                    {

                        try
                        {
                            if (length.Equals(26) && !(i.Source.Substring(0, 2).Equals("::")))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 4 Failed",
                                    parsed.Expectations);
                            }

                            if (length.Equals(35) && !(i.Source.Substring(9, 2).Equals("::")))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 4 Failed",
                                    parsed.Expectations);
                            }

                            if (legalDigits.Contains(i.Source[pos]) || legalLetters.Contains(i.Source[pos]))
                            {
                                consumed += i.Source[pos];
                                result.Add(parsed.Value);
                                remainder = parsed.Remainder;
                                parsed = parser(remainder);
                                pos++;
                            }
                            else
                            {
                                pos++;
                            }


                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);

                            return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 4 Failed",
                                parsed.Expectations);
                        }


                    }

                }

                else if (length.Equals(32) || length.Equals(41) || length.Equals(34) || length.Equals(43))
                { // parse the ls32 if it isn't an h16

                    while (pos < length - 15)
                    {

                        try
                        {
                            if ((length.Equals(32) && !(i.Source.Substring(0, 2).Equals("::"))) || i.Source.EndsWith(':'))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 4 Failed",
                                    parsed.Expectations);
                            }

                            if ((length.Equals(41) && !(i.Source.Substring(9, 2).Equals("::"))) || i.Source.EndsWith(':'))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 4 Failed",
                                    parsed.Expectations);
                            }

                            if (legalDigits.Contains(i.Source[pos]) || legalLetters.Contains(i.Source[pos]))
                            {
                                consumed += i.Source[pos];
                                result.Add(parsed.Value);
                                remainder = parsed.Remainder;
                                parsed = parser(remainder);
                                pos++;
                            }
                            else
                            {
                                pos++;
                            }


                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);

                            return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 4 Failed",
                                parsed.Expectations);
                        }


                    }

                }
                else
                {
                    return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 4 Failed",
                        parsed.Expectations);
                }

                return Result.Success<IEnumerable<T>>(result, remainder);
            };

        }

        public static Parser<IEnumerable<T>> ParseIPV6Rule5<T>(this Parser<T> parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException();
            }

            return i =>
            {
                var remainder = i;
                var result = new List<T>();
                var parsed = parser(i);
                var consumed = string.Empty;
                var pos = i.Position;
                var length = i.Source.Length;
                var legalDigits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                var legalLetters = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F', ':' };

                if (length.Equals(21) || length.Equals(35) || length.Equals(23) || length.Equals(37))
                {
                    //IPv6Address -> [ *2( h16 ":" ) h16 ] "::" 2( h16 ":" ) ls32

                    while (pos < length)
                    {

                        try
                        {
                            if (length.Equals(21) && !(i.Source.Substring(0, 2).Equals("::")))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 5 Failed",
                                    parsed.Expectations);
                            }

                            if (length.Equals(35) && !(i.Source.Substring(14, 2).Equals("::")))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 5 Failed",
                                    parsed.Expectations);
                            }

                            if (legalDigits.Contains(i.Source[pos]) || legalLetters.Contains(i.Source[pos]))
                            {
                                consumed += i.Source[pos];
                                result.Add(parsed.Value);
                                remainder = parsed.Remainder;
                                parsed = parser(remainder);
                                pos++;
                            }
                            else
                            {
                                pos++;
                            }


                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);

                            return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 5 Failed",
                                parsed.Expectations);
                        }


                    }

                }

                else if (length.Equals(27) || length.Equals(41) || length.Equals(29) || length.Equals(43))
                { // parse the ls32 if it isn't an h16

                    while (pos < length - 15)
                    {

                        try
                        {
                            if ((length.Equals(27) && !(i.Source.Substring(0, 2).Equals("::"))) || i.Source.EndsWith(':'))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 5 Failed",
                                    parsed.Expectations);
                            }

                            if ((length.Equals(41) && !(i.Source.Substring(14, 2).Equals("::"))) || i.Source.EndsWith(':'))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 5 Failed",
                                    parsed.Expectations);
                            }

                            if (legalDigits.Contains(i.Source[pos]) || legalLetters.Contains(i.Source[pos]))
                            {
                                consumed += i.Source[pos];
                                result.Add(parsed.Value);
                                remainder = parsed.Remainder;
                                parsed = parser(remainder);
                                pos++;
                            }
                            else
                            {
                                pos++;
                            }


                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);

                            return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 5 Failed",
                                parsed.Expectations);
                        }


                    }

                }
                else
                {
                    return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 5 Failed",
                        parsed.Expectations);
                }

                return Result.Success<IEnumerable<T>>(result, remainder);
            };

        }

        public static Parser<IEnumerable<T>> ParseIPV6Rule6<T>(this Parser<T> parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException();
            }

            return i =>
            {
                var remainder = i;
                var result = new List<T>();
                var parsed = parser(i);
                var consumed = string.Empty;
                var pos = i.Position;
                var length = i.Source.Length;
                var legalDigits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                var legalLetters = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F', ':' };

                if (length.Equals(16) || length.Equals(35) || length.Equals(18) || length.Equals(37))
                {
                    //IPv6Address -> [ *3( h16 ":" ) h16 ] "::"    h16 ":"   ls32

                    while (pos < length)
                    {

                        try
                        {
                            if (length.Equals(16) && !(i.Source.Substring(0, 2).Equals("::")))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 6 Failed",
                                    parsed.Expectations);
                            }

                            if (length.Equals(30) && !(i.Source.Substring(19, 2).Equals("::")))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 6 Failed",
                                    parsed.Expectations);
                            }

                            if (legalDigits.Contains(i.Source[pos]) || legalLetters.Contains(i.Source[pos]))
                            {
                                consumed += i.Source[pos];
                                result.Add(parsed.Value);
                                remainder = parsed.Remainder;
                                parsed = parser(remainder);
                                pos++;
                            }
                            else
                            {
                                pos++;
                            }


                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);

                            return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 6 Failed",
                                parsed.Expectations);
                        }


                    }

                }
                else if (length.Equals(22) || length.Equals(41) || length.Equals(24) || length.Equals(43))
                { // parse the ls32 if it isn't an h16

                    while (pos < length - 15)
                    {

                        try
                        {
                            if ((length.Equals(22) && !(i.Source.Substring(0, 2).Equals("::"))) || i.Source.EndsWith(':'))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 6 Failed",
                                    parsed.Expectations);
                            }

                            if ((length.Equals(41) && !(i.Source.Substring(19, 2).Equals("::"))) || i.Source.EndsWith(':'))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 6 Failed",
                                    parsed.Expectations);
                            }

                            if (legalDigits.Contains(i.Source[pos]) || legalLetters.Contains(i.Source[pos]))
                            {
                                consumed += i.Source[pos];
                                result.Add(parsed.Value);
                                remainder = parsed.Remainder;
                                parsed = parser(remainder);
                                pos++;
                            }
                            else
                            {
                                pos++;
                            }


                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);

                            return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 6 Failed",
                                parsed.Expectations);
                        }


                    }

                }
                else
                {
                    return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 6 Failed",
                        parsed.Expectations);
                }

                return Result.Success<IEnumerable<T>>(result, remainder);
            };

        }

        public static Parser<IEnumerable<T>> ParseIPV6Rule7<T>(this Parser<T> parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException();
            }

            return i =>
            {
                var remainder = i;
                var result = new List<T>();
                var parsed = parser(i);
                var consumed = string.Empty;
                var pos = i.Position;
                var length = i.Source.Length;
                var legalDigits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                var legalLetters = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F', ':' };

                if (length.Equals(11) || length.Equals(35) || length.Equals(13) || length.Equals(37))
                {
                    //IPv6Address ->  [ *4( h16 ":" ) h16 ] "::" ls32

                    while (pos < length)
                    {

                        try
                        {
                            if (length.Equals(11) && !(i.Source.Substring(0, 2).Equals("::")))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 7 Failed",
                                    parsed.Expectations);
                            }

                            if (length.Equals(35) && !(i.Source.Substring(24, 2).Equals("::")))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 7 Failed",
                                    parsed.Expectations);
                            }

                            if (legalDigits.Contains(i.Source[pos]) || legalLetters.Contains(i.Source[pos]))
                            {
                                consumed += i.Source[pos];
                                result.Add(parsed.Value);
                                remainder = parsed.Remainder;
                                parsed = parser(remainder);
                                pos++;
                            }
                            else
                            {
                                pos++;
                            }


                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);

                            return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 7 Failed",
                                parsed.Expectations);
                        }


                    }

                }
                else if (length.Equals(17) || length.Equals(41) || length.Equals(19) || length.Equals(43))
                { // parse the ls32 if it isn't an h16

                    while (pos < length - 15)
                    {

                        try
                        {
                            if ((length.Equals(26) && !(i.Source.Substring(0, 2).Equals("::"))) || i.Source.EndsWith(':'))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 7 Failed",
                                    parsed.Expectations);
                            }

                            if ((length.Equals(41) && !(i.Source.Substring(24, 2).Equals("::"))) || i.Source.EndsWith(':'))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 7 Failed",
                                    parsed.Expectations);
                            }

                            if (legalDigits.Contains(i.Source[pos]) || legalLetters.Contains(i.Source[pos]))
                            {
                                consumed += i.Source[pos];
                                result.Add(parsed.Value);
                                remainder = parsed.Remainder;
                                parsed = parser(remainder);
                                pos++;
                            }
                            else
                            {
                                pos++;
                            }


                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);

                            return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 7 Failed",
                                parsed.Expectations);
                        }


                    }

                }
                else
                {
                    return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 7 Failed",
                        parsed.Expectations);
                }

                return Result.Success<IEnumerable<T>>(result, remainder);
            };

        }

        public static Parser<IEnumerable<T>> ParseIPV6Rule8<T>(this Parser<T> parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException();
            }

            return i =>
            {
                var remainder = i;
                var result = new List<T>();
                var parsed = parser(i);
                var consumed = string.Empty;
                var pos = i.Position;
                var length = i.Source.Length;
                var dots = 0;
                var legalDigits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                var legalLetters = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F', ':' };

                if (length.Equals(6) || length.Equals(35) || length.Equals(8) || length.Equals(37))
                {
                    //IPv6Address ->  [ *5( h16 ":" ) h16 ] "::" h16

                    while (pos < length)
                    {

                        try
                        {
                            if (length.Equals(6) && !(i.Source.Substring(0, 2).Equals("::")))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 8 Failed",
                                    parsed.Expectations);
                            }

                            if (length.Equals(35) && !(i.Source.Substring(29, 2).Equals("::")))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 8 Failed",
                                    parsed.Expectations);
                            }

                            if (legalDigits.Contains(i.Source[pos]) || legalLetters.Contains(i.Source[pos]))
                            {
                                consumed += i.Source[pos];
                                result.Add(parsed.Value);
                                remainder = parsed.Remainder;
                                parsed = parser(remainder);
                                pos++;
                            }
                            else
                            {
                                pos++;
                            }


                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);

                            return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 8 Failed",
                                parsed.Expectations);
                        }


                    }

                }
                else
                {
                    return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 8 Failed",
                        parsed.Expectations);
                }

                return Result.Success<IEnumerable<T>>(result, remainder);
            };

        }

        public static Parser<IEnumerable<T>> ParseIPV6Rule9<T>(this Parser<T> parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException();
            }

            return i =>
            {
                var remainder = i;
                var result = new List<T>();
                var parsed = parser(i);
                var consumed = string.Empty;
                var pos = i.Position;
                var length = i.Source.Length;
                var dots = 0;
                var legalDigits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                var legalLetters = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F', ':' };

                if (length.Equals(2) || length.Equals(36) || length.Equals(4) || length.Equals(38))
                {
                    //IPv6Address ->  [ *6( h16 ":" ) h16 ] "::"

                    while (pos < length)
                    {

                        try
                        {
                            if (length.Equals(2) && !(i.Source.Substring(0, 2).Equals("::")))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 9 Failed",
                                    parsed.Expectations);
                            }

                            if (length.Equals(36) && !(i.Source.Substring(34, 2).Equals("::")))
                            {
                                return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 9 Failed",
                                    parsed.Expectations);
                            }

                            if (legalDigits.Contains(i.Source[pos]) || legalLetters.Contains(i.Source[pos]))
                            {
                                consumed += i.Source[pos];
                                result.Add(parsed.Value);
                                remainder = parsed.Remainder;
                                parsed = parser(remainder);
                                pos++;
                            }
                            else
                            {
                                pos++;
                            }


                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);

                            return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 9 Failed",
                                parsed.Expectations);
                        }


                    }

                }
                else
                {
                    return Result.Failure<IEnumerable<T>>(remainder, "Ipv6 Rule 9 Failed",
                        parsed.Expectations);
                }

                return Result.Success<IEnumerable<T>>(result, remainder);
            };

        }

    }
}
