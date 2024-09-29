using Beary.Documents.Interfaces;
using Beary.ValueTypes;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace Beary.Documents.Test.Mocks;

[ExcludeFromCodeCoverage]
internal class ContentSearchDocumentWriter : IWriteContentSearchDocuments
{
    private readonly Mock<IWriteContentSearchDocuments> _documentWriter;

    public ContentSearchDocumentWriter()
    {
        _documentWriter = new Mock<IWriteContentSearchDocuments>();
        _documentWriter.Setup(w => w.SaveAsync(It.IsAny<Identifier>(),
            It.IsAny<ArticleTitle>(),
            It.IsAny<ArticleContent>(),
            It.IsAny<TokenCount>()));
    }

    public async Task SaveAsync(Identifier id, ArticleTitle title, ArticleContent content, TokenCount tokenCount)
    {
        await _documentWriter.Object.SaveAsync(id, title, content, tokenCount);
    }
}
