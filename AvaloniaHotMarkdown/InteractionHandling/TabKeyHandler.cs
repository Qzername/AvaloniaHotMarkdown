using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling;

internal class TabKeyHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.Tab;

    public void HandleCombination(KeyModifiers keyModifiers, HotMarkdownEditor editor, ref List<string> actualText, ref MemoryBank memoryBank)
    {
        actualText[editor.CaretPositionData.Y] = actualText[editor.CaretPositionData.Y].Insert(editor.CaretPositionData.X, "    ");
        editor.CaretPositionData.X += 4; // Move the cursor forward by 4 spaces
    }
}
