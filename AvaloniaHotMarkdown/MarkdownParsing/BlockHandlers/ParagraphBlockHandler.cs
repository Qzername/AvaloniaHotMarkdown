using Avalonia.Controls;
using Markdig.Syntax;
using System.Diagnostics;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class ParagraphBlockHandler : BlockHandler
{
    public ParagraphBlockHandler(StandardMarkdownParser parser) : base(parser)
    {
    }

    public override Control Handle(Block block, bool parseAsFullText)
    {
        ParagraphBlock paragraphBlock = block as ParagraphBlock;

        return ParseInline(paragraphBlock.Inline.Descendants(), parseAsFullText);
    }

    public override void SetCaretPosition(Control control, int index)
    {
        var mainTree = (control as StackPanel).Children;

        int temp = 0;

        foreach (RichTextPresenter presenter in mainTree)
        {
            if (temp + presenter.Text.Length >= index)
            {
                presenter.CaretIndex = index - temp;
                presenter.ShowCaret();
                return;
            }

            temp += presenter.Text.Length;
        }
    }
}
