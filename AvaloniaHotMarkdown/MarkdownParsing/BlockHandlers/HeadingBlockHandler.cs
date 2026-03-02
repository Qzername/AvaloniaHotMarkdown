using Avalonia.Controls;
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

    public override Control Handle(Block block, bool parseAsFullText)
    {
        HeadingBlock headingBlock = (HeadingBlock)block;

        var container = ParseInline(headingBlock.Inline.Descendants(), parseAsFullText) as StackPanel;

        if (parseAsFullText)
        {
            var richTextPresenter = CreateNewPresenter();
            richTextPresenter.Text = new string('#', headingBlock.Level) + " ";
            container.Children.Insert(0, richTextPresenter);
        }

        foreach (RichTextPresenter item in container.Children)
            item.FontSize = Sizes[headingBlock.Level - 1];
    
        return container;
    }

    public override void SetCaretPosition(Control control, int index)
    {
        var mainTree = (control as StackPanel).Children;

        int temp = 0;

        foreach(RichTextPresenter presenter in mainTree)
        {
            if(temp + presenter.Text.Length >= index)
            {
                presenter.CaretIndex = index - temp;
                presenter.ShowCaret();
                return;
            }

            temp += presenter.Text.Length;
        }
    }
}
