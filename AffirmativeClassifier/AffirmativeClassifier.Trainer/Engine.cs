using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AffirmativeClassifier.Trainer
{
    internal class Engine
    {
        internal Model Train(IEnumerable<Utterance> utterances)
        {
            var result = new Model();

            var inputs = utterances.ToModelInputs();
            // Console.WriteLine(inputs.Max(i => i.ParsedUtterance.Count()).ToString());
            // Console.WriteLine(inputs.Average(i => i.ParsedUtterance.Count()).ToString());

            //var n = inputs.Count();
            //var mid = n / 2;
            //var median = inputs.OrderBy(i => i.ParsedUtterance.Count()).ToArray()[mid];
            //Console.WriteLine(median.ParsedUtterance.Count().ToString());            

            var words = inputs.SelectMany(i => i.ParsedUtterance);
            int longestWord = words.Max(i => i.Length);
            Console.WriteLine(longestWord.ToString());
            Console.WriteLine(words.Average(i => i.Length).ToString());
            Console.WriteLine(String.Join(";", words.Where(w => w.Length == longestWord).ToArray()));

            return result;
        }
    }
}
