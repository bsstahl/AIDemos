using Beary.Application.Extensions;
using Beary.Application.Interfaces;
using Beary.Application.Test.Builders;
using Beary.Entities;
using Beary.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beary.Application.Test;

[ExcludeFromCodeCoverage]
public class MultiShot_GetChatResponse_Should
{
    [Fact]
    public async Task NotFailIfNoPreviousContextIsSupplied()
    {
        var configElements = new List<KeyValuePair<string, string?>>();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configElements)
            .Build();

        var expectedRespose = string.Empty.GetRandom();
        var mockChatClient = new ChatCompletionsClientBuilder()
            .SetupResponseToAnyQuery(expectedRespose)
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .AddSingleton<MultiShot>()
            .AddSingleton<IGetEmbeddings, Application.Test.Mocks.EmbeddingsClient>()
            .AddSingleton<ICreateChatCompletions>(c => mockChatClient.Object)
            .AddSingleton<IReadEmbeddingsSearchDocuments, Application.Test.Mocks.Library>()
            .AddSingleton<IReadContentSearchDocuments, Application.Test.Mocks.Library>()
            .BuildServiceProvider();

        string userQuery = string.Empty.GetRandom();

        var target = services.GetRequiredService<MultiShot>();
        var actual = await target.GetChatResponse(userQuery, previousContext: null);

        Assert.NotEmpty(actual);
    }

    [Fact]
    public async Task ReturnTheResponseFromTheChatClient()
    {
        var configElements = new List<KeyValuePair<string, string?>>();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configElements)
            .Build();

        var expectedDisambiguation = string.Empty.GetRandom();
        var expectedResponse = string.Empty.GetRandom();

        var mockChatClient = new ChatCompletionsClientBuilder()
            .SetupResponseToAnyQuery(expectedDisambiguation)
            .SetupResponseToAnyQuery(expectedResponse)
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .AddSingleton<MultiShot>()
            .AddSingleton<IGetEmbeddings, Application.Test.Mocks.EmbeddingsClient>()
            .AddSingleton<ICreateChatCompletions>(c => mockChatClient.Object)
            .AddSingleton<IReadEmbeddingsSearchDocuments, Application.Test.Mocks.Library>()
            .AddSingleton<IReadContentSearchDocuments, Application.Test.Mocks.Library>()
            .BuildServiceProvider();

        string userQuery = string.Empty.GetRandom();
        var documents = new List<string>()
        {
            string.Empty.GetRandom(),
            string.Empty.GetRandom()
        };
        var previousContext = new List<ChatContent>()
        {
            ChatContent.From(string.Empty.GetRandom(), ChatRole.System),
            ChatContent.From(string.Empty.GetRandom(), ChatRole.Context),
            ChatContent.From(string.Empty.GetRandom(), ChatRole.User),
            ChatContent.From(string.Empty.GetRandom(), ChatRole.Agent)
        };

        var target = services.GetRequiredService<MultiShot>();
        var actual = await target.GetChatResponse(userQuery, previousContext);

        Assert.Equal(expectedResponse, actual.GetLastAgentResponse()?.Value);
    }

    [Fact]
    public async Task ThrowIfTheEmbeddingReturnsEmpty()
    {
        var configElements = new List<KeyValuePair<string, string?>>();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configElements)
            .Build();

        var expectedRespose = string.Empty.GetRandom();
        var mockChatClient = new ChatCompletionsClientBuilder()
            .SetupResponseToAnyQuery(expectedRespose)
            .Build();

        var embeddingsClient = new Application.Test.Mocks.EmbeddingsClient();
        embeddingsClient.SetupResult([]);

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .AddSingleton<MultiShot>()
            .AddSingleton<IGetEmbeddings>(c => embeddingsClient)
            .AddSingleton<ICreateChatCompletions>(c => mockChatClient.Object)
            .AddSingleton<IReadEmbeddingsSearchDocuments, Application.Test.Mocks.Library>()
            .AddSingleton<IReadContentSearchDocuments, Application.Test.Mocks.Library>()
            .BuildServiceProvider();

        string userQuery = string.Empty.GetRandom();

        var target = services.GetRequiredService<MultiShot>();
        await Assert.ThrowsAsync<InvalidOperationException>(() => target.GetChatResponse(userQuery, previousContext: null));
    }
}