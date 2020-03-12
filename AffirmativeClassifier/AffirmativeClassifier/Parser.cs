using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AffirmativeClassifier
{
    public class Parser
    {
        private string CleanText(String value)
        {
            return value
                .Replace(".", " ")
                .Replace(",", " ");
        }

        public IEnumerable<String> ParseText(String value)
        {
            return this
                .CleanText(value)
                .Split(' ')
                .Select(t => t.Trim());
        }
    }
}
