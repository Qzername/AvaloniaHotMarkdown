using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling;

internal class DownKeyHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.Down;

    public void HandleCombination(KeyModifiers keyModifiers, HotMarkdownEditor editor, ref List<string> actualText)
    {
        if (editor.CaretPositionData.Y != actualText.Count-1)
            editor.CaretPositionData.Y++;

        if (editor.CaretPositionData.X > actualText[editor.CaretPositionData.Y].Length)
            editor.CaretPositionData.X = actualText[editor.CaretPositionData.Y].Length;
    }
}
