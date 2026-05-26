
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace AvaloniaHotMarkdown.MarkdownParsing;

internal static class StylizationHelper
{
    /// <summary>
    /// Creates a new stylized presenter with default classes and properties for markdown rendering.
    /// </summary>
    internal static RichTextPresenter CreateNewPresenter()
    {
        RichTextPresenter currentPresenter = new();

        currentPresenter.Classes.Add("text");

        return currentPresenter;
    }
}
