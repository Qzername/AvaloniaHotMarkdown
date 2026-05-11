#if DEBUG
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Text;

namespace AvaloniaHotMarkdown.MarkdownParsing.Extensions;

public static class MarkdigAstExtensions
{
    public static string ToAstString(this MarkdownDocument document)
    {
        var sb = new StringBuilder();
        PrintNode(document, sb, 0);
        return sb.ToString();
    }

    static void PrintNode(MarkdownObject node, StringBuilder sb, int level)
    {
        string indent = new string(' ', level * 2);
        sb.AppendLine($"{indent}{node.GetType().Name}");

        if (node is ContainerBlock container)
            foreach (var subNode in container)
                PrintNode(subNode, sb, level + 1);
        else if (node is LeafBlock leaf && leaf.Inline != null)
            PrintNode(leaf.Inline, sb, level + 1);
        else if (node is ContainerInline inline)
        {
            var current = inline.FirstChild;
            while (current != null)
            {
                PrintNode(current, sb, level + 1);
                current = current.NextSibling;
            }
        }
    }
}
#endif