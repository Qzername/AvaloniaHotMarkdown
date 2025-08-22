using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling.KeyCombinations;

internal class SelectAllHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.A;

    public void HandleCombination(KeyModifiers keyModifiers, HotMarkdownEditor editor, ref List<string> actualText, ref MemoryBank memoryBank)
    {
        if (!keyModifiers.HasFlag(KeyModifiers.Control))
            return;

        editor.SelectionPositionData.X = 0;
        editor.SelectionPositionData.Y = 0;
        editor.SelectionPositionData.IsVisible = true;

        editor.CaretPositionData.X = actualText[^1].Length;
        editor.CaretPositionData.Y = actualText.Count - 1;
    }
}
