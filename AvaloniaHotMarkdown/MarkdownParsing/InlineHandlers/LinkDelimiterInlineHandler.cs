using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers;

internal class LinkDelimiterInlineHandler(StandardMarkdownParser parser) : InlineHandler(parser)
{
    public override void Handle(MarkdownObject inlineObject, InlineParsingContext context, TextUpdateRequestHandler textUpdateHandler)
    {
        LinkDelimiterInline linkDelimiterInline = (LinkDelimiterInline)inlineObject;

        context.CurrentPresenter.Text += linkDelimiterInline.Type == DelimiterType.Open ? '[' : ']';

        context.DefaultFinalizationOfLine();
    }
}
