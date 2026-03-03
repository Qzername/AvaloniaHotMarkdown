using Avalonia.Controls;
using Avalonia.Layout;
using Markdig.Syntax;
using System.Diagnostics;

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

        for(int i = 0; i<listBlock.Count; i++)
        {
            if (listBlock[i] is not ListItemBlock listItem)
                continue;

            StackPanel itemContainer = new();
            itemContainer.Orientation = Orientation.Horizontal;

            string prefix = string.Empty;

            if (lineInformations[i].ShowFullText)
                prefix = listBlock.IsOrdered ? $"{listBlock.OrderedStart + i}." : "- ";
            else
                prefix = listBlock.IsOrdered ? $"{listBlock.OrderedStart + i}." : "• ";

            var richTextPresenter = CreateNewPresenter();
            richTextPresenter.Text = prefix;
            itemContainer.Children.Add(richTextPresenter);

            foreach(var segment in listItem)
                itemContainer.Children.Add(ParseBlock(segment, lineInformations));

            mainContainer.Children.Add(itemContainer);
        }

        return mainContainer;
    }

    public override void SetCaretPosition(Control control, LineInformation[] lineInformations)
    {
        var mainTree = (control as StackPanel).Children;

        Debug.WriteLine("test: "+ lineInformations.Length);
        for (int i = 0; i<lineInformations.Length;i++)
        {
            if (lineInformations[i].CaretIndex is null)
                continue;

            if(i >= mainTree.Count)
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

                foreach (RichTextPresenter presenter in paragraphTree)
                {
                    if (temp + presenter.Text.Length >= caretIndex)
                    {
                        presenter.CaretIndex = caretIndex - temp-2;
                        presenter.ShowCaret();
                        return;
                    }

                    temp += presenter.Text.Length;
                }
            }

        }
    }
}
