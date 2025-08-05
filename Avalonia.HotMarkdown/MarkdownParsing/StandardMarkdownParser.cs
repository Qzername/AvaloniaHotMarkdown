using Avalonia.Controls.Presenters;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.HotMarkdown
{
    public class StandardMarkdownParser : IMarkdownParser
    {
        MarkdownPipeline pipeline;

        public StandardMarkdownParser()
        {
            pipeline = new MarkdownPipelineBuilder()
                .UseSoftlineBreakAsHardlineBreak()
                .Build();
        }

        public Block[] Parse(string markdownText)
        {
            List<Block> blocks = new List<Block>();

            string[] lines = markdownText.Split('\n');

            Debug.WriteLine("------");

            int textIndex = 0;

            foreach (var line in lines)
            {
                Block newBlock = new Block()
                {
                    Content = string.Empty,
                    FontSize = 16, // Default font size
                    StartIndex = textIndex,
                    ActualStartIndex = textIndex,
                    EndIndex = textIndex + line.Length,
                };

                if (line[^1]== '\r') 
                    newBlock.EndIndex--;

                var nodes = Markdown.Parse(line, pipeline);

                foreach(var node in nodes.Descendants().ToArray())
                {
                    if (node is LeafBlock leafBlock)
                        ModifyBlockBasedOnLeafBlock(ref newBlock, node);

                    Debug.WriteLine(node.GetType().Name);

                    if(node is LiteralInline literalInline)
                    {
                        var slice = literalInline.Content;

                        if (slice.Start > newBlock.StartIndex)
                            newBlock.ActualStartIndex = slice.Start;

                        Debug.WriteLine(slice.Text.Substring(slice.Start, slice.Length));
                       
                        newBlock.Content += slice.Text.Substring(slice.Start, slice.Length);
                    }


                }

                textIndex += line.Length+1;

                blocks.Add(newBlock);
            }

            return blocks.ToArray();
        }

        void ModifyBlockBasedOnLeafBlock(ref Block block, MarkdownObject leafBlock)
        {
            block.Content = GetPrefixFromObject(leafBlock);

            block.ActualStartIndex += block.Content.Length;

            if (leafBlock is HeadingBlock headingBlock)
                block.FontSize = 60 / headingBlock.Level;
        }

        string GetPrefixFromObject(MarkdownObject node) => node switch
        {
            HeadingBlock heading => new string('#', heading.Level) + " ",
            ListItemBlock listItem => "- ",
            _ => string.Empty,
        };
    }
}
