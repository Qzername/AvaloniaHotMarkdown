using Markdig.Syntax;
using AvaloniaInline = Avalonia.Controls.Documents;
using MarkdigInline = Markdig.Syntax.Inlines;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers;

public abstract class InlineHandler<T> : InlineHandlerBase where T : MarkdigInline.Inline
{
    public override bool CanHandle(object inline) => inline is T;

    public override AvaloniaInline.Inline[] Parse(object inline, CaretInformation caretInformation)
    {
        if (inline is T typedInline)
            return Parse(typedInline, caretInformation);

        throw new ArgumentException($"Expected inline of type {typeof(T).Name}, but got {inline?.GetType().Name}.");
    }

    public abstract AvaloniaInline.Inline[] Parse(T inline, CaretInformation caretInformation);

    protected static bool IsCaretWithinSpan(SourceSpan span, int index) => span.Start <= index && span.End >= index;
}
