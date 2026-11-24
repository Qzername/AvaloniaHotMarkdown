using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.Extensions;

public sealed class SpacerBlock : LeafBlock
{
    public SpacerBlock() : base(null) { }
}

/// <summary>
/// Every blank line in the source becomes one SpacerBlock in the AST.
/// ignoredPerGap = 0: every blank line is a block (block margins should be 0)
/// ignoredPerGap = 1: the first blank line of each gap is left to your normal margins
/// </summary>
public sealed class SpacerBlockExtension : IMarkdownExtension
{
    readonly int _ignoredPerGap;

    public SpacerBlockExtension(int ignoredPerGap = 0) => _ignoredPerGap = Math.Max(0, ignoredPerGap);

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        pipeline.UsePreciseSourceLocation();
        pipeline.DocumentProcessed += OnDocumentProcessed;
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) { }

    void OnDocumentProcessed(MarkdownDocument document)
    {
        var lineStarts = document.LineStartIndexes
            ?? throw new InvalidOperationException("SpacerBlockExtension needs UsePreciseSourceLocation()");

        Process(document, lineStarts);
    }

    void Process(ContainerBlock container, List<int> lineStarts)
    {
        bool isList = container is ListBlock;

        // Walk backwards so inserting blocks does not shift indices still to be visited
        for (int i = container.Count - 1; i >= 0; i--)
        {
            var block = container[i];

            if (block is ContainerBlock child)
                Process(child, lineStarts);

            if (isList)
                continue;

            // 1. Blank lines after the last block
            if (i == container.Count - 1)
            {
                int lastBlockLine = LineOf(lineStarts, block.Span.End);
                int lastTrailingLine = GetLastTrailingBlankLine(lineStarts, lastBlockLine);
                int firstTrailingSpacerLine = lastBlockLine + 1 + _ignoredPerGap;

                for (int lineIndex = firstTrailingSpacerLine; lineIndex <= lastTrailingLine; lineIndex++)
                {
                    container.Add(CreateSpacer(lineStarts, lineIndex, block.Span.End + 1));
                }
            }

            // 2. Blank lines before the first block
            if (i == 0)
            {
                int firstBlockLine = LineOf(lineStarts, block.Span.Start);
                int firstLeadingSpacerLine = _ignoredPerGap;
                int lastLeadingSpacerLine = firstBlockLine - 1;

                for (int lineIndex = lastLeadingSpacerLine; lineIndex >= firstLeadingSpacerLine; lineIndex--)
                {
                    container.Insert(0, CreateSpacer(lineStarts, lineIndex, block.Span.Start));
                }

                continue;
            }

            // 3. Blank lines between previous block and current block
            var prevBlock = container[i - 1];
            int prevLine = LineOf(lineStarts, prevBlock.Span.End);
            int currLine = LineOf(lineStarts, block.Span.Start);

            int firstSpacerLine = prevLine + 1 + _ignoredPerGap;
            int lastSpacerLine = currLine - 1;

            for (int lineIndex = lastSpacerLine; lineIndex >= firstSpacerLine; lineIndex--)
            {
                container.Insert(i, CreateSpacer(lineStarts, lineIndex, block.Span.Start));
            }
        }
    }

    static int GetLastTrailingBlankLine(List<int> lineStarts, int lastBlockLine)
    {
        if (lineStarts.Count == 0)
            return -1;

        int candidate = lineStarts.Count - 1;

        // If the final line index only marks the empty position right after a single trailing '\n',
        // ignore it so "asas\n" doesn't create an extra blank line beyond "\nasas\n\n".
        if (candidate > lastBlockLine + 1)
            return candidate;

        return candidate - 1;
    }

    static SpacerBlock CreateSpacer(List<int> lineStarts, int lineIndex, int fallbackEnd)
    {
        int start = lineStarts[lineIndex];
        int nextLineStart = lineIndex + 1 < lineStarts.Count
            ? lineStarts[lineIndex + 1]
            : Math.Max(start + 1, fallbackEnd);
        int end = Math.Max(start, nextLineStart - 1);

        return new SpacerBlock
        {
            Line = lineIndex + 1,
            Column = 0,
            Span = new SourceSpan(start, end)
        };
    }

    static int LineOf(List<int> lineStarts, int position)
    {
        int index = lineStarts.BinarySearch(position);
        return index >= 0 ? index : ~index - 1;
    }
}