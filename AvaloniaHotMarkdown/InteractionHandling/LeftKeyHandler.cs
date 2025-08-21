using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling;

internal class LeftKeyHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.Left;

    public void HandleCombination(KeyModifiers keyModifiers, ref List<string> actualText, ref TextCursor caretPositionData, ref TextCursor selectionPositionData)
    {
        if (keyModifiers.HasFlag(KeyModifiers.Control))
        {
            if (caretPositionData.X > 0)
            {
                //move through current whitespaces where cursor is to word
                while (caretPositionData.X > 0 && char.IsWhiteSpace(actualText[caretPositionData.Y][caretPositionData.X - 1]))
                    caretPositionData.X--;

                //move through current word to the start of it
                while (caretPositionData.X > 0 && !char.IsWhiteSpace(actualText[caretPositionData.Y][caretPositionData.X - 1]))
                    caretPositionData.X--;
            }
            else if (caretPositionData.Y > 0)
            {
                // Move to the end of the previous line and then to the start of the last word  
                caretPositionData.Y--;
                caretPositionData.X = actualText[caretPositionData.Y].Length;

                //move through current whitespaces where cursor is to word
                while (caretPositionData.X > 0 && char.IsWhiteSpace(actualText[caretPositionData.Y][caretPositionData.X - 1]))
                    caretPositionData.X--;

                //move through current word to the start of it
                while (caretPositionData.X > 0 && !char.IsWhiteSpace(actualText[caretPositionData.Y][caretPositionData.X - 1]))
                    caretPositionData.X--;
            }
        }
        else
        {
            if (caretPositionData.X == 0 && caretPositionData.Y != 0)
            {
                caretPositionData.Y--;
                caretPositionData.X = actualText[caretPositionData.Y].Length; // Move to the end of the previous line  
            }
            else if (caretPositionData.X != 0)
            {
                caretPositionData.X--;
            }
        }
    }
}
