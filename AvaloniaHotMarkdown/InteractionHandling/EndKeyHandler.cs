using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling;

internal class EndKeyHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.End;

    public void HandleCombination(KeyModifiers keyModifiers, ref List<string> actualText, ref TextCursor caretPositionData, ref TextCursor selectionPositionData)
    {
        caretPositionData.X = actualText[caretPositionData.Y].Length;

        if (keyModifiers.HasFlag(KeyModifiers.Control))
        {
            caretPositionData.Y = actualText.Count - 1; // Move to the last line
            caretPositionData.X = actualText[caretPositionData.Y].Length; // Move to the end of that line
        }
    }
}
