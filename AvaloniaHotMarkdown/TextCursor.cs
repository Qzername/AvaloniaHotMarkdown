using Avalonia;

namespace AvaloniaHotMarkdown;

public struct TextCursor(int x, int y, bool isVisibile = false)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public bool IsVisible { get; set; } = isVisibile;
}
