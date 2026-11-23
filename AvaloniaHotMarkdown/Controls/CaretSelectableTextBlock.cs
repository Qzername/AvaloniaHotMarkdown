using System;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.Controls;

public delegate void TextBlockClickedCallback(TextBlockSpan span, int index);

/// <summary>
/// Special textblock that supports caret rendering and has specific functionality made for AvaloniaHotMarkdown library
/// </summary>
public class CaretSelectableTextBlock : SelectableTextBlock
{
    public static readonly StyledProperty<int?> CaretIndexProperty =
        AvaloniaProperty.Register<CaretSelectableTextBlock, int?>(nameof(CaretIndex));

    /// <summary>Caret color. When null, the control's Foreground is used.</summary>
    public static readonly StyledProperty<IBrush?> CaretBrushProperty =
        AvaloniaProperty.Register<CaretSelectableTextBlock, IBrush?>(nameof(CaretBrush));

    readonly TextBlockClickedCallback _textBlockClickedCallback;
    readonly TextBlockSpan _sourceSpan;

    private readonly DispatcherTimer _blink = new() { Interval = TimeSpan.FromMilliseconds(530) };
    private bool _caretVisible = true;

    static CaretSelectableTextBlock()
    {
        AffectsRender<CaretSelectableTextBlock>(CaretIndexProperty, CaretBrushProperty);

        CaretIndexProperty.Changed.AddClassHandler<CaretSelectableTextBlock>((c, _) => c.ResetBlink());

        // Mouse selection moves SelectionEnd, so the caret follows it.
        SelectionEndProperty.Changed.AddClassHandler<CaretSelectableTextBlock>(
            (c, _) => c.CaretIndex = c.SelectionEnd);
    }

    public CaretSelectableTextBlock(TextBlockClickedCallback textBlockClickedCallback, TextBlockSpan textBlockSpan)
    {
        _textBlockClickedCallback = textBlockClickedCallback;
        _sourceSpan = textBlockSpan;

        _blink.Tick += (_, _) =>
        {
            _caretVisible = !_caretVisible;
            InvalidateVisual();
        };

        GotFocus += (_, _) => ResetBlink();
        LostFocus += (_, _) =>
        {
           // _blink.Stop();
            InvalidateVisual();
        };

        ResetBlink();
    }

    public int? CaretIndex
    {
        get => GetValue(CaretIndexProperty);
        set => SetValue(CaretIndexProperty, value);
    }

    public IBrush? CaretBrush
    {
        get => GetValue(CaretBrushProperty);
        set => SetValue(CaretBrushProperty, value);
    }

    private int TextLength => TextLayout.TextLines.Sum(l => l.Length);

    private void ResetBlink()
    {
        _caretVisible = true;
        //_blink.Stop();
        _blink.Start();
        InvalidateVisual();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
    }

    protected override void RenderTextLayout(DrawingContext context, Point origin)
    {
        // Text and selection highlight first, caret on top.
        base.RenderTextLayout(context, origin);

        if (!_caretVisible)
            return;

        if (CaretIndex is not null)
            DrawCaret(context, origin);

    }

    void DrawCaret(DrawingContext context, Point origin)
    {
        var brush = CaretBrush ?? Foreground;
        if (brush is null)
            return;

        int caretIndex = CaretIndex!.Value;

        var layout = TextLayout;
        var length = layout.TextLines.Sum(l => l.Length);
        var rect = layout.HitTestTextPosition(Math.Clamp(caretIndex, 0, length));

        // origin already contains padding and any vertical alignment offset
        var x = origin.X + rect.X;
        var y = origin.Y + rect.Y;
        var height = rect.Height > 0 ? rect.Height : FontSize * 1.2;

        context.DrawLine(new Pen(brush, 4), new Point(x, y), new Point(x, y + height));
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Handled)
            return;

        var index = CaretIndex.Value;
        switch (e.Key)
        {
            case Key.Left: index = Math.Max(0, index - 1); break;
            case Key.Right: index = Math.Min(TextLength, index + 1); break;
            case Key.Home: index = 0; break;
            case Key.End: index = TextLength; break;
            default: return;
        }

        var extend = e.KeyModifiers.HasFlag(KeyModifiers.Shift);
        if (extend)
        {
            if (SelectionStart == SelectionEnd)
                SelectionStart = CaretIndex.Value;
            SelectionEnd = index;
        }
        else
        {
            SelectionStart = index;
            SelectionEnd = index;
        }

        CaretIndex = index;
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (CaretIndex is null)
            return;

        _textBlockClickedCallback(_sourceSpan, CaretIndex.Value);
    }
}