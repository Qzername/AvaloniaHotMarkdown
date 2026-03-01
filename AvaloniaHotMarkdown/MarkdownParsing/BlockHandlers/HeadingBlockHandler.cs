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

    public override Control Handle(Block block, bool parseAsFullText)
    {
        HeadingBlock headingBlock = (HeadingBlock)block;

        var container = ParseInline(headingBlock.Inline.Descendants(), parseAsFullText) as StackPanel;

        if (parseAsFullText)
            container.Children.Insert(0, new RichTextPresenter() {
                Text = new string('#', headingBlock.Level) + " " 
            });

        foreach(RichTextPresenter item in container.Children)
            item.FontSize = Sizes[headingBlock.Level - 1];

        return container;
    }
}
