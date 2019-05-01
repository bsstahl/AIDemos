using System;
using System.IO;
using System.Linq;
using Microsoft.ML;
using NonToxic.Model;


namespace NonToxic.ConsoleApp
{
    class Program
    {

        static void Main(string[] args)
        {
            string utterance = "I know you listed your English as on the \"level 2\", but don't worry, you seem to be doing nicely otherwise, judging by the same page - so don't be taken aback. I just wanted to know if you were aware of what you wrote, and think it's an interesting case. : I would write that sentence simply as \"Theoretically I am an altruist, but only by word, not by my actions.\". : PS. You can reply to me on this same page, as I have it on my watchlist.  ";
            // string utterance = "How do you remember to breathe when you are that stupid?";
            // string utterance = "I have reviewed all your changes and though I don't agree with all of them, I find them generally acceptable";

            var predictionEngine = new Engine();
            var result = predictionEngine.Predict(utterance);

            Console.WriteLine($"Prediction: {result}");
        }

    }
}
