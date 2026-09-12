using Avalonia.Controls;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers;

public class InlineParsingContext
{
    public StackPanel Container { get; } = new StackPanel();
    public StretchWrapPanel CurrentLine { get; set; } = new StretchWrapPanel();
    public RichTextPresenter CurrentPresenter { get; set; } = StylizationHelper.CreateNewPresenter();
    public int XOffset { get; set; } = 0;
    public int YOffset { get; set; } = 0;
    public bool ParseAsFullText { get; }

    public List<RichTextPresenter> DefaultFinalizationQueue = new List<RichTextPresenter>();

    public InlineParsingContext(bool parseAsFullText, int defaultXOffset)
    {
        ParseAsFullText = parseAsFullText;
        XOffset = defaultXOffset;

        Container.Children.Add(CurrentLine);
        CurrentPresenter.Tag = new CaretPositionOffset(XOffset, YOffset);

        CurrentLine.Children.Add(CurrentPresenter);
    }

    public void DefaultFinalizationOfLine()
    {
        XOffset += CurrentPresenter.Text.Length;

        foreach (var element in DefaultFinalizationQueue)
        {
            element.Tag = new CaretPositionOffset(XOffset, 0);
            XOffset += element.Text.Length;
        }

        DefaultFinalizationQueue.Clear();

        CurrentPresenter = StylizationHelper.CreateNewPresenter();
        CurrentPresenter.Tag = new CaretPositionOffset(XOffset, 0);
        CurrentLine.Children.Add(CurrentPresenter);
    }
}
