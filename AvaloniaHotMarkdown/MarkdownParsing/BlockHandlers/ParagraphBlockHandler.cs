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

        var container = ParseInline(paragraphBlock.Inline.Descendants(), lineInformations[0].ShowFullText);

        container.Tag = lineInformations[0].LineYIndex;

        return container;
    }

    public override void UpdateTextEffects(Control control, LineInformation[] lineInformations)
    {
        var mainTree = (control as StackPanel).Children;

        int temp = 0;

        //update caret
        int? caretIndex = lineInformations[0].CaretIndex;

        if (lineInformations[0].CaretIndex is not null)
            foreach (RichTextPresenter presenter in mainTree)
            {
                if (temp + presenter.Text.Length >= caretIndex)
                {
                    presenter.CaretIndex = caretIndex.Value - temp;
                    presenter.ShowCaret();
                    break;
                }

                temp += presenter.Text.Length;
            }

        temp = 0;

        //update selection
        var selectionInformation = lineInformations[0].SelectionInformation;

        if (selectionInformation is not null)
            foreach (RichTextPresenter presenter in mainTree)
            {
                if (temp + presenter.Text.Length >= selectionInformation.Value.StartIndex &&
                    temp <= selectionInformation.Value.EndIndex)
                {
                    presenter.SelectionStart = selectionInformation.Value.StartIndex - temp;
                    presenter.SelectionEnd = selectionInformation.Value.EndIndex - temp;
                    presenter.ShowCaret();
                }

                temp += presenter.Text.Length;
            }
    }
}
