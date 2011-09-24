namespace Rison
{
    using System;

    public class RisonDecoderException : ArgumentException
    {
        public RisonDecoderException(string inputString) : base("Invalid input string for Rison decoder: " + inputString)
        {
        }
    }
}