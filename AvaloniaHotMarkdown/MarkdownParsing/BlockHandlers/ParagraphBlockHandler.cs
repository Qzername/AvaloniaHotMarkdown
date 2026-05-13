using Avalonia.Controls;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class ParagraphBlockHandler : BlockHandler
{
    public ParagraphBlockHandler(StandardMarkdownParser parser) : base(parser)
    {
    }

    public override Control Handle(Block block, string markdownText, LineInformation[] lineInformations)
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
            {
                var children = (mainTree[i] as DockPanel).Children;

                for(int j = 0; j < children.Count;j++)
                {
                    var current = children[j];

                    if(current is RichTextPresenter presenter)
                    {
                        if (temp + presenter.Text.Length >= caretIndex)
                        {
                            presenter.CaretIndex = caretIndex.Value - temp;
                            presenter.ShowCaret();
                            break;
                        }

                        temp += presenter.Text.Length;
                        
                        continue;
                    }

                    if (current.Tag is not null && current.Tag is CaretPositionOffset offset)
                        temp += offset.XInLineOffset;
                }
            }
               

            temp = 0;
        }

        //update selection
        for (int i = 0; i < lineInformations.Length; i++)
        {
            var selectionInformation = lineInformations[i].SelectionInformation;

            if (selectionInformation is null)
                continue;

            int minSelectionStart = Math.Min(selectionInformation.Value.StartIndex, selectionInformation.Value.EndIndex);
            int maxSelectionStart = Math.Max(selectionInformation.Value.StartIndex, selectionInformation.Value.EndIndex);

            foreach (RichTextPresenter presenter in (mainTree[i] as DockPanel).Children.OfType<RichTextPresenter>())
            {
                if (temp + presenter.Text.Length >= minSelectionStart &&
                    temp <= maxSelectionStart)
                {
                    presenter.SelectionStart = minSelectionStart - temp;
                    presenter.SelectionEnd = maxSelectionStart - temp;
                    presenter.ShowCaret();
                }

                temp += presenter.Text.Length;
            }

            temp = 0;
        }
    }
}
