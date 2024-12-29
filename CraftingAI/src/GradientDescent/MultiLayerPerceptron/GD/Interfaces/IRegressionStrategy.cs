using Regression.Interfaces;

namespace GD.Interfaces;

public interface IRegressionStrategy
{
    IPredictScalarValues Process(Action<int, IPredictScalarValues, double>? callback = null);
}