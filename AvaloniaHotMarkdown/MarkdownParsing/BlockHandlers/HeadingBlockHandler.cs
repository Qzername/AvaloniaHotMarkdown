using Avalonia.Controls;
using Markdig.Syntax;

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

        var container = ParseInline(headingBlock.Inline.Descendants(), lineInformations.Any(x => x.ShowFullText), prefix.Length) as StackPanel;

        container.Tag = new CaretPositionOffset(0, lineInformations[0].LineYIndex);

        if (lineInformations[0].ShowFullText)
        {
            var richTextPresenter = CreateNewPresenter();
            richTextPresenter.Text = prefix;
            (container.Children[0] as StackPanel).Children.Insert(0, richTextPresenter);
        }

        List<Control> richTexts = new List<Control>();

        foreach (StackPanel stackPanel in container.Children)
            richTexts.AddRange(stackPanel.Children.ToList());

        foreach (RichTextPresenter item in richTexts)
            item.FontSize = Sizes[headingBlock.Level - 1];

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

            if (selectionInformation is null)
                continue;

            int minSelectionStart = Math.Min(selectionInformation.Value.StartIndex, selectionInformation.Value.EndIndex);
            int maxSelectionStart = Math.Max(selectionInformation.Value.StartIndex, selectionInformation.Value.EndIndex);

            foreach (RichTextPresenter presenter in (mainTree[i] as StackPanel).Children)
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
