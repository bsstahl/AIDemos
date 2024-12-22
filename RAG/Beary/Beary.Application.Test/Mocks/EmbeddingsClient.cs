using Beary.Application.Test.Extensions;
using Beary.Entities;
using Beary.Interfaces;

namespace Beary.Application.Test.Mocks;

[ExcludeFromCodeCoverage]
internal class EmbeddingsClient : IGetEmbeddings
{
    private Exception? _exceptionToThrow;
    private IEnumerable<ContentChunk>? _resultToReturn;

    public void SetupEmbeddingFailure(Exception exceptionToThrow)
    {
        _exceptionToThrow = exceptionToThrow;
    }

    public void SetupResult(IEnumerable<ContentChunk>? resultToReturn)
    {
        _resultToReturn = resultToReturn;
    }

    public Task<ContentChunk?> GetEmbedding(string inputText, string baseId)
    {
        return this.GetEmbedding(inputText, baseId, false);
    }

    public Task<ContentChunk?> GetEmbedding(string inputText, string baseId, bool normalizeInputs)
    {
        return _exceptionToThrow is not null
            ? throw _exceptionToThrow
            : Task.FromResult<ContentChunk?>(this
                .GetEmbedding(inputText, baseId, 0, normalizeInputs));
    }

    public Task<IEnumerable<ContentChunk>> GetEmbeddings(IEnumerable<string> inputText, string baseId)
    {
        return this.GetEmbeddings(inputText, baseId, false);
    }

    public Task<IEnumerable<ContentChunk>> GetEmbeddings(IEnumerable<string> inputText, string baseId, bool normalizeInputs)
    {
        return _exceptionToThrow is not null
            ? throw _exceptionToThrow
            : Task.FromResult<IEnumerable<ContentChunk>>(inputText
                .Select((t, i) => this.GetEmbedding(t, baseId, i, normalizeInputs)));
    }

    private ContentChunk GetEmbedding(string inputText, string baseId, int index, bool normalizeInputs)
    {
        var result = _resultToReturn?.FirstOrDefault();
        if (_resultToReturn is null)
        {
            var query = normalizeInputs ? inputText.Normalize() : inputText;
            var id = $"{baseId}_{index}";
            var embedding = inputText.GetTextEmbedding();
            result = new ContentChunk(id, index, query, embedding);
        }
        return result;
    }

}
