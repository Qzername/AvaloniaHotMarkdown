using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
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

    public int CaretIndex => textProcessor.CaretIndex;
    public int SelectionStart => textProcessor.SelectionStart;
    public int SelectionEnd => textProcessor.SelectionEnd;
    public string SelectedText => textProcessor.SelectedText;

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

        //TODO: make it so it wont rerender every event
        textProcessor.TextChanged += (s, e) => ConstructChildren();
        textProcessor.KeyUp += (s, e) => ConstructChildren();
        textProcessor.PropertyChanged += TextProcessor_PropertyChanged;

        var rootPanel = new Panel();
        rootPanel.Children.Add(textProcessor);
        rootPanel.Children.Add(markdownContainer);

        Content = rootPanel;
    }

    readonly string[] PropertiesToWatch = [
            nameof(TextBox.CaretIndex),
            nameof(TextBox.SelectionStart),
            nameof(TextBox.SelectionEnd),
            nameof(TextBox.Text)
        ];

    private void TextProcessor_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (!PropertiesToWatch.Contains(e.Property.Name))
            return;

        ConstructChildren();
    }

    bool _isSelecting;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        var point = e.GetCurrentPoint(this);

        if (!point.Properties.IsLeftButtonPressed)
            return;
        
        _isSelecting = true;

        int index = FindIndexOfClickedObject(e.Source, e.GetPosition(e.Source as Visual));

        if (index == -1)
            return;

        textProcessor.SelectionStart = index;
        textProcessor.SelectionEnd = index;

        e.Pointer.Capture(this); // Keep tracking even if mouse leaves control
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (_isSelecting)
        {
            var point = e.GetPosition(this);
            var hit = this.InputHitTest(point) as Control;

            int index = FindIndexOfClickedObject(hit, e.GetPosition(e.Source as Visual));

            if (index != -1)
                textProcessor.SelectionEnd = index;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        _isSelecting = false;
        e.Pointer.Capture(null);
    }

    int FindIndexOfClickedObject(object? sender, Point position)
    {
        var control = sender as Control;

        CaretPositionOffset offset = new CaretPositionOffset();

        while (control != this)
        {
            if (control is null)
                return -1;

            if (control.Tag is not null)
                offset = offset + (CaretPositionOffset)control.Tag;

            control = control.Parent as Control;
        }

        int index = GetIndexFromPosition(offset.XInLineOffset, offset.YLineOffset);

        if (sender is RichTextPresenter rich)
        {
            rich.MoveCaretToPoint(position);
            index += rich.CaretIndex;
        }

        return index;
    }

    int GetIndexFromPosition(int caretIndexInLine, int line)
    {
        string[] lines = textProcessor.Text.Split('\n');

        int totalIndex = 0;

        for (int i = 0; i < line; i++)
            totalIndex += lines[i].Length + 1;

        totalIndex += caretIndexInLine;

        return totalIndex;
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
            CaretIndex = textProcessor.CaretIndex,
        };

        if (!string.IsNullOrEmpty(textProcessor.SelectedText))
            caretInformation.SelectionInformation = new SelectionInformation
            {
                StartIndex = textProcessor.SelectionStart,
                EndIndex = textProcessor.SelectionEnd
            };

        foreach (var control in markdownParser.Parse(currentText, caretInformation))
            markdownContainer.Children.Add(control);
    }

    public override void Render(DrawingContext context)
    {
        //avalonia will not register keys pressed without this line
        context.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, Bounds.Width, Bounds.Height));

        base.Render(context);
    }
}