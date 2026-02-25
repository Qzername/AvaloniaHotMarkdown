using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaHotMarkdown.MarkdownParsing;

namespace AvaloniaHotMarkdown;

public class HotMarkdownEditor : ContentControl
{
    public static readonly DirectProperty<HotMarkdownEditor, string> TextProperty =
           AvaloniaProperty.RegisterDirect<HotMarkdownEditor, string>(nameof(Text), (hme) => hme.Text, (hme, s) => hme.Text = s, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public string Text { get; set; } = string.Empty;

    IMarkdownParser markdownParser;

    public HotMarkdownEditor()
    {
        markdownParser = new StandardMarkdownParser();
    }

    public override void Render(DrawingContext context)
    {
        context.PushRenderOptions(new RenderOptions()
        {
            RequiresFullOpacityHandling = false,
        });

        foreach (var control in markdownParser.Parse(Text))
            control.Render(context);

        base.Render(context);
    }
}