using Avalonia.Controls;
using Avalonia.Layout;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class ListBlockHandler : BlockHandler
{
    public ListBlockHandler(StandardMarkdownParser parser) : base(parser)
    {
    }

    public override Control Handle(Block block)
    {
        var listBlock = (ListBlock)block;
        var mainContainer = new StackPanel();

        for(int i = 0; i<listBlock.Count; i++)
        {
            if (listBlock[i] is not ListItemBlock listItem)
                continue;

            StackPanel itemContainer = new();
            itemContainer.Orientation = Orientation.Horizontal;

            itemContainer.Children.Add(new TextBlock { Text = listBlock.IsOrdered ? $"{i+1}. " : "• " });

            foreach(var segment in listItem)
                itemContainer.Children.Add(ParseBlock(segment));

            mainContainer.Children.Add(itemContainer);
        }

        return mainContainer;
    }
}
