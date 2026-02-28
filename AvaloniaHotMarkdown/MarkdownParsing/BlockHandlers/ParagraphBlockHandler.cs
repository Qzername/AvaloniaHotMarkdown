using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Diagnostics;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class ParagraphBlockHandler : BlockHandler
{
    public override Control Handle(Block block)
    {
        ParagraphBlock paragraphBlock = block as ParagraphBlock;

        return ParseInline(paragraphBlock.Inline.Descendants());
    }
}
