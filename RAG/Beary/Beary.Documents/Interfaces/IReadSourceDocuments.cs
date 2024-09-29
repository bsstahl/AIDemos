using Beary.Documents.Entities;

namespace Beary.Documents.Interfaces;

public interface IReadSourceDocuments
{
    Task<IEnumerable<Document>> GetAllDocuments();
}