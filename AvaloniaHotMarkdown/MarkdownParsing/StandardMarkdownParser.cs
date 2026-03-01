using Avalonia.Controls;
using AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;
using Markdig;
using Markdig.Extensions.EmphasisExtras;
using Markdig.Syntax;
using System.Diagnostics;
using System.Drawing;

namespace AvaloniaHotMarkdown.MarkdownParsing;

public class StandardMarkdownParser : IMarkdownParser
{
    readonly Dictionary<Type, BlockHandler> handlers;
    readonly MarkdownPipeline markdownPipeline;

    public StandardMarkdownParser()
    {
        handlers = new()
        {
            { typeof(ParagraphBlock), new ParagraphBlockHandler(this) },
            { typeof(HeadingBlock), new HeadingBlockHandler(this) },
            { typeof(ListBlock), new ListBlockHandler(this) },
        };

        markdownPipeline = new MarkdownPipelineBuilder()
            .UseEmphasisExtras(EmphasisExtraOptions.Strikethrough | EmphasisExtraOptions.Marked)
            .Build();
    }

    public Control[] Parse(string markdown, CaretInformation caretInformation)
    {
        List<Control> controls = new();

        var lines = markdown.Split('\n'); //for empty line parsing

        var document = Markdown.Parse(markdown, markdownPipeline);

        int[] fullTextLinesIndexes = GetFullTextLines(caretInformation, lines);
        Point caretPosition = GetCaretPosition(caretInformation, lines);

        for (int i =0; i< document.Count; i++)
        {
            var block = document[i];

            //check for empty lines between blocks, if there are any, add an empty textblock for each of them
            if (i>0 && document[i-1].Line != block.Line -1 )
                for(int j = document[i-1].Line; j < block.Line;j++)
                    if (string.IsNullOrWhiteSpace(lines[j]))
                        controls.Add(new TextBlock());
                    
            controls.Add(ParseBlock(block, fullTextLinesIndexes.Contains(i)));
        }

        return controls.ToArray();
    }

    int[] GetFullTextLines(CaretInformation caretInformation, string[] lines)
    {
        int min = int.Min(caretInformation.Index, caretInformation.SelectionStart ?? caretInformation.Index);
        int max = int.Max(caretInformation.Index, caretInformation.SelectionStart ?? caretInformation.Index);

        //get indexes of lines that are between min and max

        List<int> result = new();

        int currentIndex = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            int lineLength = lines[i].Length + 1; //+1 for the newline character
            
            if (currentIndex + lineLength > min && currentIndex < max)
                result.Add(i);

            currentIndex += lineLength;
        }

        return result.ToArray();
    }

    Point GetCaretPosition(CaretInformation caretInformation, string[] lines)
    {
        int currentIndex = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            int lineLength = lines[i].Length + 1; //+1 for the newline character
            
            if (currentIndex + lineLength > caretInformation.Index)
                return new Point(caretInformation.Index - currentIndex, i);

            currentIndex += lineLength;
        }

        return new Point(0, lines.Length - 1);
    }

    public Control ParseBlock(Block block, bool parseAsFullText)
    {
        Type type = block.GetType();

        if (!handlers.ContainsKey(type))
            throw new NotSupportedException("This block is not supported: " + type.Name);

        return handlers[type].Handle(block, parseAsFullText);
    }
}