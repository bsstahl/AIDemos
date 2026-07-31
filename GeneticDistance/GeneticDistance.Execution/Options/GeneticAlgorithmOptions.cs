namespace GeneticDistance.Execution.Options;

public sealed class GeneticAlgorithmOptions
{
	public const string SectionName = "GeneticAlgorithm";

	public int PopulationSize { get; set; } = 10;
	public int Generations { get; set; } = 100;
	public float SurvivorRatio { get; set; } = 0.5f;
	public int MutationCountMin { get; set; } = 1;
	public int MutationCountMax { get; set; } = 3;
	public int MaxCandidateAttemptsPerIndividual { get; set; } = 8;
}
