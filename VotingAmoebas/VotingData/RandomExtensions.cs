using System;
using System.Collections.Generic;
using System.Text;

namespace VotingData
{
    public static class RandomExtensions
    {
        public static double DoubleInRange(this Random random)
        {
            return ((random.NextDouble() * 2.0) - 1.0);
        }
    }
}
