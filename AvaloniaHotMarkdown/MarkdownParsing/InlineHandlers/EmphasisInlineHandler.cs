using Avalonia.Media;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers;

public class EmphasisInlineHandler(StandardMarkdownParser parser) : InlineHandler(parser)
{
    public override void Handle(MarkdownObject inlineObject, InlineParsingContext context, TextUpdateRequestHandler textUpdateHandler)
    {
        EmphasisInline emphasisInline = (EmphasisInline)inlineObject;

        if (emphasisInline.DelimiterChar == '*')
        {
            if (emphasisInline.DelimiterCount == 1)
                context.CurrentPresenter.FontStyle = FontStyle.Italic;
            else if (emphasisInline.DelimiterCount == 2)
                context.CurrentPresenter.FontWeight = FontWeight.Bold;
        }
        else if (emphasisInline.DelimiterChar == '~')
        {
            if (emphasisInline.DelimiterCount == 1)
                context.CurrentPresenter.FontType = FontType.Subscript;
            else if (emphasisInline.DelimiterCount == 2)
                context.CurrentPresenter.ShowStrikethrough = true;
        }
        else if (emphasisInline.DelimiterChar == '_')
        {
            if (emphasisInline.DelimiterCount == 1)
                context.CurrentPresenter.FontStyle = FontStyle.Italic;
            else if (emphasisInline.DelimiterCount == 2)
                context.CurrentPresenter.ShowUnderline = true;
        }
        else if (emphasisInline.DelimiterChar == '=')
            context.CurrentPresenter.ShowHighlight = true;
        else if (emphasisInline.DelimiterChar == '^')
            context.CurrentPresenter.FontType = FontType.Superscript;

        if (context.ParseAsFullText)
        {
            string ending = new string(emphasisInline.DelimiterChar, emphasisInline.DelimiterCount);
            int index = Array.IndexOf(context.CurrentLine.Children.ToArray(), context.CurrentPresenter);

            RichTextPresenter openingEndingObject = StylizationHelper.CreateNewPresenter();
            openingEndingObject.Text = ending;
            openingEndingObject.Tag = context.CurrentPresenter.Tag;
            context.XOffset += 2;
            context.CurrentPresenter.Tag = new CaretPositionOffset(context.XOffset, 0);

            context.CurrentLine.Children.Insert(index, openingEndingObject);

            RichTextPresenter closingEndingObject = StylizationHelper.CreateNewPresenter();
            closingEndingObject.Text = ending;
            context.DefaultFinalizationQueue.Add(closingEndingObject);

            context.CurrentLine.Children.Insert(index + 2, closingEndingObject);
        }
    }
}
