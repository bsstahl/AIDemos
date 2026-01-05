using Microsoft.Extensions.DependencyInjection;
using AxiomVectorRepository;

namespace AddAxioms;

internal class Program
{
	const string _axiomFilePath = @"./Data/axioms.md";
	const string _dbHostName = "localhost";


	static async Task Main(string[] args)
	{
		var services = new ServiceCollection()
			.AddLocalEmbeddingClient()
			.AddAxiomFileDataSource(_axiomFilePath)
			.AddQdrantWriteClient(_dbHostName)
			.AddSingleton<Engine>()
			.BuildServiceProvider();

		var engine = services.GetRequiredService<Engine>();
		await engine.Process(Notify);
	}

	private static void Notify(string message)
	{
		Console.WriteLine(message);
	}
}
