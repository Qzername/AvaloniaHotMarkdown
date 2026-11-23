using Avalonia.Controls;
using AvaloniaHotMarkdown.Controls;

namespace AvaloniaHotMarkdown.MarkdownParsing;

/// <summary>
/// Converts markdown text into an Avalonia Controls that can be rendered in the UI.
/// </summary>
public abstract class MarkdownParser
{
    protected TextBlockClickedCallback TextBlockClickedCallback { get; private set; }

    public MarkdownParser(TextBlockClickedCallback textBlockClickedCallback)
    {
        TextBlockClickedCallback = textBlockClickedCallback;
    }

    public abstract Control[] Parse(string markdown, CaretInformation caretInformation);
}
