using GeneticDistance.Data.Qdrant;
using GeneticDistance.Domain.Entities;
using GeneticDistance.Domain.Interfaces;
using GeneticDistance.Execution.Extensions;
using GeneticDistance.Execution.Options;
using GeneticDistance.Execution.Reporting;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GeneticDistance.Execution.Services;

public sealed class GeneticAlgorithmService
{
	private sealed record PopulationMember(Expression Expression, LexicalCharacteristics Characteristics);
	private sealed record PairDistance(PopulationMember First, PopulationMember Second, float Distance);
	private sealed record GenerationEvaluation(PairDistance BestPair, float AverageDistance, IReadOnlyDictionary<PopulationMember, float> FitnessByMember);
	private sealed record CreatedExpression(Expression Expression, bool ReusedEmbedding, int DuplicateAttemptsRejected);

	private readonly IChatClient _chatClient;
	private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingsClient;
	private readonly IEmbeddingRepository _embeddingRepository;
	private readonly IGenerationStrategy _generationStrategy;
	private readonly IOptions<GeneticAlgorithmOptions> _options;
	private readonly ILogger<GeneticAlgorithmService> _logger;
	private readonly IGeneticAlgorithmReporter _reporter;

	public GeneticAlgorithmService(
		IChatClient chatClient,
		IEmbeddingGenerator<string, Embedding<float>> embeddingsClient,
		IEmbeddingRepository embeddingRepository,
		IGenerationStrategy generationStrategy,
		IOptions<GeneticAlgorithmOptions> options,
		ILogger<GeneticAlgorithmService> logger,
		IGeneticAlgorithmReporter reporter)
	{
		_chatClient = chatClient;
		_embeddingsClient = embeddingsClient;
		_embeddingRepository = embeddingRepository;
		_generationStrategy = generationStrategy;
		_options = options;
		_logger = logger;
		_reporter = reporter;
	}

	public async Task<GeneticAlgorithmRunResult> RunAsync(CancellationToken cancellationToken = default)
	{
		var options = _options.Value;
		ValidateOptions(options);

		_logger.LogInformation(
			"Starting GA run with population={PopulationSize}, generations={Generations}, survivorRatio={SurvivorRatio}, mutationRange={MutationCountMin}-{MutationCountMax}, candidateAttempts={MaxCandidateAttemptsPerIndividual}.",
			options.PopulationSize,
			options.Generations,
			options.SurvivorRatio,
			options.MutationCountMin,
			options.MutationCountMax,
			options.MaxCandidateAttemptsPerIndividual);

		await _reporter.RunStartedAsync(options, cancellationToken);

		var population = await InitializePopulationAsync(options, cancellationToken);
		await _reporter.InitialPopulationCreatedAsync(population.Select(member => member.Expression.Text).ToList(), cancellationToken);

		PairDistance? bestOverall = null;
		float? previousBestDistance = null;

		for (var generation = 1; generation <= options.Generations; generation++)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var evaluation = EvaluatePopulation(population);
			bestOverall = bestOverall is null || evaluation.BestPair.Distance > bestOverall.Distance
				? evaluation.BestPair
				: bestOverall;

			var generationReport = new GenerationReport(
				Generation: generation,
				TotalGenerations: options.Generations,
				BestThisGeneration: ToSummary(evaluation.BestPair),
				BestOverall: ToSummary(bestOverall),
				BestOverallDelta: previousBestDistance is null ? null : bestOverall.Distance - previousBestDistance.Value,
				AverageDistance: evaluation.AverageDistance,
				PopulationSize: population.Count,
				Survivors: [],
				Offspring: [],
				DuplicateAttemptsRejected: 0);

			if (generation == options.Generations)
			{
				await _reporter.GenerationCompletedAsync(generationReport, cancellationToken);
				previousBestDistance = bestOverall.Distance;
				break;
			}

			var evolution = await EvolvePopulationAsync(population, evaluation, options, cancellationToken);

			generationReport = generationReport with
			{
				Survivors = evolution.Survivors.Select(member => member.Expression.Text).ToList(),
				Offspring = evolution.Offspring,
				DuplicateAttemptsRejected = evolution.DuplicateAttemptsRejected
			};

			_logger.LogInformation(
				"Generation {Generation}: best pair '{FirstWord}' <-> '{SecondWord}' ({Distance:F5}), avg distance={AverageDistance:F5}, survivors={SurvivorCount}, offspring={OffspringCount}, duplicateAttemptsRejected={DuplicateAttemptsRejected}.",
				generation,
				evaluation.BestPair.First.Expression.Text,
				evaluation.BestPair.Second.Expression.Text,
				evaluation.BestPair.Distance,
				evaluation.AverageDistance,
				evolution.Survivors.Count,
				evolution.Offspring.Count,
				evolution.DuplicateAttemptsRejected);

			foreach (var offspring in evolution.Offspring)
			{
				var changeSummary = string.Join(
					", ",
					offspring.Changes.Select(change => $"{change.Characteristic}: {change.Before} -> {change.After}"));

				_logger.LogDebug(
					"Offspring parent='{Parent}' child='{Child}' reusedEmbedding={ReusedEmbedding}; changes={Changes}",
					offspring.Parent,
					offspring.Child,
					offspring.ReusedEmbedding,
					changeSummary);
			}

			await _reporter.GenerationCompletedAsync(generationReport, cancellationToken);

			population = evolution.NextGeneration;
			previousBestDistance = bestOverall.Distance;
		}

		if (bestOverall is null)
			throw new InvalidOperationException("Unable to compute a best pair from the genetic algorithm run.");

		var result = new GeneticAlgorithmRunResult
		{
			PopulationSize = options.PopulationSize,
			GenerationsExecuted = options.Generations,
			BestDistance = bestOverall.Distance,
			FirstWord = bestOverall.First.Expression.Text,
			SecondWord = bestOverall.Second.Expression.Text,
			FirstWordId = bestOverall.First.Expression.Id,
			SecondWordId = bestOverall.Second.Expression.Id
		};

		_logger.LogInformation(
			"GA run complete. Best pair: '{FirstWord}' <-> '{SecondWord}' with cosine distance {Distance:F5}.",
			result.FirstWord,
			result.SecondWord,
			result.BestDistance);

		await _reporter.RunCompletedAsync(result, cancellationToken);
		return result;
	}

	private async Task<List<PopulationMember>> InitializePopulationAsync(
		GeneticAlgorithmOptions options,
		CancellationToken cancellationToken)
	{
		var population = new List<PopulationMember>(options.PopulationSize);
		var exclusionSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		while (population.Count < options.PopulationSize)
		{
			var characteristics = LexicalCharacteristics.GetRandom();
			var created = await CreateExpressionAsync(characteristics, exclusionSet, options, cancellationToken);

			if (exclusionSet.Add(created.Expression.Text))
			{
				population.Add(new PopulationMember(created.Expression, characteristics));
				_logger.LogDebug(
					"Seeded population member '{Candidate}' (reusedEmbedding={ReusedEmbedding}, duplicateAttemptsRejected={DuplicateAttemptsRejected}).",
					created.Expression.Text,
					created.ReusedEmbedding,
					created.DuplicateAttemptsRejected);
			}
		}

		return population;
	}

	private async Task<(List<PopulationMember> NextGeneration, IReadOnlyList<PopulationMember> Survivors, IReadOnlyList<OffspringReport> Offspring, int DuplicateAttemptsRejected)> EvolvePopulationAsync(
		IReadOnlyList<PopulationMember> currentPopulation,
		GenerationEvaluation evaluation,
		GeneticAlgorithmOptions options,
		CancellationToken cancellationToken)
	{
		var survivorCount = Math.Max(2, (int)Math.Ceiling(options.PopulationSize * options.SurvivorRatio));
		var survivors = evaluation.FitnessByMember
			.OrderByDescending(pair => pair.Value)
			.Take(survivorCount)
			.Select(pair => pair.Key)
			.ToList();

		var nextGeneration = survivors.ToList();
		var exclusions = nextGeneration
			.Select(member => member.Expression.Text)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var offspringReports = new List<OffspringReport>();
		var duplicateAttemptsRejected = 0;

		while (nextGeneration.Count < options.PopulationSize)
		{
			var parent = survivors[Random.Shared.Next(survivors.Count)];
			var mutatedCharacteristics = _generationStrategy.Transform(parent.Characteristics);
			var created = await CreateExpressionAsync(mutatedCharacteristics, exclusions, options, cancellationToken);
			duplicateAttemptsRejected += created.DuplicateAttemptsRejected;

			if (!exclusions.Add(created.Expression.Text))
			{
				duplicateAttemptsRejected++;
				_logger.LogDebug(
					"Rejected already-selected offspring '{Candidate}' from parent '{Parent}'.",
					created.Expression.Text,
					parent.Expression.Text);
				continue;
			}

			nextGeneration.Add(new PopulationMember(created.Expression, mutatedCharacteristics));
			offspringReports.Add(new OffspringReport(
				Parent: parent.Expression.Text,
				Child: created.Expression.Text,
				ReusedEmbedding: created.ReusedEmbedding,
				Changes: DescribeMutation(parent.Characteristics, mutatedCharacteristics)));
		}

		return (nextGeneration, survivors, offspringReports, duplicateAttemptsRejected);
	}

	private async Task<CreatedExpression> CreateExpressionAsync(
		LexicalCharacteristics characteristics,
		ISet<string> exclusions,
		GeneticAlgorithmOptions options,
		CancellationToken cancellationToken)
	{
		var duplicateAttemptsRejected = 0;

		for (var attempt = 1; attempt <= options.MaxCandidateAttemptsPerIndividual; attempt++)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var rawCandidate = await _chatClient.GetCandidateAsync(characteristics, exclusions);
			var candidate = NormalizeCandidate(rawCandidate);

			if (string.IsNullOrWhiteSpace(candidate) || exclusions.Contains(candidate))
			{
				duplicateAttemptsRejected++;
				_logger.LogDebug(
					"Rejected generated candidate '{Candidate}' on attempt {Attempt} (empty or already excluded).",
					candidate,
					attempt);
				continue;
			}

			var existing = await _embeddingRepository.GetByTextAsync(candidate.NormalizeText());
			if (existing?.Vector is not null && existing.Id is not null)
			{
				_logger.LogDebug("Reused existing embedding for candidate '{Candidate}' with id '{Id}'.", existing.Text, existing.Id);
				return new CreatedExpression(
					new Expression(existing.Id, existing.Text, existing.Vector, characteristics),
					ReusedEmbedding: true,
					DuplicateAttemptsRejected: duplicateAttemptsRejected);
			}

			var embeddingVector = await _embeddingsClient.GetEmbeddingAsync(candidate);
			var candidateId = Guid.NewGuid().ToString("D");
			var persistedId = await _embeddingRepository.GetOrCreateAsync(candidateId, candidate, embeddingVector);
			var persistedExpression = await _embeddingRepository.GetByIdAsync(persistedId)
				?? throw new InvalidOperationException($"Embedding '{persistedId}' was persisted but could not be loaded.");

			if (persistedExpression.Vector is null || persistedExpression.Id is null)
				throw new InvalidOperationException($"Embedding '{persistedId}' loaded without vector or id.");

			_logger.LogDebug("Generated new candidate '{Candidate}' with id '{Id}'.", persistedExpression.Text, persistedExpression.Id);

			return new CreatedExpression(
				new Expression(
					persistedExpression.Id,
					persistedExpression.Text,
					persistedExpression.Vector,
					characteristics),
				ReusedEmbedding: false,
				DuplicateAttemptsRejected: duplicateAttemptsRejected);
		}

		throw new InvalidOperationException(
			$"Unable to generate a unique candidate in {options.MaxCandidateAttemptsPerIndividual} attempts.");
	}

	private static GenerationEvaluation EvaluatePopulation(IReadOnlyList<PopulationMember> population)
	{
		if (population.Count < 2)
			throw new InvalidOperationException("Population must contain at least two members.");

		var fitness = new Dictionary<PopulationMember, float>();
		foreach (var member in population)
			fitness[member] = float.MinValue;

		PairDistance? bestPair = null;
		float distanceSum = 0f;
		int pairCount = 0;

		for (var i = 0; i < population.Count - 1; i++)
		{
			for (var j = i + 1; j < population.Count; j++)
			{
				var left = population[i];
				var right = population[j];

				if (left.Expression.Vector is null || right.Expression.Vector is null)
					throw new InvalidOperationException("Population member is missing vector data.");

				var distance = left.Expression.Vector.CosineDistanceFrom(right.Expression.Vector);
				distanceSum += distance;
				pairCount++;

				if (distance > fitness[left])
					fitness[left] = distance;
				if (distance > fitness[right])
					fitness[right] = distance;

				if (bestPair is null || distance > bestPair.Distance)
					bestPair = new PairDistance(left, right, distance);
			}
		}

		if (bestPair is null)
			throw new InvalidOperationException("Unable to evaluate population pair distances.");

		return new GenerationEvaluation(
			bestPair,
			distanceSum / pairCount,
			fitness);
	}

	private static DistancePairSummary ToSummary(PairDistance pair)
		=> new(pair.First.Expression.Text, pair.Second.Expression.Text, pair.Distance);

	private static IReadOnlyList<MutationChange> DescribeMutation(LexicalCharacteristics before, LexicalCharacteristics after)
	{
		var changes = new List<MutationChange>();
		AddChangeIfDifferent(changes, nameof(LexicalCharacteristics.PartOfSpeech), before.PartOfSpeech, after.PartOfSpeech);
		AddChangeIfDifferent(changes, nameof(LexicalCharacteristics.Register), before.Register, after.Register);
		AddChangeIfDifferent(changes, nameof(LexicalCharacteristics.ScientificDiscipline), before.ScientificDiscipline, after.ScientificDiscipline);
		AddChangeIfDifferent(changes, nameof(LexicalCharacteristics.Morphology), before.Morphology, after.Morphology);
		AddChangeIfDifferent(changes, nameof(LexicalCharacteristics.Animacy), before.Animacy, after.Animacy);
		AddChangeIfDifferent(changes, nameof(LexicalCharacteristics.Polarity), before.Polarity, after.Polarity);
		AddChangeIfDifferent(changes, nameof(LexicalCharacteristics.Idiomaticity), before.Idiomaticity, after.Idiomaticity);
		AddChangeIfDifferent(changes, nameof(LexicalCharacteristics.Concreteness), before.Concreteness, after.Concreteness);
		return changes;
	}

	private static void AddChangeIfDifferent<T>(ICollection<MutationChange> changes, string name, T before, T after)
	{
		if (!EqualityComparer<T>.Default.Equals(before, after))
			changes.Add(new MutationChange(name, before?.ToString() ?? string.Empty, after?.ToString() ?? string.Empty));
	}

	private static string NormalizeCandidate(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return string.Empty;

		var firstLine = value.Split('\n', StringSplitOptions.RemoveEmptyEntries)[0];
		var trimmed = firstLine.Trim().Trim('"', '\'', '`');
		return trimmed.NormalizeText();
	}

	private static void ValidateOptions(GeneticAlgorithmOptions options)
	{
		if (options.PopulationSize < 2)
			throw new InvalidOperationException("GeneticAlgorithm:PopulationSize must be at least 2.");
		if (options.Generations < 1)
			throw new InvalidOperationException("GeneticAlgorithm:Generations must be at least 1.");
		if (options.SurvivorRatio is <= 0f or >= 1f)
			throw new InvalidOperationException("GeneticAlgorithm:SurvivorRatio must be between 0 and 1.");
		if (options.MutationCountMin < 1)
			throw new InvalidOperationException("GeneticAlgorithm:MutationCountMin must be at least 1.");
		if (options.MutationCountMax < options.MutationCountMin)
			throw new InvalidOperationException("GeneticAlgorithm:MutationCountMax must be greater than or equal to MutationCountMin.");
		if (options.MutationCountMax > 8)
			throw new InvalidOperationException("GeneticAlgorithm:MutationCountMax must be less than or equal to 8.");
		if (options.MaxCandidateAttemptsPerIndividual < 1)
			throw new InvalidOperationException("GeneticAlgorithm:MaxCandidateAttemptsPerIndividual must be at least 1.");
	}
}
