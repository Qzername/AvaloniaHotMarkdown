using Avalonia.Controls;
using AvaloniaHotMarkdown.Controls;
using AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers;
using AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers.Implementations;
using Markdig.Syntax.Inlines;
using AvaloniaInline = Avalonia.Controls.Documents;
using MarkdigInline = Markdig.Syntax.Inlines;

namespace AvaloniaHotMarkdown.MarkdownParsing;

public static class InlineParser
{
    readonly static InlineHandlerBase[] handlers = [
        new LiteralInlineHandler(),
        new EmphasisInlineHandler(),
        new CodeInlineHandler(),
        new TaskListInlineHandler(),
        new LineBreakInlineHandler(),
        new LinkInlineHandler(),
    ];

    public static CaretSelectableTextBlock ParseContainer(ContainerInline containerInline)
    {
        List<AvaloniaInline.Inline> inlines = new();

        foreach(var inline in containerInline)
        {
            if (inline is null)
                continue;

            if (inline is LineBreakInline && inline.NextSibling is null)
                continue;

            inlines.AddRange(ParseInline(inline));
        }

        return new CaretSelectableTextBlock()
        {
            Inlines = [.. inlines],
            CaretIndex = 3
        };
    }

    public static AvaloniaInline.Inline[] ParseInline(MarkdigInline.Inline inline) => FindHandler(inline).Parse(inline);

    static InlineHandlerBase FindHandler(MarkdigInline.Inline inline)
    {
        return handlers.Single(x => x.CanHandle(inline));
    }
}