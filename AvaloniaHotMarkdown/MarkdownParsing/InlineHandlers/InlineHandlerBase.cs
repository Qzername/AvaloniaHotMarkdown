using Avalonia.Controls.Documents;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers;

public abstract class InlineHandlerBase
{
    public abstract bool CanHandle(object inline);
    public abstract Inline[] Parse(object inline);
}