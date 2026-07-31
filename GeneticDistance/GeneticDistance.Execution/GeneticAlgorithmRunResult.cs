namespace GeneticDistance.Execution;

public sealed class GeneticAlgorithmRunResult
{
	public int PopulationSize { get; init; }
	public int GenerationsExecuted { get; init; }
	public float BestDistance { get; init; }
	public string FirstWord { get; init; } = string.Empty;
	public string SecondWord { get; init; } = string.Empty;
	public string? FirstWordId { get; init; }
	public string? SecondWordId { get; init; }
}
