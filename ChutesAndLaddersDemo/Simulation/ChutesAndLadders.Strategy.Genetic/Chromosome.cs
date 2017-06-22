using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChutesAndLadders.Extensions;

namespace ChutesAndLadders.Strategy.Genetic
{
    internal class Chromosome
    {
        public int StartingPoint { get; set; }
        public byte Spin { get; set; }
        public IEnumerable<int> LegalEndpoints { get; set; }
        public int SelectedEndpoint { get; set; }

        public Chromosome(int startingPoint, byte spin, IEnumerable<int> legalEndpoints, int selectedEndpoint)
        {
            if (!legalEndpoints.Contains(selectedEndpoint))
                throw new ArgumentException($"Invalid Chromosome. {selectedEndpoint} not found among legal endpoints ({string.Join(",", legalEndpoints)}) for starting point {startingPoint} and spin {spin}.");

            StartingPoint = startingPoint;
            Spin = spin;
            LegalEndpoints = legalEndpoints;
            SelectedEndpoint = selectedEndpoint;
        }

        public Chromosome Copy()
        {
            return new Chromosome(this.StartingPoint, this.Spin, this.LegalEndpoints.Copy(), this.SelectedEndpoint);
        }

        public Chromosome Evolve()
        {
            return new Chromosome(this.StartingPoint, this.Spin, this.LegalEndpoints.Copy(), this.LegalEndpoints.GetRandom());
        }
    }
}
