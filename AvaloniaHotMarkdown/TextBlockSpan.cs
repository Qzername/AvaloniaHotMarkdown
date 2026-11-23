
using Markdig.Syntax;

namespace AvaloniaHotMarkdown;

/// <summary>
/// Information about offset in text of current textblock
/// </summary>
public struct TextBlockSpan
{
    public int Start { get; set; }
    public int End { get; set; }
    public readonly int Length => End - Start + 1;

    public static implicit operator TextBlockSpan(SourceSpan sourceSpan) 
        => new()
        {
            Start = sourceSpan.Start,
            End = sourceSpan.End
        };
}
