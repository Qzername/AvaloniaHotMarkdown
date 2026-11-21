using Avalonia.Controls;
using AvaloniaHotMarkdown.Controls;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers.Implementations;

internal class ParagraphBlockHandler : BlockHandler<ParagraphBlock>
{
    public override Control[] Parse(ParagraphBlock content)
    {
        if (content.Inline is null)
            return [];
    
        return [InlineParser.ParseContainer(content.Inline)];
    }

    public override void UpdateSelectionAndCaret(Control[] controls, CaretInformation caretInformation)
    {
        foreach (var control in controls)
        {
            if (control is not CaretSelectableTextBlock textBlock)
                continue;

            if (caretInformation.SelectionInformation is { } selection)
            {
                textBlock.SelectionStart = selection.StartIndex;
                textBlock.SelectionEnd = selection.EndIndex;
            }

            if (caretInformation.CaretIndex >= 0)
                textBlock.CaretIndex = caretInformation.CaretIndex;
        }
    }
}
