using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling.KeyCombinations;

internal class SelectAllHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.A;

    public void HandleCombination(KeyModifiers keyModifiers, HotMarkdownEditor editor, ref List<string> actualText, ref MemoryBank memoryBank)
    {
        if (!keyModifiers.HasFlag(KeyModifiers.Control))
            return;

        var selectionPositionData = editor.SelectionPositionData;

        selectionPositionData.X = 0;
        selectionPositionData.Y = 0;
        selectionPositionData.IsVisible = true;

        editor.SelectionPositionData = selectionPositionData;

        editor.CaretPositionData.X = actualText[^1].Length;
        editor.CaretPositionData.Y = actualText.Count - 1;
    }
}