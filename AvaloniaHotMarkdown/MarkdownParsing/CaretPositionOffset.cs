namespace AvaloniaHotMarkdown.MarkdownParsing;

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
}
