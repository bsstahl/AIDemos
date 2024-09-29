using Beary.Documents.Interfaces;
using Beary.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Beary.Documents.Test;

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
            .AddSingleton<IGetEmbeddings, Mocks.EmbeddingsClient>()
            //.AddSingleton<IReadEmbeddingsSearchDocuments, Mocks.Library>()
            //.AddSingleton<IReadContentSearchDocuments, Mocks.Library>()
            .AddSingleton<IReadSourceDocuments, Mocks.Library>()
            .AddSingleton<IWriteContentSearchDocuments, Mocks.ContentSearchDocumentWriter>()
            .AddSingleton<IWriteEmbeddingsSearchDocuments, Mocks.EmbeddingsSearchDocumentWriter>()
            .BuildServiceProvider();

        var target = services.GetRequiredService<Import>();
        await target.Execute();
    }
}
