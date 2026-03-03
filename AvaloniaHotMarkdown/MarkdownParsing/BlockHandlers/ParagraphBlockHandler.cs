using Avalonia.Controls;
using Markdig.Syntax;
using System.Diagnostics;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class ParagraphBlockHandler : BlockHandler
{
    public ParagraphBlockHandler(StandardMarkdownParser parser) : base(parser)
    {
    }

    public override Control Handle(Block block, LineInformation[] lineInformations)
    {
        ParagraphBlock paragraphBlock = block as ParagraphBlock;

        return ParseInline(paragraphBlock.Inline.Descendants(), lineInformations[0].ShowFullText);
    }

    public override void SetCaretPosition(Control control, LineInformation[] lineInformations)
    {
        if (lineInformations[0].CaretIndex is null)
            return;

        int caretIndex = lineInformations[0].CaretIndex.Value;

        var mainTree = (control as StackPanel).Children;

        int temp = 0;

        foreach (RichTextPresenter presenter in mainTree)
        {
            if (temp + presenter.Text.Length >= caretIndex)
            {
                presenter.CaretIndex = caretIndex - temp;
                presenter.ShowCaret();
                return;
            }

            temp += presenter.Text.Length;
        }
    }
}
