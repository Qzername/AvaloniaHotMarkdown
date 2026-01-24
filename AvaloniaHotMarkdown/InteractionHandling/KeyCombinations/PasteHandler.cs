using Avalonia.Controls;
using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling.KeyCombinations;

internal class PasteHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.V;

    public void HandleCombination(KeyModifiers keyModifiers, HotMarkdownEditor editor, ref List<string> actualText, ref MemoryBank memoryBank)
    {
        if (!keyModifiers.HasFlag(KeyModifiers.Control))
            return;

        var clipboard = TopLevel.GetTopLevel(editor)?.Clipboard;

        if (clipboard is null)
            return;

        var text = clipboard.GetTextAsync().Result!.Replace("\r\n", "\n");

        if (string.IsNullOrEmpty(text))
            return;

        int globalIndex = IKeyInteractionHandler.GetGlobalIndexFromLines(editor.CaretPositionData, actualText);
        memoryBank.Append(globalIndex, text);

        if (editor.SelectionPositionData.IsVisible)
            editor.ReplaceSelectionWith(text);
        else
        {
            editor.Text = editor.Text.Insert(globalIndex, text);

            var lines = text.Split('\n');

            if (lines.Length == 1)
                editor.CaretPositionData.X += lines[0].Length;
            else
            {
                editor.CaretPositionData.Y += lines.Length - 1;
                editor.CaretPositionData.X = lines[^1].Length;
            }
        }
    }
}
