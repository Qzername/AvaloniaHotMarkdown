using Avalonia.Controls;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class HeadingBlockHandler(StandardMarkdownParser parser) : BlockHandler(parser)
{
    //TODO: make this customizable
    readonly int[] Sizes = [60, 45, 30];

    public override Control Handle(Block block, string markdownText, LineInformation[] lineInformations)
    {
        HeadingBlock headingBlock = (HeadingBlock)block;
        string prefix = new string('#', headingBlock.Level) + " ";

        var container = ParseInline(headingBlock.Inline.Descendants(), lineInformations.Any(x => x.ShowFullText), prefix.Length) as StackPanel;

        container.Tag = new CaretPositionOffset(0, lineInformations[0].LineYIndex);

        if (lineInformations[0].ShowFullText)
        {
            var richTextPresenter = StylizationHelper.CreateNewPresenter();
            richTextPresenter.Text = prefix;
            (container.Children[0] as WrapPanel).Children.Insert(0, richTextPresenter);
        }

        List<Control> richTexts = [];

        foreach (WrapPanel wrapPanel in container.Children)
            richTexts.AddRange(wrapPanel.Children.ToList());

        foreach (RichTextPresenter item in richTexts)
            item.FontSize = Sizes[headingBlock.Level - 1];

        return container;
    }
}
