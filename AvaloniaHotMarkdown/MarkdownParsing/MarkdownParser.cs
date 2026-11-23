using Avalonia.Controls;
using AvaloniaHotMarkdown.Controls;
using AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;
using AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers.Implementations;
using AvaloniaHotMarkdown.MarkdownParsing.Extensions;
using Markdig;
using Markdig.Extensions.EmphasisExtras;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing;

/// <summary>
/// New markdown parser aiming to move onto new Run controls instead of creating seperate textblocks for every Inline type
/// </summary>
internal class StandardMarkdownParser : MarkdownParser
{
    readonly BlockHandlerBase[] handlers;

    public StandardMarkdownParser(TextBlockClickedCallback textBlockClickedCallback) : base(textBlockClickedCallback)
    {
        handlers = [
            new BlankBlockHandler(textBlockClickedCallback),
            new EmptyBlockHandler(textBlockClickedCallback),
            new HeadingBlockHandler(textBlockClickedCallback),
            new ListBlockHandler(textBlockClickedCallback),
            new QuoteBlockHandler(textBlockClickedCallback),
            new TableHandler(textBlockClickedCallback),
            new ParagraphBlockHandler(textBlockClickedCallback),
            new ThematicBreakBlockHandler(textBlockClickedCallback),
        ];
    }

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

    public override Control[] Parse(string markdown, CaretInformation caretInformation)
    {
        List<Control> resultControls = new();

        var document = Markdown.Parse(markdown, BuildPipeline());

        foreach(var block in document)
        {
            var handler = FindHandler(block);

            var parsedControls = handler.Parse(block, caretInformation);

            int start = block.Span.Start;
            int end = block.Span.End;

            int currentCaretIndex = caretInformation.CaretIndex;

            if(start < currentCaretIndex && end > currentCaretIndex)
                handler.UpdateSelectionAndCaret(parsedControls, new CaretInformation()
                {
                    CaretIndex = currentCaretIndex - start
                });

            resultControls.AddRange(parsedControls);
        }

        return resultControls.ToArray();
    }

    BlockHandlerBase FindHandler(Block block) 
        => handlers.Single(x => x.CanHandle(block));
}