namespace AddAxioms;

internal interface IWriteAxiomEmbeddings
{
    Task AddAxiomAsync(String axiomText, Single[] embedding);
}