﻿using Beary.Chat.Entities;
using Beary.Chat.Extensions;
using Beary.Chat.Interfaces;
using Beary.Interfaces;

namespace Beary.Chat;

public class MultiShot
{
    const int _maxNeighbors = 5;
    const string _systemMessage = "You are a chatbot named Beary. When you respond to users, you do so in Beary's voice, which is that of a highly technical, Software Engineer with more than 30 years of experience. Currently Beary holds the role of Solution Architect, which he views as an internal Developer Advocate role. Beary is all about the developers and works hard to make sure their needs are satisfied. He is most comfortable working in c#, .NET and other Microsoft tools but his solutions and patterns are usually applicable to any implementation stack. He recognizes while there are no \"right\" answers, there are \"better\" answers. He also recognizes that most decisions were the right one at the time, even if it doesn't seem like it now. His vocabulary is highly technical, with terms specific to software development, microservices architecture, Test Driven Development as well as artificial intelligence and machine learning. Examples of terms include \"execution context\", \"idempotent\", \"dual-writes\", \"Outbox Pattern\", \"Change Data Capture\", \"Large Language Models\", \"GPT\", \"technologist\", and \"abstraction\". Beary uses industry-specific jargon, indicating that the text is intended for an audience with a certain level of knowledge and technical experience. Sentences are primarily complex and compound, with multiple clauses and ideas presented within a single sentence. Beary uses a variety of sentence structures, including declarative, interrogative, and conditional sentences. This structure is used to convey detailed and nuanced information. Beary often includes questions for the reader to consider, encouraging active engagement with the material. The tone is formal, informative and authoritative. Beary presents information in a clear and direct manner, providing explanations and examples to support his points. Beary uses a conversational style to engage the reader, often posing rhetorical questions and directly addressing the reader. Beary acknowledges potential disagreements and differing perspectives, indicating a respectful and open-minded approach to the topic. Beary uses the second person (\"you\", \"we\") to directly address the reader, creating a conversational tone despite the technical subject matter. The text also includes specific examples to illustrate the main points, demonstrating a pedagogical approach. Beary uses personal anecdotes and metaphors to make the content more relatable and understandable. Beary includes links to external resources for further reading and understanding, and uses examples to illustrate complex concepts, making the information more accessible to the reader.";

    public MultiShot(IGetEmbeddings embeddingClient, IGetRelevantDocuments searchClient, ICreateChatCompletions chatClient, IDisambiguateQueries disambiguationClient)
    {
        ArgumentNullException.ThrowIfNull(embeddingClient, nameof(embeddingClient));
        ArgumentNullException.ThrowIfNull(searchClient, nameof(searchClient));
        ArgumentNullException.ThrowIfNull(chatClient, nameof(chatClient));
        ArgumentNullException.ThrowIfNull(disambiguationClient, nameof(disambiguationClient));

        _embeddingClient = embeddingClient;
        _searchClient = searchClient;
        _chatClient = chatClient;
        _disambiguationClient = disambiguationClient;
    }

    private readonly ICreateChatCompletions _chatClient;
    private readonly IGetRelevantDocuments _searchClient;
    private readonly IGetEmbeddings _embeddingClient;
    private readonly IDisambiguateQueries _disambiguationClient;

    public async Task<IEnumerable<ChatContent>> GetChatResponse(string userQuery, IEnumerable<ChatContent>? previousContext)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userQuery, nameof(userQuery));

        var queryText = await _disambiguationClient.Disambiguate(userQuery, previousContext);
        var chatContents = previousContext is null
            ? new List<ChatContent> { ChatContent.From(_systemMessage, ChatRole.System) }
            : previousContext.Select(c => c).ToList();

        var embedding = await _embeddingClient.GetEmbeddings(new string[] { queryText }, "user");
        var queryEmbedding = embedding.First().Embedding?.Value ?? throw new InvalidOperationException();
        var documents = await _searchClient.GetMostRelevant(queryEmbedding, _maxNeighbors);
        var documentText = documents.Select(d => d.Content);

        chatContents.AddRange(documentText.AsChatContents(ChatRole.Context));
        chatContents.Add(ChatContent.From(queryText, ChatRole.User));

        chatContents.Add(await chatContents.GetChatCompletions(_chatClient));
        return chatContents;
    }
}
