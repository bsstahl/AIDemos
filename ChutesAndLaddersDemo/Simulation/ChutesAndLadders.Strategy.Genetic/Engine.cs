using ChutesAndLadders.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Strategy.Genetic
{
    public class Engine : Rules.Engine
    {
        const int _dnaStrandLength = 298;
        const double _defaultMisspellingRate = 0.10;

        internal Chromosome[] DNA { get; private set; }

        public Engine() : this(Guid.NewGuid().ToString()) { }
        public Engine(string name) : base(name)
        {
            LoadLinearDNA();
        }

        internal Engine(Engine parent) : this(parent, Guid.NewGuid().ToString(), _defaultMisspellingRate) { }
        internal Engine(Engine parent, double misspellingRate) : this(parent, Guid.NewGuid().ToString(), misspellingRate) { }
        internal Engine(Engine parent, string name) : this(parent, name, _defaultMisspellingRate) { }
        internal Engine(Engine parent, string name, double misspellingRate) : base(name)
        {
            EvolveFrom(parent, misspellingRate);
        }

        public Engine Evolve()
        {
            return new Engine(this);
        }

        public Engine Evolve(string name)
        {
            return new Engine(this, name);
        }

        public Engine Evolve(double misspellingRate)
        {
            return new Engine(this, misspellingRate);
        }

        public Engine Evolve(string name, double misspellingRate)
        {
            return new Engine(this, name, misspellingRate);
        }

        public string ContrastWith(Engine contrastTarget)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < _dnaStrandLength; i++)
            {
                var c = this.DNA[i];
                var c1 = contrastTarget.DNA[i];
                if (c.SelectedEndpoint != c1.SelectedEndpoint)
                    sb.AppendLine($"({c1.StartingPoint},{c1.Spin}) = {c.SelectedEndpoint} and {c1.SelectedEndpoint}");
            }

            return sb.ToString();
        }

        private void LoadRulesFromDNA()
        {
            for (int i = 0; i < _dnaStrandLength; i++)
            {
                // Capture variables for lambda
                var c = this.DNA[i];
                int sPoint = c.StartingPoint;
                int sValue = c.Spin;
                int bestEndpoint = c.SelectedEndpoint;
                this.AddRule(sPoint, sValue, bestEndpoint);
            }
        }

        private void EvolveFrom(Engine parent, double misspellingRate)
        {
            this.DNA = new Chromosome[_dnaStrandLength];
            var random = new Random();
            for (int i = 0; i < _dnaStrandLength; i++)
            {
                var originalChromasome = parent.DNA[i];
                if (random.NextDouble() < misspellingRate)
                    this.DNA[i] = originalChromasome.Evolve();
                else
                    this.DNA[i] = originalChromasome.Copy();
            }

            LoadRulesFromDNA();
        }

        private void LoadLinearDNA()
        {
            this.DNA = new Chromosome[_dnaStrandLength];

            var board = new Entities.GameBoard();
            int index = 0;
            for (int startingPoint = 0; startingPoint < 100; startingPoint++)
            {
                for (byte spin = 1; spin <= 6; spin++)
                {
                    var legalEndpoints = board.GetLegalEndpoints(startingPoint, spin);
                    if ((legalEndpoints.Count() > 1) && (!legalEndpoints.Contains(100)))
                    {
                        int selectedEndpoint = startingPoint + spin;
                        if (!legalEndpoints.Contains(selectedEndpoint))
                            selectedEndpoint = legalEndpoints.ClosestTo(selectedEndpoint);

                        this.DNA[index] = new Chromosome(startingPoint, spin, legalEndpoints, selectedEndpoint);
                        index++;
                    }
                }
            }

            LoadRulesFromDNA();
        }
    }
}
