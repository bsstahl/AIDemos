using System;
using System.Collections.Generic;
using System.Text;

namespace VotingData
{
    public class PredictionResult
    {
        public Voter Voter { get; set; }
        public Party Value { get; set; }
        public double Score { get; set; }
    }
}
