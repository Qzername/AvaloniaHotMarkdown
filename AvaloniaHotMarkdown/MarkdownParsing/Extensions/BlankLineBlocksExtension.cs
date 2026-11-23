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
public sealed class BlankLineBlocksExtension : IMarkdownExtension
{
    readonly int _ignoredPerGap;

    public BlankLineBlocksExtension(int ignoredPerGap = 0) => _ignoredPerGap = ignoredPerGap;

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        pipeline.UsePreciseSourceLocation();
        pipeline.DocumentProcessed += OnDocumentProcessed;
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) { }

    void OnDocumentProcessed(MarkdownDocument document)
    {
        var lineStarts = document.LineStartIndexes
            ?? throw new InvalidOperationException("BlankLineBlocksExtension needs UsePreciseSourceLocation()");

        Process(document, lineStarts);
    }

    void Process(ContainerBlock container, List<int> lineStarts)
    {
        // backwards, so inserting doesn't shift indices still to be visited
        for (int i = container.Count - 1; i >= 0; i--)
        {
            var block = container[i];

            if (block is ContainerBlock child)
                Process(child, lineStarts);

            if (i == 0 || container is ListBlock)
                continue;

            int blankLines = LineOf(lineStarts, block.Span.Start)
                           - LineOf(lineStarts, container[i - 1].Span.End) - 1;

            for (int k = 0; k < blankLines - _ignoredPerGap; k++)
                container.Insert(i, new SpacerBlock());
        }
    }

    static int LineOf(List<int> lineStarts, int position)
    {
        int index = lineStarts.BinarySearch(position);
        return index >= 0 ? index : ~index - 1;
    }
}