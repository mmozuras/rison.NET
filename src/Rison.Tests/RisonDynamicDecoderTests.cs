namespace Rison.Tests
{
    using System;
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
        [TestCase("-")]
        [TestCase("4abc")]
        [TestCase("'")]
        [TestCase("'abc")]
        [TestCase("'a!'!'")]
        [TestCase("(1not:'id')")]
        [ExpectedException(typeof(RisonDecoderException))]
        public void Should_throw_exception_when_input_is_invalid(string risonString)
        {
            risonDecoder.Decode(risonString);
        }

        [TestCase("!t", Result = true)]
        [TestCase("!f", Result = false)]        
        public dynamic Should_decode_bool_values(string risonString)
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

        [TestCase("1", Result = 1)]
        [TestCase("-17", Result = -17)]
        [TestCase("1.5", Result = 1.5)]
        [TestCase("-123.456", Result = -123.456)]
        [TestCase("1e30", Result = 1E+30)]
        [TestCase("2e-20", Result = 2E-20)]
        public dynamic Should_decode_numbers(string risonString)
        {
            return risonDecoder.Decode(risonString);
        }

        [TestCase("'a'", Result = "a")]
        [TestCase("'-b'", Result = "-b")]
        [TestCase("''", Result = "")]
        [TestCase("'don!'t'", Result = "don't")]
        [TestCase("'double!!'", Result = "double!")]
        [TestCase("'a b c'", Result = "a b c")]
        [TestCase("'user@domain.com'", Result = "user@domain.com")]
        public dynamic Should_decode_strings(string risonString)
        {
            return risonDecoder.Decode(risonString);
        }

        [Test]
        public void Should_decode_simple_rison_object()
        {
            var result = risonDecoder.Decode("(i:1,j:2)");
            Assert.AreEqual(1, result.i);
            Assert.AreEqual(2, result.j);
        }

        [Test]
        public void Should_decode_complex_rison_object()
        {
            var result = risonDecoder.Decode("(name:!n,author:/writer~,comments:!('great','not.so.great'))");
            Assert.AreEqual(null, result.name);
            Assert.AreEqual("/writer~", result.author);
            CollectionAssert.AreEqual(new dynamic[] {"great", "not.so.great"}, result.comments);
        }
    }
}