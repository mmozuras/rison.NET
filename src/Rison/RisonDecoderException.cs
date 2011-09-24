namespace Rison
{
    using System;

    public class RisonDecoderException : ArgumentException
    {
        private readonly string inputString;

        public RisonDecoderException(string message, string inputString) 
            : base(message)
        {
            this.inputString = inputString;
        }

        public RisonDecoderException(string inputString)
            : this("Invalid input string for Rison decoder.", inputString)
        {
        }

        public override string ToString()
        {
            return Message + " Input string: " + inputString;
        }
    }
}