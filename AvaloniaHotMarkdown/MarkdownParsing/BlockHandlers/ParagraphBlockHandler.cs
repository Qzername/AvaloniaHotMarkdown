using Avalonia.Controls;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class ParagraphBlockHandler : BlockHandler
{
    public ParagraphBlockHandler(StandardMarkdownParser parser) : base(parser)
    {
    }

    public override Control Handle(Block block, LineInformation[] lineInformations)
    {
        ParagraphBlock paragraphBlock = block as ParagraphBlock;

        var container = ParseInline(paragraphBlock.Inline.Descendants(), lineInformations.Any(x => x.ShowFullText));

        container.Tag = new CaretPositionOffset(0, lineInformations[0].LineYIndex);

        return container;
    }

    public override void UpdateTextEffects(Control control, LineInformation[] lineInformations)
    {
        var mainTree = (control as StackPanel).Children;

        int temp = 0;

        //update caret
        for (int i = 0; i < lineInformations.Length; i++)
        {
            int? caretIndex = lineInformations[i].CaretIndex;

            if (lineInformations[i].CaretIndex is not null)
                foreach (RichTextPresenter presenter in (mainTree[i] as StackPanel).Children)
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
        }

        //update selection
        for (int i = 0; i < lineInformations.Length; i++)
        {
            var selectionInformation = lineInformations[i].SelectionInformation;

            if (selectionInformation is not null)
                foreach (RichTextPresenter presenter in (mainTree[i] as StackPanel).Children)
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

            temp = 0;
        }
    }
}
