using Avalonia.Controls;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class ParagraphBlockHandler : BlockHandler
{
    public ParagraphBlockHandler(StandardMarkdownParser parser) : base(parser)
    {
    }

    public override Control Handle(Block block, bool parseAsFullText)
    {
        ParagraphBlock paragraphBlock = block as ParagraphBlock;

        return ParseInline(paragraphBlock.Inline.Descendants(), parseAsFullText);
    }
}
