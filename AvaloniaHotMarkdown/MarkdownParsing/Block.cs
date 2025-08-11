using Avalonia.Controls.Presenters;

namespace AvaloniaHotMarkdown.MarkdownParsing
{
    public struct Block
    {
        public TextInfo[] Content { get; set; }
        public int FontSize { get; set; }
        public int StartIndex { get; set; }
        public int ActualStartIndex { get; set; }
        public int EndIndex { get; set; }
        public TextInfo? ReplacementPrefix { get; set; }
        public TextPresenter TextPresenter { get; set; }
    }
}
