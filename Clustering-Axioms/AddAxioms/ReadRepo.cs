namespace AddAxioms;

internal class ReadRepo : IReadAxioms
{
    private readonly string _axiomFilePath;

	public ReadRepo(string axiomFilePath)
    {
        _axiomFilePath = axiomFilePath;
    }

    public Task<IEnumerable<string>> GetAllAxiomsAsync()
    {
        var results = new List<string>();
        var lines = File.ReadAllLines(_axiomFilePath);
		return Task.FromResult(lines.AsEnumerable());
    }
}
