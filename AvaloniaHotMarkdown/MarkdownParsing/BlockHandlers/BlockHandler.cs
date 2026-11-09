using Avalonia.Controls;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

public abstract class BlockHandler<T> : BlockHandlerBase where T : Block 
{
    public override bool CanHandle(object content) => content is T;

    public override Control[] Parse(object content)
    {
        if (content is T block)
            return Parse(block);

        throw new ArgumentException("Content is not of the expected type", nameof(content));
    }
    
    public abstract Control[] Parse(T content);
}