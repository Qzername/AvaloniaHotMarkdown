namespace AvaloniaHotMarkdown.MarkdownParsing
{
    public interface IMarkdownParser
    {
        public Block[] Parse(string markdownText);
    }
}
