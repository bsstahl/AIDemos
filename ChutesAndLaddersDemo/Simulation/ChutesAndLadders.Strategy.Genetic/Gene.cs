using ChutesAndLadders.Extensions;

namespace ChutesAndLadders.Strategy.Genetic;

internal sealed class Gene
{
    public int StartingPoint { get; set; }
    public byte Spin { get; set; }
    public IEnumerable<int> LegalEndpoints { get; set; }
    public int SelectedEndpoint { get; set; }

    public Gene(int startingPoint, byte spin, IEnumerable<int> legalEndpoints, int selectedEndpoint)
    {
        Validate(startingPoint, spin, legalEndpoints, selectedEndpoint);
        StartingPoint = startingPoint;
        Spin = spin;
        LegalEndpoints = legalEndpoints;
        SelectedEndpoint = selectedEndpoint;
    }

    public Gene Copy()
    {
        return new Gene(this.StartingPoint, this.Spin, this.LegalEndpoints.Copy(), this.SelectedEndpoint);
    }

    public Gene Evolve()
    {
        return new Gene(this.StartingPoint, this.Spin, this.LegalEndpoints.Copy(), this.LegalEndpoints.GetRandom());
    }

    private static void Validate(Int32 startingPoint, Byte spin, IEnumerable<Int32> legalEndpoints, Int32 selectedEndpoint)
    {
        if (!legalEndpoints.Contains(selectedEndpoint))
            throw new ArgumentException($"Invalid gene. {selectedEndpoint} not found among legal endpoints ({string.Join(",", legalEndpoints)}) for starting point {startingPoint} and spin {spin}.");
    }

}
