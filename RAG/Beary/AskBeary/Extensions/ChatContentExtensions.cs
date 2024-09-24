using Beary.Chat.Entities;

namespace AskBeary.Extensions;

internal static class ChatContentExtensions
{
    internal static bool HasUserContext(this IEnumerable<ChatContent> contents)
    {
        return contents.Any(c => c.Role == ChatRole.User);
    }

    internal static IEnumerable<ChatContent> GetDisambiguationContents(this IEnumerable<ChatContent> chatContents, string text)
    {
        string systemPrompt = "You are an expert at interpreting and clarify ambiguous language in user input. Your primary objective is to fully qualify all ambiguous words and phrases based on the context of the conversation, ensuring clear and precise communication.";
        string disambiguationRequest = "Restate the user's most recent request, and ONLY their most recent request, fully disambiguated. Respond only with the restated user request in the voice of the user.";
        
        var contents = new List<ChatContent>()
        { 
            ChatContent.From(systemPrompt, ChatRole.System)
        };
        
        contents.AddRange(chatContents.Where(c => c.Role != ChatRole.System));
        contents.Add(ChatContent.From(disambiguationRequest, ChatRole.User));
        contents.Add(ChatContent.From(text, ChatRole.User));

        return contents;
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
