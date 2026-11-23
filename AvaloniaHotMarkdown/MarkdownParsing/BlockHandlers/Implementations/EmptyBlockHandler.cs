using Avalonia.Controls;
using AvaloniaHotMarkdown.Controls;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers.Implementations;

internal class EmptyBlockHandler(TextBlockClickedCallback textBlockClickedCallback) : BlockHandler<EmptyBlock>(textBlockClickedCallback)
{
    public override Control[] Parse(EmptyBlock content, CaretInformation caretInformation) => [
        new CaretSelectableTextBlock(TextBlockClickedCallback, content.Span){
            CaretIndex = 0
        }    
    ];
}