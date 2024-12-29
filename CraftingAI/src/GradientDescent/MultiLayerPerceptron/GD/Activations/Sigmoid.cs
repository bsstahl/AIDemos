using GD.Interfaces;

namespace GD.Activations;

public class Sigmoid : IActivateNeurons
{
    public double Activate(double input)
    {
        return 1.0 / (1.0 + Math.Exp(-input));
    }

 }
