using Beary.Chat.Entities;

namespace Beary.Chat.Interfaces;

public interface IDisambiguateQueries
{
    Task<string> Disambiguate(string text, IEnumerable<ChatContent> chatContents);
}
