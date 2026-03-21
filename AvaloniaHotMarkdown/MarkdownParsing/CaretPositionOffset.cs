namespace AvaloniaHotMarkdown.MarkdownParsing;

/// <summary>
/// Usually contained in .Tag of control defines the offset from the 0,0 point in text
/// </summary>
public struct CaretPositionOffset
{
    public int XInLineOffset;
    public int YLineOffset;

    public CaretPositionOffset(int xInLineOffset, int yLineOffset)
    {
        XInLineOffset = xInLineOffset;
        YLineOffset = yLineOffset;
    }

    public static CaretPositionOffset operator +(CaretPositionOffset a, CaretPositionOffset b)
    {
        return new CaretPositionOffset()
        {
            XInLineOffset = a.XInLineOffset + b.XInLineOffset,
            YLineOffset = a.YLineOffset + b.YLineOffset
        };
    }

    public override string ToString()
    {
        return $"X: {XInLineOffset} Y: {YLineOffset}";
    }
}
