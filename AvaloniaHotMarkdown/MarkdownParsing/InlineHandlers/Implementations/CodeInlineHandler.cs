using Avalonia.Controls.Documents;
using Avalonia.Media;
using Markdig.Syntax.Inlines;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers.Implementations
{
    internal class CodeInlineHandler : InlineHandler<CodeInline>
    {
        public override Run[] Parse(CodeInline inline, CaretInformation caretInformation)
        {
            var codeText = inline.Content.ToString();

            if(IsCaretWithinSpan(inline.Span, caretInformation.CaretIndex))
            {
                string prefixSuffix = new(inline.Delimiter, inline.DelimiterCount);
                codeText = $"{prefixSuffix}{codeText}{prefixSuffix}";
            }

            var run = new Run(codeText)
            {
                FontFamily = new FontFamily("Consolas, Courier New, monospace"),
                Background = Brushes.LightGray,
            };

            return [ run ];
        }
    }
}
