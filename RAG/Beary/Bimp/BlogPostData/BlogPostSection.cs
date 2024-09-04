namespace Bimp.BlogPostData;

internal class BlogPostSection
{
    public Article Content { get; set; }
    public BlogPostMetadata Metadata { get; set; }

    public BlogPostSection(Article content, BlogPostMetadata metadata)
    {
        this.Content = content ?? throw new ArgumentNullException(nameof(content));
        this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
    }
}

