using Avalonia;

namespace AvaloniaHotMarkdown;

public struct TextCursor(int index, bool isVisibile)
{
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public bool IsVisible { get; set; } = isVisibile;
}
