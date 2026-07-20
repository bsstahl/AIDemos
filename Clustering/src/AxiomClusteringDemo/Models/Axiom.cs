namespace AxiomClusteringDemo.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Performance",
    "CA1812:Avoid uninstantiated internal classes",
    Justification = "The type is instantiated by System.Text.Json during deserialization.")]
internal sealed record Axiom
{
    public required string Text { get; init; }

    public required IReadOnlyList<float> Embedding { get; init; }
}
