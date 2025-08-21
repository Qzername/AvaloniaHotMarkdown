using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling;

internal class RightKeyHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.Right;

    public void HandleCombination(KeyModifiers keyModifiers, HotMarkdownEditor editor, ref List<string> actualText)
    {
        var caretPositionData = editor.CaretPositionData;

        if (keyModifiers.HasFlag(KeyModifiers.Control))
        {
            if (caretPositionData.X == actualText[caretPositionData.Y].Length && caretPositionData.Y != actualText.Count - 1)
            {
                caretPositionData.Y++;
                caretPositionData.X = 0; // Move to the start of the next line  
            }
            else
            {
                var currentLine = actualText[caretPositionData.Y];

                //move through current whitespaces where cursor is to word
                while (caretPositionData.X < currentLine.Length && !char.IsWhiteSpace(currentLine[caretPositionData.X]))
                    caretPositionData.X++;

                //move through current word to the start of it
                while (caretPositionData.X < currentLine.Length && char.IsWhiteSpace(currentLine[caretPositionData.X]))
                    caretPositionData.X++;
            }
        }
        else
        {
            if (caretPositionData.X == actualText[caretPositionData.Y].Length && caretPositionData.Y != actualText.Count - 1)
            {
                caretPositionData.Y++;
                caretPositionData.X = 0; // Move to the start of the next line  
            }
            else if (caretPositionData.X != actualText[caretPositionData.Y].Length)
            {
                caretPositionData.X++;
            }
        }

        editor.CaretPositionData = caretPositionData;
    }
}
