using GD.Interfaces;
using Regression.Interfaces;

namespace GD.Strategies;

internal class MultilayerPerceptronStrategy : IRegressionStrategy
{
    public MultilayerPerceptronStrategy(string dataPath)
    {
        throw new NotImplementedException();
    }

    public IPredictScalarValues Process(Action<int, IPredictScalarValues, double>? callback = null)
    {
        throw new NotImplementedException();
    }
}