using System.Text;

namespace ChutesAndLadders.Strategy.Genetic;

public class Engine : Rules.Engine
{
    const int _genomeLength = 298;
    const double _defaultMisspellingRate = 0.10;
    const double _crossoverRate = 0.5;

    internal Gene[] Genome { get; private set; }

    public Engine() : this(Guid.NewGuid().ToString()) { }
    public Engine(string name) : base(name)
    {
        LoadRandomGenome();
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

    public Engine CrossoverWith(Engine crossoverTarget)
    {
        ArgumentNullException.ThrowIfNull(crossoverTarget);
        var rnd = new Random();
        for (int i = 0; i < _genomeLength; i++)
            if (rnd.NextDouble() < _crossoverRate)
                this.Genome[i] = crossoverTarget.Genome[i];
        return this;
    }

    public string ContrastWith(Engine contrastTarget)
    {
        ArgumentNullException.ThrowIfNull(contrastTarget);
        var sb = new StringBuilder();

        for (int i = 0; i < _genomeLength; i++)
        {
            var c = this.Genome[i];
            var c1 = contrastTarget.Genome[i];
            if (c.SelectedEndpoint != c1.SelectedEndpoint)
                sb.AppendLine(System.Globalization.CultureInfo.InvariantCulture, $"({c1.StartingPoint},{c1.Spin}) = {c.SelectedEndpoint} and {c1.SelectedEndpoint}");
        }

        return sb.ToString();
    }

    private void LoadRulesFromGenome()
    {
        for (int i = 0; i < _genomeLength; i++)
        {
            // Capture variables for lambda
            var c = this.Genome[i];
            int sPoint = c.StartingPoint;
            int sValue = c.Spin;
            int bestEndpoint = c.SelectedEndpoint;
            this.AddRule(sPoint, sValue, bestEndpoint);
        }
    }

    private void EvolveFrom(Engine parent, double misspellingRate)
    {
        this.Genome = new Gene[_genomeLength];
        var random = new Random();
        for (int i = 0; i < _genomeLength; i++)
        {
            var originalGene = parent.Genome[i];
            if (random.NextDouble() < misspellingRate)
                this.Genome[i] = originalGene.Evolve();
            else
                this.Genome[i] = originalGene.Copy();
        }

        LoadRulesFromGenome();
    }

    public void LoadLinearGenome()
    {
        this.Genome = new Gene[_genomeLength];

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

                    this.Genome[index] = new Gene(startingPoint, spin, legalEndpoints, selectedEndpoint);
                    index++;
                }
            }
        }

        LoadRulesFromGenome();
    }

    public void LoadRandomGenome()
    {
        this.Genome = new Gene[_genomeLength];

        var board = new Entities.GameBoard();
        int index = 0;
        for (int startingPoint = 0; startingPoint < 100; startingPoint++)
        {
            for (byte spin = 1; spin <= 6; spin++)
            {
                var legalEndpoints = board.GetLegalEndpoints(startingPoint, spin);
                if ((legalEndpoints.Count() > 1) && (!legalEndpoints.Contains(100)))
                {
                    int selectedEndpoint = legalEndpoints.GetRandom();
                    System.Diagnostics.Debug.Assert(legalEndpoints.Contains(selectedEndpoint));
                    this.Genome[index] = new Gene(startingPoint, spin, legalEndpoints, selectedEndpoint);
                    index++;
                }
            }
        }

        LoadRulesFromGenome();
    }

    public void LoadBestPathGenome()
    {
        this.Genome = new Gene[_genomeLength];
        var shortestPathStrategy = new ChutesAndLadders.Strategy.ShortestPath.Engine();

        var board = new Entities.GameBoard();
        int index = 0;
        for (int startingPoint = 0; startingPoint < 100; startingPoint++)
        {
            for (byte spin = 1; spin <= 6; spin++)
            {
                var legalEndpoints = board.GetLegalEndpoints(startingPoint, spin);
                if ((legalEndpoints.Count() > 1) && (!legalEndpoints.Contains(100)))
                {
                    var situation = new Entities.GameSituation() { BoardLocation = startingPoint, Spin = spin, LegalMoves = legalEndpoints, PlayerLocations = new int[] { } };
                    var selectedEndpoint = shortestPathStrategy.GetMove(situation);

                    this.Genome[index] = new Gene(startingPoint, spin, legalEndpoints, selectedEndpoint);
                    index++;
                }
            }
        }

        LoadRulesFromGenome();
    }

}
