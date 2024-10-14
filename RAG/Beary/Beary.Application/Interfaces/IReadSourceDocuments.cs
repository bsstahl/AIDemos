using Beary.Entities;

namespace Beary.Application.Interfaces;

public interface IReadSourceDocuments
{
    Task<IEnumerable<Document>> GetAllDocuments();
}