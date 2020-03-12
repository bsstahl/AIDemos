using System;
using Xunit;

namespace AffirmativeClassifier.Test
{
    public class Tokenizer_TokenizeWord_Should
    {
        [Theory]
        [InlineData("", 0)]
        [InlineData("  ", 0)]
        [InlineData("\t", 0)]
        public void ReturnZeroForNullOrEmptyStrings(String value, UInt64 expected)
        {
            var target = new Tokenizer();
            var actual = target.TokenizeWord(value);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Hello")]
        [InlineData("Done")]
        [InlineData("no")]
        [InlineData("yes")]
        [InlineData("WooHoo!!")]
        public void ReturnAPositiveValueForNonNullStrings(String value)
        {
            var target = new Tokenizer();
            var actual = target.TokenizeWord(value);
            Assert.True(actual > 0);
        }

        [Theory]
        [InlineData("Hello")]
        [InlineData("Done")]
        [InlineData("no")]
        [InlineData("yes")]
        [InlineData("WooHoo!!")]
        public void ReturnAConsistantValueForTheSameWord(String value)
        {
            var target1 = new Tokenizer();
            var target2 = new Tokenizer();

            var actual1 = target1.TokenizeWord(value);
            var actual2 = target2.TokenizeWord(value);

            Assert.Equal(actual1, actual2);
        }

        [Theory]
        [InlineData("Hello", "Done")]
        [InlineData("Done", "no")]
        [InlineData("no", "yes")]
        [InlineData("yes", "WooHoo!")]
        [InlineData("WooHoo!!", "Maybe")]
        public void ReturnADifferentValueForDifferentWords(String value1, String value2)
        {
            var target = new Tokenizer();

            var actual1 = target.TokenizeWord(value1);
            var actual2 = target.TokenizeWord(value2);

            Assert.False(actual1 == actual2);
        }

    }
}
