using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Diagnostics;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers;

public class LinkInlineHandler(StandardMarkdownParser parser) : InlineHandler(parser)
{
    public override void Handle(MarkdownObject inlineObject, InlineParsingContext context, TextUpdateRequestHandler textUpdateHandler)
    {
        LinkInline linkInline = (LinkInline)inlineObject;

        if (context.ParseAsFullText)
            FullText(linkInline, context, textUpdateHandler);
        else
        {
            if (linkInline.IsImage)
            {
                Image image = new Image()
                {
                    Stretch = Stretch.None,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left
                };

                var uri = new Uri(linkInline.Url);

                try
                {
                    image.Source = new Bitmap(AssetLoader.Open(uri));
                }
                catch (Exception)
                {
                    FullText(linkInline, context, textUpdateHandler);
                    return;
                }

                context.CurrentLine.Children.Add(image);
                RemoveChildren(linkInline);
            }
            else
            {
                string resourceKey = $"HotMarkdownLinkForeground";

                if (context.CurrentLine.TryFindResource(resourceKey, out var resValue) && resValue is Brush custom)
                    context.CurrentPresenter.Foreground = custom;
                else
                    context.CurrentPresenter.Foreground = Brushes.Blue;

                context.CurrentPresenter.PointerPressed += (s, e) =>
                    {
                        ProcessStartInfo psi = new()
                        {
                            FileName = linkInline.Url,
                            UseShellExecute = true
                        };

                        Process.Start(psi);
                    };
            }
        }
    }

    void FullText(LinkInline linkInline, InlineParsingContext context, TextUpdateRequestHandler textUpdateHandler)
    {
        string prefix = (linkInline.IsImage ? "!" : string.Empty) + "[";

        context.CurrentPresenter.Text = $"[";
        context.DefaultFinalizationOfLine();

        var stackPanel = ParseInline(linkInline.ToArray(), context.ParseAsFullText, context.XOffset + prefix.Length) as StackPanel;
        var wrapPanel = stackPanel.Children[0] as StretchWrapPanel;

        for (int i = wrapPanel.Children.Count - 1; i >= 0; i--)
        {
            var item = wrapPanel.Children[i];

            wrapPanel.Children.Remove(item);
            context.CurrentLine.Children.Insert(context.CurrentLine.Children.Count - 1, item);
        }

        context.CurrentPresenter.Text = $"]({linkInline.Url})";
        context.DefaultFinalizationOfLine();

        RemoveChildren(linkInline);
    }

    void RemoveChildren(LinkInline linkInline)
    {
        foreach (var children in linkInline)
            children.Remove();
    }
}
