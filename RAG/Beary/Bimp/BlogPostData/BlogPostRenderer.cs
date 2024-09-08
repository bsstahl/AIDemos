using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using Markdig.Syntax;

namespace Bimp.BlogPostData;

internal class BlogPostRenderer : TextRendererBase<BlogPostRenderer>
{
    public BlogPostRenderer(TextWriter writer)
        : base(writer)
    {
        ObjectRenderers.Add(new ListRenderer());
        ObjectRenderers.Add(new CodeBlockRenderer());
        ObjectRenderers.Add(new HtmlBlockRenderer());
        ObjectRenderers.Add(new QuoteBlockRenderer());
        ObjectRenderers.Add(new ThematicBreakRenderer());
        ObjectRenderers.Add(new ParagraphRenderer());
        ObjectRenderers.Add(new FencedCodeBlockRenderer());
        ObjectRenderers.Add(new HeadingRenderer());

        // Default inline renderers
        ObjectRenderers.Add(new AutolinkInlineRenderer());
        ObjectRenderers.Add(new CodeInlineRenderer());
        ObjectRenderers.Add(new DelimiterInlineRenderer());
        ObjectRenderers.Add(new EmphasisInlineRenderer());
        ObjectRenderers.Add(new LineBreakInlineRenderer());
        ObjectRenderers.Add(new HtmlInlineRenderer());
        ObjectRenderers.Add(new HtmlEntityInlineRenderer());
        ObjectRenderers.Add(new LinkInlineRenderer());
        ObjectRenderers.Add(new LiteralInlineRenderer());
    }
}

internal abstract class BlogPostObjectRenderer<T> : MarkdownObjectRenderer<BlogPostRenderer, T>
    where T : MarkdownObject
{
    protected override void Write(BlogPostRenderer renderer, T obj)
    {
        if (obj is ContainerBlock container)
            renderer.WriteChildren(container);
        else if (obj is LeafBlock leaf)
            renderer.WriteLeafInline(leaf);
        else if (obj is Inline inline)
            renderer.WriteLine($"\r\n<!-- Inline: {inline.Line} -->");
        else
            renderer.WriteLine($"\r\n<!-- {obj.Line} -->");
    }
}

internal class CodeBlockRenderer : BlogPostObjectRenderer<CodeBlock>
{ }

internal class HtmlBlockRenderer : BlogPostObjectRenderer<HtmlBlock>
{ }

internal class QuoteBlockRenderer : BlogPostObjectRenderer<QuoteBlock>
{ }

internal class ThematicBreakRenderer : BlogPostObjectRenderer<ThematicBreakBlock>
{ }

internal class ParagraphRenderer : BlogPostObjectRenderer<ParagraphBlock>
{ }


internal class ListRenderer : MarkdownObjectRenderer<BlogPostRenderer, ListBlock>
{
    protected override void Write(BlogPostRenderer renderer, ListBlock listBlock)
    {
        WriteList(renderer, listBlock, 0);
    }

    private void WriteList(BlogPostRenderer renderer, ListBlock listBlock, int indentLevel)
    {
        foreach (var item in listBlock)
        {
            if (item is ListItemBlock listItem)
            {
                renderer.Write(new string(' ', indentLevel * 2));
                renderer.Write("- ");
                foreach (var subItem in listItem)
                {
                    if (subItem is ParagraphBlock paragraph)
                    {
                        renderer.WriteChildren(paragraph.Inline);
                        // renderer.WriteLeafInline(paragraph.Inline);
                        renderer.WriteLine();
                    }
                    else if (subItem is ListBlock subListBlock)
                    {
                        WriteList(renderer, subListBlock, indentLevel + 1);
                    }
                }
            }
        }
    }
}

internal class HeadingRenderer : MarkdownObjectRenderer<BlogPostRenderer, HeadingBlock>
{
    protected override void Write(BlogPostRenderer renderer, HeadingBlock obj)
    {
        // Determine the heading level (e.g., <h1>, <h2>, etc.)
        var headingTag = new string('#', obj.Level);

        // Open the heading tag
        renderer.Write($"{headingTag} ");

        // Render the inline elements within the heading
        foreach (var inline in obj.Inline)
        {
            WriteInline(renderer, inline);
        }
    }

    private void WriteInline(BlogPostRenderer renderer, Inline inline)
    {
        switch (inline)
        {
            case LiteralInline literal:
                renderer.Write(literal.Content);
                break;
            case LineBreakInline lineBreak:
                renderer.Write(lineBreak.IsHard ? "<br />" : "\n");
                break;
            case EmphasisInline emphasis:
                var tag = emphasis.DelimiterChar == '*' || emphasis.DelimiterChar == '_' ? "em" : "strong";
                renderer.Write($"<{tag}>");
                foreach (var child in emphasis)
                {
                    WriteInline(renderer, child);
                }
                renderer.Write($"</{tag}>");
                break;
            case LinkInline link:
                if (link.IsImage)
                {
                    renderer.Write($"<img src=\"{link.Url}\" alt=\"{link.FirstChild}");
                    if (!string.IsNullOrEmpty(link.Title))
                    {
                        renderer.Write($"\" title=\"{link.Title}");
                    }
                    renderer.Write("\" />");
                }
                else
                {
                    renderer.Write($"<a href=\"{link.Url}\"");
                    if (!string.IsNullOrEmpty(link.Title))
                    {
                        renderer.Write($" title=\"{link.Title}\"");
                    }
                    renderer.Write(">");
                    foreach (var child in link)
                    {
                        WriteInline(renderer, child);
                    }
                    renderer.Write("</a>");
                }
                break;
            case CodeInline code:
                renderer.Write($"<code>{code.Content}</code>");
                break;
            case AutolinkInline autolink:
                renderer.Write($"<a href=\"{autolink.Url}\">{autolink.Url}</a>");
                break;
            case HtmlEntityInline htmlEntity:
                renderer.Write(htmlEntity.Transcoded.ToString());
                break;
            default:
                WriteInline(renderer, inline);
                break;
        }
    }
}

internal class LiteralInlineRenderer : MarkdownObjectRenderer<BlogPostRenderer, LiteralInline>
{
    protected override void Write(BlogPostRenderer renderer, LiteralInline obj)
    {
        renderer.Write(obj.Content);
    }
}

internal class CodeInlineRenderer : MarkdownObjectRenderer<BlogPostRenderer, CodeInline>
{
    protected override void Write(BlogPostRenderer renderer, CodeInline obj)
    {
        renderer.Write($"```\r\n{obj.Content}\r\n```");
    }
}

internal class LinkInlineRenderer : MarkdownObjectRenderer<BlogPostRenderer, LinkInline>
{
    protected override void Write(BlogPostRenderer renderer, LinkInline link)
    {
        if (link.IsImage)
        {
            // For images, render as ![alt](url "title")
            renderer.Write("![").Write(link.FirstChild ?? new LiteralInline("link"));
            renderer.Write("](").Write(link.Url);
            if (!string.IsNullOrEmpty(link.Title))
            {
                renderer.Write(" \"").Write(link.Title).Write("\"");
            }
            renderer.Write(")");
        }
        else
        {
            // For regular links, render as [text](url "title")
            renderer.Write("[").WriteChildren(link);
            renderer.Write("](").Write(link.Url);
            if (!string.IsNullOrEmpty(link.Title))
            {
                renderer.Write(" \"").Write(link.Title).Write("\"");
            }
            renderer.Write(")");
        }


        //renderer.Write("[");
        //renderer.Write(linkText);
        //renderer.Write("]");
        // renderer.Write($"({link.Url})");
        // renderer.Write(link.Url?.ToString() ?? "link");
    }
}

internal class FencedCodeBlockRenderer : MarkdownObjectRenderer<BlogPostRenderer, FencedCodeBlock>
{
    protected override void Write(BlogPostRenderer renderer, FencedCodeBlock obj)
    {
        var openingFence = new string(obj.FencedChar, obj.OpeningFencedCharCount) + obj.Info ?? string.Empty;
        var closingFence = new string(obj.FencedChar, obj.ClosingFencedCharCount);
        var text = string.Join("\r\n", obj.Lines.Lines.Where(l => !string.IsNullOrEmpty(l.ToString())));
        renderer.Write($"{openingFence}\r\n{text}\r\n{closingFence}");
    }
}

internal class AutolinkInlineRenderer : MarkdownObjectRenderer<BlogPostRenderer, AutolinkInline>
{
    protected override void Write(BlogPostRenderer renderer, AutolinkInline obj)
    {
        throw new NotImplementedException();
    }
}

internal class DelimiterInlineRenderer : MarkdownObjectRenderer<BlogPostRenderer, DelimiterInline>
{
    protected override void Write(BlogPostRenderer renderer, DelimiterInline obj)
    {
        renderer.Write(obj.ToLiteral());
        renderer.WriteChildren(obj);
    }
}

internal class EmphasisInlineRenderer : MarkdownObjectRenderer<BlogPostRenderer, EmphasisInline>
{
    protected override void Write(BlogPostRenderer renderer, EmphasisInline obj)
    {
        renderer.Write(obj.DelimiterChar);
        renderer.WriteChildren(obj);
        renderer.Write(obj.DelimiterChar);
    }
}

internal class LineBreakInlineRenderer : MarkdownObjectRenderer<BlogPostRenderer, LineBreakInline>
{
    protected override void Write(BlogPostRenderer renderer, LineBreakInline obj)
    {
        // No need to render hard line breaks
    }
}

internal class HtmlInlineRenderer : MarkdownObjectRenderer<BlogPostRenderer, HtmlInline>
{
    protected override void Write(BlogPostRenderer renderer, HtmlInline obj)
    {
        renderer.Write(obj.Tag);
    }
}

internal class HtmlEntityInlineRenderer : MarkdownObjectRenderer<BlogPostRenderer, HtmlEntityInline>
{
    protected override void Write(BlogPostRenderer renderer, HtmlEntityInline obj)
    {
        renderer.Write(obj.Original);
    }
}
