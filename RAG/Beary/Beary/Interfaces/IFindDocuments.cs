using Beary.ValueTypes;

namespace Beary.Interfaces;

public interface IFindDocuments
{
    Task<IEnumerable<Entities.Article>> GetRelevantArticles(string text, TokenCount maxTokenCount);
}
