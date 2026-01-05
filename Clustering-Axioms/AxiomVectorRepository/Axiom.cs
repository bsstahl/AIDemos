namespace AxiomVectorRepository;

public class Axiom(string id, string axiomText, float[] embedding, float? score = null)
{
    public string Id { get; set; } = id;
    public string AxiomText { get; set; } = axiomText;
    public float[] Embedding { get; set; } = embedding;
    public float? Score { get; set; } = score;
}
