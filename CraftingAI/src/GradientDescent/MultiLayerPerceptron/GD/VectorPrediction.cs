﻿﻿﻿using GD.Extensions;

namespace GD;

public class VectorPrediction
{
    public int InputNodeCount { get; }
    public int OutputNodeCount { get; }

    public double[] Input { get; set; }
    public double[] Predicted { get; set; }
    public double?[] Expected { get; set; }

    public double? Error => this.Expected.Where(e => e.HasValue).Count().Equals(this.OutputNodeCount)
        ? 0.5 * this.Errors.Where(e => e.HasValue).Sum(e => Math.Pow(e!.Value, 2)) / this.OutputNodeCount
        : throw new ArgumentNullException("Expected values must be provided for all output nodes");

    internal double?[] Errors
        => (this.Expected.Length.Equals(this.OutputNodeCount))
            ? this.Expected.Select((e, i) => e - this.Predicted[i]).ToArray() // Expected - Predicted for correct gradient direction
            : Enumerable.Repeat<double?>(null, this.OutputNodeCount).ToArray();

    public double?[] WeightsErrors
    {
        get
        {
            var cleanedErrors = this.Errors.Clean();
            var depth = this.Input.Length * cleanedErrors.Length;
            var results = new double?[depth];

            // Calculate errors for each weight
            for (int i = 0; i < cleanedErrors.Length; i++) // Loop over each output
            {
                for (int j = 0; j < this.Input.Length; j++) // Loop over each input
                {
                    // Error for a specific weight is the error of the output multiplied by the corresponding input
                    results[i * this.Input.Length + j] = cleanedErrors[i] * this.Input[j];
                }
            }

            return results;
        }
    }

    public double?[] BiasesErrors
        => (this.Expected.Clean().Length.Equals(this.OutputNodeCount))
            ? this.Errors.Clean().Select(e => (double?)e).ToArray()
            : Enumerable.Repeat<double?>(null, this.OutputNodeCount).ToArray();

    public VectorPrediction(int inputNodeCount, int outputNodeCount, double[] input, double[] value, double?[] expected)
    {
        InputNodeCount = inputNodeCount;
        OutputNodeCount = outputNodeCount;

        Input = input;
        Predicted = value;
        Expected = expected;
    }

}
