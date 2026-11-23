using Avalonia.Controls;
using Avalonia.Layout;
using AvaloniaHotMarkdown.Controls;
using Markdig.Extensions.Tables;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers.Implementations;

/// <summary>
/// Handles rendering of Markdown block quotes with a styled left border.
/// </summary>
internal class QuoteBlockHandler(TextBlockClickedCallback textBlockClickedCallback) : BlockHandler<QuoteBlock>(textBlockClickedCallback)
{
    public override Control[] Parse(QuoteBlock quoteBlock, CaretInformation caretInformation)
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
            var childControls = ParseBlock(childBlock, caretInformation);
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
   //TODO: change this!
    private Control[] ParseBlock(Block block, CaretInformation caretInformation)
    {
        // This will delegate to the appropriate handler based on block type
        if (block is HeadingBlock headingBlock)
            return new HeadingBlockHandler(TextBlockClickedCallback).Parse(headingBlock, caretInformation);
        else if (block is ListBlock listBlock)
            return new ListBlockHandler(TextBlockClickedCallback).Parse(listBlock, caretInformation);
        else if (block is ParagraphBlock paragraphBlock)
            return new ParagraphBlockHandler(TextBlockClickedCallback).Parse(paragraphBlock, caretInformation);
        else if (block is QuoteBlock innerQuoteBlock)
            return Parse(innerQuoteBlock, caretInformation);
        else if (block is Table table)
            return new TableHandler(TextBlockClickedCallback).Parse(table, caretInformation);
        else
            return [];
    }
}
