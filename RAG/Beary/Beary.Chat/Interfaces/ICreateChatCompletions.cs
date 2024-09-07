using Beary.Chat.Entities;

namespace Beary.Chat.Interfaces;

public interface ICreateChatCompletions
{
    // Execute an asynchronous chat completion request using a
    // history of chat messages as the input and return the results.
    Task<ChatContent> CreateChatCompletionsAsync(IEnumerable<ChatContent> chatContent);
}
