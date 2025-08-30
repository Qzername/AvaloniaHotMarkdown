using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling;

internal class DeleteKeyHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.Delete;

    public void HandleCombination(KeyModifiers keyModifiers, HotMarkdownEditor editor, ref List<string> actualText, ref MemoryBank memoryBank)
    {
        if (keyModifiers.HasFlag(KeyModifiers.Shift))
            return;

        var caretPositionData = editor.CaretPositionData;

        if (caretPositionData.X == actualText[caretPositionData.Y].Length && caretPositionData.Y == actualText.Count - 1)
            return;

        if (caretPositionData.X == actualText[caretPositionData.Y].Length)
        {
            memoryBank.Shorten(editor.CaretPositionData, '\n'.ToString());

            actualText[caretPositionData.Y] += actualText[caretPositionData.Y + 1];
            actualText.RemoveAt(caretPositionData.Y + 1);
        }
        else //in other case remove just next character
            actualText[caretPositionData.Y] = actualText[caretPositionData.Y].Remove(caretPositionData.X, 1);
    
        editor.CaretPositionData = caretPositionData;
    }
}
