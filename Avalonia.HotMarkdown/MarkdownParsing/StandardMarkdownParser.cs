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

namespace Avalonia.HotMarkdown.MarkdownParsing
{
    public class StandardMarkdownParser : IMarkdownParser
    {
        MarkdownPipeline pipeline;

        public StandardMarkdownParser()
        {
            pipeline = new MarkdownPipelineBuilder()
                .UseSoftlineBreakAsHardlineBreak()
                .UseEmphasisExtras()
                .Build();
        }

        public Block[] Parse(string markdownText)
        {
            List<Block> blocks = new List<Block>();

            string[] lines = markdownText.Split('\n');

            int textIndex = 0;

            foreach (var line in lines)
            {
                Block newBlock = new Block()
                {
                    FontSize = 16, // Default font size
                    StartIndex = textIndex,
                    ActualStartIndex = textIndex,
                    EndIndex = textIndex + line.Length,
                    Content = [],
                };

                if (line.Length > 0 && line[^1] == '\r')
                    newBlock.EndIndex--;

                var nodes = Markdown.Parse(line, pipeline);

                List<TextInfo> textInfos = new List<TextInfo>();

                TextInfo currentTextInfo = new();

                bool prefixGenerated = false;

                foreach (var node in nodes.Descendants().ToArray())
                {
                    if (node is LeafBlock leafBlock || node is ListBlock)
                    {
                        if(!prefixGenerated)
                            ModifyBlockBasedOnLeafBlock(ref newBlock, node);

                        prefixGenerated = true;
                    }

                    if(node is EmphasisInline emphasisInline)
                    {
                        if(emphasisInline.DelimiterChar =='*')
                        {
                            if (emphasisInline.DelimiterCount == 1)
                                currentTextInfo.IsItalic = true;
                            else if (emphasisInline.DelimiterCount == 2)
                                currentTextInfo.IsBold = true;
                        }
                        else if(emphasisInline.DelimiterChar == '~')
                            currentTextInfo.IsStrikethrough = true;
                        else if(emphasisInline.DelimiterChar == '+')
                            currentTextInfo.IsUnderline = true;
                    }

                    if (node is LiteralInline literalInline)
                    {
                        var slice = literalInline.Content;

                        if (slice.Start > newBlock.StartIndex)
                            newBlock.ActualStartIndex = slice.Start;

                        currentTextInfo.Text = slice.Text.Substring(slice.Start, slice.Length);
                        textInfos.Add(currentTextInfo);

                        currentTextInfo = new TextInfo();
                    }
                }

                newBlock.Content = [..newBlock.Content, ..textInfos.ToArray()];

                textIndex += line.Length + 1;

                blocks.Add(newBlock);
            }

            return blocks.ToArray();
        }

        void ModifyBlockBasedOnLeafBlock(ref Block block, MarkdownObject markdownObject)
        {
            block.Content = [new(GetPrefixFromObject(markdownObject)), ..block.Content];
                
            block.ActualStartIndex += block.Content.Length;

            if (markdownObject is HeadingBlock headingBlock)
                block.FontSize = 60 / headingBlock.Level;

            if(markdownObject is ListBlock)
                block.ReplacementPrefix = new TextInfo()
                {
                    Text= " •  ",
                    IsBold = true,
                };
        }

        string GetPrefixFromObject(MarkdownObject node) => node switch
        {
            HeadingBlock heading => new string('#', heading.Level) + " ",
            ListBlock listItem => "- ",
            _ => string.Empty,
        };
    }
}
