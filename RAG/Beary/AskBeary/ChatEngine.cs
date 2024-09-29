using AskBeary.Extensions;
using Beary.Documents.Entities;
using Beary.Chat.Entities;
using Beary.Chat.Extensions;

namespace AskBeary;

public class ChatEngine
{
    private readonly Beary.Chat.MultiShot _chatClient;
    private readonly Beary.Documents.Search _searchClient;

    public ChatEngine(Beary.Documents.Search searchClient, Beary.Chat.MultiShot chatClient)
    {
        ArgumentNullException.ThrowIfNull(searchClient, nameof(searchClient));
        ArgumentNullException.ThrowIfNull(chatClient, nameof(chatClient));

        _searchClient = searchClient;
        _chatClient = chatClient;
    }

    public async Task Execute()
    {
        var maxTokenCount = 4092;
        var chatContents = new List<ChatContent>();

        bool done = false;
        while (!done)
        {
            var text = this.GetUserInput();
            done = (text is null);

            if (!done)
            {
                IEnumerable<Document> documents = await _searchClient
                    .GetRelevantArticles(text!, maxTokenCount);

                var supportingDocuments = documents.Select(d => d.FullText);

                var previousContext = chatContents.HasUserContext()
                    ? chatContents
                    : null as IEnumerable<ChatContent>;

                var chatResponses = await _chatClient
                    .GetChatResponse(text!, supportingDocuments, previousContext);
                
                chatContents = chatResponses.ToList();
                chatContents.GetLastAgentResponse()?.OutputToUser();
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
