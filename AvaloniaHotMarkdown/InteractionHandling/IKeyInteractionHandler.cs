using Avalonia.Input;
using System.Diagnostics.Contracts;

namespace AvaloniaHotMarkdown.InteractionHandling;

public interface IKeyInteractionHandler
{
    public Key MainKey { get; }

    public abstract void HandleCombination(KeyModifiers keyModifiers, HotMarkdownEditor editor, ref List<string> actualText, ref MemoryBank memoryBank);

    public static int GetGlobalIndexFromLines(TextCursor cursor, List<string> actualText)
    {
        int index = 0;

        for (int i = 0; i < cursor.Y; i++)
            index += actualText[i].Length + 1; // +1 for newline character

        index += cursor.X;

        return index;
    }

    public static TextCursor GetTextPositionFromGlobalIndex(int globalIndex, List<string> actualText)
    {
        int runningIndex = 0;
        int lineIndex = 0;
        while (lineIndex < actualText.Count)
        {
            int lineLengthWithNewline = actualText[lineIndex].Length + 1; // +1 for newline character
            if (runningIndex + lineLengthWithNewline > globalIndex)
            {
                int columnIndex = globalIndex - runningIndex;
                return new TextCursor(columnIndex, lineIndex);
            }
            runningIndex += lineLengthWithNewline;
            lineIndex++;
        }

        if (actualText.Count > 0)
        {
            int lastLineIndex = actualText.Count - 1;
            return new TextCursor(actualText[lastLineIndex].Length, lastLineIndex);
        }
        else
            return new TextCursor(0, 0);
    }
}
