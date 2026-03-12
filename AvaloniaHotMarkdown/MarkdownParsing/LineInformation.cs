
namespace AvaloniaHotMarkdown.MarkdownParsing;

public struct LineInformation
{
    public int? CaretIndex { get; set; }
    public int LineYIndex { get; set; }
    public bool ShowFullText { get; set; }
    public SelectionInformation? SelectionInformation { get; set; }
}

public struct SelectionInformation
{
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
}