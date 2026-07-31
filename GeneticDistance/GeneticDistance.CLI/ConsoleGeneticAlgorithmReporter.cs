using GeneticDistance.Execution;
using GeneticDistance.Execution.Options;
using GeneticDistance.Execution.Reporting;

namespace GeneticDistance.CLI;

public sealed class ConsoleGeneticAlgorithmReporter : IGeneticAlgorithmReporter
{
	public Task RunStartedAsync(GeneticAlgorithmOptions options, CancellationToken cancellationToken = default)
	{
		Console.WriteLine("=== GeneticDistance GA Run ===");
		Console.WriteLine($"Population: {options.PopulationSize}");
		Console.WriteLine($"Generations: {options.Generations}");
		Console.WriteLine($"Survivor ratio: {options.SurvivorRatio:P0}");
		Console.WriteLine($"Mutation range: {options.MutationCountMin}-{options.MutationCountMax}");
		Console.WriteLine();
		return Task.CompletedTask;
	}

	public Task InitialPopulationCreatedAsync(IReadOnlyList<string> seedWords, CancellationToken cancellationToken = default)
	{
		Console.WriteLine("Initial population:");
		Console.WriteLine($"  {string.Join(", ", seedWords)}");
		Console.WriteLine();
		return Task.CompletedTask;
	}

	public Task GenerationCompletedAsync(GenerationReport report, CancellationToken cancellationToken = default)
	{
		Console.WriteLine($"Generation {report.Generation}/{report.TotalGenerations}");
		Console.WriteLine($"  Best this gen: \"{report.BestThisGeneration.FirstWord}\" <-> \"{report.BestThisGeneration.SecondWord}\" = {report.BestThisGeneration.Distance:F5}");
		Console.WriteLine($"  Best overall : \"{report.BestOverall.FirstWord}\" <-> \"{report.BestOverall.SecondWord}\" = {report.BestOverall.Distance:F5}{FormatDelta(report.BestOverallDelta)}");
		Console.WriteLine($"  Population   : {report.PopulationSize} total, {report.Survivors.Count} survivors, {report.Offspring.Count} offspring");
		Console.WriteLine($"  Stats        : avg distance={report.AverageDistance:F5}, duplicate retries={report.DuplicateAttemptsRejected}");

		if (report.Survivors.Count > 0)
			Console.WriteLine($"  Survivors    : {string.Join(", ", report.Survivors)}");

		if (report.Offspring.Count > 0)
		{
			Console.WriteLine("  Offspring:");
			foreach (var offspring in report.Offspring)
			{
				var changes = offspring.Changes.Count == 0
					? "no characteristic change"
					: string.Join(", ", offspring.Changes.Select(change => $"{change.Characteristic}: {change.Before}->{change.After}"));

				Console.WriteLine($"    {offspring.Parent} -> {offspring.Child} ({(offspring.ReusedEmbedding ? "reused embedding" : "new embedding")})");
				Console.WriteLine($"      mutation: {changes}");
			}
		}

		Console.WriteLine();
		return Task.CompletedTask;
	}

	public Task RunCompletedAsync(GeneticAlgorithmRunResult result, CancellationToken cancellationToken = default)
	{
		Console.WriteLine("=== Run complete ===");
		Console.WriteLine($"Best pair: \"{result.FirstWord}\" <-> \"{result.SecondWord}\"");
		Console.WriteLine($"Distance : {result.BestDistance:F5}");
		Console.WriteLine($"Generations executed: {result.GenerationsExecuted}");
		return Task.CompletedTask;
	}

	private static string FormatDelta(float? delta)
	{
		if (delta is null)
			return " (initial best)";

		return delta > 0
			? $" (+{delta.Value:F5})"
			: delta < 0
				? $" ({delta.Value:F5})"
				: " (no change)";
	}
}
