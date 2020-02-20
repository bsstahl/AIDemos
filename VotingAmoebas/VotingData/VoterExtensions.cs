using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VotingData
{
    public static class VoterExtensions
    {
        public static (IEnumerable<Voter> trainingSet, IEnumerable<Voter> testSet) Split(this IEnumerable<Voter> voters, double trainingPercentage)
        {
            var random = new Random();

            var trainingSet = new List<Voter>();
            var testSet = new List<Voter>();

            foreach (var voter in voters)
            {
                if (random.NextDouble() < trainingPercentage)
                    trainingSet.Add(voter);
                else
                    testSet.Add(voter);
            }

            return (trainingSet, testSet);
        }

    }
}
