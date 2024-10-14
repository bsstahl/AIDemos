using Beary.Application.Extensions;
using Beary.Application.Test.Builders;
using Beary.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beary.Application.Test;

[ExcludeFromCodeCoverage]
public class OneShot_GetChatResponse_Should
{
    [Fact]
    public async Task NotFailIfNoDocumentsAreSupplied()
    {
        var configElements = new List<KeyValuePair<string, string?>>();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configElements)
            .Build();

        var mockChatClient = new ChatCompletionsClientBuilder()
            .SetupResponseToAnyQuery(string.Empty.GetRandom())
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .AddSingleton<OneShot>()
            .AddSingleton<IGetEmbeddings, Application.Test.Mocks.EmbeddingsClient>()
            .AddSingleton<ICreateChatCompletions>(c => mockChatClient.Object)
            .BuildServiceProvider();

        string userQuery = string.Empty.GetRandom();
        IEnumerable<string>? documents = null;

        var target = services.GetRequiredService<OneShot>();
        var actual = await target.GetChatResponse(userQuery, documents!);

        Assert.NotEmpty(actual);
    }

    [Fact]
    public async Task ReturnTheResponseFromTheChatClient()
    {
        var configElements = new List<KeyValuePair<string, string?>>();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configElements)
            .Build();

        var expected = string.Empty.GetRandom();
        var mockChatClient = new ChatCompletionsClientBuilder()
            .SetupResponseToAnyQuery(expected)
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .AddSingleton<OneShot>()
            .AddSingleton<IGetEmbeddings, Application.Test.Mocks.EmbeddingsClient>()
            .AddSingleton<ICreateChatCompletions>(c => mockChatClient.Object)
            .BuildServiceProvider();

        string userQuery = string.Empty.GetRandom();
        var documents = new List<string>()
        {
            string.Empty.GetRandom(),
            string.Empty.GetRandom()
        };

        var target = services.GetRequiredService<OneShot>();
        var actualResponse = await target.GetChatResponse(userQuery, documents);

        var actual = actualResponse.GetLastAgentResponse();
        Assert.Equal(expected, actual?.Value);
    }

    [Fact]
    public async Task SupplyTheCorrectQueryToTheChatClient()
    {
        var configElements = new List<KeyValuePair<string, string?>>();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configElements)
            .Build();

        var expected = string.Empty.GetRandom();
        var mockChatClient = new ChatCompletionsClientBuilder()
            .SetupResponseToAnyQuery(expected)
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .AddSingleton<OneShot>()
            .AddSingleton<IGetEmbeddings, Application.Test.Mocks.EmbeddingsClient>()
            .AddSingleton<ICreateChatCompletions>(c => mockChatClient.Object)
            .BuildServiceProvider();

        string userQuery = string.Empty.GetRandom();
        var documents = new List<string>()
        {
            string.Empty.GetRandom(),
            string.Empty.GetRandom()
        };

        var target = services.GetRequiredService<OneShot>();
        var actualResponse = await target.GetChatResponse(userQuery, documents);

        var actual = actualResponse.GetLastAgentResponse();
        Assert.Equal(expected, actual?.Value);
    }

}
