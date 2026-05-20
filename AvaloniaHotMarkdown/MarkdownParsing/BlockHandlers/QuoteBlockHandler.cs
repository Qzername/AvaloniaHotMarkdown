using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Markdig;
using Markdig.Extensions.Tables;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class QuoteBlockHandler(StandardMarkdownParser parser) : BlockHandler(parser)
{
    public override Control Handle(Block block, string markdown, LineInformation[] lineInformations)
    {
        var showFull = lineInformations.Any(x => x.ShowFullText);

        QuoteBlock quoteBlock = (QuoteBlock)block;

        Control result;

        if (showFull)
            result = ParseAsText(quoteBlock, markdown, lineInformations);
        else
            result = ParseAsQuoteBlock(quoteBlock, markdown, lineInformations);
            
        return result;
    }

    Control ParseAsText(QuoteBlock block, string markdownText, LineInformation[] lineInformation)
    {
        StackPanel container = new();
        container.Spacing = 0;

        string tableText = markdownText.Substring(block.Span.Start, block.Span.End - block.Span.Start + 1);
        string[] lines = tableText.Split(["\n", "\r\n"], StringSplitOptions.None);

        for (int i = 0; i < lines.Length; i++)
        {
            var presenter = StylizationHelper.CreateNewPresenter();

            presenter.Text = lines[i];
            presenter.Tag = new CaretPositionOffset(0, lineInformation[i].LineYIndex);

            //dockpanel for builtin updatetexteffects to work
            DockPanel lineContainer = new();
            lineContainer.Children.Add(presenter);

            container.Children.Add(lineContainer);
        }

        container.ApplyTemplate();

        return container;
    }

    Control ParseAsQuoteBlock(QuoteBlock quoteBlock, string markdown, LineInformation[] lineInformations)
    {
        //TODO: this should be stylized...
        Border mainContainer = new()
        {
            BorderThickness = new Thickness(4, 0, 0, 0),
            BorderBrush = Brushes.Gray,
            Padding = new Thickness(15, 10),
            Margin = new Thickness(0, 10)
        };

        StackPanel linesContainer = new()
        {
            Orientation = Orientation.Vertical
        };
        mainContainer.Child = linesContainer;

        foreach (var childBlock in quoteBlock)
            linesContainer.Children.Add(ParseBlock(quoteBlock[0], markdown, lineInformations));

        return mainContainer;
    }
}
