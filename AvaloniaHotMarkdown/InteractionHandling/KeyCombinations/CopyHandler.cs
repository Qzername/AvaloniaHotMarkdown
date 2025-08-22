using Avalonia.Controls;
using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling.KeyCombinations;

internal class CopyHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.C;

    public void HandleCombination(KeyModifiers keyModifiers, HotMarkdownEditor editor, ref List<string> actualText)
    {
        if (!keyModifiers.HasFlag(KeyModifiers.Control))
            return;

        var clipboard = TopLevel.GetTopLevel(editor)?.Clipboard;

        if (clipboard != null)
            clipboard.SetTextAsync(editor.SelectedText);
    }
}
