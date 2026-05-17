using Avalonia.Controls;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers;

public class InlineParsingContext
{
    public StackPanel Container { get; } = new StackPanel();
    public DockPanel CurrentLine { get; set; } = new DockPanel();
    public RichTextPresenter CurrentPresenter { get; set; } = StylizationHelper.CreateNewPresenter();
    public int XOffset { get; set; } = 0;
    public int YOffset { get; set; } = 0;
    public bool ParseAsFullText { get; }

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

        CurrentPresenter = StylizationHelper.CreateNewPresenter();
        CurrentPresenter.Tag = new CaretPositionOffset(XOffset, 0);
        CurrentLine.Children.Add(CurrentPresenter);
    }
}
