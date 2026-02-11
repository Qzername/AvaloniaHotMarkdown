namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class ParagraphBlockHandler : IBlockHandler
{
    public Block[] Handle(Markdig.Syntax.Block markdownBlock, int textIndex)
    {
        List<Block> blocks = new List<Block>();

        return blocks.ToArray();
    }
}
