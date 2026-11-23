using Avalonia.Controls;
using AvaloniaHotMarkdown.Controls;
using AvaloniaHotMarkdown.MarkdownParsing.Extensions;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers.Implementations;

internal class BlankBlockHandler(TextBlockClickedCallback textBlockClickedCallback) : BlockHandler<SpacerBlock>(textBlockClickedCallback)
{
    public override Control[] Parse(SpacerBlock content, CaretInformation caretInformation) => [new CaretSelectableTextBlock(TextBlockClickedCallback, content.Span)];
}