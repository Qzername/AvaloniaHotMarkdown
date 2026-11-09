using Avalonia.Controls;
using Avalonia.Controls.Documents;
using AvaloniaInline = Avalonia.Controls.Documents;
using AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers;
using AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers.Implementations;
using Markdig.Syntax.Inlines;
using System.Diagnostics;
using System.Text;
using MarkdigInline = Markdig.Syntax.Inlines;

namespace AvaloniaHotMarkdown.MarkdownParsing;

public static class InlineParser
{
    static InlineHandlerBase[] handlers = [
        new LiteralInlineHandler(),
        new TaskListInlineHandler(),
        new EmphasisInlineHandler(),
        new CodeInlineHandler(),
        new LineBreakInlineHandler(),
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