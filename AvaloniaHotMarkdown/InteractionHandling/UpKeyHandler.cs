using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling;

internal class UpKeyHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.Up;

    public void HandleCombination(KeyModifiers keyModifiers, ref List<string> actualText, ref TextCursor caretPositionData, ref TextCursor selectionPositionData)
    {
        if(caretPositionData.Y != 0)
            caretPositionData.Y--;

        if (caretPositionData.X > actualText[caretPositionData.Y].Length)
            caretPositionData.X = actualText[caretPositionData.Y].Length;
    }
}
