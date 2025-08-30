using Avalonia;

namespace AvaloniaHotMarkdown;

public struct TextCursor(int x, int y, bool isVisibile = false)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;

    bool _previousIsVisible = isVisibile;
    public bool PreviousIsVisible { get => _previousIsVisible; }

    bool _isVisible = isVisibile;
    public bool IsVisible 
    {
        get => _isVisible;
        set
        {
            _previousIsVisible = _isVisible;
            _isVisible = value;
        } 
    }

    public override string ToString()
    {
        return $"X: {X} Y: {Y} IsVisible: {IsVisible}";
    }
}
