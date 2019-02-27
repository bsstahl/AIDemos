using System;

namespace CSVCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            string source = @"C:\Users\barry\Source\Temp\AffirmativeClassifier\IsAffirmative\Data\KaggleMovieDialog_ThirdClean.csv";
            string target = $"C:\\Users\\barry\\Source\\Temp\\AffirmativeClassifier\\IsAffirmative\\Data\\KaggleMovieDialog_CleanSample_{DateTime.Now.Ticks}.csv";

            var reader = new System.IO.StreamReader(source);
            var writer = new System.IO.StreamWriter(target);

            // Clean(reader, writer);
            Sample(reader, writer);

            reader.Close();
            writer.Close();
        }

        private static void Sample(System.IO.StreamReader reader, System.IO.StreamWriter writer)
        {
            double count = 157400.0;
            var rnd = new Random();

            var line = reader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                var r = rnd.NextDouble() * count;
                if (r < 15.74)
                    writer.WriteLine(line);
                line = reader.ReadLine();
            }
        }

        private static void Clean(System.IO.StreamReader reader, System.IO.StreamWriter writer)
        {
            string previousLine = string.Empty;
            var line = reader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                var values = line.Split(',');
                string isAffirmative = values[0].Trim();
                string utterance = values[1].Trim()
                    .RemoveLeadingCharacters('-').RemoveTrailingCharacters('-')
                    .RemoveLeadingCharacters('.').RemoveTrailingCharacters('.')
                    .RemoveLeadingCharacters('*').RemoveTrailingCharacters('*')
                    .Replace("\"", "")
                    .RemoveTags("i").RemoveTags("u");

                string result = $"{isAffirmative},{utterance}";
                if (result != previousLine)
                {
                    writer.WriteLine(result);
                    previousLine = result;
                }

                line = reader.ReadLine();
            }
        }
    }
}