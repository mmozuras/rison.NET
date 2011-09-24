namespace Rison
{
    using System;

    public class RisonDecoder : IRisonDecoder
    {
        private string input;
        private CharEnumerator charEnumerator;

        public dynamic Decode(string risonString)
        {
            return Decode<dynamic>(risonString);
        }

        public T Decode<T>(string risonString)
        {
            if (string.IsNullOrWhiteSpace(risonString))
            {
                throw new RisonDecoderException(risonString);
            }
            charEnumerator = risonString.GetEnumerator();
            input = risonString;

            var value = ReadValue<T>();
            if (charEnumerator.MoveNext())
            {
                throw new RisonDecoderException(risonString);
            }
            return value;
        }

        public T ReadValue<T>()
        {
            var c = Next();
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
            switch (Next())
            {
                case ('t'):
                    return true;
                case ('f'):
                    return false;
            }
            throw new RisonDecoderException(input);
        }

        public char Next()
        {
            if (!charEnumerator.MoveNext())
            {
                throw new RisonDecoderException(input);
            }
            return charEnumerator.Current;
        }
    }
}