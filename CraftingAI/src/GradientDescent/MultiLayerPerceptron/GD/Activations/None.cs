using GD.Interfaces;

namespace GD.Activations;

public class None : IActivateNeurons
{
    public double Activate(double input)
    {
        return input;
    }
}
