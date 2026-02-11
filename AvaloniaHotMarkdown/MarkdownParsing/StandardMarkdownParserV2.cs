using Markdig;
using Markdig.Syntax;
using MS = Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Diagnostics;
using System.Reflection.Metadata;
using AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;
using System.Diagnostics.Contracts;

namespace AvaloniaHotMarkdown.MarkdownParsing
{
    //V2

    /*
     * Goals: 
     * - dont use stupid line splitting method
     * - support multiple lines per block
     * - more modulable code
     */

    public class StandardMarkdownParserV2 : IMarkdownParser
    {
        readonly MarkdownPipeline pipeline;

        readonly Dictionary<Type, IBlockHandler> handlers = new()
        {
            { typeof(HeadingBlock), new HeadingBlockHandler() },
            { typeof(ParagraphBlock), new ParagraphBlockHandler() },
        };

        public StandardMarkdownParserV2()
        {
            pipeline = new MarkdownPipelineBuilder()
                .UseSoftlineBreakAsHardlineBreak()
                .Build();
        }

        public Block[] Parse(string markdownText)
        {
            List<Block> blocks = new List<Block>();

            var document = Markdown.Parse(markdownText, pipeline);

            int textIndex = 0;

            foreach (var block in document)
            {
                Type type = block.GetType();

                if (!handlers.ContainsKey(type))
                    continue;

                blocks.AddRange(handlers[type].Handle(block, textIndex));

                //TODO: remove startIndex
                textIndex = 0;

                foreach(var actualBlock in blocks)
                {
                    foreach (var contentElement in actualBlock.Content)
                        textIndex += contentElement.Text.Length;

                    textIndex++;
                }
            }

            return blocks.ToArray();
        }

    }
}
