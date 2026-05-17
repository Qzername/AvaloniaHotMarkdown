using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers;

public class LiteralInlineHandler : IInlineHandler
{
    public void Handle(MarkdownObject inlineObject, InlineParsingContext context, TextUpdateRequestHandler textUpdateHandler)
    {
        LiteralInline literal = (LiteralInline)inlineObject;

        context.CurrentPresenter.Text = literal.Content.ToString();

        context.DefaultFinalizationOfLine();
    }
}
