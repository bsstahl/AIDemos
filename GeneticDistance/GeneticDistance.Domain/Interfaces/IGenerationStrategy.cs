using GeneticDistance.Domain.Entities;
using GeneticDistance.Domain.Enumerations;

namespace GeneticDistance.Domain.Interfaces;

public interface IGenerationStrategy
{
	public CharacteristicMap<PartOfSpeech> PartOfSpeechMap { get; set; }
	public CharacteristicMap<Register> RegisterMap { get; set; }
	public CharacteristicMap<Morphology> MorphologyMap { get; set; }
	public CharacteristicMap<Animacy> AnimacyMap { get; set; }
	public CharacteristicMap<ScientificDiscipline> ScientificDisciplineMap { get; set; }
	public CharacteristicMap<Polarity> PolarityMap { get; set; }
	public CharacteristicMap<Idiomaticity> IdiomaticityMap { get; set; }
	public CharacteristicMap<Concreteness> ConcretenessMap { get; set; }


	LexicalCharacteristics Transform(LexicalCharacteristics start)
		=> new LexicalCharacteristics(
			this.PartOfSpeechMap.Map(start.PartOfSpeech),
			this.RegisterMap.Map(start.Register),
			this.ScientificDisciplineMap.Map(start.ScientificDiscipline),
			this.MorphologyMap.Map(start.Morphology),
			this.AnimacyMap.Map(start.Animacy),
			this.PolarityMap.Map(start.Polarity),
			this.IdiomaticityMap.Map(start.Idiomaticity),
			this.ConcretenessMap.Map(start.Concreteness));
}
