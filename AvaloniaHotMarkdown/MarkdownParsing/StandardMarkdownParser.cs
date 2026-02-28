using Avalonia.Controls;
using AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;
using AvaloniaHotMarkdown.MarkdownParsing.EmptyLineParsing;
using Markdig;
using Markdig.Extensions.EmphasisExtras;
using Markdig.Syntax;
using System.Diagnostics;

namespace AvaloniaHotMarkdown.MarkdownParsing;

public class StandardMarkdownParser : IMarkdownParser
{
    readonly Dictionary<Type, BlockHandler> handlers;
    readonly MarkdownPipeline markdownPipeline;

    public StandardMarkdownParser()
    {
        handlers = new()
        {
            { typeof(ParagraphBlock), new ParagraphBlockHandler(this) },
            { typeof(HeadingBlock), new HeadingBlockHandler(this) },
            { typeof(ListBlock), new ListBlockHandler(this) },
        };

        markdownPipeline = new MarkdownPipelineBuilder()
            .UseEmphasisExtras(EmphasisExtraOptions.Strikethrough | EmphasisExtraOptions.Marked)
            .Build();
    }

    public Control[] Parse(string markdown)
    {
        List<Control> controls = new();

        var document = Markdown.Parse(markdown, markdownPipeline);

        foreach(var block in document)
            controls.Add(ParseBlock(block));

        return controls.ToArray();
    }

    public Control ParseBlock(Block block)
    {
        Type type = block.GetType();

        if (!handlers.ContainsKey(type))
            throw new NotSupportedException("This block is not supported: " + type.Name);

        return handlers[type].Handle(block);
    }
}