namespace Rison
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public class RisonDynamicDecoder : IRisonDynamicDecoder
    {
        private RisonStringWalker walker;

        public dynamic Decode(string risonString)
        {
            if (string.IsNullOrWhiteSpace(risonString))
            {
                throw new RisonDecoderException(risonString);
            }
            walker = new RisonStringWalker(risonString);

            var value = ReadValue();
            if (walker.HasNext())
            {
                throw new RisonDecoderException(risonString);
            }
            return value;
        }

        private dynamic ReadValue()
        {
            var c = walker.Next();
            if (c == '!')
            {
                return ParseBang();
            }
            if (c == '\'')
            {
                return ParseSingleQuote();
            }
            if ("-01234567890".Contains(c))
            {
                return ParseNumber();
            }
            return null;
        }

        private dynamic ParseBang()
        {
            switch (walker.Next())
            {
                case ('t'):
                    return true;
                case ('f'):
                    return false;
                case ('n'):
                    return null;
                case('('):
                    return ParseArray();
            }
            throw new RisonDecoderException(walker.RisonString);
        }

        private dynamic ParseSingleQuote()
        {
            var risonString = walker.RisonString;
            walker.Next();
            var index = walker.Index;
            var start = walker.Index;
            var segments = new List<dynamic>();

            while (true)
            {
                if (index >= risonString.Length)
                {
                    throw new RisonDecoderException("Unmatched '");
                }

                var c = risonString[index];
                if (c == '\'')
                    break;

                index++;
                if (c == '!')
                {
                    if (start < index - 1)
                    {
                        segments.Add(risonString.Substring(start, index - start - 1));
                    }
                    c = risonString[index];
                    index++;
                    if ("!'".Contains(c))
                    {
                        segments.Add(c);
                    }
                    else
                    {
                        throw new RisonDecoderException(string.Format("Invalid string escape: '!{0}'", c));
                    }

                    start = index;
                }
            }

            if (start < index)
            {
                segments.Add(risonString.Substring(start, index - start));
            }

            walker.Index = index;
            return string.Join("", segments);
        }

        private dynamic ParseNumber()
        {
            var risonString = walker.RisonString;
            var index = walker.Index;
            var start = walker.Index;
            var state = "int";
            var dashPermitted = true;
            var transitions = new Dictionary<string, string>
                                  {
                                      {"int+.", "frac"},
                                      {"int+e", "exp"},
                                      {"frac+e", "exp"},
                                  };
            while (true)
            {
                if (index >= risonString.Length)
                {
                    index++;
                    break;
                }

                var c = risonString[index];
                index++;

                if (('0' <= c && c <= '9') || (dashPermitted && c == '-'))
                {
                    dashPermitted = false;
                    continue;
                }

                if (!transitions.ContainsKey(state + '+' + char.ToLower(c)))
                {
                    break;
                }

                state = transitions[state + '+' + char.ToLower(c)];
                dashPermitted = state == "exp";
            }

            walker.Index = index - 1;
            var result = risonString.Substring(start, walker.Index - start);
            if (result == "-")
            {
                throw new RisonDecoderException("Invalid number", walker.RisonString);
            }
            if (result.Contains("e") || result.Contains("."))
            {
                var cultureInfo = (CultureInfo) CultureInfo.CurrentCulture.Clone();
                cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
                return double.Parse(result, NumberStyles.Any, cultureInfo);
            }
            return int.Parse(result);
        }

        private IEnumerable<dynamic> ParseArray()
        {
            var result = new List<dynamic>();
            while (true)
            {
                var c = walker.Next();
                if (c == ')')
                {
                    return result;
                }
                if (result.Any())
                {
                    if (c != ',')
                    {
                        throw new RisonDecoderException("Missing ',", walker.RisonString);
                    }
                }
                else if (c == ',')
                {
                    throw new RisonDecoderException("Extra ',");
                }
                else
                {
                    walker.Previous();
                }

                var value = ReadValue();
                result.Add(value);
            }
        }
    }
}