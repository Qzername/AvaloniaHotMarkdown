namespace AvaloniaHotMarkdown;

public struct TextCursor(int index, bool showCursor)
{
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public bool ShowCursor { get; set; } = showCursor;
}
