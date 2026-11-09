using Avalonia.Controls.Documents;
using Markdig.Syntax.Inlines;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers.Implementations;

internal class LiteralInlineHandler : InlineHandler<LiteralInline>
{
    public override Run[] Parse(LiteralInline literal)
    {
        return [new()
        {
            Text = literal.Content.ToString()
        }];
    }
}
