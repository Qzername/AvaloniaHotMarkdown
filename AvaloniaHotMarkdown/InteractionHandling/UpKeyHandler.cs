using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling;

internal class UpKeyHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.Up;

    public void HandleCombination(KeyModifiers keyModifiers, HotMarkdownEditor editor, ref List<string> actualText, ref MemoryBank memoryBank)
    {
        if(editor.CaretPositionData.Y != 0)
            editor.CaretPositionData.Y--;

        if (editor.CaretPositionData.X > actualText[editor.CaretPositionData.Y].Length)
            editor.CaretPositionData.X = actualText[editor.CaretPositionData.Y].Length;
    }
}
