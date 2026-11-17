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

        builder.Extensions.Insert(0, new StrictListExtension());
        builder.Extensions.Insert(1, new UnlistTaskListExtension());

        return builder.Build();
    }

    public Control[] Parse(string markdown, CaretInformation caretInformation)
    {
        List<Control> resultControls = new();

        string preparedMarkdown = MarkdownPreparer.PrepareForcedBreaks(markdown);
        var document = Markdown.Parse(preparedMarkdown, BuildPipeline());

        foreach(var block in document)
        {
            var handler = FindHandler(block);

            int start = block.Span.Start;
            int end = block.Span.End;          // inclusive
            int length = block.Span.Length;

            Debug.WriteLine($"{start} {end} {length}");

            var parsedControls = handler.Parse(block);

            foreach(var parsedControl in parsedControls)
                MarkdownPreparer.RemovePreparationFromControl(parsedControl);

            resultControls.AddRange(parsedControls);
        }

        return resultControls.ToArray();
    }

    static BlockHandlerBase FindHandler(Block block) 
        => handlers.Single(x => x.CanHandle(block));
}