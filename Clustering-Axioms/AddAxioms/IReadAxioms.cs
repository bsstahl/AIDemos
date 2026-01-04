namespace AddAxioms;

internal interface IReadAxioms
{
    Task<IEnumerable<String>> GetAllAxiomsAsync();
}