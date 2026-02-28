using Avalonia.Controls;
using AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;
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

        var lines = markdown.Split('\n'); //for empty line parsing

        var document = Markdown.Parse(markdown, markdownPipeline);

        for(int i =0; i< document.Count; i++)
        {
            var block = document[i];

            //check for empty lines between blocks, if there are any, add an empty textblock for each of them
            if (i>0 && document[i-1].Line != block.Line -1 )
                for(int j = document[i-1].Line; j < block.Line;j++)
                    if (string.IsNullOrWhiteSpace(lines[j]))
                        controls.Add(new TextBlock());
                    
            controls.Add(ParseBlock(block));
        }

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