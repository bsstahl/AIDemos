using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AffirmativeClassifier.Trainer
{
    public static class Extensions
    {
        public static Utterance ToUtterance(this String line)
        {
            Utterance result = null;
            var s = line.Split(',');
            if (Int32.TryParse(s[0].Trim(), out var isAffirmative))
                result = new Utterance()
                {
                    Text = s[1].Trim(),
                    IsAffirmative = Convert.ToBoolean(isAffirmative)
                };
            return result;
        }

        internal static IEnumerable<ModelInput> ToModelInputs(this IEnumerable<Utterance> utterances)
        {
            var parser = new Parser();
            return utterances.Select(u => u.ToModelInput(parser));
        }

        internal static ModelInput ToModelInput(this Utterance utterance, Parser parser)
        {
            return new ModelInput()
            {
                IsAffirmative = utterance.IsAffirmative,
                Text = utterance.Text,
                ParsedUtterance = parser.ParseText(utterance.Text),
                TokenizedUtterance = 0 // TODO: Implement
            };
        }
    }
}
