using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling;

internal class DownKeyHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.Down;

    public void HandleCombination(KeyModifiers keyModifiers, ref List<string> actualText, ref TextCursor caretPositionData)
    {
        if (caretPositionData.Y != actualText.Count-1)
            caretPositionData.Y++;

        if (caretPositionData.X > actualText[caretPositionData.Y].Length)
            caretPositionData.X = actualText[caretPositionData.Y].Length;
    }
}
