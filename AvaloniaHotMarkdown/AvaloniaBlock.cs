using AvaloniaHotMarkdown.MarkdownParsing;

namespace AvaloniaHotMarkdown
{
    public struct AvaloniaBlock
    {
        public Block BaseBlock { get; set; }
        public LineHandler LineHandler { get; set; }
    }
}
