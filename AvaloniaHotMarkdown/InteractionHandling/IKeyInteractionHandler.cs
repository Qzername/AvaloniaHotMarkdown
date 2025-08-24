using Avalonia.Input;

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
}
