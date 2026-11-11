using Avalonia.Controls.Documents;
using Avalonia.Media;
using Markdig.Syntax.Inlines;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers.Implementations
{
    internal class CodeInlineHandler : InlineHandler<CodeInline>
    {
        public override Run[] Parse(CodeInline inline)
        {
            var codeText = inline.Content.ToString();

            var run = new Run(codeText)
            {
                FontFamily = new FontFamily("Consolas, Courier New, monospace"),
                Background = Brushes.LightGray,
            };

            return [ run ];
        }
    }
}
