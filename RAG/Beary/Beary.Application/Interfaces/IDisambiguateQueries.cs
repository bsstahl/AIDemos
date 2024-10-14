using Beary.Entities;

namespace Beary.Application.Interfaces;

public interface IDisambiguateQueries
{
    Task<string> Disambiguate(string text, IEnumerable<ChatContent> chatContents);
}
