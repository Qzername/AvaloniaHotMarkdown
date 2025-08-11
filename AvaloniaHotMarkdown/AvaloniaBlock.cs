using AvaloniaHotMarkdown.MarkdownParsing;

namespace AvaloniaHotMarkdown
{
    public struct AvaloniaBlock
    {
        public string ShortText { get; set; }
        public string LongText { get; set; }
        public Block BaseBlock { get; set; }
        public LineHandler LineHandler { get; set; }
    }
}
