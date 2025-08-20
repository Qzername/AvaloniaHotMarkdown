using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling;

internal class LeftKeyHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.Left;

    public void HandleCombination(KeyModifiers keyModifiers, ref List<string> actualText, ref TextCursor caretPositionData)
    {
        if (caretPositionData.X == 0 && caretPositionData.Y != 0)
        {
            caretPositionData.Y--;
            caretPositionData.X = actualText[caretPositionData.Y].Length; //move to the end of the previous line
        }
        else if (caretPositionData.X != 0)
            caretPositionData.X--;
    }
}
