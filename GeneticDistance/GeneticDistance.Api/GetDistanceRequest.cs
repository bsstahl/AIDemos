using System.Text.Json.Serialization;
using GeneticDistance.Domain.Entities;
using GeneticDistance.Domain.ValueTypes;

namespace GeneticDistance.Api;

public class GetDistanceRequest
{
	public string SourceText { get; set; }
	public Vector SourceVector { get; set; }
	public LexicalCharacteristics TargetCharacteristics { get; set; }
	public IEnumerable<string> AdditionalExclusions { get; set; }

	public GetDistanceRequest(string sourceText, float[] sourceVector, LexicalCharacteristics targetCharacteristics, IEnumerable<string> additionalExclusions)
		: this(sourceText, Vector.From(sourceVector), targetCharacteristics, additionalExclusions)
	{ }

	[JsonConstructor]
	public GetDistanceRequest(string sourceText, Vector sourceVector, LexicalCharacteristics targetCharacteristics, IEnumerable<string> additionalExclusions)
	{
		this.SourceText = sourceText;
		this.SourceVector = sourceVector;
		this.TargetCharacteristics = targetCharacteristics;
		this.AdditionalExclusions = additionalExclusions;
	}
}
