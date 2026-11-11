using Avalonia.Controls.Documents;
using Markdig.Syntax.Inlines;
using AvaloniaInlines = Avalonia.Controls.Documents;
using MarkdigInlines = Markdig.Syntax.Inlines;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers.Implementations;

internal class LinkDelimiterInlineHandler : InlineHandler<MarkdigInlines.LinkDelimiterInline>
{
    public override AvaloniaInlines.Inline[] Parse(MarkdigInlines.LinkDelimiterInline inline)
    {
        return [new Run() {
            Text = inline.Type == DelimiterType.Open ? "[" : "]"
        }];
    }
}
