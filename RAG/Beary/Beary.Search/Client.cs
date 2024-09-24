using Beary.Interfaces;
using Beary.Search.Extensions;
using Beary.ValueTypes;

namespace Beary.Search;

public class Client: IFindDocuments
{
    private readonly IGetEmbeddings _embeddingsClient;
    private readonly IReadContent _readRepo;


    public Client(IGetEmbeddings embeddingsClient, IReadContent readRepo)
    {
        _embeddingsClient = embeddingsClient;
        _readRepo = readRepo;
    }

    public async Task<IEnumerable<Entities.Article>> GetRelevantArticles(string text, TokenCount maxTokenCount)
    {
        IEnumerable<Beary.Entities.Article> articles = [];
        var requestId = Guid.NewGuid().ToString();
        var embeddedText = await _embeddingsClient.GetEmbedding(text, requestId);
        if (embeddedText.IsPopulated())
            articles = await _readRepo.GetRelevantArticles(embeddedText!.Embedding!, maxTokenCount);
        return articles;
    }


}
