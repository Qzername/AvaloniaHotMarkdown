using Avalonia.Controls;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class HeadingBlockHandler(StandardMarkdownParser parser) : BlockHandler(parser)
{
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

        string resourceKey = $"HotMarkdownHeading{headingBlock.Level}Size";

        foreach (RichTextPresenter item in richTexts)
        {
            if (container.TryFindResource(resourceKey, out var resValue) && resValue is double customSize)
                item.FontSize = customSize;
            else
                item.FontSize = headingBlock.Level switch
                {
                    1 => 60,
                    2 => 45,
                    _ => 30
                };
        }

        return container;
    }
}
