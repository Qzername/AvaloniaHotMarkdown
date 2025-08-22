using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling;

internal class BackKeyHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.Back;

    public void HandleCombination(KeyModifiers keyModifiers, HotMarkdownEditor editor, ref List<string> actualText, ref MemoryBank memoryBank)
    {
        //do nothing if caret is at the beginning of the text
        if (editor.CaretPositionData.X == 0 && editor.CaretPositionData.Y == 0)
            return;

        //previous line
        //|current line
        if (editor.CaretPositionData.X == 0)
        {
            var currentLine = actualText[editor.CaretPositionData.Y];

            //previous linecurrent line
            //|current line
            actualText[editor.CaretPositionData.Y - 1] += currentLine;

            //previous linecurrent line
            actualText.RemoveAt(editor.CaretPositionData.Y);

            //previous line|current line
            editor.CaretPositionData.Y--;
            editor.CaretPositionData.X = actualText[editor.CaretPositionData.Y].Length - currentLine.Length;
        }
        else //in other case remove just last character
        {
            actualText[editor.CaretPositionData.Y] = actualText[editor.CaretPositionData.Y].Remove(editor.CaretPositionData.X - 1, 1);
            editor.CaretPositionData.X--;
        }
    }
}
