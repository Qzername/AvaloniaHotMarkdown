using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaHotMarkdown.MarkdownParsing;
using System;
using System.Diagnostics;

namespace AvaloniaHotMarkdown;

public class HotMarkdownEditor : ContentControl
{
    public static readonly DirectProperty<HotMarkdownEditor, string> TextProperty =
           AvaloniaProperty.RegisterDirect<HotMarkdownEditor, string>(nameof(Text), (hme) => hme.Text, (hme, s) => hme.Text = s, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    string _text = string.Empty;
    public string Text 
    { 
        get => _text;
        set
        {
            _text = value;
            ConstructChildren();
        }
    }

    IMarkdownParser markdownParser;

    public HotMarkdownEditor()
    {
        markdownParser = new StandardMarkdownParser();
        Content = new StackPanel();
    }

    void ConstructChildren()
    {
        var contentPanel = Content as StackPanel;

        contentPanel.Children.Clear();

        foreach (var control in markdownParser.Parse(Text))
            contentPanel.Children.Add(control);
    }

    public override void Render(DrawingContext context)
    {
        //TODO: this is for debug reasons remove it
        context.FillRectangle(Brushes.Gray, new Rect(Bounds.Size));

        base.Render(context);
    }
}