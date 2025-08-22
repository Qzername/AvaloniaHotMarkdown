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
                editor.Text = editor.Text.Insert(editor.CaretPositionData.X, text.Result);
        }
    }
}
