using Avalonia.Controls;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class HeadingBlockHandler : IBlockHandler
{
    public Control Handle(Block block)
    {
        return new RichTextPresenter() {
            Text = "test"
        };
    }
}
