using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling;

internal class EndKeyHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.End;

    public void HandleCombination(KeyModifiers keyModifiers, ref List<string> actualText, ref TextCursor caretPositionData)
    {
        caretPositionData.X = actualText[caretPositionData.Y].Length;
    }
}
