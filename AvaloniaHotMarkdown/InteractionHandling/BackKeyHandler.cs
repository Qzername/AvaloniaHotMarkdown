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

        //something is selected => remove selection
        if (editor.SelectionPositionData.IsVisible || editor.SelectionPositionData.PreviousIsVisible)
            editor.ReplaceSelectionWith(string.Empty);
        else
        {
            if (keyModifiers.HasFlag(KeyModifiers.Control))
                HandleWordRemoval(editor, ref actualText);
            else
                HandleSingleCharRemoval(editor, ref actualText, ref memoryBank);
        }
    }

    void HandleWordRemoval(HotMarkdownEditor editor, ref List<string> actualText)
    {
        var selectionPositionData = editor.CaretPositionData;

        if(selectionPositionData.X == 0 && selectionPositionData.Y > 0)
        {
            // Move to the end of the previous line and then to the start of the last word  
            selectionPositionData.Y--;
            selectionPositionData.X = actualText[selectionPositionData.Y].Length;
        }

        //move through current whitespaces where cursor is to word
        while (selectionPositionData.X > 0 && char.IsWhiteSpace(actualText[selectionPositionData.Y][selectionPositionData.X - 1]))
            selectionPositionData.X--;

        //move through current word to the start of it
        while (selectionPositionData.X > 0 && !char.IsWhiteSpace(actualText[selectionPositionData.Y][selectionPositionData.X - 1]))
            selectionPositionData.X--;


        editor.SelectionPositionData = selectionPositionData;

        editor.ReplaceSelectionWith(string.Empty);
    }

    void HandleSingleCharRemoval(HotMarkdownEditor editor, ref List<string> actualText, ref MemoryBank memoryBank)
    {
        //we are at the start of the line
        if (editor.CaretPositionData.X == 0)
        {
            int globalIndex = IKeyInteractionHandler.GetGlobalIndexFromLines(new TextCursor(editor.CaretPositionData.X - 1, editor.CaretPositionData.Y), actualText);
            memoryBank.Shorten(globalIndex, '\n'.ToString());

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
            int globalIndex = IKeyInteractionHandler.GetGlobalIndexFromLines(new TextCursor(editor.CaretPositionData.X - 1, editor.CaretPositionData.Y), actualText);
            memoryBank.Shorten(globalIndex, actualText[editor.CaretPositionData.Y].Substring(editor.CaretPositionData.X - 1, 1));

            actualText[editor.CaretPositionData.Y] = actualText[editor.CaretPositionData.Y].Remove(editor.CaretPositionData.X - 1, 1);
            editor.CaretPositionData.X--;
        }
    }
}
