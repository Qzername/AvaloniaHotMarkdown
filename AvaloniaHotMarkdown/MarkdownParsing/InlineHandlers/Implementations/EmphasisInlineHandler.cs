using Avalonia.Controls.Documents;
using AvaloniaInlines = Avalonia.Controls.Documents;
using Avalonia.Media;
using Markdig.Syntax.Inlines;
using System.Diagnostics;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers.Implementations;

internal class EmphasisInlineHandler : InlineHandler<EmphasisInline>
{
    public override AvaloniaInlines.Inline[] Parse(EmphasisInline inline, CaretInformation caretInformation)
    {
        List<AvaloniaInlines.Inline> inlines = new();

        // Get the first child only (emphasis typically contains a single container with all the content)
        var firstChild = inline.FirstOrDefault();

        if (firstChild is null)
            return inlines.ToArray();

        var parsedInlines = InlineParser.ParseInline(firstChild, caretInformation);

        foreach (var parsedInline in parsedInlines)
        {
            // Only Run elements can have styling applied
            if (parsedInline is Run run)
                ApplyEmphasisStyling(run, inline);

            inlines.Add(parsedInline);
        }

        if (IsCaretWithinSpan(inline.Span, caretInformation.CaretIndex))
        {
            string prefixSuffix = new(inline.DelimiterChar, inline.DelimiterCount);

            Run prefixSuffixRun = new()
            {
                Text = prefixSuffix
            };

            inlines = [prefixSuffixRun, .. inlines, prefixSuffixRun];
        }
        else
            caretInformation.CaretIndex -= inline.DelimiterCount * 2;

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
