using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling;

internal class RightKeyHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.Right;

    public void HandleCombination(KeyModifiers keyModifiers, ref List<string> actualText, ref TextCursor caretPositionData, ref TextCursor selectionPositionData)
    {
        if (caretPositionData.X == actualText[caretPositionData.Y].Length && caretPositionData.Y != actualText.Count - 1)
        {
            caretPositionData.Y++;
            caretPositionData.X = 0; //move to the start of the next line
        }
        else if (caretPositionData.X != actualText[caretPositionData.Y].Length)
            caretPositionData.X++;
    }
}
