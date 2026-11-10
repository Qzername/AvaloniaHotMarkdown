using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Markdig.Syntax;
using System.Diagnostics;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers.Implementations;

internal class ListBlockHandler : BlockHandler<ListBlock>
{
    public override Control[] Parse(ListBlock listBlock)
    {
        var mainContainer = new StackPanel();

        for (int i = 0; i < listBlock.Count; i++)
        {
            if (listBlock[i] is not ListItemBlock listItem)
                continue;

            // Create container for this list item
            var itemPanel = new StackPanel { Spacing = 5 };

            // Create prefix (bullet or number)
            string prefix = listBlock.IsOrdered ? $"{i + 1}." : "•";

            // Create the list item content
            var itemContent = new TextBlock();
            var inlines = new List<Inline>
            {
                // Add the prefix
                new Run { Text = prefix + " " }
            };

            // Parse list item blocks
            foreach (var block in listItem)
            {
                if (block is ParagraphBlock paragraphBlock && paragraphBlock.Inline is not null)
                {
                    // Parse the inline content of the paragraph
                    var inlineRuns = InlineParser.ParseInline(paragraphBlock.Inline.FirstChild!);
                    inlines.AddRange(inlineRuns);

                    // Add remaining children
                    var currentChild = paragraphBlock.Inline.FirstChild?.NextSibling;
                    while (currentChild is not null)
                    {
                        if (currentChild is Markdig.Syntax.Inlines.LineBreakInline)
                        {
                            currentChild = currentChild.NextSibling;
                            continue;
                        }

                        inlines.AddRange(InlineParser.ParseInline(currentChild));
                        currentChild = currentChild.NextSibling;
                    }
                }
                else if (block is ListBlock nestedList)
                {
                    // Handle nested lists - add them as separate controls
                    var nestedControls = Parse(nestedList);
                 
                    foreach (var nestedControl in nestedControls)
                        itemPanel.Children.Add(nestedControl);
                }
            }

            // Only add content if we have inlines
            if (inlines.Count > 0)
            {
                itemContent.Inlines!.AddRange(inlines);
                itemPanel.Children.Insert(0, itemContent);
            }

            mainContainer.Children.Add(itemPanel);
        }

        return [mainContainer];
    }
}
