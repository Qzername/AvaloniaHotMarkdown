using AvaloniaInline = Avalonia.Controls.Documents;
using Markdig.Syntax.Inlines;
using Avalonia.Controls.Documents;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers.Implementations;

internal class LineBreakInlineHandler : InlineHandler<LineBreakInline>
{
    public override AvaloniaInline.Inline[] Parse(LineBreakInline inline)
    {
        return [new LineBreak()];
    }
}
