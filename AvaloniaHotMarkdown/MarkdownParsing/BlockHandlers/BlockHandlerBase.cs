using Avalonia.Controls;
using AvaloniaHotMarkdown.Controls;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

public abstract class BlockHandlerBase
{
    public abstract bool CanHandle(object content);
    public abstract Control[] Parse(object content, CaretInformation caretInformation);
    public virtual void UpdateSelectionAndCaret(Control[] controls, CaretInformation caretInformation)
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
