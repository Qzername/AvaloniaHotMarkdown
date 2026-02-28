using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;
using Markdig;
using Markdig.Syntax;
using System.Diagnostics;

namespace AvaloniaHotMarkdown.MarkdownParsing;

public class StandardMarkdownParser : IMarkdownParser
{
    readonly MarkdownPipeline markdownPipeline;

    readonly Dictionary<Type, IBlockHandler> handlers = new()
    {
        { typeof(HeadingBlock), new HeadingBlockHandler() },
        { typeof(ParagraphBlock), new ParagraphBlockHandler() },
    };

    public StandardMarkdownParser()
    {
        markdownPipeline = new MarkdownPipelineBuilder().Build();
    }

    public Control[] Parse(string markdown)
    {
        List<Control> controls = new();

        var document = Markdown.Parse(markdown, markdownPipeline);

        foreach(var block in document)
        {
            Type type = block.GetType();

            if (!handlers.ContainsKey(type))
                continue;

            var parsedControls = handlers[type].Handle(block);
            controls.Add(parsedControls);
        }

        return controls.ToArray();
    }
}
