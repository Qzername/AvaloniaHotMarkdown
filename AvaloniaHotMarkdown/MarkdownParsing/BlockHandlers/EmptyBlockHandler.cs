using Avalonia.Controls;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class EmptyBlockHandler(StandardMarkdownParser parser) : BlockHandler(parser)
{
    public override Control? Handle(Block block, string markdown, LineInformation[] lineInformations)
    {
        return null;
    }
}
