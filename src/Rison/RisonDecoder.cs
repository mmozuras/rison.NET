namespace Rison
{
    using System.Collections.Generic;
    using System.Linq;

    public class RisonDecoder : IRisonDecoder
    {
        private RisonStringEnumerator enumerator;

        public T Decode<T>(string risonString)
        {
            if (string.IsNullOrWhiteSpace(risonString))
            {
                throw new RisonDecoderException(risonString);
            }
            enumerator = new RisonStringEnumerator(risonString);

            var value = ReadValue<T>();
            if (enumerator.HasNext())
            {
                throw new RisonDecoderException(risonString);
            }
            return value;
        }

        private T ReadValue<T>()
        {
            var c = enumerator.Next();
            if (c == '!')
            {
                var o = ParseBang();
                if (o is T)
                {
                    return (T) o;
                }
            }
            return default(T);
        }

        private object ParseBang()
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

                var value = ReadValue<dynamic>();
                array.Add(value);
            }
        }
    }
}