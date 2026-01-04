using Microsoft.Extensions.DependencyInjection;
using Qdrant.Client;

namespace AddAxioms;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddAxiomFileDataSource(this IServiceCollection services, string filePath)
		=> services.AddSingleton<IReadAxioms, ReadRepo>(r => new ReadRepo(filePath));

	public static IServiceCollection AddQdrantClient(this IServiceCollection services, string hostName)
		=> services.AddSingleton<QdrantClient>(c => new QdrantClient(hostName));

	public static IServiceCollection AddQdrantWriteClient(this IServiceCollection services, string hostName)
		=> services.AddQdrantClient(hostName).AddSingleton<IWriteAxiomEmbeddings, WriteRepo>();

	public static IServiceCollection AddLocalEmbeddingClient(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddSingleton<IGetTextEmbeddings, TextEmbedder>();
		serviceCollection.AddHttpClient();
		return serviceCollection;
	}
}
