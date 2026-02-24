using Avalonia;
using Avalonia.Controls;

namespace AvaloniaHotMarkdown;

public class HotMarkdownEditor : ContentControl
{
    public static readonly DirectProperty<HotMarkdownEditor, string> TextProperty =
           AvaloniaProperty.RegisterDirect<HotMarkdownEditor, string>(nameof(Text), (hme) => hme.Text, (hme, s) => hme.Text = s, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public string Text { get; set; }
}