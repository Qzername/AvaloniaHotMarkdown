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
    public abstract void UpdateTextEffects(Control control, LineInformation[] lineInformations);

    /// <summary>
    /// Parses the specified block and returns a corresponding control representation.
    /// 
    /// This is due to the fact that some blocks (like list blocks) have nested blocks and inlines, so this method is used to parse those nested elements.
    /// </summary>
    protected Control ParseBlock(Block block, LineInformation[] lineInformation) => _parser.ParseBlock(block, lineInformation);

    /// <summary>
    /// Parses a collection of inline Markdown objects and returns a control that visually represents the formatted text
    /// according to Markdown styling rules.
    /// </summary>
    /// <param name="inlineObjects">An enumerable collection of MarkdownObject instances representing the inline elements to be parsed and rendered.</param>
    /// <param name="parseAsFullText">true to parse the content as full text, applying emphasis delimiters to the entire literal; otherwise, false to
    /// render the content without additional emphasis.</param>
    /// <param name="defaultXOffset">The initial horizontal offset, in pixels, to apply to the parsed content for positioning within the container.</param>
    /// <returns>A StackPanel control containing the formatted text representation of the parsed inline Markdown objects.</returns>
    protected Control ParseInline(IEnumerable<MarkdownObject> inlineObjects, bool parseAsFullText, int defaultXOffset = 0)
    {
        StackPanel container = new StackPanel();

        StackPanel line = new StackPanel();
        line.Orientation = Orientation.Horizontal;
        container.Children.Add(line);

        RichTextPresenter currentPresenter = CreateNewPresenter();
        string endings = string.Empty;

        int xOffset = defaultXOffset;
        currentPresenter.Tag = new CaretPositionOffset(xOffset, 0);

        int yOffset = 0;

        foreach (var markdownObject in inlineObjects)
        {
            Type type = markdownObject.GetType();

            if (markdownObject is LineBreakInline)
            {
                line = new StackPanel();
                line.Orientation = Orientation.Horizontal;
                container.Children.Add(line);
                xOffset = 0;
                line.Tag = new CaretPositionOffset(0, ++yOffset);
            }

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
                if (parseAsFullText)
                    currentPresenter.Text = $"{endings}{literal.Content.ToString()}{new string(endings.Reverse().ToArray())}";
                else
                    currentPresenter.Text = literal.Content.ToString();

            xOffset += currentPresenter.Text.Length;

            line.Children.Add(currentPresenter);

            currentPresenter = CreateNewPresenter();
            currentPresenter.Tag = new CaretPositionOffset(xOffset, 0);
            endings = string.Empty;
        }

        container.ApplyTemplate();

        return container;
    }

    /// <summary>
    /// Creates a new stylized presenter 
    /// </summary>
    protected RichTextPresenter CreateNewPresenter()
    {
        RichTextPresenter currentPresenter = new();
        //TODO: replace this with a style
        currentPresenter.Foreground = Brushes.White;
        currentPresenter.FontSize = 14;
        currentPresenter.HighlightBrush = Brushes.Wheat;
        currentPresenter.CaretBrush = Brushes.White;
        currentPresenter.SelectionBrush = Brushes.Cyan;

        return currentPresenter;
    }
}
