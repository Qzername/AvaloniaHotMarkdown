using Avalonia.Controls;
using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling.KeyCombinations;

internal class CutHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.X;

    public void HandleCombination(KeyModifiers keyModifiers, HotMarkdownEditor editor, ref List<string> actualText, ref MemoryBank memoryBank)
    {
        if (!keyModifiers.HasFlag(KeyModifiers.Control))
            return;

        if (string.IsNullOrEmpty(editor.SelectedText))
            return;

        var clipboard = TopLevel.GetTopLevel(editor)?.Clipboard;

        if (clipboard != null)
            clipboard.SetTextAsync(editor.SelectedText);

        int selectionStartX = Math.Min(editor.CaretPositionData.X, editor.SelectionPositionData.X);
        int selectionEndX = Math.Max(editor.CaretPositionData.X, editor.SelectionPositionData.X);

        int selectionStartY = Math.Min(editor.CaretPositionData.Y, editor.SelectionPositionData.Y);
        int selectionEndY = Math.Max(editor.CaretPositionData.Y, editor.SelectionPositionData.Y);

        memoryBank.Shorten(new TextCursor(selectionStartX, selectionStartY), editor.SelectedText);

        actualText[selectionStartY] = actualText[selectionStartY].Remove(selectionStartX);
        actualText[selectionEndY] = actualText[selectionEndY].Remove(0, selectionEndX);

        for (int i = selectionEndY - 1; i >= selectionStartY + 1; i--)
            actualText.RemoveAt(i);

    }
}
