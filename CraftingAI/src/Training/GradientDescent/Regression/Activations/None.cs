using Regression.Interfaces;

namespace Regression.Activations;

public class None : IActivateNeurons
{
    public double Activate(double input)
    {
        return input;
    }
}
