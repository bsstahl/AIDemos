using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.DataView;
using Microsoft.ML;
using Microsoft.ML.Core.Data;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;

namespace Classifier.MLDotNet
{
    public class AffirmativeClassifier
    {
        ITransformer _model;
        MLContext _mlContext = new MLContext(seed: 0);

        public string TrainingMethodName { get; set; } = "Unknown";

        public AffirmativeClassifier(byte[] model)
        {
            _model = _mlContext.Model.Load(new MemoryStream(model));
        }

        public AffirmativeClassifier(string trainingDataFilePath, string testDataFilePath)
        {
            var results = Train(_mlContext, trainingDataFilePath, testDataFilePath);
            this.TrainingMethodName = results.TrainingMethodName;
            _model = results.Model;
        }

        public byte[] Export()
        {
            return SaveModel(_mlContext, _model);
        }

        public (double Accuracy, double Auc, double F1Score) Evaluate(string testDataFilePath)
        {
            var results = EvaluateModel(_mlContext, _model, testDataFilePath, "Unknown");
            return (results.Accuracy, results.Auc, results.F1Score);
        }

        public (bool Prediction, float Probability) Predict(string utterance)
        {
            var predictionFunction = _model.CreatePredictionEngine<AffirmativeData, AffirmativePrediction>(_mlContext);
            var predictionData = new AffirmativeData() { UtteranceText = utterance };
            var predictionResult = predictionFunction.Predict(predictionData);
            return (Convert.ToBoolean(predictionResult.Prediction), predictionResult.Probability);
        }

        private static EvaluationResults EvaluateModel(MLContext mlContext, ITransformer model, string testDataFilePath, string trainingMethodName)
        {
            // Load test data
            var dataView = mlContext.LoadData(testDataFilePath);

            // Evaluating Model accuracy
            var predictions = model.Transform(dataView);
            var metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");

            return new EvaluationResults() {
                Accuracy = metrics.Accuracy,
                Auc = metrics.Auc,
                F1Score = metrics.F1Score,
                Model = model,
                TrainingMethodName = trainingMethodName
            };
        }

        private static EvaluationResults Train(MLContext mlContext, string trainingDataFilePath, string testDataFilePath)
        {
            IDataView dataView = mlContext.LoadData(trainingDataFilePath);

            // Create the different pipelines
            var sgdPipeline = mlContext.Transforms.Text.FeaturizeText(inputColumnName: "UtteranceText", outputColumnName: "Features")
                .Append(mlContext.BinaryClassification.Trainers.StochasticGradientDescent());

            var ftGeneralPipeline = mlContext.Transforms.Text.FeaturizeText(inputColumnName: "UtteranceText", outputColumnName: "Features")
                .Append(mlContext.BinaryClassification.Trainers.FastTree());

            var ftSpecificPipeline = mlContext.Transforms.Text.FeaturizeText(inputColumnName: "UtteranceText", outputColumnName: "Features")
                .Append(mlContext.BinaryClassification.Trainers.FastTree(numLeaves: 30, numTrees: 30, minDatapointsInLeaves: 10));

            // Train the Model using each pipeline
            var ftSpecificModel = ftSpecificPipeline.Fit(dataView);
            var ftGeneralModel = ftGeneralPipeline.Fit(dataView);
            var sgdModel = sgdPipeline.Fit(dataView);

            // Evaluate each model
            var sgdResults = EvaluateModel(mlContext, sgdModel, testDataFilePath, "Stochastic Gradient Descent");
            var ftGeneralResults = EvaluateModel(mlContext, ftGeneralModel, testDataFilePath, "Fast Tree (general)");
            var ftSpecificResults = EvaluateModel(mlContext, ftSpecificModel, testDataFilePath, "Fast Tree (specific)");

            // Return the best model
            return (new List<EvaluationResults>() { sgdResults, ftGeneralResults, ftSpecificResults }).OrderBy(r => r.Accuracy).ThenBy(r => r.Auc).ThenBy(r=> r.F1Score).First();
        }

        private static byte[] SaveModel(MLContext mlContext, ITransformer model)
        {
            byte[] result;

            using (var s = new MemoryStream())
            {
                mlContext.Model.Save(model, s);
                s.Position = 0;

                result = new byte[s.Length + 10];
                int numBytesToRead = (int)s.Length;
                int numBytesRead = 0;
                do
                {
                    // Read may return anything from 0 to 10.
                    int n = s.Read(result, numBytesRead, 10);
                    numBytesRead += n;
                    numBytesToRead -= n;
                } while (numBytesToRead > 0);
                s.Close();
            }

            return result;
        }

    }
}
