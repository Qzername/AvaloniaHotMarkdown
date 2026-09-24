using Avalonia.Controls;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class ListBlockHandler : BlockHandler
{
    public ListBlockHandler(StandardMarkdownParser parser) : base(parser)
    {
    }

    public override Control Handle(Block block, string markdownText, LineInformation[] lineInformations)
    {
        var listBlock = (ListBlock)block;
        var mainContainer = new StackPanel();

        for (int i = 0; i < listBlock.Count; i++)
        {
            if (listBlock[i] is not ListItemBlock listItem)
                continue;

            StretchWrapPanel itemContainer = new()
            {
                Tag = new CaretPositionOffset(0, lineInformations[i].LineYIndex)
            };

            string prefix = string.Empty;

            if (lineInformations[i].ShowFullText) 
                prefix = listBlock.IsOrdered ? $"{i + 1}." : listBlock.BulletType.ToString();
            else
                prefix = listBlock.IsOrdered ? $"{i + 1}." : "•";

            var prefixTextPresenter = StylizationHelper.CreateNewPresenter();
            prefixTextPresenter.Text = prefix;

            if (listItem.Count == 0)
                prefixTextPresenter.Text += " ";

            itemContainer.Children.Add(prefixTextPresenter);

            foreach (var segment in listItem)
            {
                var container = ParseBlock(segment, markdownText, lineInformations);

                //move children of container to current object
                if (container is not StackPanel stackPanel || stackPanel.Children[0] is not StretchWrapPanel wrapPanel)
                {
                    itemContainer.Children.Add(container);
                    continue;
                }

                CaretPositionOffset curretOffset = new(prefix.Length, 0);

                while (wrapPanel.Children.Count > 0)
                {
                    var child = wrapPanel.Children[0];

                    if (child is RichTextPresenter presenter)
                    {
                        child.Tag = curretOffset;
                        curretOffset += new CaretPositionOffset(presenter.Text.Length, 0);
                    }

                    wrapPanel.Children.Remove(child);
                    itemContainer.Children.Add(child);
                }
            }

            mainContainer.Children.Add(itemContainer);
        }

        return mainContainer;
    }
}
