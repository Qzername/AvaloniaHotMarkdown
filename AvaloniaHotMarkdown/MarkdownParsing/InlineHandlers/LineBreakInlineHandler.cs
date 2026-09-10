using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers;

public class LineBreakInlineHandler(StandardMarkdownParser parser) : InlineHandler(parser)
{
    public override void Handle(MarkdownObject inlineObject, InlineParsingContext context, TextUpdateRequestHandler textUpdateHandler)
    {
        context.CurrentLine = new StretchWrapPanel();
        context.Container.Children.Add(context.CurrentLine);
        context.XOffset = 0;
        context.CurrentLine.Tag = new CaretPositionOffset(0, ++context.YOffset);

        context.DefaultFinalizationOfLine();
    }
}
