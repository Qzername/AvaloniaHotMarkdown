
using Avalonia.Layout;
using Avalonia.Media;

namespace AvaloniaHotMarkdown.MarkdownParsing;

//TODO: replace this with something more suitable for customization
internal static class StylizationHelper
{
    /// <summary>
    /// Creates a new stylized presenter 
    /// </summary>
    internal static RichTextPresenter CreateNewPresenter()
    {
        RichTextPresenter currentPresenter = new();

        currentPresenter.Foreground = Brushes.White;
        currentPresenter.FontSize = 14;
        currentPresenter.HighlightBrush = Brushes.Wheat;
        currentPresenter.CaretBrush = Brushes.White;
        currentPresenter.SelectionBrush = Brushes.Cyan;
        currentPresenter.VerticalAlignment = VerticalAlignment.Center;

        return currentPresenter;
    }
}
