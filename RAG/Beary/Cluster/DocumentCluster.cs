using Beary.Entities;
using Beary.Chat.Entities;
using Beary.Chat.Interfaces;
using Beary.ValueTypes;
using Cluster.Extensions;
using System.Text;

namespace Cluster;

internal class DocumentCluster
{
    public int Index { get; set; }
    public string? Name { get; set; }
    public Vector Centroid { get; set; }

    private readonly List<SearchResult> _documents = new List<SearchResult>();
    public IEnumerable<SearchResult> Documents => _documents;

    public DocumentCluster(int index, string? name, Vector centroid, IEnumerable<SearchResult> documents)
    {
        this.Index = index;
        this.Name = name;
        this.Centroid = centroid;
        this.AddDocuments(documents);
    }

    public void AddDocument(SearchResult document)
    {
        _documents.Add(document);
    }

    public void AddDocuments(IEnumerable<SearchResult> documents)
    {
        _documents.AddRange(documents);
    }

    internal async Task Identify(ICreateChatCompletions chatClient)
    {
        const string userPrompt = "In one word or short phrase, describe the common characteristics of the paragraphs above";
        const string systemPrompt = "You are an expert at categorizing the paragraphs of multiple related documents, highlighting key themes and information while avoiding redundancy. You analyze content for main ideas, synthesize common themes, ensure logical structure, and balance brevity with completeness.";

        var content = new List<ChatContent>()
        {
            ChatContent.From(systemPrompt, ChatRole.System)
        };

        content.AddRange(this.Documents.AsChatContents(ChatRole.User));
        content.Add(ChatContent.From(userPrompt, ChatRole.User));

        var result = await chatClient.CreateChatCompletionsAsync(content);
        this.Name = result.Value;
    }

    override public string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"### {this.Name}");
        sb.AppendLine();

        foreach (var item in this.Documents.OrderBy(d => d.Content))
            sb.AppendLine(item.Content);

        return sb.ToString();
    }
}
