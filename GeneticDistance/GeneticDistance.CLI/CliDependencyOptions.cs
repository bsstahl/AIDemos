namespace GeneticDistance.CLI;

public sealed class CliDependencyOptions
{
	public const string SectionName = "Dependencies";

	public string OllamaEndpoint { get; set; } = "http://localhost:11434";
	public string QdrantHost { get; set; } = "localhost";
	public int QdrantGrpcPort { get; set; } = 6334;
	public string? QdrantApiKey { get; set; }
}
