namespace Rison.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using NUnit.Framework;

    [TestFixture]
    public class RisonEncoderTests
    {
        private RisonEncoder risonEncoder;

        [SetUp]
        public void SetUp()
        {
            risonEncoder = new RisonEncoder();
        }

        [TestCase(true, Result = "!t")]
        [TestCase(false, Result = "!f")]
        public string Should_encode_bool_values(bool value)
        {
            return risonEncoder.Encode(value);
        }

        [Test]
        public void Should_encode_null()
        {
            var result = risonEncoder.Encode((string) null);
            Assert.AreEqual("!n", result);
        }

        [Test]
        public void Should_encode_an_empty_int_array()
        {
            var result = risonEncoder.Encode(new int[] {});
            Assert.AreEqual("!()", result);
        }

        [Test]
        public void Should_encode_an_array_of_bools()
        {
            var result = risonEncoder.Encode(new[] {true, false, true});
            Assert.AreEqual("!(!t,!f,!t)", result);
        }

        [Test]
        public void Should_encode_a_list_of_ints()
        {
            var result = risonEncoder.Encode(new List<int> {1, 4, 7});
            Assert.AreEqual("!(1,4,7)", result);
        }

        [Test]
        public void Should_encode_a_collection_of_chars()
        {
            var result = risonEncoder.Encode(new Collection<char> {'a', 'z', 'f'});
            Assert.AreEqual("!(\"a\",\"z\",\"f\")", result);
        }

        [TestCase(1, Result = "1")]
        [TestCase(-17, Result = "-17")]
        [TestCase(-1.5, Result = "-1.5")]
        [TestCase(-123.456, Result = "-123.456")]
        [TestCase(1E+30, Result = "1e30")]
        [TestCase(2E-20, Result = "2e-20")]
        public string Should_encode_numbers(object obj)
        {
            return risonEncoder.Encode(obj);
        }

        [Test]
        public void Should_encode_a_char()
        {
            var result = risonEncoder.Encode('a');
            Assert.AreEqual("\"a\"", result);
        }

        [Test]
        public void Should_encode_a_string()
        {
            var result = risonEncoder.Encode("test");
            Assert.AreEqual("\"test\"", result);
        }

        [Test]
        public void Should_encode_an_enum()
        {
            var result = risonEncoder.Encode(BindingFlags.GetField);
            Assert.AreEqual("\"GetField\"", result);
        }

        [Test]
        public void Should_encode_a_datetime()
        {
            //TODO: Write a more full-featured datetime encoding implementation?
            var result = risonEncoder.Encode(new DateTime(2011, 10, 2, 16, 21, 30));
            Assert.AreEqual("\"2011-10-02T16:21:30.000\"", result);
        }

        [Test]
        public void Should_encode_a_guid()
        {
            var guid = Guid.Parse("{21EC2020-3AEA-1069-A2DD-08002B30309D}");
            var result = risonEncoder.Encode(guid);
            Assert.AreEqual("\"21ec2020-3aea-1069-a2dd-08002b30309d\"", result);
        }

        [Test]
        public void Should_encode_a_dictionary()
        {
            var dictionary = new Dictionary<string, int>
                                 {
                                     {"first", 1},
                                     {"second", 2},
                                 };
            var result = risonEncoder.Encode(dictionary);
            Assert.AreEqual("!((k:\"first\",v:1),(k:\"second\",v:2))", result);
        }

        [Test]
        public void Should_encode_a_complex_class()
        {
            var person = new Person
                             {
                                 Name = "name",
                                 Age = 11,
                                 Friends = new List<Person>
                                               {
                                                   new Person
                                                       {
                                                           Name = "friend",
                                                           Age = 12
                                                       }
                                               }
                             };
            var result = risonEncoder.Encode(person);
            Assert.AreEqual("(Name:\"name\",Age:11,Friends:!((Name:\"friend\",Age:12,Friends:!n)))", result);
        }
    }
}