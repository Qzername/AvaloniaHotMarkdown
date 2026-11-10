using Avalonia.Controls;
using Avalonia.Controls.Documents;
using AvaloniaInline = Avalonia.Controls.Documents.Inline;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers.Implementations;

internal class HeadingBlockHandler : BlockHandler<HeadingBlock>
{
    public override Control[] Parse(HeadingBlock heading)
    {
        if (heading.Inline is null)
            return [];

        // Rebuild line breaks for Setext headings
        if (heading.IsSetext)
            RebuildSetextInlines(heading);

        // Parse inline content into a TextBlock
        var textBlock = InlineParser.ParseContainer(heading.Inline);

        // Apply heading-level font sizing
        //TODO: Consider making this configurable via a resource dictionary or theme
        var fontSize = heading.Level switch
        {
            1 => 60,
            2 => 45,
            3 => 36,
            4 => 32,
            5 => 28,
            _ => 24
        };

        // Try to load custom resource size if available
        string resourceKey = $"HotMarkdownHeading{heading.Level}Size";
        if (textBlock.TryFindResource(resourceKey, out var resValue) && resValue is double customSize)
            fontSize = (int)customSize;

        // Apply font size to all runs in the TextBlock
        foreach (var inline in textBlock.Inlines)
            ApplyFontSizeToInline(inline, fontSize);

        return [textBlock];
    }

    /// <summary>
    /// Restores line breaks in Setext headings that Markdig may merge into a
    /// single <see cref="LiteralInline"/>.
    /// 
    /// For example, Markdig can parse:
    /// <code>
    /// This is a heading
    /// With multiple lines
    /// ===================
    /// </code>
    /// as a single LiteralInline("This is a heading With multiple lines")
    /// instead of preserving the source line break.
    /// 
    /// This method restores those missing line breaks while preserving the
    /// remaining inline elements.
    /// </summary>
    static void RebuildSetextInlines(HeadingBlock heading)
    {
        if (heading.Inline is null)
            return;

        var root = new ContainerInline();
        var currentLine = 0;

        foreach (var inline in heading.Inline.ToList())
        {
            while (currentLine + 1 < heading.Lines.Count && inline.Span.Start >= heading.Lines.Lines[currentLine + 1].Slice.Start)
            {
                root.AppendChild(new LineBreakInline());
                currentLine++;
            }

            inline.Remove();

            if (inline is not LiteralInline literal)
            {
                root.AppendChild(inline);
                continue;
            }

            var lines = literal.Content
                .ToString()
                .Split(["\r\n", "\r", "\n"], StringSplitOptions.None);

            for (var i = 0; i < lines.Length; i++)
            {
                if (i > 0)
                    root.AppendChild(new LineBreakInline());

                if (lines[i].Length > 0)
                    root.AppendChild(new LiteralInline(lines[i]));
            }
        }

        heading.Inline = root;
    }

    /// <summary>
    /// Recursively applies font size to all inline elements, including nested spans.
    /// </summary>
    static void ApplyFontSizeToInline(AvaloniaInline inline, int fontSize)
    {
        if (inline is Run run)
            run.FontSize = fontSize;
        else if (inline is Span span)
        {
            span.FontSize = fontSize;
            foreach (var child in span.Inlines)
                ApplyFontSizeToInline(child, fontSize);
        }
        // LineBreak and other inline types don't have text styling
    }
}
