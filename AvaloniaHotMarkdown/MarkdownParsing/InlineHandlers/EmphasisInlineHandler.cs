using Avalonia.Media;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Diagnostics;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers;

public class EmphasisInlineHandler : IInlineHandler
{
    public void Handle(MarkdownObject inlineObject, InlineParsingContext context, TextUpdateRequestHandler textUpdateHandler)
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
            context.CurrentPresenter.ShowStrikethrough = true;
        else if (emphasisInline.DelimiterChar == '_')
        {
            if (emphasisInline.DelimiterCount == 1)
                context.CurrentPresenter.FontStyle = FontStyle.Italic;
            else if (emphasisInline.DelimiterCount == 2)
                context.CurrentPresenter.ShowUnderline = true;
        }
        else if (emphasisInline.DelimiterChar == '=')
            context.CurrentPresenter.ShowHighlight = true;
        
        if(context.ParseAsFullText)
        {
            string ending = new string(emphasisInline.DelimiterChar, emphasisInline.DelimiterCount);
            int index = Array.IndexOf(context.CurrentLine.Children.ToArray(), context.CurrentPresenter);

            RichTextPresenter openingEndingObject = StylizationHelper.CreateNewPresenter();
            openingEndingObject.Text = ending;

            context.CurrentLine.Children.Insert(index, openingEndingObject);

            RichTextPresenter closingEndingObject = StylizationHelper.CreateNewPresenter();
            closingEndingObject.Text = ending;

            context.CurrentLine.Children.Insert(index+2, closingEndingObject);
        }
    }
}
