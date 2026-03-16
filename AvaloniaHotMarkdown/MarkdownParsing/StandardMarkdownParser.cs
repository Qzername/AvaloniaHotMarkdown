using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;
using Markdig;
using Markdig.Extensions.EmphasisExtras;
using Markdig.Syntax;
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
        Point caretPosition = IndexToTextPosition(caretInformation.CaretIndex, lines);

        Point selectionStart = new Point(0, 0);
        Point selectionEnd = new Point(0, 0);

        if (caretInformation.SelectionInformation is not null)
        {
            selectionStart = IndexToTextPosition(caretInformation.SelectionInformation.Value.StartIndex, lines);
            selectionEnd = IndexToTextPosition(caretInformation.SelectionInformation.Value.EndIndex, lines);

            if (selectionEnd.Y < selectionStart.Y)
            {
                var temp = selectionStart;
                selectionStart = selectionEnd;
                selectionEnd = temp;
            }
        }

        for (int i = 0; i < document.Count; i++)
        {
            var block = document[i];

            //check for empty lines between blocks, if there are any, add an empty textblock for each of them
            if (i > 0 && document[i - 1].Line != block.Line - 1)
                for (int j = document[i - 1].Line; j < block.Line; j++)
                    if (string.IsNullOrWhiteSpace(lines[j]))
                    {
                        var emptyBlock = new RichTextPresenter();

                        //check for caret as well
                        if (caretPosition.Y == j)
                        {
                            //TODO: change this...
                            emptyBlock.CaretBrush = Brushes.White;
                            emptyBlock.CaretIndex = 0;
                            emptyBlock.ShowCaret();
                        }
                        emptyBlock.Tag = new CaretPositionOffset(0, j);

                        controls.Add(emptyBlock);
                    }

            int blockEnd = (i == document.Count - 1 ? lines.Length : document[i + 1].Line);

            List<LineInformation> lineInformation = new();
            for (int j = block.Line; j < blockEnd; j++)
            {
                SelectionInformation? selectionInformation = null;

                if (caretInformation.SelectionInformation is not null &&
                    j >= selectionStart.Y
                    && j <= selectionEnd.Y)
                {
                    selectionInformation = new SelectionInformation
                    {
                        StartIndex = j == selectionStart.Y ? selectionStart.X : 0,
                        EndIndex = j == selectionEnd.Y ? selectionEnd.X : int.MaxValue
                    };
                }

                lineInformation.Add(new LineInformation
                {
                    LineYIndex = j,
                    CaretIndex = j == caretPosition.Y ? caretPosition.X : null,
                    ShowFullText = fullTextLinesIndexes.Contains(j),
                    SelectionInformation = selectionInformation
                });
            }

            var control = ParseBlock(block, lineInformation.ToArray());
            handlers[block.GetType()].UpdateTextEffects(control, lineInformation.ToArray());

            controls.Add(control);
        }

        return controls.ToArray();
    }

    int[] GetFullTextLines(CaretInformation caretInformation, string[] lines)
    {
        int min = caretInformation.CaretIndex;
        int max = caretInformation.CaretIndex;

        if (caretInformation.SelectionInformation is not null)
        {
            int start = caretInformation.SelectionInformation.Value.StartIndex;
            int end = caretInformation.SelectionInformation.Value.EndIndex;

            min = int.Min(start, end);
            max = int.Max(start, end);
        }

        //get indexes of lines that are between min and max

        List<int> result = new();

        int currentIndex = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            int lineLength = lines[i].Length + 1; //+1 for the newline character

            if (currentIndex + lineLength > min && currentIndex <= max)
                result.Add(i);

            currentIndex += lineLength;
        }

        return result.ToArray();
    }

    Point IndexToTextPosition(int index, string[] lines)
    {
        int currentIndex = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            int lineLength = lines[i].Length + 1; //+1 for the newline character

            if (currentIndex + lineLength > index)
                return new Point(index - currentIndex, i);

            currentIndex += lineLength;
        }

        return new Point(0, lines.Length - 1);
    }

    public Control ParseBlock(Block block, LineInformation[] lineInformation)
    {
        Type type = block.GetType();

        if (!handlers.ContainsKey(type))
            throw new NotSupportedException("This block is not supported: " + type.Name);

        return handlers[type].Handle(block, lineInformation);
    }
}