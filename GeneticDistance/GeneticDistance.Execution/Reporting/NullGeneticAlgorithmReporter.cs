using GeneticDistance.Execution.Options;

namespace GeneticDistance.Execution.Reporting;

public sealed class NullGeneticAlgorithmReporter : IGeneticAlgorithmReporter
{
	public Task RunStartedAsync(GeneticAlgorithmOptions options, CancellationToken cancellationToken = default) => Task.CompletedTask;

	public Task InitialPopulationCreatedAsync(IReadOnlyList<string> seedWords, CancellationToken cancellationToken = default) => Task.CompletedTask;

	public Task GenerationCompletedAsync(GenerationReport report, CancellationToken cancellationToken = default) => Task.CompletedTask;

	public Task RunCompletedAsync(GeneticAlgorithmRunResult result, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
