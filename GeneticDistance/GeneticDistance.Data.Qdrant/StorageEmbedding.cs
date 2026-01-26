namespace GeneticDistance.Data.Qdrant;

public sealed class StorageEmbedding
{
	public string Id { get; init; }
	public string Text { get; init; } = default!;
	public string NormalizedText { get; init; } = default!;
	public float[] Vector { get; init; } = default!;

	public StorageEmbedding(string id, string text, float[] vector)
	{
		this.Id = id;
		this.Text = text;
		this.NormalizedText = text.NormalizeText();
		this.Vector = vector;
	}
}