using Beary.Entities;
using Beary.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Beary.Application.Test.Mocks;

[ExcludeFromCodeCoverage]
internal class EmbeddingsClient : IGetEmbeddings
{
    public Task<ContentChunk?> GetEmbedding(string inputText, string baseId)
    {
        return this.GetEmbedding(inputText, baseId, false);
    }

    public Task<ContentChunk?> GetEmbedding(string inputText, string baseId, bool normalizeInputs)
    {
        return Task.FromResult<ContentChunk?>(this
            .GetEmbedding(inputText, baseId, 0, normalizeInputs));
    }

    public Task<IEnumerable<ContentChunk>> GetEmbeddings(IEnumerable<string> inputText, string baseId)
    {
        return this.GetEmbeddings(inputText, baseId, false);
    }

    public Task<IEnumerable<ContentChunk>> GetEmbeddings(IEnumerable<string> inputText, string baseId, bool normalizeInputs)
    {
        return Task.FromResult<IEnumerable<ContentChunk>>(inputText
            .Select((t, i) => this.GetEmbedding(t, baseId, i, normalizeInputs)));
    }

    private ContentChunk GetEmbedding(string inputText, string baseId, int index, bool normalizeInputs)
    {
        var query = normalizeInputs ? inputText.Normalize() : inputText;
        var id = $"{baseId}_{index}";
        var embedding = this.GetTextEmbedding(inputText);
        return new ContentChunk(id, index, query, embedding);
    }

    private IEnumerable<float> GetTextEmbedding(string text)
        => BitConverter.GetBytes(text.GetHashCode()).Select(b => (float)b / 255.0f).ToArray();
}
