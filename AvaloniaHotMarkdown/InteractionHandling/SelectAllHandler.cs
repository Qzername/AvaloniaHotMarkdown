using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling;

internal class SelectAllHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.A;

    public void HandleCombination(KeyModifiers keyModifiers, ref List<string> actualText, ref TextCursor caretPositionData, ref TextCursor selectionPositionData)
    {
        if (!keyModifiers.HasFlag(KeyModifiers.Control))
            return;

        selectionPositionData.X = 0;
        selectionPositionData.Y = 0;
        selectionPositionData.IsVisible = true;

        caretPositionData.X = actualText[^1].Length;
        caretPositionData.Y = actualText.Count - 1;
    }
}
