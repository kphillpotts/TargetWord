using TargetWord.Core;

namespace TargetWord.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void Test2()
        {
            var c1 = new Class1();
            var c2 = new Class1();
            Assert.That(c1.ReturnTrue, Is.True);
        }
    }
}