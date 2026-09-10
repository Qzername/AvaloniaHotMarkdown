using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers;

public class CodeInlineHandler(StandardMarkdownParser parser) : InlineHandler(parser)
{
    public override void Handle(MarkdownObject inlineObject, InlineParsingContext context, TextUpdateRequestHandler textUpdateHandler)
    {
        CodeInline codeInline = (CodeInline)inlineObject;

        var presenter = context.CurrentPresenter;

        if (context.ParseAsFullText)
            presenter.Text = $"`{codeInline.Content.ToString()}`";
        else
            presenter.Text = codeInline.Content.ToString();

        presenter.IsCodeInline = true;
    }
}
