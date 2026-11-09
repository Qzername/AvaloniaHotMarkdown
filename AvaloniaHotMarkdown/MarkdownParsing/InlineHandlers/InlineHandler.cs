using AvaloniaInline = Avalonia.Controls.Documents;
using MarkdigInline = Markdig.Syntax.Inlines;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers;

public abstract class InlineHandler<T> : InlineHandlerBase where T : MarkdigInline.Inline
{
    public override bool CanHandle(object inline) => inline is T;

    public override AvaloniaInline.Inline[] Parse(object inline)
    {
        if (inline is T typedInline)
            return Parse(typedInline);

        throw new ArgumentException($"Expected inline of type {typeof(T).Name}, but got {inline?.GetType().Name}.");
    }

    public abstract AvaloniaInline.Inline[] Parse(T inline);
}
