using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using System.Diagnostics;

namespace Avalonia.HotMarkdown;

/*
 * i hate the fact that this control has to exist
 * because TextPresenter dosen't support text decorations
 * and its Render method is sealed
 */
public class RichTextPresenter : Control
{
    TextPresenter _textPresenter;

    public string Text
    {
        get => _textPresenter.Text;
        set
        {
            _textPresenter.Text = value;
            InvalidateVisual();
        }
    }

    public double FontSize
    {
        get => _textPresenter.FontSize;
        set => _textPresenter.FontSize = value;
    }

    public IBrush? CaretBrush
    {
        get => _textPresenter.CaretBrush;
        set => _textPresenter.CaretBrush = value;
    }

    public int CaretIndex
    {
        get => _textPresenter.CaretIndex;
        set => _textPresenter.CaretIndex = value;
    }

    public FontWeight FontWeight
    {
        get => _textPresenter.FontWeight;
        set => _textPresenter.FontWeight = value;
    }

    public FontStyle FontStyle
    {
        get => _textPresenter.FontStyle;
        set => _textPresenter.FontStyle = value;
    }

    public bool ShowUnderline;
    public bool ShowStrikethrough;

    public RichTextPresenter()
    {
        _textPresenter = new()
        {
            Foreground = Brushes.White
        };
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        _textPresenter.Render(context);
        var rightDownCorner = new Point(_textPresenter.DesiredSize.Width, _textPresenter.DesiredSize.Height);
        
        var pen = new Pen(CaretBrush, 5);

        if(ShowUnderline)
            context.DrawLine(pen, new Point(0, rightDownCorner.X), rightDownCorner);

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
}