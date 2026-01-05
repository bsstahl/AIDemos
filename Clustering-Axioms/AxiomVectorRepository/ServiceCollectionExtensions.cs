using Microsoft.Extensions.DependencyInjection;
using Qdrant.Client;

namespace AxiomVectorRepository;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddQdrantClient(this IServiceCollection services, string hostName)
		=> services.AddSingleton<QdrantClient>(c => new QdrantClient(hostName));

	public static IServiceCollection AddQdrantWriteClient(this IServiceCollection services, string hostName)
		=> services.AddQdrantClient(hostName).AddSingleton<IWriteAxiomEmbeddings, WriteRepo>();

    public static IServiceCollection AddQdrantReadClient(this IServiceCollection services, string hostName)
        => services.AddQdrantClient(hostName).AddSingleton<IGetAxiomVectors, ReadRepo>();
}
