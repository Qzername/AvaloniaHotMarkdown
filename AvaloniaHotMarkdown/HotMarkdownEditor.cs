using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaHotMarkdown.MarkdownParsing;

namespace AvaloniaHotMarkdown;

public class HotMarkdownEditor : ContentControl
{
    public static readonly DirectProperty<HotMarkdownEditor, string> TextProperty =
           AvaloniaProperty.RegisterDirect<HotMarkdownEditor, string>(
               nameof(Text),
               o => o.Text,
               (o, v) => o.Text = v,
               defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    //this is a bit of a hack,
    //but it allows us to use the TextBox's built-in text editing capabilities
    //while still rendering the markdown in real-time.
    TextBox textProcessor;

    StackPanel markdownContainer; 
    IMarkdownParser markdownParser;

    public string Text
    {
        get => textProcessor.Text ?? string.Empty;
        set
        {
            textProcessor.Text = value;
            ConstructChildren();
        }
    }

    static HotMarkdownEditor()
    {
        FocusableProperty.OverrideDefaultValue<HotMarkdownEditor>(true);
    }

    public HotMarkdownEditor()
    {
        markdownParser = new StandardMarkdownParser();

        markdownContainer = new StackPanel();
        textProcessor = new TextBox
        {
            Opacity = 0, 
            Width = 0,  
            Height = 0,
            IsHitTestVisible = false,
            AcceptsReturn = true
        };
        textProcessor.TextChanged += (s, e) => ConstructChildren();

        var rootPanel = new Panel();
        rootPanel.Children.Add(textProcessor);
        rootPanel.Children.Add(markdownContainer);

        Content = rootPanel;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        textProcessor.Focus();
    }

    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        base.OnGotFocus(e);
        textProcessor.Focus();
    }

    void ConstructChildren()
    {
        if (markdownContainer == null) return;

        markdownContainer.Children.Clear();

        var currentText = Text ?? string.Empty;

        var caretInformation = new CaretInformation
        {
            Index = textProcessor.CaretIndex,
            SelectionStart = textProcessor.SelectionStart
        };

        foreach (var control in markdownParser.Parse(currentText, caretInformation))
            markdownContainer.Children.Add(control);
    }
}