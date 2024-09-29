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

    protected override void Validate()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(this.Value, nameof(this.Value));
        base.Validate();
    }
}
