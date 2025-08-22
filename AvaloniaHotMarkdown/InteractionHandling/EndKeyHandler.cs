using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling;

internal class EndKeyHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.End;

    public void HandleCombination(KeyModifiers keyModifiers, HotMarkdownEditor editor, ref List<string> actualText, ref MemoryBank memoryBank)
    {
        editor.CaretPositionData.X = actualText[editor.CaretPositionData.Y].Length;

        if (keyModifiers.HasFlag(KeyModifiers.Control))
        {
            editor.CaretPositionData.Y = actualText.Count - 1; // Move to the last line
            editor.CaretPositionData.X = actualText[editor.CaretPositionData.Y].Length; // Move to the end of that line
        }
    }
}
