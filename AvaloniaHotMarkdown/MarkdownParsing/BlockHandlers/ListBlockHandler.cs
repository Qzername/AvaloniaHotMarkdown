using Avalonia.Controls;
using Avalonia.Layout;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class ListBlockHandler : BlockHandler
{
    public ListBlockHandler(StandardMarkdownParser parser) : base(parser)
    {
    }

    public override Control Handle(Block block, bool parseAsFullText)
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

            if (parseAsFullText)
                prefix = listBlock.IsOrdered ? $"{listBlock.OrderedStart + i}." : "- ";
            else
                prefix = listBlock.IsOrdered ? $"{listBlock.OrderedStart + i}." : "•";

            itemContainer.Children.Add(new TextBlock { Text = prefix });

            foreach(var segment in listItem)
                itemContainer.Children.Add(ParseBlock(segment, parseAsFullText));

            mainContainer.Children.Add(itemContainer);
        }

        return mainContainer;
    }

    public override void SetCaretPosition(Control control, int index)
    {

    }
}
