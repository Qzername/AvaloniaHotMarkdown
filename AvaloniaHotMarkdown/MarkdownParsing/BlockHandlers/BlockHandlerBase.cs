using Avalonia.Controls;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

public abstract class BlockHandlerBase
{
    public abstract bool CanHandle(object content);
    public abstract Control[] Parse(object content);
}
