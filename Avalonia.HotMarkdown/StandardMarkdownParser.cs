using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Diagnostics;

namespace Avalonia.HotMarkdown
{
    public class StandardMarkdownParser : IMarkdownParser
    {
        public Block[] Parse(string markdownText)
        {
            List<Block> blocks = new List<Block>();

            var pipeline = new MarkdownPipelineBuilder()
                .UseSoftlineBreakAsHardlineBreak()
                .Build();
            MarkdownDocument doc = Markdown.Parse(markdownText, pipeline);

            foreach (var child in doc.Descendants().ToArray())
            {
                if (child is LeafBlock || child is LineBreakInline || child is HtmlInline)
                {
                    Block newBlock = new Block()
                    {
                        Content = string.Empty,
                        FontSize = 16, // Default font size
                        StartIndex = child.Span.Start,
                        ActualStartIndex = child.Span.Start,
                        EndIndex = child.Span.End
                    };

                    if (child is HeadingBlock headingBlock)
                        newBlock.FontSize = 60 / headingBlock.Level;

                    blocks.Add(newBlock);

                    continue;
                }

                if (child is not LeafInline inline)
                    continue;

                var currentBlock = blocks[^1];

                var slice = (inline as LiteralInline)!.Content;

                currentBlock.ActualStartIndex = slice.Start;
                currentBlock.Content += slice.Text.Substring(slice.Start, slice.Length);

                blocks[^1] = currentBlock;
            }

            return [.. blocks];
        }
    }
}
