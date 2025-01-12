using GD.Interfaces;
using GD.Test.Extensions;

namespace GD.Test
{
    public class FullyConnectedLayer_Predict_Should
    {
        [Theory]
        // If the inputs are all 0, the output should be the bias
        [InlineData(2, VectorType.AllZeros, 0.0, null, VectorType.AllZeros, 0.0)]
        [InlineData(2, VectorType.AllZeros, 0.5, null, VectorType.AllZeros, 0.5)]
        [InlineData(2, VectorType.AllZeros, 1.0, null, VectorType.AllZeros, 1.0)]
        [InlineData(2, VectorType.AllPointFives, 0.0, null, VectorType.AllZeros, 0.0)]
        [InlineData(2, VectorType.AllPointFives, 0.5, null, VectorType.AllZeros, 0.5)]
        [InlineData(2, VectorType.AllPointFives, 1.0, null, VectorType.AllZeros, 1.0)]
        [InlineData(2, VectorType.AllOnes, 0.0, null, VectorType.AllZeros, 0.0)]
        [InlineData(2, VectorType.AllOnes, 0.5, null, VectorType.AllZeros, 0.5)]
        [InlineData(2, VectorType.AllOnes, 1.0, null, VectorType.AllZeros, 1.0)]

        // If the weights are all 0, the output should be the bias
        [InlineData(2, VectorType.AllZeros, 0.0, null, VectorType.AllPointFives, 0.0)]
        [InlineData(2, VectorType.AllZeros, 0.5, null, VectorType.AllPointFives, 0.5)]
        [InlineData(2, VectorType.AllZeros, 1.0, null, VectorType.AllPointFives, 1.0)]
        [InlineData(2, VectorType.AllZeros, 0.0, null, VectorType.AllOnes, 0.0)]
        [InlineData(2, VectorType.AllZeros, 0.5, null, VectorType.AllOnes, 0.5)]
        [InlineData(2, VectorType.AllZeros, 1.0, null, VectorType.AllOnes, 1.0)]

        // If the biases are all 0, the output should be the sum of the weighted products
        [InlineData(2, VectorType.AllPointFives, 0.0, null, VectorType.AllPointFives, 0.5)]
        [InlineData(2, VectorType.AllPointFives, 0.0, null, VectorType.AllOnes, 1.0)]
        [InlineData(2, VectorType.AllOnes, 0.0, null, VectorType.AllPointFives, 1.0)]
        [InlineData(2, VectorType.AllOnes, 0.0, null, VectorType.AllOnes, 2.0)]

        // All the inputs are 0.5
        [InlineData(2, VectorType.AllPointFives, 0.5, null, VectorType.AllPointFives, 1.0)]
        [InlineData(2, VectorType.AllPointFives, 1.0, null, VectorType.AllPointFives, 1.5)]
        [InlineData(2, VectorType.AllOnes, 0.5, null, VectorType.AllPointFives, 1.5)]
        [InlineData(2, VectorType.AllOnes, 1.0, null, VectorType.AllPointFives, 2.0)]

        // All the inputs are 1
        [InlineData(2, VectorType.AllPointFives, 0.5, null, VectorType.AllOnes, 1.5)]
        [InlineData(2, VectorType.AllPointFives, 1.0, null, VectorType.AllOnes, 2.0)]
        [InlineData(2, VectorType.AllOnes, 0.5, null, VectorType.AllOnes, 2.5)]
        [InlineData(2, VectorType.AllOnes, 1.0, null, VectorType.AllOnes, 3.0)]
        public void ProduceTheProperScalarResponse(int inputCount, VectorType weights, double bias, IActivateNeurons? activation,
            VectorType inputs, double expected)
        {
            activation ??= new Activations.None();
            var w = weights.GetVector(inputCount);
            var x = inputs.GetVector(inputCount);

            var target = new FullyConnectedLayer(inputCount, 1, w, [bias], activation);
            var actual = target.Predict(x).Single();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(2, 1, VectorType.AllZeros, VectorType.AllZeros, null, VectorType.AllZeros, new double[] { 0.0 })]
        [InlineData(3, 2, VectorType.AllZeros, VectorType.AllZeros, null, VectorType.AllZeros, new double[] { 0.0, 0.0 })]
        [InlineData(2, 1, VectorType.AllZeros, VectorType.AllZeros, null, VectorType.AllOnes, new double[] { 0.0 })]
        [InlineData(3, 2, VectorType.AllZeros, VectorType.AllZeros, null, VectorType.AllOnes, new double[] { 0.0, 0.0 })]

        [InlineData(2, 1, VectorType.AllZeros, VectorType.AllOnes, null, VectorType.AllZeros, new double[] { 1.0 })]
        [InlineData(3, 2, VectorType.AllZeros, VectorType.AllOnes, null, VectorType.AllZeros, new double[] { 1.0, 1.0 })]
        [InlineData(2, 1, VectorType.AllZeros, VectorType.AllOnes, null, VectorType.AllOnes, new double[] { 1.0 })]
        [InlineData(3, 2, VectorType.AllZeros, VectorType.AllOnes, null, VectorType.AllOnes, new double[] { 1.0, 1.0 })]

        [InlineData(2, 1, VectorType.AllOnes, VectorType.AllOnes, null, VectorType.AllOnes, new double[] { 3.0 })]
        [InlineData(3, 2, VectorType.AllOnes, VectorType.AllOnes, null, VectorType.AllOnes, new double[] { 4.0, 4.0 })]

        [InlineData(2, 1, VectorType.AllPointFives, VectorType.AllPointFives, null, VectorType.AllPointFives, new double[] { 1.0 })]
        [InlineData(3, 2, VectorType.AllPointFives, VectorType.AllPointFives, null, VectorType.AllPointFives, new double[] { 1.25, 1.25 })]
        public void ProduceTheProperVectorResponse(int inputCount, int outputCount, VectorType weights, VectorType biases, IActivateNeurons? activation,
            VectorType inputs, double[] expected)
        {
            activation ??= new Activations.None();
            var w = weights.GetVector(inputCount * outputCount + outputCount);
            var b = biases.GetVector(outputCount + 1);
            var x = inputs.GetVector(inputCount);

            var target = new FullyConnectedLayer(inputCount, outputCount, w, b, activation);
            var actual = target.Predict(x);

            Assert.Equal(expected, actual);
        }
    }
}