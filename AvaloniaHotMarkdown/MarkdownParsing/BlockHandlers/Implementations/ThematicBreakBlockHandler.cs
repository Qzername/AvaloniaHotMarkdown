using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaHotMarkdown.Controls;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers.Implementations;

internal class ThematicBreakBlockHandler(TextBlockClickedCallback textBlockClickedCallback) : BlockHandler<ThematicBreakBlock>(textBlockClickedCallback)
{
    public override Control[] Parse(ThematicBreakBlock content, CaretInformation caretInformation)
    {
        //todo: make it customizable 
        return [new Border
        {
            Height = 2,
            Background = Brushes.Gray,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center
        }];
    }
}
