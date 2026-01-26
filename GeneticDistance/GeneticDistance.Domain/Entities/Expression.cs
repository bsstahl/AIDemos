using GeneticDistance.Domain.ValueTypes;

namespace GeneticDistance.Domain.Entities;

public class Expression
{
	public string? Id { get; set; } = string.Empty;
	public string Text { get; set; } = string.Empty;
	public Vector? Vector { get; set; }
	public LexicalCharacteristics? Characteristics { get; set; }

	public Expression(string text)
	{
		this.Text = text;
	}

	public Expression(string id, string text, Vector vector, LexicalCharacteristics characteristics)
	{
		this.Id = id;
		this.Text = text;
		this.Vector = vector;
		this.Characteristics = characteristics;
	}
}
