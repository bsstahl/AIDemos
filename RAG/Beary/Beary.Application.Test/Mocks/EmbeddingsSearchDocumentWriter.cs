using Beary.Application.Interfaces;
using Beary.ValueTypes;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace Beary.Application.Test.Mocks;

[ExcludeFromCodeCoverage]
internal class EmbeddingsSearchDocumentWriter : IWriteEmbeddingsSearchDocuments
{
    private readonly Mock<IWriteEmbeddingsSearchDocuments> _writer;

    public EmbeddingsSearchDocumentWriter()
    {
        _writer = new Mock<IWriteEmbeddingsSearchDocuments>();
        _writer.Setup(w => w.SaveAsync(It.IsAny<Identifier>(), 
            It.IsAny<ElementIndex>(), 
            It.IsAny<ArticleContent>(), 
            It.IsAny<Identifier>(), 
            It.IsAny<Vector?>()));
    }

    public async Task SaveAsync(Identifier id, ElementIndex elementIndex, ArticleContent contentChunk, Identifier fullArticleId, Vector? embedding)
    {
        await _writer.Object.SaveAsync(id, elementIndex, contentChunk, fullArticleId, embedding);
    }
}
