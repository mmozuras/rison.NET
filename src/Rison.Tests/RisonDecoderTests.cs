namespace Rison.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class RisonDecoderTests
    {
        private RisonDecoder risonDecoder;

        [SetUp]
        public void SetUp()
        {
            risonDecoder = new RisonDecoder();
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("   ")]
        [TestCase("!")]
        [TestCase("!z")]
        [TestCase("!tf")]
        [ExpectedException(typeof(RisonDecoderException))]
        public void Should_throw_exception_when_input_is_invalid(string risonString)
        {
            risonDecoder.Decode(risonString);
        }

        [TestCase("!t", Result = true)]
        [TestCase("!f", Result = false)]
        public dynamic Should_decode_simple_values(string risonString)
        {
            return risonDecoder.Decode(risonString);
        }
    }
}