using Avalonia.Controls;
using Avalonia.Controls.Documents;
using System.Text.RegularExpressions;

namespace AvaloniaHotMarkdown.MarkdownParsing;

/// <summary>
/// to force markdig to make breakes, this class helps on preparation of markdown and later on removing the preparation from ready to render control
/// </summary>
internal class MarkdownPreparer
{
    const string ZERO_WIDTH_SPACE = "\u200B"; // Zero-width space to prevent paragraph breaks

    public static string PrepareForcedBreaks(string rawMarkdown)
    {
        // Replace multiple consecutive newlines with explicit markdown line breaks (two spaces + newline)
        // To prevent Markdown from interpreting multiple blank lines as paragraph breaks,
        // we use a zero-width space (U+200B) between consecutive line breaks.
        // This keeps them in the same paragraph while still allowing multiple line breaks.
        var normalized = rawMarkdown.Replace("\r\n", "\n");

        return Regex.Replace(normalized, "\n{2,}", m =>
        {
            // For n consecutive newlines, create (n-1) hard line breaks separated by zero-width space
            // \n\n -> \n  \n (1 break)
            // \n\n\n -> \n  \n​\n  \n (2 breaks with zero-width space between them)
            // This prevents Markdown from treating multiple blank lines as paragraph breaks
            int count = m.Value.Length - 1;
            var breaks = new List<string>();

            for (int i = 0; i < count; i++)
            {
                breaks.Add("  \n");

                if (i < count - 1) // Add zero-width space between breaks (but not after the last one)
                    breaks.Add(ZERO_WIDTH_SPACE);
            }

            return "\n" + string.Concat(breaks);
        });
    }

    /// <summary>
    /// Recursively removes zero-width space markers from all text content within a control.
    /// This handles TextBlocks and their inlines, as well as any other controls with text content.
    /// </summary>
    public static void RemovePreparationFromControl(Control control)
    {
        if (control is not TextBlock textBlock)
            return;

        // Remove zero-width spaces from all inlines in the TextBlock
        foreach (var inline in textBlock.Inlines)
            RemoveZeroWidthSpacesFromInline(inline);
    }

    /// <summary>
    /// Recursively removes zero-width space markers from text content within an inline element.
    /// </summary>
    static void RemoveZeroWidthSpacesFromInline(Inline inline)
    {
        if (inline is Run run)
        {
            // Remove zero-width spaces from Run text
            run.Text = run.Text.Replace(ZERO_WIDTH_SPACE, string.Empty);
        }
        else if (inline is Span span)
        {
            // Recursively process Span's children
            foreach (var child in span.Inlines)
                RemoveZeroWidthSpacesFromInline(child);
        }
        // LineBreak and other inline types don't have text content, so nothing to do
    }
}