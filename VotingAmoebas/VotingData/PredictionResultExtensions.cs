using System;
using System.Collections.Generic;
using System.Text;

namespace VotingData
{
    public static class PredictionResultExtensions
    {
        public static String AsResultsList(this IEnumerable<PredictionResult> predictions)
        {
            var sb = new StringBuilder();
            foreach (var prediction in predictions)
            {
                sb.AppendLine($"{prediction.Voter.ToString()} ({prediction.Score:0.0000})");
            }
            return sb.ToString();
        }

    }
}
