using Avalonia.Controls;
using Avalonia.Controls.Documents;
using AvaloniaHotMarkdown.MarkdownParsing;
using AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;
using AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers.Implementations;
using AvaloniaHotMarkdown.MarkdownParsing.Extensions;
using Markdig;
using Markdig.Extensions.EmphasisExtras;
using Markdig.Syntax;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AvaloniaHotMarkdown.MarkdownParsing;

/// <summary>
/// New markdown parser aiming to move onto new Run controls instead of creating seperate textblocks for every Inline type
/// </summary>
internal class MarkdownParser : IMarkdownParser
{
    readonly static BlockHandlerBase[] handlers = [
        new BlankBlockHandler(),
        new EmptyBlockHandler(),
        new HeadingBlockHandler(),
        new ListBlockHandler(),
        new QuoteBlockHandler(),
        new TableHandler(),
        new ParagraphBlockHandler(),
        new ThematicBreakBlockHandler(),
    ];

    static MarkdownPipeline BuildPipeline()
    {
        var builder = new MarkdownPipelineBuilder()
            .UseTaskLists()
            .UsePipeTables()
            .UseEmphasisExtras(EmphasisExtraOptions.Strikethrough |
                               EmphasisExtraOptions.Marked |
                               EmphasisExtraOptions.Superscript |
                               EmphasisExtraOptions.Subscript)
            .EnableTrackTrivia()
            .DisableHtml()
            .UseSoftlineBreakAsHardlineBreak();

        builder.Extensions.Insert(0, new BlankLineBlocksExtension());
        builder.Extensions.Insert(1, new StrictListExtension());
        builder.Extensions.Insert(2, new UnlistTaskListExtension());

        return builder.Build();
    }

    public Control[] Parse(string markdown, CaretInformation caretInformation)
    {
        List<Control> resultControls = new();

        var document = Markdown.Parse(markdown, BuildPipeline());
        Debug.WriteLine(document.ToAstString());

        foreach(var block in document)
        {
            var handler = FindHandler(block);

            var parsedControls = handler.Parse(block);

            resultControls.AddRange(parsedControls);
        }

        return resultControls.ToArray();
    }

    static BlockHandlerBase FindHandler(Block block) 
        => handlers.Single(x => x.CanHandle(block));
}