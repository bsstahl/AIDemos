using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML.Data;

namespace Classifier.MLDotNet
{
    internal class AffirmativePrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        [ColumnName("Probability")]
        public float Probability { get; set; }

        [ColumnName("Score")]
        public float Score { get; set; }
    }
}
