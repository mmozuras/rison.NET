namespace Rison.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class RisonDynamicDecoderTests
    {
        private IRisonDynamicDecoder risonDecoder;

        [SetUp]
        public void SetUp()
        {
            risonDecoder = new RisonDynamicDecoder();
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase("!")]
        [TestCase("!z")]
        [TestCase("!tf")]
        [TestCase(null)]
        [TestCase("!(,)")]
        [TestCase("!(!t!f)")]
        [ExpectedException(typeof(RisonDecoderException))]
        public void Should_throw_exception_when_input_is_invalid(string risonString)
        {
            risonDecoder.Decode(risonString);
        }

        [TestCase("!t", Result = true)]
        [TestCase("!f", Result = false)]        
        public bool Should_decode_bool_values(string risonString)
        {
            return risonDecoder.Decode(risonString);
        }

        [Test]
        public void Should_decode_null()
        {
            var result = risonDecoder.Decode("!n");
            Assert.IsNull(result);
        }

        [Test]
        public void Should_decode_an_empty_array()
        {
            var result = risonDecoder.Decode("!()");
            CollectionAssert.AreEqual(new dynamic[]{}, result);
        }

        [Test]
        public void Should_decode_an_array_with_same_types()
        {
            var result = risonDecoder.Decode("!(!t,!f,!t)");
            CollectionAssert.AreEqual(new dynamic[] { true, false, true }, result);
        }

        [Test]
        public void Should_decode_an_array_with_different_types()
        {
            var result = risonDecoder.Decode("!(!t,!(!f))");
            CollectionAssert.AreEqual(new dynamic[] { true, new[] { false } }, result);
        }
    }
}