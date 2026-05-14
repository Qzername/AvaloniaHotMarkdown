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
            var richTextPresenter = CreateNewPresenter();
            richTextPresenter.Text = prefix;
            (container.Children[0] as DockPanel).Children.Insert(0, richTextPresenter);
        }

        List<Control> richTexts = [];

        foreach (DockPanel dockPanel in container.Children)
            richTexts.AddRange(dockPanel.Children.ToList());

        foreach (RichTextPresenter item in richTexts)
            item.FontSize = Sizes[headingBlock.Level - 1];

        return container;
    }
}
