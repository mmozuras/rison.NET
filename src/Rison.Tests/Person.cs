namespace Rison.Tests
{
    using System.Collections.Generic;

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public IEnumerable<Person> Friends { get; set; }
    }
}