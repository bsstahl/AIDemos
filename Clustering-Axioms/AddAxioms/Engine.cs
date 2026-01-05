using AxiomVectorRepository;

namespace AddAxioms;

internal class Engine
{
	private readonly IReadAxioms _readRepo;
	private readonly IGetTextEmbeddings _embedder;
    private readonly IWriteAxiomEmbeddings _writeRepo;

	public Engine(IReadAxioms readRepo, IGetTextEmbeddings embedder, IWriteAxiomEmbeddings writeRepo)
    {
        _readRepo = readRepo;
		_embedder = embedder;
		_writeRepo = writeRepo;
	}

    public async Task Process(Action<string> notifications = null)
	{
		var axioms = await _readRepo.GetAllAxiomsAsync();
		foreach (var axiom in axioms)
		{
			var embedding = await _embedder.GetEmbeddingAsync(axiom);
			if (notifications is not null) 
				notifications.Invoke($"Adding axiom: {axiom} - Embedding Length: {embedding.Length}");
			await _writeRepo.AddAxiomAsync(axiom, embedding);
		}
	}

}
