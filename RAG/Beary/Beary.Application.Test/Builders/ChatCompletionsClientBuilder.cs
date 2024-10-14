using Beary.Entities;
using Beary.Interfaces;

namespace Beary.Application.Test.Builders;

[ExcludeFromCodeCoverage]
internal class ChatCompletionsClientBuilder
{
    private readonly Mock<ICreateChatCompletions> _client = new();

    internal Mock<ICreateChatCompletions> Build() => _client;


    internal ChatCompletionsClientBuilder SetupResponseToAnyQuery(string response)
    {
        _client
            .Setup(c => c.CreateChatCompletionsAsync(It.IsAny<IEnumerable<ChatContent>>()))
            .Returns(Task.FromResult(ChatContent.From(response, ChatRole.Agent)));

        return this;
    }

    internal ChatCompletionsClientBuilder SetupSingleRequestAndResponse(string request, string response)
    {
        // TODO: Fix this so that it actually works
        _client
            .Setup(c => c.CreateChatCompletionsAsync(It.IsAny<IEnumerable<ChatContent>>()))
            .Returns(Task.FromResult(ChatContent.From(response, ChatRole.Agent)));

        return this;
    }
}

