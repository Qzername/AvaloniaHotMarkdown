using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using AvaloniaHotMarkdown.MarkdownParsing;
using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace AvaloniaHotMarkdown;

public class HotMarkdownEditor : ContentControl
{
    public static readonly DirectProperty<HotMarkdownEditor, string> TextProperty =
           AvaloniaProperty.RegisterDirect<HotMarkdownEditor, string>(
               nameof(Text),
               o => o.Text,
               (o, v) => o.Text = v,
               defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly DirectProperty<HotMarkdownEditor, bool> IsReadOnlyProperty =
           AvaloniaProperty.RegisterDirect<HotMarkdownEditor, bool>(
               nameof(IsReadOnly),
               o => o.IsReadOnly,
               (o, v) => o.IsReadOnly = v);

    public static readonly DirectProperty<HotMarkdownEditor, bool> IsSelectableProperty =
           AvaloniaProperty.RegisterDirect<HotMarkdownEditor, bool>(
               nameof(IsSelectable),
               o => o.IsSelectable,
               (o, v) => o.IsSelectable = v);


    //this is a bit of a hack,
    //but it allows us to use the TextBox's built-in text editing capabilities
    //while still rendering the markdown in real-time.
    readonly TextBox textProcessor;

    public int CaretIndex => textProcessor.CaretIndex;
    public int SelectionStart => textProcessor.SelectionStart;
    public int SelectionEnd => textProcessor.SelectionEnd;
    public string SelectedText => textProcessor.SelectedText;

    readonly StackPanel markdownContainer;
    readonly IMarkdownParser markdownParser;
    readonly MyTextInputClient textInputClient;

    const int IndexZeroFailsafe = 100;

    public string Text
    {
        get => textProcessor.Text ?? string.Empty;
        set
        {
            textProcessor.Text = value;
            ConstructChildren();
            RaisePropertyChanged(TextProperty, value, textProcessor.Text);
        }
    }

    public bool IsReadOnly
    {
        get => textProcessor.IsReadOnly;
        set => textProcessor.IsReadOnly = value;
    }

    bool _isSelectable;
    public bool IsSelectable
    {
        get => _isSelectable;
        set
        {
            _isSelectable = value;

            Focusable = value;
            IsHitTestVisible = value;

            textProcessor.CaretIndex = -1;
        }
    }

    static HotMarkdownEditor()
    {
        FocusableProperty.OverrideDefaultValue<HotMarkdownEditor>(true);
    }

    public HotMarkdownEditor()
    {
        markdownParser = new StandardMarkdownParser(TextUpdateRequestHandler);
        AddHandler(TextInputMethodClientRequestedEvent, (s, e) =>
        {
            e.Client = textInputClient;
        });

        textInputClient = new MyTextInputClient(this);

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

        if(e.Property.Name == nameof(TextBox.Text))
            RaisePropertyChanged(TextProperty, (string)e.OldValue, textProcessor.Text);

        ConstructChildren();
    }

    bool _isSelecting;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        var pointer = e.GetCurrentPoint(this);

        if (!pointer.Properties.IsLeftButtonPressed)
            return;

        var point = e.GetPosition(this);
        
        Control hit = e.Source as Control;

        if (hit is HotMarkdownEditor)
            hit = FindRealHit(e);
        
        if (hit is null)
            return;

        int index = FindIndexOfClickedObject(hit, e.GetPosition(e.Source as Visual));

        if (index == -1)
            return;

        if (index == 0 && point.Y > IndexZeroFailsafe)
            return;

        _isSelecting = true;

        textProcessor.CaretIndex = index;
        textProcessor.SelectionStart = index;
        textProcessor.SelectionEnd = index;

        e.Pointer.Capture(this); // Keep tracking even if mouse leaves control
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (!_isSelecting)
            return;

        var point = e.GetPosition(this);
        var hit = this.InputHitTest(point) as Control;

        int index = FindIndexOfClickedObject(hit, e.GetPosition(e.Source as Visual));

        if (index > 0)
            textProcessor.SelectionEnd = index;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        _isSelecting = false;
        e.Pointer.Capture(null);
    }

    Control FindRealHit(PointerEventArgs e)
    {
        var pointInRoot = e.GetPosition(this);
        var visuals = this.GetVisualsAt(new Point(10, pointInRoot.Y));

        foreach (var visual in visuals)
            if (visual is Control control && control != this)
                return control;

        return null;
    }

    int FindIndexOfClickedObject(object? sender, Point? position)
    {
        var control = sender as Control;

        CaretPositionOffset offset = new();

        while (control != this)
        {
            if (control is null)
                return -1;

            if (control.Tag is not null)
                offset += (CaretPositionOffset)control.Tag;

            control = control.Parent as Control;
        }

        int index = GetIndexFromPosition(offset.XInLineOffset, offset.YLineOffset);

        var trueSender = sender;

        if (sender is TextPresenter presenter && presenter.Parent is RichTextPresenter richParent)
            trueSender = richParent;

        if (trueSender is RichTextPresenter rich)
        {
            if (position is not null)
                rich.MoveCaretToPoint(position.Value);

            index += rich.CaretIndex;
        }

        return index;
    }

    int GetIndexFromPosition(int caretIndexInLine, int line)
    {
        if (textProcessor.Text is null)
            return 0;

        string[] lines = textProcessor.Text.Split('\n');

        int totalIndex = 0;

        for (int i = 0; i < line; i++)
            totalIndex += lines[i].Length + 1;

        totalIndex += caretIndexInLine;

        return totalIndex;
    }

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);
        textProcessor.Focus();
    }

    void ConstructChildren()
    {
        if (markdownContainer == null) 
            return;

        markdownContainer.Children.Clear();

        var currentText = Text ?? string.Empty;

        var caretInformation = new CaretInformation
        {
            CaretIndex = IsSelectable ? textProcessor.CaretIndex : -1,
        };

        if (!string.IsNullOrEmpty(textProcessor.SelectedText))
            caretInformation.SelectionInformation = new SelectionInformation
            {
                StartIndex = textProcessor.SelectionStart,
                EndIndex = textProcessor.SelectionEnd
            };

        foreach (var control in markdownParser.Parse(currentText, caretInformation))
            markdownContainer.Children.Add(control);

        textProcessor.Focus(NavigationMethod.Pointer);
        textInputClient.OnTapped();
    }

    public override void Render(DrawingContext context)
    {
        //avalonia will not register keys pressed without this line
        context.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, Bounds.Width, Bounds.Height));

        base.Render(context);
    }

    void TextUpdateRequestHandler(Control sender, int oldTextLength, string newText)
    {
        int index = FindIndexOfClickedObject(sender, null);

        string currentText = Text ?? string.Empty;
        Text = currentText.Substring(0, index - oldTextLength) + newText + currentText.Substring(index);
    }
}