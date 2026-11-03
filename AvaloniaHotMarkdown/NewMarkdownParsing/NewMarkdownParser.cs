using Avalonia.Controls;
using AvaloniaHotMarkdown.MarkdownParsing;
using System;
namespace AvaloniaHotMarkdown.NewMarkdownParsing;

/// <summary>
/// New markdown parser aiming to move onto new Run controls instead of creating seperate textblocks for every Inline type
/// </summary>
internal class NewMarkdownParser : IMarkdownParser
{
    public Control[] Parse(string markdown, CaretInformation caretInformation)
    {
        return [];
    }
}
