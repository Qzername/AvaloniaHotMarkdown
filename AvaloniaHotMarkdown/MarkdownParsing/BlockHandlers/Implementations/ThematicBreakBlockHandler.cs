using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers.Implementations;

internal class ThematicBreakBlockHandler : BlockHandler<ThematicBreakBlock>
{
    public override Control[] Parse(ThematicBreakBlock content)
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
