using Avalonia.Controls;
using AvaloniaHotMarkdown.Controls;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

public abstract class BlockHandler<T> : BlockHandlerBase where T : Block 
{
    protected BlockHandler(TextBlockClickedCallback textBlockClickedCallback) : base(textBlockClickedCallback)
    {
    }

    public override bool CanHandle(object content) => content is T;

    public override Control[] Parse(object content, CaretInformation caretInformation)
    {
        if (content is T block)
            return Parse(block, caretInformation);

        throw new ArgumentException("Content is not of the expected type", nameof(content));
    }
    
    public abstract Control[] Parse(T content, CaretInformation caretInformation);
}