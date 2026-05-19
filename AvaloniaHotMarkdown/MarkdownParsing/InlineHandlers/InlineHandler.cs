using Avalonia.Controls;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers;

public abstract class InlineHandler(StandardMarkdownParser parser)
{
    public abstract void Handle(MarkdownObject inlineObject, InlineParsingContext context, TextUpdateRequestHandler textUpdateHandler);

    protected Control ParseInline(IEnumerable<MarkdownObject> inlineObjects, bool parseAsFullText, int defaultXOffset = 0) => parser.ParseInline(inlineObjects, parseAsFullText, defaultXOffset);
}
