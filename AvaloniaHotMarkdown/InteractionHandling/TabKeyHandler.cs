using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling;

internal class TabKeyHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.Tab;

    public void HandleCombination(KeyModifiers keyModifiers, ref List<string> actualText, ref TextCursor caretPositionData, ref TextCursor selectionPositionData)
    {
        actualText[caretPositionData.Y] = actualText[caretPositionData.Y].Insert(caretPositionData.X, "    ");
        caretPositionData.X += 4; // Move the cursor forward by 4 spaces
    }
}
