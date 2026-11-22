using Avalonia.Controls;
using AvaloniaHotMarkdown.Controls;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers.Implementations;

internal class EmptyBlockHandler : BlockHandler<EmptyBlock>
{
    public override Control[] Parse(EmptyBlock content, CaretInformation caretInformation) => [
        new CaretSelectableTextBlock(){
            CaretIndex = 0
        }    
    ];
}