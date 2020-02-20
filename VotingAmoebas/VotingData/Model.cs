using Amoeba.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VotingData
{
    public class Model
    {
        readonly Func<double, double> _activation;

        public Model(double bias, double[] weights, Func<double, double> activation)
        {
            this.Bias = bias;
            this.Weights = weights;
            _activation = activation;
        }

        public double Bias { get; set; }
        public double[] Weights { get; set; }


        public PredictionResult Predict(Voter voter)
        {
            var voteList = voter.Votes;
            if (voteList.Count() != this.Weights.Length)
                throw new ArgumentException("The number of votes must match that of the trained model");

            var votes = voteList.Select(v => (double)v.VoteStatus).ToArray();

            double modelValue = this.Bias;
            for (int i = 0; i < this.Weights.Length; i++)
            {
                modelValue += this.Weights[i] * votes[i];
            }

            modelValue = _activation.Invoke(modelValue);

            var result = Party.democrat;
            if (modelValue >= 0.5)
                result = Party.republican;

            return new PredictionResult() { Voter = voter, Value = result, Score = modelValue };
        }


        public (IEnumerable<PredictionResult> Pass, IEnumerable<PredictionResult> Fail) Test(IEnumerable<Voter> testSet)
        {
            var pass = new List<PredictionResult>();
            var fail = new List<PredictionResult>();

            foreach (var voter in testSet)
            {
                var prediction = this.Predict(voter);
                if (prediction.Value == voter.PartyAffiliation)
                    pass.Add(prediction);
                else
                    fail.Add(prediction);
            }

            return (pass, fail);
        }

        public double CalculateError(Voter voter)
        {
            double expected = Convert.ToDouble(voter.PartyAffiliation);
            var actual = this.CalculateScore(voter);
            return (actual - expected);
        }

        public Double CalculateScore(Voter voter)
        {
            double result = this.Bias;
            var actual = voter.Votes.ToArray();
            for (int i = 0; i < this.Weights.Length; i++)
            {
                int currentVoteStatus = (int)actual[i].VoteStatus;
                if (currentVoteStatus == -1)
                    currentVoteStatus = 0;  // Make Nay and No Vote equivalent
                result += Convert.ToDouble(currentVoteStatus) * this.Weights[i];
            }

            return _activation.Invoke(result);
        }


        public static Model Train(IEnumerable<Voter> voters, Func<double[], IEnumerable<Voter>, Func<double, double>, double> objective, Func<double, double> activation)
        {
            // var random = new Random();
            var config = new SimulationConfiguration()
            {
                AmoebaSize = 13,
                Dimensions = 17,
                MaxEpochs = 70,
                MaxX = 1.0,
                MinX = 0.0,
                Alpha = 1.0,
                Beta = 0.5,
                Gamma = 2.0,
                OutputFolder = string.Empty
            };

            Func<double[], double> objectiveFunction = vector => objective(vector, voters, activation);
            var a = new Organism(config, objectiveFunction);  // an amoeba method optimization solver
            var solution = a.Solve();
            return new Model(solution.vector[0], solution.vector[1..^0], activation);
        }


    }

}
