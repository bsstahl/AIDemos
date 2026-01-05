namespace AxiomVectorRepository;

public interface IGetAxiomVectors
{
    Task<IEnumerable<Axiom>> GetAxiomVectors(uint maxCount = 1000);

    Task<Axiom?> GetAxiomVector(string id);
    Task<Axiom?> GetNearestAxiom(float[] vector);
}