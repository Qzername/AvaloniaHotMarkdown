using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Diagnostics;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class ParagraphBlockHandler : BlockHandler
{
    public ParagraphBlockHandler(StandardMarkdownParser parser) : base(parser)
    {
    }

    public override Control Handle(Block block, string markdownText, LineInformation[] lineInformations)
    {
        ParagraphBlock paragraphBlock = block as ParagraphBlock;

        var container = TestParseInline(paragraphBlock.Inline.Descendants(), lineInformations.Any(x => x.ShowFullText));

        if (lineInformations.Length != 0)
            container.Tag = new CaretPositionOffset(0, lineInformations[0].LineYIndex);

        return container;
    }

    Control TestParseInline(IEnumerable<MarkdownObject> inlineObjects, bool parseAsFullText, int defaultXOffset = 0)
    {
        InlineCollection inlineCollection = new();

        foreach(var inline in inlineObjects)
        {
            Run[] runs = [];

            if (inline is LiteralInline literal)
                runs = [CreateLiteralInline(literal)];
            else if (inline is EmphasisInline emphasis)
                runs = CreateEmphasisInline(emphasis);

            inlineCollection.AddRange(runs);

            Debug.WriteLine(inline.GetType().Name);
        }

        return new TextBlock()
        {
            Inlines = inlineCollection
        };
    }

    Run CreateLiteralInline(LiteralInline literal)
    {
        return new Run()
        {
            Text = literal.Content.ToString()
        };
    }

    Run[] CreateEmphasisInline(EmphasisInline emphasis)
    {
        var test = emphasis.GetEnumerator();

        List<Run> runs = new List<Run>();

        do
        {
            if (test.Current is LiteralInline literal)
            {
                var run = CreateLiteralInline(literal);
                run.FontWeight = FontWeight.Bold;

                runs.Add(run);
            }
        }
        while (test.MoveNext());

        return runs.ToArray();
    }
}