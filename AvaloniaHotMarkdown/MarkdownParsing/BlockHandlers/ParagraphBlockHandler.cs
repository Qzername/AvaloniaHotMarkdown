using Avalonia.Controls;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class ParagraphBlockHandler : BlockHandler
{
    public ParagraphBlockHandler(StandardMarkdownParser parser) : base(parser)
    {
    }

    public override Control Handle(Block block, string markdownText, LineInformation[] lineInformations)
    {
        ParagraphBlock paragraphBlock = block as ParagraphBlock;

        var container = ParseInline(paragraphBlock.Inline.Descendants(), lineInformations.Any(x => x.ShowFullText));

        container.Tag = new CaretPositionOffset(0, lineInformations[0].LineYIndex);

        return container;
    }
}
