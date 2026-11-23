using Avalonia.Controls;
using AvaloniaHotMarkdown.Controls;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers.Implementations;

internal class ParagraphBlockHandler(TextBlockClickedCallback textBlockClickedCallback) : BlockHandler<ParagraphBlock>(textBlockClickedCallback)
{
    public override Control[] Parse(ParagraphBlock content, CaretInformation caretInformation)
    {
        if (content.Inline is null)
            return [];
    
        return [InlineParser.ParseContainer(content.Inline, caretInformation, TextBlockClickedCallback)];
    }
}
