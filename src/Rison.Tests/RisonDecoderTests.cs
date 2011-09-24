namespace Rison.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class RisonDecoderTests
    {
        private IRisonDecoder risonDecoder;

        [SetUp]
        public void SetUp()
        {
            risonDecoder = new RisonDecoder();
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase("!")]
        [TestCase("!z")]
        [TestCase("!tf")]
        [ExpectedException(typeof(RisonDecoderException))]
        public void Should_throw_exception_when_input_is_invalid_for_simple_value(string risonString)
        {
            risonDecoder.Decode<bool>(risonString);
        }

        [TestCase(null)]
        [TestCase("!(,)")]
        [TestCase("!(!t!f)")]
        [ExpectedException(typeof(RisonDecoderException))]

        public void Should_throw_exception_when_input_is_invalid(string risonString)
        {
            risonDecoder.Decode<bool[]>(risonString);
        }

        [TestCase("!t", Result = true)]
        [TestCase("!f", Result = false)]        
        public bool Should_decode_bool_values(string risonString)
        {
            return risonDecoder.Decode<bool>(risonString);
        }

        [Test]
        public void Should_decode_null()
        {
            var result = risonDecoder.Decode<string>("!n");
            Assert.IsNull(result);
        }

        [Test]
        public void Should_decode_an_empty_array()
        {
            var result = risonDecoder.Decode<string[]>("!()");
            CollectionAssert.AreEqual(new string[]{}, result);
        }

        [Test]
        public void Should_decode_an_array_with_same_types()
        {
            var result = risonDecoder.Decode<bool[]>("!(!t,!f,!t)");
            CollectionAssert.AreEqual(new[] {true, false, true}, result);
        }

        [Test]
        public void Should_decode_an_array_with_different_types()
        {
            var result = risonDecoder.Decode<object[]>("!(!t,!(!f))");
            CollectionAssert.AreEqual(new object[] {true, new[] {false}}, result);
        }
    }
}