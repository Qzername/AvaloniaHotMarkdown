using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.Diagnostics;
using System.Text;
using AvaloniaInlines = Avalonia.Controls.Documents;
using MarkdigInlines = Markdig.Syntax.Inlines;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers.Implementations;

internal class LinkInlineHandler : InlineHandler<MarkdigInlines.LinkInline>
{
    const string LinkForegroundKey = "HotMarkdownLinkForeground";

    public override AvaloniaInlines.Inline[] Parse(MarkdigInlines.LinkInline inline)
    {
        return inline.IsImage ? ParseImage(inline) : ParseLink(inline);
    }

    static AvaloniaInlines.Inline[] ParseLink(MarkdigInlines.LinkInline inline)
    {
        var textBlock = new TextBlock
        {
            Foreground = Brushes.Blue,
            TextDecorations = TextDecorations.Underline,
            Cursor = new Cursor(StandardCursorType.Hand)
        };

        // Link text (may contain bold/italic/code etc.)
        foreach (var child in inline)
            textBlock.Inlines!.AddRange(InlineParser.ParseInline(child)); // <- your base-class helper

        // Theme override, resolved once the control is in the tree
        textBlock.AttachedToVisualTree += (_, _) =>
        {
            if (textBlock.TryFindResource(LinkForegroundKey, textBlock.ActualThemeVariant, out var res)
                && res is IBrush brush)
                textBlock.Foreground = brush;
        };

        string? url = inline.Url;
        
        textBlock.PointerPressed += (_, e) =>
        {
            if (string.IsNullOrWhiteSpace(url) || !e.GetCurrentPoint(textBlock).Properties.IsLeftButtonPressed)
                return;

            try
            {
                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to open link '{url}': {ex.Message}");
            }
        };

        return [new AvaloniaInlines.InlineUIContainer(textBlock)];
    }

    AvaloniaInlines.Inline[] ParseImage(MarkdigInlines.LinkInline inline)
    {
        try
        {
            var uri = new Uri(inline.Url!, UriKind.RelativeOrAbsolute);

            var image = new Image
            {
                Stretch = Stretch.None,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                Source = new Bitmap(AssetLoader.Open(uri))
            };

            return [new AvaloniaInlines.InlineUIContainer(image)];
        }
        catch (Exception)
        {
            return
            [
                new AvaloniaInlines.Run($"![{GetPlainText(inline)}]({inline.Url})")
            ];
        }
    }

    static string GetPlainText(MarkdigInlines.ContainerInline container)
    {
        var sb = new StringBuilder();
        
        foreach (var child in container)
            switch (child)
            {
                case MarkdigInlines.LiteralInline lit:
                    sb.Append(lit.Content.ToString());
                    break;
                case MarkdigInlines.CodeInline code:
                    sb.Append(code.Content);
                    break;
                case MarkdigInlines.ContainerInline nested:
                    sb.Append(GetPlainText(nested));
                    break;
            }
        
        return sb.ToString();
    }
}