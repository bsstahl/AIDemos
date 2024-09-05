using ValueOf;

namespace Beary.Chat.Entities;

public class ChatContent : ValueOf<string, ChatContent>
{
    public ChatRole Role { get; set; }

    public static ChatContent From(string content, ChatRole role)
    {
        var result = ChatContent.From(content);
        result.Role = role;
        return result;
    }
}
