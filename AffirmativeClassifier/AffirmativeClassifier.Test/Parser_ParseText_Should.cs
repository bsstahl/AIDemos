using System;
using System.Linq;
using Xunit;

namespace AffirmativeClassifier.Test
{
    public class Parser_ParseText_Should
    {
        [Theory]
        [InlineData("", 0)]
        [InlineData("\t", 0)]
        [InlineData("OnlyOne", 1)]
        [InlineData("Exactly Two", 2)]
        [InlineData("This is Three", 3)]
        [InlineData("Also, just Three", 3)]
        public void ReturnTheCorrectNumberOfWords(String text, int expected)
        {
            var target = new Parser();
            var actual = target.ParseText(text);
            Assert.Equal(expected, actual.Count());
        }

        [Theory]
        [InlineData("OnlyOne", "OnlyOne")]
        [InlineData("Exactly Two", "Exactly")]
        [InlineData("This is Three", "This")]
        [InlineData("Also, just Three", "Also,")]
        public void ReturnTheCorrectFirstWord(String text, string expected)
        {
            var target = new Parser();
            var actual = target.ParseText(text);
            Assert.Equal(expected, actual.ToArray()[0]);
        }

        [Theory]
        [InlineData("OnlyOne", "OnlyOne")]
        [InlineData("Exactly Two", "Two")]
        [InlineData("This is Three", "Three")]
        [InlineData("Also, just Three", "Three")]
        public void ReturnTheCorrectLastWord(String text, string expected)
        {
            var target = new Parser();
            var actual = target.ParseText(text);
            Assert.Equal(expected, actual.ToArray()[^1]);
        }
    }
}
