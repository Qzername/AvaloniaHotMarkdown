using Avalonia.Controls;
using Avalonia.Media;
using Markdig.Syntax;
using System.Diagnostics;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class HeadingBlockHandler : BlockHandler
{
    //TODO: make this customizable
    readonly int[] Sizes = [60, 45, 30];

    public HeadingBlockHandler(StandardMarkdownParser parser) : base(parser)
    {
    }

    public override Control Handle(Block block, LineInformation[] lineInformations)
    {
        HeadingBlock headingBlock = (HeadingBlock)block;
        string prefix = new string('#', headingBlock.Level) + " ";

        var container = ParseInline(headingBlock.Inline.Descendants(), lineInformations[0].ShowFullText, prefix.Length) as StackPanel;

        container.Tag = new CaretPositionOffset(0,lineInformations[0].LineYIndex);

        if (lineInformations[0].ShowFullText)
        {
            var richTextPresenter = CreateNewPresenter();
            richTextPresenter.Text = prefix;
            container.Children.Insert(0, richTextPresenter);
        }

        foreach (RichTextPresenter item in container.Children)
            item.FontSize = Sizes[headingBlock.Level - 1];
    
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
