using Regression.Interfaces;

namespace Regression.Activations;

public class Relu : IActivateNeurons
{
    public double Activate(double input)
    {
        return Math.Max(0, input);
    }
}
