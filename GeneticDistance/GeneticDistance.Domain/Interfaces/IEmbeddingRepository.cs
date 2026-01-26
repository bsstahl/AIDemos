using GeneticDistance.Domain.Entities;

namespace GeneticDistance.Domain.Interfaces;

public interface IEmbeddingRepository
{
	public Task<Expression?> GetByTextAsync(string normalizedText);
	public Task<Expression?> GetByIdAsync(string id);

	public Task<string> GetOrCreateAsync(string id, string originalText, float[] vector);

	public Task<IReadOnlyList<Expression>> GetAllAsync(int batchSize = 500);
}
