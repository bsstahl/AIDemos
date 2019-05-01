using Microsoft.ML.Data;

namespace NonToxic.Model
{
    public class SampleObservation
    {
        [ColumnName("Sentiment"), LoadColumn(0)]
        public bool Sentiment { get; set; }


        [ColumnName("SentimentText"), LoadColumn(1)]
        public string SentimentText { get; set; }

        public SampleObservation() { }

        public SampleObservation(string sentimentText)
        {
            this.SentimentText = sentimentText;
        }
    }
}
