using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using Beary.Articles.FileSystem.Entities;

namespace Beary.Articles.FileSystem.Extensions;

internal static class MarkdownDocumentExtensions
{
    internal static BlogPostMetadata GetMetadata(this MarkdownDocument content)
    {
        var yamlBlock = content.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
        var yaml = yamlBlock?.Lines.ToString() ?? string.Empty;
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        return deserializer.Deserialize<BlogPostMetadata>(yaml);
    }

    internal static IEnumerable<ArticleChunk> GetContentChunks(this MarkdownDocument content)
    {
        var result = new List<ArticleChunk>();

        int i = 0;
        var blocks = content.ToList();
        foreach (var block in blocks)
        {
            var skippableBlock = (block is YamlFrontMatterBlock); // || block is LinkReferenceDefinitionGroup);
            if (!skippableBlock)
            {
                var renderer = new BlogPostRenderer(new StringWriter());
                string blockContent = renderer.Render(block).ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(blockContent)) // Links and images
                {
                    var isHeader = (block is HeadingBlock);
                    result.Add(new ArticleChunk(i, blockContent, isHeader));
                    i++;
                }
            }
        }

        return result;
    }
}
