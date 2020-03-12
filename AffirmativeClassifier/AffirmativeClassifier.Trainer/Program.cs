using System;
using System.Linq;

namespace AffirmativeClassifier.Trainer
{
    public class Program
    {
        public static void Main()
        {
            const string dataFilePath = @"..\..\..\..\..\AffirmativeClassifier\Data\AffirmativeCorpus_Full.csv";

            var repo = new UtteranceRepository(dataFilePath);
            var utterances = repo
                .GetAllUtterances();

            var trainer = new Engine();
            var model = trainer.Train(utterances);

            var maxLength = utterances.Max(u => u.Text.Length);
            Console.WriteLine(maxLength.ToString());
        }
    }
}
