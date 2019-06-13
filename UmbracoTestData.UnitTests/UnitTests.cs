using NUnit.Framework;

namespace UmbracoTestData.UnitTests
{
    public class Tests
    {

        [Test]
        [TestCase(1, 5, ExpectedResult = "1")]
        [TestCase(2, 5, ExpectedResult = "2")]
        [TestCase(3, 5, ExpectedResult = "3")]
        [TestCase(4, 5, ExpectedResult = "4")]
        [TestCase(5, 5, ExpectedResult = "5")]
        [TestCase(6, 5, ExpectedResult = "1.1")]
        [TestCase(7, 5, ExpectedResult = "1.2")]
        [TestCase(8, 5, ExpectedResult = "1.3")]
        [TestCase(9, 5, ExpectedResult = "1.4")]
        [TestCase(10, 5, ExpectedResult = "1.5")]
        [TestCase(1, 10, ExpectedResult = "1")]
        [TestCase(2, 10, ExpectedResult = "2")]
        [TestCase(3, 10, ExpectedResult = "3")]
        [TestCase(4, 10, ExpectedResult = "4")]
        [TestCase(5, 10, ExpectedResult = "5")]
        [TestCase(6, 10, ExpectedResult = "6")]
        [TestCase(7, 10, ExpectedResult = "7")]
        [TestCase(8, 10, ExpectedResult = "8")]
        [TestCase(9, 10, ExpectedResult = "9")]
        [TestCase(10, 10, ExpectedResult = "10")]
        [TestCase(11, 10, ExpectedResult = "1.1")]
        [TestCase(500, 10, ExpectedResult = "4.9.10")]
        public string GetPositionInTree(int currentItemNumber, int numberOfItemsOnEachLevel)
        {
            var result = DataCreator.GetPositionInTree((uint)currentItemNumber, (uint)numberOfItemsOnEachLevel );

            return string.Join(".", result);
        }
      
    }
}