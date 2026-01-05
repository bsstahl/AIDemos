namespace AxiomVectorRepository;

public interface IWriteAxiomEmbeddings
{
    Task AddAxiomAsync(String axiomText, Single[] embedding);
}