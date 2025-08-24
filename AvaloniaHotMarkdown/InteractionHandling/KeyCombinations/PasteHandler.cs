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

        if (clipboard != null)
        {
            var text = clipboard.GetTextAsync();

            if (!string.IsNullOrEmpty(text.Result))
            {
                memoryBank.Append(editor.CaretPositionData, text.Result);

                if (editor.SelectionPositionData.IsVisible)
                    editor.ReplaceSelectionWith(text.Result);
                else
                {
                    int globalIndex = IKeyInteractionHandler.GetGlobalIndexFromLines(editor.CaretPositionData, actualText);
                    editor.Text = editor.Text.Insert(globalIndex, text.Result);
                }
            }
        }
    }
}
