using Beary.Chat.Entities;
using OpenAI.Chat;

namespace Beary.Chat.AzureGpt.Extensions;

internal static class ChatContentExtensions
{
    internal static IEnumerable<ChatMessage> AsChatMessages(this IEnumerable<ChatContent> chatContent)
    {
        return chatContent.Select(c => 
            {
                var part = ChatMessageContentPart.CreateTextMessageContentPart(c.Value);
                ChatMessage message = c.Role switch
                {
                    ChatRole.User => ChatMessage.CreateUserMessage(part),
                    ChatRole.Agent => ChatMessage.CreateAssistantMessage(part),
                    ChatRole.System => ChatMessage.CreateSystemMessage(part),
                    ChatRole.Context => ChatMessage.CreateAssistantMessage(part),
                    _ => throw new InvalidOperationException($"Invald Role {c.Role}")
                };
                return message;
            });
    }
}
