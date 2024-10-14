using Beary.Entities;
using Beary.Interfaces;

namespace Beary.Application.Extensions;

public static class ChatContentExtensions
{
    public static ChatContent? GetLastAgentResponse(this IEnumerable<ChatContent> contents)
    {
        return contents.Last(c => c.Role == ChatRole.Agent);
    }

    internal async static Task<ChatContent> GetChatCompletions(this IEnumerable<ChatContent> chatContents, ICreateChatCompletions chatEngine)
    {
        return await chatEngine.CreateChatCompletionsAsync(chatContents);
    }

}
