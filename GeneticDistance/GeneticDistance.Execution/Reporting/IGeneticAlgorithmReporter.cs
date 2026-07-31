using GeneticDistance.Execution.Options;

namespace GeneticDistance.Execution.Reporting;

public interface IGeneticAlgorithmReporter
{
	Task RunStartedAsync(GeneticAlgorithmOptions options, CancellationToken cancellationToken = default);
	Task InitialPopulationCreatedAsync(IReadOnlyList<string> seedWords, CancellationToken cancellationToken = default);
	Task GenerationCompletedAsync(GenerationReport report, CancellationToken cancellationToken = default);
	Task RunCompletedAsync(GeneticAlgorithmRunResult result, CancellationToken cancellationToken = default);
}
