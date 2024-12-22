using AskBeary.Extensions;
using Beary.Application;
using Beary.Entities;
using Beary.Application.Extensions;

namespace AskBeary;

public class ChatEngine
{
    private readonly Beary.Application.MultiShot _chatClient;

    public ChatEngine(MultiShot chatClient)
    {
        ArgumentNullException.ThrowIfNull(chatClient, nameof(chatClient));
        _chatClient = chatClient;
    }

    public async Task Execute()
    {
        List<ChatContent>? chatContents = null;  // Start with a null context

        bool done = false;
        while (!done)
        {
            var text = this.GetUserInput();
            done = (text is null);

            if (!done)
            {
                var chatResponses = await _chatClient.GetChatResponse(text!, chatContents);
                chatResponses.OutputToUser();
                chatContents = chatResponses.ToList();
            }
        };
    }

    private string? GetUserInput()
    {
        Console.Write("Query ('<return>' to quit): ");
        var response = Console.ReadLine();
        return string.IsNullOrWhiteSpace(response) ? null : response;

        // return "I need to encourage devs to write unit tests. What are my options?";
        // return "I'm thinking about gating my code check-ins to require 80% test coverage. What do you think of this?";

        // return "Best ways to learn about my problem domain?";
        // return "Tell me more";

        // return "What are some critical questions that development teams should ask themselves?";
        // return "What are the goals of these conversations?";

        // return "Introduce yourself";
    }
}
