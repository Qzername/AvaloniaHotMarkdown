using Avalonia.Controls;
using AvaloniaInline = Avalonia.Controls.Documents;
using AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers;
using AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers.Implementations;
using Markdig.Syntax.Inlines;
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

    public static TextBlock ParseContainer(ContainerInline containerInline)
    {
        List<AvaloniaInline.Inline> inlines = new();

        foreach(var inline in containerInline)
        {
            if (inline is null)
                continue;

            inlines.AddRange(ParseInline(inline));
        }

        return new TextBlock()
        {
            Inlines = [.. inlines]
        };
    }

    public static AvaloniaInline.Inline[] ParseInline(MarkdigInline.Inline inline) => FindHandler(inline).Parse(inline);

    static InlineHandlerBase FindHandler(MarkdigInline.Inline inline)
    {
        return handlers.Single(x => x.CanHandle(inline));
    }
}