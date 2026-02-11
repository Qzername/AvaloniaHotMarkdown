using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Diagnostics;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class HeadingBlockHandler : IBlockHandler
{
    const int Heading1FontSize = 40;
    const int Heading2FontSize = 32;
    const int Heading3FontSize = 28;

    public Block[] Handle(Markdig.Syntax.Block markdownBlock, int textIndex)
    {
        HeadingBlock headingBlock = (HeadingBlock)markdownBlock;

        Block finalBlock = new Block()
        {
            FontSize = GetHeadingFontSize(headingBlock.Level),
            StartIndex = textIndex,
            ActualStartIndex = textIndex,
            EndIndex = 0,
            Content = [],
        };

        finalBlock.Content = IBlockHandler.HandleInlines(headingBlock.Inline.ToArray(), ref finalBlock);

        //TODO: remove end index 
        if (finalBlock.Content.Any(x => x.Text.Length > 0))
            finalBlock.EndIndex = 1;

        return [finalBlock];
    }

    static int GetHeadingFontSize(int headingLevel)
    {
        return headingLevel switch
        {
            1 => Heading1FontSize,
            2 => Heading2FontSize,
            3 => Heading3FontSize,
            _ => 16,
        };
    }
}