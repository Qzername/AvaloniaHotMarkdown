using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal abstract class BlockHandler
{
    StandardMarkdownParser _parser;

    public BlockHandler(StandardMarkdownParser parser)
    {
        _parser = parser;
    }

    public abstract Control Handle(Block block, LineInformation[] lineInformations);
    public abstract void SetCaretPosition(Control control, LineInformation[] lineInformations);

    /// <summary>
    /// Parses the specified block and returns a corresponding control representation.
    /// 
    /// This is due to the fact that some blocks (like list blocks) have nested blocks and inlines, so this method is used to parse those nested elements.
    /// </summary>
    protected Control ParseBlock(Block block, LineInformation[] lineInformation) => _parser.ParseBlock(block, lineInformation);

    protected Control ParseInline(IEnumerable<MarkdownObject> inlineObjects, bool parseAsFullText)
    {
        StackPanel container = new StackPanel();
        container.Orientation = Orientation.Horizontal;
        container.Children.Clear();

        RichTextPresenter currentPresenter = CreateNewPresenter();
        string endings = string.Empty;

        foreach (var markdownObject in inlineObjects)
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

                endings = new string(emphasisInline.DelimiterChar, emphasisInline.DelimiterCount);

                continue;
            }

            if (markdownObject is LiteralInline literal)
                if(parseAsFullText)
                    currentPresenter.Text = $"{endings}{literal.Content.ToString()}{new string(endings.Reverse().ToArray())}";
                else
                    currentPresenter.Text = literal.Content.ToString();

            container.Children.Add(currentPresenter);

            currentPresenter = CreateNewPresenter();
            endings = string.Empty;
        }

        container.ApplyTemplate();

        return container;
    }

    protected RichTextPresenter CreateNewPresenter()
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
