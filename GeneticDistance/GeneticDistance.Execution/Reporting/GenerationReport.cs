namespace GeneticDistance.Execution.Reporting;

public sealed record DistancePairSummary(string FirstWord, string SecondWord, float Distance);

public sealed record MutationChange(string Characteristic, string Before, string After);

public sealed record OffspringReport(
	string Parent,
	string Child,
	bool ReusedEmbedding,
	IReadOnlyList<MutationChange> Changes);

public sealed record GenerationReport(
	int Generation,
	int TotalGenerations,
	DistancePairSummary BestThisGeneration,
	DistancePairSummary BestOverall,
	float? BestOverallDelta,
	float AverageDistance,
	int PopulationSize,
	IReadOnlyList<string> Survivors,
	IReadOnlyList<OffspringReport> Offspring,
	int DuplicateAttemptsRejected);
