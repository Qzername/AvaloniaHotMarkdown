using Avalonia.Controls;
using Avalonia.Controls.Documents;
using AvaloniaHotMarkdown.MarkdownParsing;
using AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;
using AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers.Implementations;
using AvaloniaHotMarkdown.MarkdownParsing.Extensions;
using Markdig;
using Markdig.Extensions.EmphasisExtras;
using Markdig.Syntax;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AvaloniaHotMarkdown.MarkdownParsing;

/// <summary>
/// New markdown parser aiming to move onto new Run controls instead of creating seperate textblocks for every Inline type
/// </summary>
internal class MarkdownParser : IMarkdownParser
{
    readonly static BlockHandlerBase[] handlers = [
        new EmptyBlockHandler(),
        new HeadingBlockHandler(),
        new ListBlockHandler(),
        new QuoteBlockHandler(),
        new TableHandler(),
        new ParagraphBlockHandler(),
        new ThematicBreakBlockHandler(),
    ];

    static MarkdownPipeline BuildPipeline()
    {
        var builder = new MarkdownPipelineBuilder()
            .UseTaskLists()
            .UsePipeTables()
            .UseEmphasisExtras(EmphasisExtraOptions.Strikethrough |
                            EmphasisExtraOptions.Marked |
                            EmphasisExtraOptions.Superscript |
                            EmphasisExtraOptions.Subscript)
            .EnableTrackTrivia()
            .DisableHtml()
            .UseSoftlineBreakAsHardlineBreak();

        builder.Extensions.Insert(0, new StrictListExtension());
        builder.Extensions.Insert(1, new UnlistTaskListExtension());

        return builder.Build();
    }

    public Control[] Parse(string markdown, CaretInformation caretInformation)
    {
        List<Control> controls = new();

        var document = Markdown.Parse(PrepareMarkdownForForcedBreaks(markdown), BuildPipeline());

        foreach(var control in document)
        {
            var handler = FindHandler(control);
            controls.AddRange(handler.Parse(control));
        }

        // Post-process all controls to remove zero-width space markers
        foreach (var control in controls)
        {
            RemoveZeroWidthSpacesFromControl(control);
        }

        return controls.ToArray();
    }

    static BlockHandlerBase FindHandler(Block block)
    {
        return handlers.Single(x => x.CanHandle(block));
    }

    const string ZERO_WIDTH_SPACE = "\u200B"; // Zero-width space to prevent paragraph breaks

    /// <summary>
    /// Recursively removes zero-width space markers from all text content within a control.
    /// This handles TextBlocks and their inlines, as well as any other controls with text content.
    /// </summary>
    static void RemoveZeroWidthSpacesFromControl(Control control)
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

    public string PrepareMarkdownForForcedBreaks(string rawMarkdown)
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
}