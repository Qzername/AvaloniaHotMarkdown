using Avalonia.Controls;
using Avalonia.Layout;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class ListBlockHandler : BlockHandler
{
    public ListBlockHandler(StandardMarkdownParser parser) : base(parser)
    {
    }

    public override Control Handle(Block block, LineInformation[] lineInformations)
    {
        var listBlock = (ListBlock)block;
        var mainContainer = new StackPanel();

        for (int i = 0; i < listBlock.Count; i++)
        {
            if (listBlock[i] is not ListItemBlock listItem)
                continue;

            StackPanel itemContainer = new();
            itemContainer.Orientation = Orientation.Horizontal;
            itemContainer.Tag = new CaretPositionOffset(0, lineInformations[i].LineYIndex);

            string prefix = string.Empty;

            if (lineInformations[i].ShowFullText)
                prefix = listBlock.IsOrdered ? $"{i + 1}. " : "- ";
            else
                prefix = listBlock.IsOrdered ? $"{i + 1}. " : "• ";

            var prefixTextPresenter = CreateNewPresenter();
            prefixTextPresenter.Text = prefix;
            itemContainer.Children.Add(prefixTextPresenter);

            foreach (var segment in listItem)
            {
                var container = ParseBlock(segment, lineInformations);
                CaretPositionOffset prefixOffset = new CaretPositionOffset(prefix.Length, 0);

                //remove y offset from container
                container.Tag = (container.Tag is CaretPositionOffset offset ?
                    new CaretPositionOffset(offset.XInLineOffset + prefixOffset.XInLineOffset, 0) :
                    prefixOffset);

                itemContainer.Children.Add(container);
            }

            mainContainer.Children.Add(itemContainer);
        }

        return mainContainer;
    }

    public override void UpdateTextEffects(Control control, LineInformation[] lineInformations)
    {
        //TODO: part of this code is vibecoded, it would be nice to optimize it in the future xd
        UpdateCaret(control, lineInformations);
        UpdateSelection(control, lineInformations);
    }

    void UpdateCaret(Control control, LineInformation[] lineInformations)
    {
        var mainTree = (control as StackPanel).Children;

        for (int i = 0; i < lineInformations.Length; i++)
        {
            if (lineInformations[i].CaretIndex is null)
                continue;

            if (i >= mainTree.Count)
                return;

            var itemTree = (mainTree[i] as StackPanel).Children;

            var caretIndex = lineInformations[i].CaretIndex!.Value;

            if (caretIndex <= 2)
            {
                var richTextPresenter = (itemTree[0] as RichTextPresenter);
                richTextPresenter.CaretIndex = caretIndex;
                richTextPresenter.ShowCaret();
            }
            else
            {
                var paragraphTree = (itemTree[1] as StackPanel).Children;

                int temp = 1;

                List<RichTextPresenter> texts = new();

                foreach (StackPanel line in paragraphTree)
                    texts.AddRange(line.Children.ToList().Cast<RichTextPresenter>());

                foreach (RichTextPresenter presenter in texts)
                {
                    if (temp + presenter.Text.Length >= caretIndex)
                    {
                        presenter.CaretIndex = caretIndex - temp - 2;
                        presenter.ShowCaret();
                        return;
                    }

                    temp += presenter.Text.Length;
                }
            }
        }
    }

    void UpdateSelection(Control control, LineInformation[] lineInformations)
    {
        var mainTree = (control as StackPanel).Children;
        for (int i = 0; i < lineInformations.Length; i++)
        {
            if (lineInformations[i].SelectionInformation is null)
                continue;
            if (i >= mainTree.Count)
                return;
            var itemTree = (mainTree[i] as StackPanel).Children;
            var selectionInformation = lineInformations[i].SelectionInformation!.Value;
            if (selectionInformation.EndIndex <= 2)
            {
                var richTextPresenter = (itemTree[0] as RichTextPresenter);
                richTextPresenter.SelectionStart = selectionInformation.StartIndex;
                richTextPresenter.SelectionEnd = selectionInformation.EndIndex;
                richTextPresenter.ShowCaret();
            }
            else
            {
                var paragraphTree = (itemTree[1] as StackPanel).Children;
                int temp = 1;

                List<RichTextPresenter> texts = new();

                foreach (StackPanel line in paragraphTree)
                    texts.AddRange(line.Children.ToList().Cast<RichTextPresenter>());

                foreach (RichTextPresenter presenter in texts)
                {
                    if (temp + presenter.Text.Length >= selectionInformation.StartIndex &&
                        temp <= selectionInformation.EndIndex)
                    {
                        presenter.SelectionStart = selectionInformation.StartIndex - temp - 2;
                        presenter.SelectionEnd = selectionInformation.EndIndex - temp - 2;
                        presenter.ShowCaret();
                    }
                    temp += presenter.Text.Length;
                }
            }
        }
    }
}
