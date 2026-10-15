using Avalonia.Controls;
using Avalonia.Controls.Documents;
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
            Run run;

            if (inline is LiteralInline literal)
                run = CreateLiteralInline(literal);

            else if (inline is EmphasisInline emphasis)
                run = CreateEmphasisInline(emphasis);

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

    Run CreateEmphasisInline(EmphasisInline emphasis)
    {
        return new Run()
        {
        };
    }
}
