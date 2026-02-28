using Avalonia.Controls;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class ListBlockHandler : BlockHandler
{
    public override Control Handle(Block block)
    {
        return new RichTextPresenter()
        {
            Text = "list block"
        };
    }
}
