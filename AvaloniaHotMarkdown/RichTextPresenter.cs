using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace AvaloniaHotMarkdown;

public enum FontType
{
    Normal,
    Superscript,
    Subscript,
}

// i hate the fact this control has to exist
// because TextPresenter dosen't support text decorations
// and its Render method is sealed
public class RichTextPresenter : Control
{
    readonly TextPresenter _textPresenter;

    public static readonly StyledProperty<double> FontSizeProperty = AvaloniaProperty.Register<RichTextPresenter, double>(nameof(FontSize));
    public static readonly StyledProperty<IBrush?> CaretBrushProperty = AvaloniaProperty.Register<RichTextPresenter, IBrush?>(nameof(CaretBrush), Brushes.White);
    public static readonly StyledProperty<IBrush?> SelectionBrushProperty = AvaloniaProperty.Register<RichTextPresenter, IBrush?>(nameof(SelectionBrush), Brushes.Cyan);
    public static readonly StyledProperty<IBrush?> ForegroundProperty = AvaloniaProperty.Register<RichTextPresenter, IBrush?>(nameof(Foreground));
    public static readonly StyledProperty<IBrush?> HighlightBrushProperty = AvaloniaProperty.Register<RichTextPresenter, IBrush?>(nameof(HighlightBrush), Brushes.Wheat);
    public static readonly StyledProperty<IBrush?> CodeInlineBrushProperty = AvaloniaProperty.Register<RichTextPresenter, IBrush?>(nameof(CodeInlineBrush), new ImmutableSolidColorBrush(Color.FromRgb(53, 55, 72)));
    public static readonly StyledProperty<FontWeight> FontWeightProperty = AvaloniaProperty.Register<RichTextPresenter, FontWeight>(nameof(FontWeight));
    public static readonly StyledProperty<FontStyle> FontStyleProperty = AvaloniaProperty.Register<RichTextPresenter, FontStyle>(nameof(FontStyle));
    
    public string Text
    {
        get => _textPresenter.Text;
        set => _textPresenter.Text = value;
    }

    public double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public IBrush? CaretBrush
    {
        get => GetValue(CaretBrushProperty);
        set => SetValue(CaretBrushProperty, value);
    }

    public IBrush? SelectionBrush
    {
        get => GetValue(SelectionBrushProperty);
        set => SetValue(SelectionBrushProperty, value);
    }

    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    public IBrush? HighlightBrush
    {
        get => GetValue(HighlightBrushProperty);
        set => SetValue(HighlightBrushProperty, value);
    }

    public IBrush? CodeInlineBrush
    {
        get => GetValue(CodeInlineBrushProperty);
        set => SetValue(CodeInlineBrushProperty, value);
    }

    public FontWeight FontWeight
    {
        get => GetValue(FontWeightProperty);
        set => SetValue(FontWeightProperty, value);
    }

    public FontStyle FontStyle
    {
        get => GetValue(FontStyleProperty);
        set => SetValue(FontStyleProperty, value);
    }

    public bool ShowUnderline;
    public bool ShowStrikethrough;
    public bool ShowHighlight;

    public int SelectionStart
    {
        get => _textPresenter.SelectionStart;
        set => _textPresenter.SelectionStart = value;
    }

    public int SelectionEnd
    {
        get => _textPresenter.SelectionEnd;
        set => _textPresenter.SelectionEnd = value;
    }

    public int CaretIndex
    {
        get => _textPresenter.CaretIndex;
        set => _textPresenter.CaretIndex = value;
    }

    readonly FontFamily _defaultFontFamily;
    bool _isCodeInline;
    public bool IsCodeInline
    {
        get => _isCodeInline;
        set
        {
            _isCodeInline = value;

            if (value)
                _textPresenter.FontFamily = new FontFamily("Consolas");
            else
                _textPresenter.FontFamily = _defaultFontFamily;
        }
    }

    readonly double _defaultFontSize;
    FontType _fontType;
    public FontType FontType
    {
        get => _fontType;
        set
        {
            _fontType = value;

            if (value == FontType.Normal)
                _textPresenter.FontSize = _defaultFontSize;
            else
            {
                _textPresenter.FontSize = _defaultFontSize * 0.75;
                _textPresenter.Margin = new Thickness(0, -2, 0, -2);

                if (value == FontType.Subscript)
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom;
                else//if (value == FontSize.Superscript)
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
            }
        }
    }

    public RichTextPresenter()
    {
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;

        _textPresenter = new()
        {
            Background = Brushes.Transparent,
            CaretBrush = CaretBrush,
            SelectionBrush = SelectionBrush,
        };

        _defaultFontFamily = _textPresenter.FontFamily;
        _defaultFontSize = _textPresenter.FontSize;

        LogicalChildren.Add(_textPresenter);
        VisualChildren.Add(_textPresenter);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == FontSizeProperty)
            _textPresenter.FontSize = change.GetNewValue<double>();
        else if (change.Property == CaretBrushProperty)
            _textPresenter.CaretBrush = change.GetNewValue<IBrush?>();
        else if (change.Property == SelectionBrushProperty)
            _textPresenter.SelectionBrush = change.GetNewValue<IBrush?>();
        else if (change.Property == ForegroundProperty)
            _textPresenter.Foreground = change.GetNewValue<IBrush?>();
        else if (change.Property == FontWeightProperty)
            _textPresenter.FontWeight = change.GetNewValue<FontWeight>();
        else if (change.Property == FontStyleProperty)
            _textPresenter.FontStyle = change.GetNewValue<FontStyle>();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        context.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, Bounds.Width, Bounds.Height));

        if (ShowHighlight)
            context.DrawRectangle(HighlightBrush, null, new Rect(0, 0, _textPresenter.DesiredSize.Width, _textPresenter.DesiredSize.Height));

        if (IsCodeInline)
            context.DrawRectangle(CodeInlineBrush, null, new Rect(0, 0, _textPresenter.DesiredSize.Width, _textPresenter.DesiredSize.Height), 5);

        Point rightDownCorner = new Point(_textPresenter.DesiredSize.Width, _textPresenter.DesiredSize.Height);

        Pen pen = new Pen(CaretBrush, 2);

        if (ShowUnderline)
            context.DrawLine(pen, new Point(0, rightDownCorner.Y), rightDownCorner);

        if (ShowStrikethrough)
            context.DrawLine(pen, new Point(0, rightDownCorner.Y / 2), new Point(rightDownCorner.X, rightDownCorner.Y / 2));
    }

    public void ShowCaret() => _textPresenter.ShowCaret();
    public void HideCaret() => _textPresenter.HideCaret();
    public void MoveCaretToPoint(Point point) => _textPresenter.MoveCaretToPoint(point);

    protected override Size MeasureOverride(Size availableSize)
    {
        _textPresenter.Measure(availableSize);

        return _textPresenter.DesiredSize;
    }

    public void AddClass(string className) => _textPresenter.Classes.Add(className);
}