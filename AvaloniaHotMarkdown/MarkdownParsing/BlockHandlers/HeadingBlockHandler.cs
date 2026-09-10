using Avalonia.Controls;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class HeadingBlockHandler(StandardMarkdownParser parser) : BlockHandler(parser)
{
    public override Control Handle(Block block, string markdownText, LineInformation[] lineInformations)
    {
        HeadingBlock headingBlock = (HeadingBlock)block;

        int defaultXOffset = headingBlock.Level;

        if (headingBlock.IsSetext)
        {
            RebuildSetextInlines(headingBlock);
            defaultXOffset = 0;
        }

        bool showFullText = lineInformations.Any(x => x.ShowFullText);
        var container = ParseInline(headingBlock.Inline.Descendants(), showFullText, defaultXOffset) as StackPanel;

        container.Tag = new CaretPositionOffset(0, lineInformations[0].LineYIndex);

        if (showFullText)
            if (headingBlock.IsSetext)
            {
                StretchWrapPanel setextSuffixContainer = new()
                {
                    Tag = new CaretPositionOffset(0, lineInformations[^1].LineYIndex)
                };

                var setextSuffixPresenter = StylizationHelper.CreateNewPresenter();

                char setextSuffixChar = headingBlock.Level == 1 ? '=' : '-';
                string setextSuffix = new(setextSuffixChar, headingBlock.HeaderCharCount);

                setextSuffixPresenter.Text = setextSuffix;

                setextSuffixContainer.Children.Add(setextSuffixPresenter);
                container.Children.Add(setextSuffixContainer);
            }
            else
            {
                var richTextPresenter = StylizationHelper.CreateNewPresenter();
                richTextPresenter.Text = new string('#', headingBlock.Level) + " ";
                (container.Children[0] as StretchWrapPanel).Children.Insert(0, richTextPresenter);
            }


        // --- stylization --- 
        List<Control> richTexts = [];

        foreach (StretchWrapPanel wrapPanel in container.Children.Where(x => x is StretchWrapPanel))
            richTexts.AddRange(wrapPanel.Children.ToList());

        string resourceKey = $"HotMarkdownHeading{headingBlock.Level}Size";

        foreach (RichTextPresenter item in richTexts)
        {
            if (container.TryFindResource(resourceKey, out var resValue) && resValue is double customSize)
                item.FontSize = customSize;
            else
                item.FontSize = headingBlock.Level switch
                {
                    1 => 60,
                    2 => 45,
                    _ => 30
                };
        }

        return container;
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
    /// as:
    /// <code>
    /// HeadingBlock
    ///   ContainerInline
    ///     LiteralInline("This is a heading With multiple lines")
    /// </code>
    /// 
    /// instead of preserving the source line break:
    /// <code>
    /// HeadingBlock
    ///   ContainerInline
    ///     LiteralInline("This is a heading")
    ///     LineBreakInline
    ///     LiteralInline("With multiple lines")
    /// </code>
    /// 
    /// <see cref="MarkdownPipelineBuilderExtensions.UseSoftlineBreakAsHardlineBreak"/>
    /// normally converts soft line breaks to <see cref="LineBreakInline"/>, but
    /// Setext heading parsing can merge the lines before that processing occurs.
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
}
