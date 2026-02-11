using Markdig.Syntax.Inlines;
using System.Xml.Linq;
using MS = Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

public interface IBlockHandler
{
    public Block[] Handle(MS.Block markdownBlock, int textIndex);

    public static TextInfo[] HandleInlines(Inline[] inlines, ref Block block)
    {
        List<TextInfo> textInfos = new List<TextInfo>();

        foreach(var inline in inlines)
        {
            TextInfo currentTextInfo = new TextInfo();

            if (inline is EmphasisInline emphasisInline)
            {
                for (int i = 0; i < emphasisInline.DelimiterCount; i++)
                    currentTextInfo.DelimiterText += emphasisInline.DelimiterChar;

                if (emphasisInline.DelimiterChar == '*')
                {
                    if (emphasisInline.DelimiterCount == 1)
                        currentTextInfo.IsItalic = true;
                    else if (emphasisInline.DelimiterCount == 2)
                        currentTextInfo.IsBold = true;
                }
                else if (emphasisInline.DelimiterChar == '~')
                    currentTextInfo.IsStrikethrough = true;
                else if (emphasisInline.DelimiterChar == '_')
                {

                    if (emphasisInline.DelimiterCount == 1)
                        currentTextInfo.IsItalic = true;
                    else if (emphasisInline.DelimiterCount == 2)
                        currentTextInfo.IsUnderline = true;
                }
                else if (emphasisInline.DelimiterChar == '=')
                    currentTextInfo.IsHighlighted = true;

                continue;
            }

            if (inline is LiteralInline literalInline)
            {
                //sometimes markdig creates literal inlines next to each other, even if they are directly connected, we need to merge them (_test)
                if (literalInline.NextSibling is LiteralInline nextLiteral)
                {
                    literalInline.Content = new Markdig.Helpers.StringSlice(
                        literalInline.Content.ToString() + nextLiteral.Content.ToString()
                    );

                    nextLiteral.Content = new Markdig.Helpers.StringSlice(string.Empty);
                    nextLiteral.Remove();
                }

                var slice = literalInline.Content;

                if (slice.Start > block.StartIndex)
                    block.ActualStartIndex = slice.Start;

                currentTextInfo.Text = slice.Text.Substring(slice.Start, slice.Length);

                textInfos.Add(currentTextInfo);

                currentTextInfo = new TextInfo();
            }

            textInfos.Add(currentTextInfo);
        }

        return textInfos.ToArray();
    }
}
