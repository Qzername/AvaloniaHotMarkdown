using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
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
            //TODO: change it later...
            context.CurrentPresenter.Foreground = Brushes.Blue;

            context.CurrentPresenter.PointerPressed += (s,e) => {
                ProcessStartInfo psi = new()
                {
                    FileName = linkInline.Url,
                    UseShellExecute = true
                };

                Process.Start(psi);
            };
        }
    }

    void FullText(LinkInline linkInline, InlineParsingContext context, TextUpdateRequestHandler textUpdateHandler)
    {
        context.CurrentPresenter.Text = $"[";
        context.DefaultFinalizationOfLine();

        var stackPanel = ParseInline(linkInline.ToArray(), context.ParseAsFullText, context.XOffset + 1) as StackPanel;
        var dockPanel = stackPanel.Children[0] as DockPanel;

        for (int i = dockPanel.Children.Count - 1; i >= 0; i--)
        {
            var item = dockPanel.Children[i];

            dockPanel.Children.Remove(item);
            context.CurrentLine.Children.Insert(context.CurrentLine.Children.Count - 1, item);
        }
        
        context.CurrentPresenter.Text = $"]({linkInline.Url})";
        context.DefaultFinalizationOfLine();

        foreach (var children in linkInline)
            children.Remove();
    }
}
