namespace AxiomVectorRepository;

public interface IGetAxiomVectors
{
    Task<IEnumerable<Axiom>> GetAxiomVectors(uint maxCount = 1000);
    Task<IEnumerable<Axiom>> GetAxiomVectorsByIds(IEnumerable<string> ids);

    Task<Axiom?> GetAxiomVector(string id);
    Task<Axiom?> GetNearestAxiom(float[] vector);
}