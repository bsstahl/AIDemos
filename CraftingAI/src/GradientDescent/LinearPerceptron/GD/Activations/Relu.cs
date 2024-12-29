using GD.Interfaces;

namespace GD.Activations;

public class Relu : IActivateNeurons
{
    public double Activate(double input)
    {
        return Math.Max(0, input);
    }
}
