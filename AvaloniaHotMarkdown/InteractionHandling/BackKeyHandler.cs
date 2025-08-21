using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling;

internal class BackKeyHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.Back;

    public void HandleCombination(KeyModifiers keyModifiers, ref List<string> actualText, ref TextCursor caretPositionData)
    {
        //do nothing if caret is at the beginning of the text
        if (caretPositionData.X == 0 && caretPositionData.Y == 0)
            return;

        //previous line
        //|current line
        if (caretPositionData.X == 0)
        {
            var currentLine = actualText[caretPositionData.Y];

            //previous linecurrent line
            //|current line
            actualText[caretPositionData.Y - 1] += currentLine;

            //previous linecurrent line
            actualText.RemoveAt(caretPositionData.Y);

            //previous line|current line
            caretPositionData.Y--;
            caretPositionData.X = actualText[caretPositionData.Y].Length - currentLine.Length;
        }
        else //in other case remove just last character
        {
            actualText[caretPositionData.Y] = actualText[caretPositionData.Y].Remove(caretPositionData.X - 1, 1);
            caretPositionData.X--;
        }
    }
}
