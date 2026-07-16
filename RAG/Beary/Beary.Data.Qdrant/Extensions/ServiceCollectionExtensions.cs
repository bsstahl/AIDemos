using Beary.Data.Db;
using Beary.Data.Db.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beary.Data.Qdrant.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseQdrantBearyDb(this IServiceCollection services)
    {
        return services
            .AddSingleton<IBearyDataStore>(c =>
            {
                var config = c.GetRequiredService<IConfiguration>();
                var host = config["Qdrant:Host"] ?? "localhost";
                var port = config.GetValue("Qdrant:GrpcPort", 6334);
                var apiKey = config["Qdrant:ApiKey"];
                var vectorSize = config.GetValue("Qdrant:VectorSize", 768u);
                return new QdrantDataStore(host, port, apiKey, vectorSize);
            })
            .UseBearyDataStoreAbstraction();
    }
}
