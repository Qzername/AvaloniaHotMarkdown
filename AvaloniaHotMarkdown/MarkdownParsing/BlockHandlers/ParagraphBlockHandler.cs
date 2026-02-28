using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Diagnostics;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class ParagraphBlockHandler : IBlockHandler
{
    public Control Handle(Block block)
    {
        ParagraphBlock paragraphBlock = block as ParagraphBlock;

        StackPanel container = new StackPanel();
        container.Orientation = Orientation.Horizontal;
        container.Children.Clear();

        RichTextPresenter currentPresenter = CreateNewPresenter();
        
        foreach (var markdownObject in paragraphBlock.Inline.Descendants())
        {
            Type type = markdownObject.GetType();

            if (markdownObject is EmphasisInline emphasisInline)
            {
                if (emphasisInline.DelimiterChar == '*')
                {
                    if (emphasisInline.DelimiterCount == 1)
                        currentPresenter.FontStyle = FontStyle.Italic;
                    else if (emphasisInline.DelimiterCount == 2)
                        currentPresenter.FontWeight = FontWeight.Bold;
                }
                else if (emphasisInline.DelimiterChar == '~')
                    currentPresenter.ShowStrikethrough = true;
                else if (emphasisInline.DelimiterChar == '_')
                {
                    if (emphasisInline.DelimiterCount == 1)
                        currentPresenter.FontStyle = FontStyle.Italic;
                    else if (emphasisInline.DelimiterCount == 2)
                        currentPresenter.ShowUnderline = true;
                }
                else if (emphasisInline.DelimiterChar == '=')
                    currentPresenter.ShowHighlight = true;

                continue;
            }

            if(markdownObject is LiteralInline literal)
                currentPresenter.Text = literal.Content.ToString();
            
            container.Children.Add(currentPresenter);

            currentPresenter = CreateNewPresenter();
        }

        container.ApplyTemplate();

        return container;
    }

    RichTextPresenter CreateNewPresenter()
    {
        RichTextPresenter currentPresenter = new();
        //TODO: replace this with a style
        currentPresenter.Foreground = Brushes.White;
        currentPresenter.FontSize = 14;
        currentPresenter.HighlightBrush = Brushes.Wheat;
        currentPresenter.CaretBrush = Brushes.White;

        return currentPresenter;
    }
}
