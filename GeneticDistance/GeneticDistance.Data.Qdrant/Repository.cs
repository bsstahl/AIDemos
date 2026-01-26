using GeneticDistance.Domain.Entities;
using GeneticDistance.Domain.Interfaces;

namespace GeneticDistance.Data.Qdrant;

public class Repository : IEmbeddingRepository
{
	public Task<IReadOnlyList<Expression>> GetAllAsync(Int32 batchSize = 500)
	{
		throw new NotImplementedException();
	}

	public Task<Expression?> GetByIdAsync(String id)
	{
		throw new NotImplementedException();
	}

	public Task<Expression?> GetByTextAsync(String normalizedText)
	{
		throw new NotImplementedException();
	}

	public Task<String> GetOrCreateAsync(String id, String originalText, Single[] vector)
	{
		throw new NotImplementedException();
	}
}
