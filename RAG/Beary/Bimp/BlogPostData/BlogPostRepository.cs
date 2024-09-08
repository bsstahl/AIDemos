using Markdig;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bimp.BlogPostData;

internal class Repository
{
    const string _repoPathKey = "BlogPostRepoPath";

    private string _repoPath = string.Empty;

    public Repository(IServiceProvider services)
    {
        var config = services.GetRequiredService<IConfiguration>();
        _repoPath = config[_repoPathKey]
            ?? throw new InvalidOperationException($"Invalid configuration value '{_repoPathKey}'");
    }

    private IEnumerable<BlogPostSection>? _allPosts;
    private IEnumerable<BlogPostSection> AllPosts
    {
        get
        {
            _allPosts ??= this.Load();
            return _allPosts;
        }
    }

    public Task<IEnumerable<Article>> GetAllPosts()
    {
        return Task.FromResult(this.AllPosts.Select(p => p.Content));
    }

    private IEnumerable<BlogPostSection> Load()
    {
        var mdPipeline = new Markdig.MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseYamlFrontMatter()
            .Build();

        var results = new List<BlogPostSection>();

        var files = Directory.GetFiles(_repoPath, "*.md", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            var rawText = File.ReadAllText(file).Clean();
            var content = Markdown.Parse(rawText, mdPipeline);
            BlogPostMetadata postMetadata = content.GetMetadata();

            var articleDescription = postMetadata.description;
            var tags = postMetadata.tags ?? Array.Empty<string>().ToList();
            var categories = postMetadata.categories ?? Array.Empty<string>().ToList();
            var contentChunks = content.GetContentChunks();

            var newPost = new Article(postMetadata.id.ToString(), postMetadata.title, articleDescription, null, postMetadata.isPublished, postMetadata.includeAlways, categories, tags);
            newPost.AddChunks(contentChunks);
            results.Add(new BlogPostSection(newPost, postMetadata));
        }

        return results;
    }

}