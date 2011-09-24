namespace Rison
{
    internal class RisonStringEnumerator
    {
        private readonly string risonString;
        private int index;

        public RisonStringEnumerator(string risonString)
        {
            this.risonString = risonString;
            index = -1;
        }

        public string RisonString
        {
            get { return risonString; }
        }

        public char Next()
        {
            if (!HasNext())
            {
                throw new RisonDecoderException(risonString);
            }
            index++;
            return risonString[index];
        }

        public bool HasNext()
        {
            return index + 1 < risonString.Length;
        }

        public char Previous()
        {
            if (index == 0)
            {
                throw new RisonDecoderException(risonString);
            }
            index--;
            return risonString[index];
        }
    }
}