using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;
using AvaloniaHotMarkdown.MarkdownParsing.Extensions;
using AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers;
using Markdig;
using Markdig.Extensions.EmphasisExtras;
using Markdig.Extensions.Tables;
using Markdig.Syntax;
using System.Drawing;
using Markdig.Extensions.TaskLists;
using Markdig.Syntax.Inlines;



#if DEBUG
using System.Diagnostics;
#endif

namespace AvaloniaHotMarkdown.MarkdownParsing;

public delegate void TextUpdateRequestHandler(Control control, int oldTextLength, string newText);

public class StandardMarkdownParser : IMarkdownParser
{
    readonly Dictionary<Type, BlockHandler> blockHandlers;
    readonly Dictionary<Type, IInlineHandler> inlineHandlers;
    readonly MarkdownPipeline markdownPipeline;

    //TODO: change this when inline parsing will be reworked
    public TextUpdateRequestHandler TextUpdateRequestHandler { get => _textUpdateRequestHandler; }
    readonly TextUpdateRequestHandler _textUpdateRequestHandler;

    public StandardMarkdownParser(TextUpdateRequestHandler textUpdateRequestHandler)
    {
        _textUpdateRequestHandler = textUpdateRequestHandler;

        blockHandlers = new()
        {
            { typeof(ParagraphBlock), new ParagraphBlockHandler(this) },
            { typeof(HeadingBlock), new HeadingBlockHandler(this) },
            { typeof(ListBlock), new ListBlockHandler(this) },
            { typeof(Table), new TableHandler(this)  },
        };

        inlineHandlers = new()
        {
            { typeof(TaskList), new TaskListInlineHandler() },
            { typeof(LineBreakInline), new LineBreakInlineHandler() },
            { typeof(EmphasisInline), new EmphasisInlineHandler() },
            { typeof(LiteralInline), new LiteralInlineHandler() },
        };

        markdownPipeline = BuildPipeline(); 
    }

    static MarkdownPipeline BuildPipeline()
    {
        var builder = new MarkdownPipelineBuilder()
         .UseTaskLists()
         .UsePipeTables()
         .UseEmphasisExtras(EmphasisExtraOptions.Strikethrough | EmphasisExtraOptions.Marked)
         .DisableHtml()
         .Use<UnwrapTaskListExtension>()
         .Use<StrictListExtension>()
         .UseSoftlineBreakAsHardlineBreak();

        builder.Extensions.Insert(0, new UnwrapTaskListExtension());
        builder.Extensions.Insert(1, new StrictListExtension());

        return builder.Build();
    }

    public Control[] Parse(string markdown, CaretInformation caretInformation)
    {
        List<Control> controls = [];

        var lines = markdown.Split('\n'); //for empty line parsing

        int[] fullTextLinesIndexes = GetFullTextLines(caretInformation, lines);
        Point caretPosition = IndexToTextPosition(caretInformation.CaretIndex, lines);

        //check for empty lines at start
        int endOfEmptyLinesAtStart = 0;
        for (int i = 0; i < lines.Length; i++)
            if (!string.IsNullOrWhiteSpace(lines[i]))
            {
                endOfEmptyLinesAtStart = i;
                break;
            }

        controls.AddRange(GenerateEmptyLines(0, endOfEmptyLinesAtStart, caretPosition));

        var document = Markdown.Parse(markdown, markdownPipeline);

        //Debug.WriteLine(document.ToAstString());

        Point selectionStart = new(0, 0);
        Point selectionEnd = new(0, 0);

        if (caretInformation.SelectionInformation is not null)
        {
            selectionStart = IndexToTextPosition(caretInformation.SelectionInformation.Value.StartIndex, lines);
            selectionEnd = IndexToTextPosition(caretInformation.SelectionInformation.Value.EndIndex, lines);

            if (selectionEnd.Y < selectionStart.Y)
                (selectionEnd, selectionStart) = (selectionStart, selectionEnd);
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

            //check where does block end
            int blockEnd = (i == document.Count - 1 ? lines.Length : document[i + 1].Line);

            List<LineInformation> lineInformation = [];
            for (int j = block.Line; j < blockEnd; j++)
            {
                // we need to check for empty lines
                if (string.IsNullOrWhiteSpace(lines[j].Replace('\n', ' ')))
                    break;

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

            var control = ParseBlock(block, markdown, [.. lineInformation]);
            blockHandlers[block.GetType()].UpdateTextEffects(control, [.. lineInformation]);

            controls.Add(control);
        }

        //check for empty lines at the end
        int startOfEmptyLinesAtEnd = 0;
        for (int i = lines.Length - 1; i >= 0; i--)
            if (!string.IsNullOrWhiteSpace(lines[i]))
            {
                startOfEmptyLinesAtEnd = i+1;
                break;
            }

        //we dont want to duplicate empty lines since they were already added at the start
        if(startOfEmptyLinesAtEnd != 0)
            controls.AddRange(GenerateEmptyLines(startOfEmptyLinesAtEnd, lines.Length, caretPosition));

        return [.. controls];
    }

    static Control[] GenerateEmptyLines(int start, int end, Point caretPosition)
    {
        List<Control> controls = [];

        for (int i = start; i < end; i++)
        {
            var emptyBlock = new RichTextPresenter();

            //check for caret as well
            if (caretPosition.Y == i)
            {
                //TODO: change this...
                emptyBlock.CaretBrush = Brushes.White;
                emptyBlock.CaretIndex = 0;
                emptyBlock.ShowCaret();
            }

            emptyBlock.Tag = new CaretPositionOffset(0, i);

            controls.Add(emptyBlock);
        }

        return [.. controls];
    }

    static int[] GetFullTextLines(CaretInformation caretInformation, string[] lines)
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

        List<int> result = [];

        int currentIndex = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            int lineLength = lines[i].Length + 1; //+1 for the newline character

            if (currentIndex + lineLength > min && currentIndex <= max)
                result.Add(i);

            currentIndex += lineLength;
        }

        return [.. result];
    }

    static Point IndexToTextPosition(int index, string[] lines)
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

    public Control ParseBlock(Block block, string markdownText, LineInformation[] lineInformation)
    {
        Type type = block.GetType();

        if (!blockHandlers.TryGetValue(type, out BlockHandler? value))
            throw new NotSupportedException("This block is not supported: " + type.Name);

        return value.Handle(block, markdownText, lineInformation);
    }

    public Control ParseInline(IEnumerable<MarkdownObject> inlineObjects, bool parseAsFullText, int defaultXOffset = 0)
    {
        InlineParsingContext context = new InlineParsingContext(parseAsFullText, defaultXOffset);

        foreach (var markdownObject in inlineObjects)
        {
            Type type = markdownObject.GetType();

            if (!inlineHandlers.TryGetValue(type, out IInlineHandler? value))
                continue;
                //throw new NotSupportedException("This Inline element is not supported: " + type.Name);

            value.Handle(markdownObject, context, _textUpdateRequestHandler);
        }

        return context.Container;
    }
}