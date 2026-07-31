using GeneticDistance.Execution.Options;
using GeneticDistance.Domain;
using GeneticDistance.Domain.Entities;
using GeneticDistance.Domain.Enumerations;
using GeneticDistance.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace GeneticDistance.Execution.Services;

public sealed class RandomGenerationStrategy : IGenerationStrategy
{
	private readonly IOptions<GeneticAlgorithmOptions> _options;

	public RandomGenerationStrategy(IOptions<GeneticAlgorithmOptions> options)
	{
		_options = options;
	}

	public CharacteristicMap<PartOfSpeech> PartOfSpeechMap { get; set; } = CreateIdentityMap<PartOfSpeech>();
	public CharacteristicMap<Register> RegisterMap { get; set; } = CreateIdentityMap<Register>();
	public CharacteristicMap<Morphology> MorphologyMap { get; set; } = CreateIdentityMap<Morphology>();
	public CharacteristicMap<Animacy> AnimacyMap { get; set; } = CreateIdentityMap<Animacy>();
	public CharacteristicMap<ScientificDiscipline> ScientificDisciplineMap { get; set; } = CreateIdentityMap<ScientificDiscipline>();
	public CharacteristicMap<Polarity> PolarityMap { get; set; } = CreateIdentityMap<Polarity>();
	public CharacteristicMap<Idiomaticity> IdiomaticityMap { get; set; } = CreateIdentityMap<Idiomaticity>();
	public CharacteristicMap<Concreteness> ConcretenessMap { get; set; } = CreateIdentityMap<Concreteness>();

	public LexicalCharacteristics Transform(LexicalCharacteristics start)
	{
		ArgumentNullException.ThrowIfNull(start);

		var candidate = new LexicalCharacteristics(
			start.PartOfSpeech,
			start.Register,
			start.ScientificDiscipline,
			start.Morphology,
			start.Animacy,
			start.Polarity,
			start.Idiomaticity,
			start.Concreteness);

		var mutationActions = new List<Action>
		{
			() => candidate.PartOfSpeech = Enum.GetValues<PartOfSpeech>().GetRandom(),
			() => candidate.Register = Enum.GetValues<Register>().GetRandom(),
			() => candidate.ScientificDiscipline = Enum.GetValues<ScientificDiscipline>().GetRandom(),
			() => candidate.Morphology = Enum.GetValues<Morphology>().GetRandom(),
			() => candidate.Animacy = Enum.GetValues<Animacy>().GetRandom(),
			() => candidate.Polarity = Enum.GetValues<Polarity>().GetRandom(),
			() => candidate.Idiomaticity = Enum.GetValues<Idiomaticity>().GetRandom(),
			() => candidate.Concreteness = Enum.GetValues<Concreteness>().GetRandom()
		};

		var min = _options.Value.MutationCountMin;
		var max = _options.Value.MutationCountMax;
		var mutationCount = Math.Min(mutationActions.Count, Random.Shared.Next(min, max + 1));
		foreach (var index in Enumerable.Range(0, mutationActions.Count).OrderBy(_ => Random.Shared.Next()).Take(mutationCount))
			mutationActions[index]();

		return new LexicalCharacteristics(
			candidate.PartOfSpeech,
			candidate.Register,
			candidate.ScientificDiscipline,
			candidate.Morphology,
			candidate.Animacy,
			candidate.Polarity,
			candidate.Idiomaticity,
			candidate.Concreteness);
	}

	private static CharacteristicMap<TEnum> CreateIdentityMap<TEnum>() where TEnum : struct, Enum
	{
		var map = Enum.GetValues<TEnum>().ToDictionary(value => value, value => value);
		return new CharacteristicMap<TEnum>(map);
	}
}
