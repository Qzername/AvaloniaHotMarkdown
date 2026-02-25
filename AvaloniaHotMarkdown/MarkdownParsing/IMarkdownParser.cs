using Avalonia.Controls;

namespace AvaloniaHotMarkdown.MarkdownParsing;

/// <summary>
/// Converts markdown text into an Avalonia Controls that can be rendered in the UI.
/// </summary>
public interface IMarkdownParser
{
    public Control[] Parse(string markdown);
}
