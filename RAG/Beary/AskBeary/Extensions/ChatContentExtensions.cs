using Beary.Entities;

namespace AskBeary.Extensions;

internal static class ChatContentExtensions
{
    internal static bool HasUserContext(this IEnumerable<ChatContent> contents)
    {
        return contents.Any(c => c.Role == ChatRole.User);
    }

    internal static void OutputToUser(this ChatContent content)
    {
        var startingColor = Console.ForegroundColor;
        Console.ForegroundColor = content.Role.AsConsoleColor();

        Console.WriteLine();
        Console.WriteLine(content.Value);
        Console.WriteLine();

        Console.ForegroundColor = startingColor;
    }

    internal static void OutputToUser(this IEnumerable<ChatContent> contents)
    {
        contents.ToList().ForEach(c => c.OutputToUser());
    }
}
