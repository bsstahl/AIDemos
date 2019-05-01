using System;
using Microsoft.ML.Data;

namespace NonToxic.Model
{
    public class SamplePrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        public float Score { get; set; }
    }
}
