using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Diagnostics;

namespace Avalonia.HotMarkdown
{
    public class StandardMarkdownParser : IMarkdownParser
    {
        public AvaloniaBlock[] Parse(string markdownText)
        {/*
            markdownText = markdownText.Replace("\n\n", "\n<br>");
            if (markdownText.EndsWith("\n"))
                markdownText += "<br>";*/

            List<AvaloniaBlock> blocks = new List<AvaloniaBlock>();

            var pipeline = new MarkdownPipelineBuilder()
                .UseSoftlineBreakAsHardlineBreak()
                .Build();
            MarkdownDocument doc = Markdown.Parse(markdownText, pipeline);

            Debug.WriteLine("---");

            foreach (var child in doc.Descendants().ToArray())
            {
                Debug.WriteLine(child.GetType().Name);

                if (child is LeafBlock || child is LineBreakInline || child is HtmlInline)
                {
                    AvaloniaBlock newBlock = new AvaloniaBlock()
                    {
                        Content = string.Empty,
                        FontSize = 16, // Default font size
                        StartIndex = child.Span.Start,
                        EndIndex = child.Span.End
                    };

                    Debug.WriteLine($"{newBlock.StartIndex} - {newBlock.EndIndex}");

                    if (child is HeadingBlock headingBlock)
                        newBlock.FontSize = 60 / headingBlock.Level;

                    blocks.Add(newBlock);

                    continue;
                }

                if (child is not LeafInline inline)
                    continue;

                var currentBlock = blocks[^1];

                var slice = (inline as LiteralInline)!.Content;
                currentBlock.Content += slice.Text.Substring(slice.Start, slice.Length);

                blocks[^1] = currentBlock;
            }

            return [.. blocks];
        }
    }
}
