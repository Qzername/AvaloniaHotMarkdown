using Avalonia.Controls;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers.Implementations;

internal class EmptyBlockHandler : BlockHandler<EmptyBlock>
{
    public override Control[] Parse(EmptyBlock content) => [];

    public override void UpdateSelectionAndCaret(Control[] controls, CaretInformation caretInformation)
    {
    }
}