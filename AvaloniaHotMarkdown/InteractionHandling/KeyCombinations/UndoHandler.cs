using Avalonia.Input;
using System.Diagnostics;

namespace AvaloniaHotMarkdown.InteractionHandling.KeyCombinations;

internal class UndoHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.Z;
    public void HandleCombination(KeyModifiers keyModifiers, HotMarkdownEditor editor, ref List<string> actualText, ref MemoryBank memoryBank)
    {
        if (!keyModifiers.HasFlag(KeyModifiers.Control))
            return;

        var memoryNullable = memoryBank.Undo();

        if (memoryNullable is null)
            return;
        
        var memory = memoryNullable.Value;

        int globalIndex = GetGlobalIndexFromLines(memory.Position, actualText);

        if (memory.OperationType == MemoryOperationType.Append)
        {
            editor.CaretPositionData = memory.Position;
            editor.Text = editor.Text.Remove(globalIndex, memory.Text.Length);
        }
        else
        {
            editor.Text = editor.Text.Insert(globalIndex, memory.Text);

            var splited = memory.Text.Split('\n');

            if(splited.Length> 1)
            {
                editor.CaretPositionData.Y += splited.Length - 1;
                editor.CaretPositionData.X = splited[^1].Length;
            }
            else
                editor.CaretPositionData.X += memory.Text.Length;
        }
    }

    int GetGlobalIndexFromLines(TextCursor cursor, List<string> actualText)
    {
        int index = 0;

        for (int i = 0; i < cursor.Y; i++)
            index += actualText[i].Length + 1; // +1 for newline character

        index += cursor.X;

        return index;
    }
}
