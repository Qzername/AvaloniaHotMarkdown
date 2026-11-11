using Avalonia.Controls;
using Avalonia.Layout;
using AvaloniaHotMarkdown.Controls;
using Markdig.Extensions.Tables;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers.Implementations;

/// <summary>
/// Handles rendering of Markdown block quotes with a styled left border.
/// </summary>
internal class QuoteBlockHandler : BlockHandler<QuoteBlock>
{
    public override Control[] Parse(QuoteBlock quoteBlock)
    {
        var quoteBorderControl = new QuoteBorderControl();

        var linesContainer = new StackPanel
        {
            Orientation = Orientation.Vertical
        };

        quoteBorderControl.Child = linesContainer;

        // Parse each child block within the quote
        foreach (var childBlock in quoteBlock)
        {
            // Recursively parse child blocks and add them to the quote container
            var childControls = ParseBlock(childBlock);
            foreach (var control in childControls)
            {
                linesContainer.Children.Add(control);
            }
        }

        return [quoteBorderControl];
    }

    /// <summary>
    /// Recursively parses a block using the appropriate handler.
    /// </summary>
    private Control[] ParseBlock(Block block)
    {
        // This will delegate to the appropriate handler based on block type
        if (block is HeadingBlock headingBlock)
            return new HeadingBlockHandler().Parse(headingBlock);
        else if (block is ListBlock listBlock)
            return new ListBlockHandler().Parse(listBlock);
        else if (block is ParagraphBlock paragraphBlock)
            return new ParagraphBlockHandler().Parse(paragraphBlock);
        else if (block is QuoteBlock innerQuoteBlock)
            return Parse(innerQuoteBlock);
        else if (block is Table table)
            return new TableHandler().Parse(table);
        else
            return [];
    }
}
