using Avalonia.Controls.Documents;
using Avalonia.Media;
using Markdig.Syntax.Inlines;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers.Implementations;

internal class EmphasisInlineHandler : InlineHandler<EmphasisInline>
{
    public override Avalonia.Controls.Documents.Inline[] Parse(EmphasisInline inline)
    {
        List<Avalonia.Controls.Documents.Inline> inlines = new();

        // Get the first child only (emphasis typically contains a single container with all the content)
        var firstChild = inline.FirstOrDefault();
        if (firstChild != null)
        {
            var parsedInlines = InlineParser.ParseInline(firstChild);

            foreach (var parsedInline in parsedInlines)
            {
                // Only Run elements can have styling applied
                if (parsedInline is Run run)
                    ApplyEmphasisStyling(run, inline);

                inlines.Add(parsedInline);
            }
        }

        return inlines.ToArray();
    }

    private void ApplyEmphasisStyling(Run run, EmphasisInline emphasis)
    {
        if (emphasis.DelimiterChar == '~')
        {
            run.TextDecorations = TextDecorations.Strikethrough;
            return;
        }

        if (emphasis.DelimiterCount == 1)
            run.FontStyle = FontStyle.Italic;
        else if (emphasis.DelimiterCount == 2)
            run.FontWeight = FontWeight.Bold;
        else if (emphasis.DelimiterCount >= 3)
        {
            run.FontWeight = FontWeight.Bold;
            run.FontStyle = FontStyle.Italic;
        }
    }
}
