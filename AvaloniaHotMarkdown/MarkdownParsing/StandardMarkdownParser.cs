using Avalonia.Controls;
using Avalonia.Media;
using Markdig;

namespace AvaloniaHotMarkdown.MarkdownParsing;

public class StandardMarkdownParser : IMarkdownParser
{
    public Control[] Parse(string markdown)
    {
        return [new RichTextPresenter() {
            Text = "test",
            Foreground = Brushes.White
        }];
    }
}
