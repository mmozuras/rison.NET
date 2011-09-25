namespace Rison
{
    internal class RisonStringWalker
    {
        private readonly string risonString;
        private int index;

        public RisonStringWalker(string risonString)
        {
            this.risonString = risonString;
            index = -1;
        }

        public string RisonString
        {
            get { return risonString; }
        }

        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        public char Next()
        {
            if (!HasNext())
            {
                throw new RisonDecoderException(risonString);
            }
            index++;
            return risonString[Index];
        }

        public bool HasNext()
        {
            return Index + 1 < risonString.Length;
        }

        public char Previous()
        {
            if (Index == 0)
            {
                throw new RisonDecoderException(risonString);
            }
            index--;
            return risonString[Index];
        }
    }
}