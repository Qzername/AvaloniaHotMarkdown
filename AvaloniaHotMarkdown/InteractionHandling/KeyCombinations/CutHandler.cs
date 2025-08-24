using Avalonia.Controls;
using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling.KeyCombinations;

internal class CutHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.X;

    public void HandleCombination(KeyModifiers keyModifiers, HotMarkdownEditor editor, ref List<string> actualText, ref MemoryBank memoryBank)
    {
        if (!keyModifiers.HasFlag(KeyModifiers.Control))
            return;

        if (string.IsNullOrEmpty(editor.SelectedText))
            return;

        var clipboard = TopLevel.GetTopLevel(editor)?.Clipboard;

        if (clipboard != null)
            clipboard.SetTextAsync(editor.SelectedText);

        editor.ReplaceSelectionWith(string.Empty);
    }
}
