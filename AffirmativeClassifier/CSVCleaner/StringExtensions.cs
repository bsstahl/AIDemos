using System;
using System.Collections.Generic;
using System.Text;

namespace CSVCleaner
{
    public static class StringExtensions
    {
        public static string RemoveLeadingCharacters(this string data, char charToRemove)
        {
            string result = data;

            while (result.StartsWith(charToRemove))
                result = result.Substring(1).Trim();

            return result;
        }

        public static string RemoveTrailingCharacters(this string data, char charToRemove)
        {
            string result = data;

            while (result.EndsWith(charToRemove))
                result = data.Substring(0, result.Length - 1).Trim();

            return result;
        }

        public static string RemoveTags(this string data, string tagName)
        {
            string result = data;

            string openTag = $"<{tagName}>";
            string closeTag = $"</{tagName}>";

            result = result.Replace(openTag, "", StringComparison.CurrentCultureIgnoreCase)
                .Replace(closeTag, "", StringComparison.CurrentCultureIgnoreCase);

            return result.Trim();
        }
    }
}
