using Beary.Application;
using Beary.Application.Interfaces;
using Beary.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beary.Application.Test;

[ExcludeFromCodeCoverage]
public class Import_Execute_Should
{
    [Fact]
    public async Task NotFail()
    {
        var configElements = new List<KeyValuePair<string, string?>>();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configElements)
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .AddSingleton<Import>()
            .AddSingleton<IGetEmbeddings, Application.Test.Mocks.EmbeddingsClient>()
            //.AddSingleton<IReadEmbeddingsSearchDocuments, Mocks.Library>()
            //.AddSingleton<IReadContentSearchDocuments, Mocks.Library>()
            .AddSingleton<IReadSourceDocuments, Application.Test.Mocks.Library>()
            .AddSingleton<IWriteContentSearchDocuments, Application.Test.Mocks.ContentSearchDocumentWriter>()
            .AddSingleton<IWriteEmbeddingsSearchDocuments, Application.Test.Mocks.EmbeddingsSearchDocumentWriter>()
            .BuildServiceProvider();

        var target = services.GetRequiredService<Import>();
        await target.Execute();
    }
}
