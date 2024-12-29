namespace Regression.Interfaces;

public interface IActivateNeurons
{
    string Name => this.GetType().Name;

    double Activate(double input);

    double[] Activate(double[] inputs) => inputs.Select(i => Activate(i)).ToArray();

}
