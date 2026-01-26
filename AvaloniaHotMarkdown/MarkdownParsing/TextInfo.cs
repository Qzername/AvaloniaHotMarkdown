namespace AvaloniaHotMarkdown.MarkdownParsing
{
    public struct TextInfo
    {
        public string Text;
        public bool IsBold;
        public bool IsItalic;
        public bool IsStrikethrough;
        public bool IsUnderline;

        public string DelimiterText;

        public TextInfo(string text, bool isBold = false, bool isItalic = false, bool isStrikethrough = false, bool isUnderline = false)
        {
            Text = text;
            IsBold = isBold;
            IsItalic = isItalic;
            IsStrikethrough = isStrikethrough;
            IsUnderline = isUnderline;

            DelimiterText = string.Empty;
        }

        public TextInfo()
        {
            Text = string.Empty;
            IsBold = false;
            IsItalic = false;
            IsStrikethrough = false;
            IsUnderline = false;
        }
    }
}
