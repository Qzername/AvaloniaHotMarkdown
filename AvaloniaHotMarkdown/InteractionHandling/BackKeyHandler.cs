using Avalonia.Input;
using System.Diagnostics;

namespace AvaloniaHotMarkdown.InteractionHandling;

internal class BackKeyHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.Back;

    public void HandleCombination(KeyModifiers keyModifiers, HotMarkdownEditor editor, ref List<string> actualText, ref MemoryBank memoryBank)
    {
        //do nothing if caret is at the beginning of the text
        //be sure though that the we are not selecting something
        if (editor.CaretPositionData.X == 0 && editor.CaretPositionData.Y == 0 
            && !editor.SelectionPositionData.IsVisible)
            return;

        if (editor.SelectionPositionData.IsVisible || editor.SelectionPositionData.PreviousIsVisible)
            editor.ReplaceSelectionWith(string.Empty);
        else if (editor.CaretPositionData.X == 0)
        {
            memoryBank.Shorten(new TextCursor(editor.CaretPositionData.X - 1, editor.CaretPositionData.Y), '\n'.ToString());

            //previous line
            //|current line
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
            memoryBank.Shorten(new TextCursor(editor.CaretPositionData.X - 1, editor.CaretPositionData.Y), actualText[editor.CaretPositionData.Y].Substring(editor.CaretPositionData.X - 1, 1));

            actualText[editor.CaretPositionData.Y] = actualText[editor.CaretPositionData.Y].Remove(editor.CaretPositionData.X - 1, 1);
            editor.CaretPositionData.X--;
        }
    }
}
