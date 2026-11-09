using Avalonia.Controls;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers.Implementations;

internal class ParagraphBlockHandler : BlockHandler<ParagraphBlock>
{
    public override Control[] Parse(ParagraphBlock content)
    {
        if (content.Inline is null)
            return [];
    
        return [InlineParser.ParseContainer(content.Inline)];
    }
}
