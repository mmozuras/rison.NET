namespace Rison
{
    using System.Collections.Generic;
    using System.Linq;

    public class RisonDynamicDecoder : IRisonDynamicDecoder
    {
        private RisonStringEnumerator enumerator;

        public dynamic Decode(string risonString)
        {
            if (string.IsNullOrWhiteSpace(risonString))
            {
                throw new RisonDecoderException(risonString);
            }
            enumerator = new RisonStringEnumerator(risonString);

            var value = ReadValue();
            if (enumerator.HasNext())
            {
                throw new RisonDecoderException(risonString);
            }
            return value;
        }

        private dynamic ReadValue()
        {
            var c = enumerator.Next();
            if (c == '!')
            {
                return ParseBang();
            }
            return null;
        }

        private dynamic ParseBang()
        {
            switch (enumerator.Next())
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
            throw new RisonDecoderException(enumerator.RisonString);
        }

        private dynamic[] ParseArray()
        {
            var array = new List<object>();
            while (true)
            {
                var c = enumerator.Next();
                if (c == ')')
                {
                    return array.ToArray();
                }
                if (array.Any())
                {
                    if (c != ',')
                    {
                        throw new RisonDecoderException("Missing ',", enumerator.RisonString);
                    }
                }
                else if (c == ',')
                {
                    throw new RisonDecoderException("Extra ',");
                }
                else
                {
                    enumerator.Previous();
                }

                var value = ReadValue();
                array.Add(value);
            }
        }
    }
}