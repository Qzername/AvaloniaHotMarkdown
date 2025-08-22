using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling;

internal class HomeKeyHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.Home;

    public void HandleCombination(KeyModifiers keyModifiers, HotMarkdownEditor editor, ref List<string> actualText, ref MemoryBank memoryBank)
    {
        editor.CaretPositionData.X = 0;

        if (keyModifiers.HasFlag(KeyModifiers.Control))
            editor.CaretPositionData.Y = 0;
    }
}
