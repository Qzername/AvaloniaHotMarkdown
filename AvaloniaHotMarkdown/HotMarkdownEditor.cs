using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaHotMarkdown.MarkdownParsing;
using System.Diagnostics;

namespace AvaloniaHotMarkdown;

public class HotMarkdownEditor : ContentControl
{
    public static readonly DirectProperty<HotMarkdownEditor, string> TextProperty =
           AvaloniaProperty.RegisterDirect<HotMarkdownEditor, string>(nameof(Text), (hme) => hme.Text, (hme, s) => hme.Text = s, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public string Text { get; set; } = string.Empty;

    IMarkdownParser markdownParser;
    StackPanel mainContainer;

    public HotMarkdownEditor()
    {
        markdownParser = new StandardMarkdownParser();
        
        mainContainer = new StackPanel();
        Content = mainContainer;
    }

    public override void Render(DrawingContext context)
    {
        mainContainer.Children.Clear();

        context.PushRenderOptions(new RenderOptions()
        {
            RequiresFullOpacityHandling = false,
        });

        //TODO: this is for debug reasons remove it
        context.FillRectangle(Brushes.Gray, new Rect(Bounds.Size));

        foreach (var control in markdownParser.Parse(Text))
        {
            control.InvalidateArrange();
            control.InvalidateMeasure();
            control.InvalidateVisual();
        
            control.Measure(Bounds.Size);
            control.Render(context);

            foreach(var child in (control as StackPanel).Children)
            {
                child.InvalidateArrange();
                child.InvalidateMeasure();
                child.InvalidateVisual();

                child.Measure(Bounds.Size);
                child.Render(context);
            }
        }

        base.Render(context);
    }
}