using Avalonia.Controls;
using AvaloniaHotMarkdown.Controls;
using AvaloniaHotMarkdown.MarkdownParsing.Extensions;
using Markdig.Syntax;
using System.Diagnostics;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers.Implementations;

internal class SpacerBlockHandler(TextBlockClickedCallback textBlockClickedCallback) : BlockHandler<SpacerBlock>(textBlockClickedCallback)
{
    public override Control[] Parse(SpacerBlock content, CaretInformation caretInformation) => [
            new CaretSelectableTextBlock(TextBlockClickedCallback, content.Span) {
                CaretIndex = caretInformation.CaretIndex == content.Span.Start ? 0 : null
            }
        ];
}