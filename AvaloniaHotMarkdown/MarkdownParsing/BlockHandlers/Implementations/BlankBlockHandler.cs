using Avalonia.Controls;
using AvaloniaHotMarkdown.Controls;
using AvaloniaHotMarkdown.MarkdownParsing.Extensions;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers.Implementations;

internal class BlankBlockHandler : BlockHandler<SpacerBlock>
{
    public override Control[] Parse(SpacerBlock content) => [new CaretSelectableTextBlock()];
    public override void UpdateSelectionAndCaret(Control[] controls, CaretInformation caretInformation)
    {
    }
}