using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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
        var results = new List<BlogPostSection>();

        var mdPipeline = new Markdig.MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseYamlFrontMatter()
            .Build();

        var files = Directory.GetFiles(_repoPath, "*.md", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            var content = Markdown.Parse(File.ReadAllText(file), mdPipeline);
            BlogPostMetadata postMetadata = GetMetadata(content);

            var articleDescription = postMetadata.description;
            var tags = postMetadata.tags ?? Array.Empty<string>().ToList();
            var categories = postMetadata.categories ?? Array.Empty<string>().ToList();

            var newPost = new Article(postMetadata.id.ToString(), postMetadata.title, articleDescription, null, postMetadata.isPublished, postMetadata.includeAlways, categories, tags);

            int i = 0;
            var blocks = content.ToList();
            foreach (var block in blocks)
            {
                var skippableBlock = (block is YamlFrontMatterBlock || block is LinkReferenceDefinitionGroup);
                if (!skippableBlock)
                {
                    var renderer = new BlogPostRenderer(new StringWriter());
                    string blockContent = renderer.Render(block).ToString() ?? string.Empty;

                    if (!string.IsNullOrEmpty(blockContent)) // Links and images
                    {
                        var isHeader = (block is HeadingBlock);
                        newPost.AddChunk(i, blockContent, isHeader);
                        i++;
                    }
                }
            }

            results.Add(new BlogPostSection(newPost, postMetadata));
        }

        return results;
    }

    private static BlogPostMetadata GetMetadata(MarkdownDocument content)
    {
        var yamlBlock = content.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
        var yaml = yamlBlock?.Lines.ToString() ?? string.Empty;
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        return deserializer.Deserialize<BlogPostMetadata>(yaml);
    }
}