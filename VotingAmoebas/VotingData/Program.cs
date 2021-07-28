using Amoeba.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VotingData
{
    class Program
    {
        static void Main(string[] args)
        {
            const int MAX_EXECUTIONS = 250;


            // Load the voter data
            var repo = new VotingDataRepository(@".");
            var allVoters = repo.GetAllVoters();

            // Initialize stats
            int count = 0;
            var resultPercentages = new List<Double>();
            Double highestAccuracy = 0.0;
            Model bestModel = null;

            (var trainingSet, var testSet) = allVoters.Split(0.75);

            while (count < MAX_EXECUTIONS)
            {
                // Train and test
                var (accuracy, model) = TrainAndTestModel(trainingSet, testSet);

                if (accuracy > highestAccuracy)
                {
                    highestAccuracy = accuracy;
                    bestModel = model;
                }

                // Update stats
                count++;
                resultPercentages.Add(accuracy);
            }

            var sum = resultPercentages.Sum(r => r);
            var avg = sum / Convert.ToDouble(count);
            var dev = Math.Sqrt(resultPercentages.Sum(r => Math.Pow(avg - r, 2.0)) / Convert.ToDouble(count));
            var max = resultPercentages.Max(r => r);
            var min = resultPercentages.Min(r => r);

            // Training Results
            Console.WriteLine($"Training - Mean: {avg}  Min: {min}  Max {max}  StDev: {dev}");

            // Run all voters through the best model and show the failures
            var (bestModelPasses, bestModelFailures) = bestModel.Test(allVoters);
            Console.WriteLine($"Incorrect Predictions:\r\n{bestModelFailures.AsResultsList()}");
        }

        private static (double accuracy, Model model) TrainAndTestModel(IEnumerable<Voter> trainingSet, IEnumerable<Voter> testSet)
        {
            // Train the model
            var model = Model.Train(trainingSet, ErrorFunction, Sigmoid);

            // Test the model
            var (pass, fail) = model.Test(testSet);

            // Calculate statistics
            int correctCount = pass.Count();
            int count = fail.Count() + correctCount;
            double pct = (Convert.ToDouble(correctCount) / Convert.ToDouble(count)) * 100;

            return (pct, model);
        }

        private static double ErrorFunction(double[] vectors, IEnumerable<Voter> voters, Func<double, double> activation)
        {
            // Returns the sum of the squared errors for all voters in the set
            var model = new Model(vectors[0], vectors[1..^0], activation);
            return voters.Sum(v => Math.Pow(model.CalculateError(v), 2));
        }

        private static double NoActivation(double value) => value;

        private static Double Relu(Double value) => Math.Max(0.0, value);

        public static Double Sigmoid(Double value)
        {
            var eX = Math.Exp(value);
            return eX / (eX + 1.0);
        }

    }
}
