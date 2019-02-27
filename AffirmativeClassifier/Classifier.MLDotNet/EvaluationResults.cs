using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML.Core.Data;

namespace Classifier.MLDotNet
{
    internal class EvaluationResults
    {
        public double Accuracy { get; set; }
        public double Auc { get; set; }
        public double F1Score { get; set; }

        public ITransformer Model { get; set; }
        public string TrainingMethodName { get; set; }
    }
}
