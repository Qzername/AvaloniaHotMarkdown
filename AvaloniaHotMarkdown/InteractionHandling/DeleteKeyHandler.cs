using Avalonia.Input;
using System.Diagnostics;

namespace AvaloniaHotMarkdown.InteractionHandling;

internal class DeleteKeyHandler : IKeyInteractionHandler
{
    public Key MainKey => Key.Delete;

    public void HandleCombination(KeyModifiers keyModifiers, ref List<string> actualText, ref TextCursor caretPositionData)
    {
        if (caretPositionData.X == actualText[caretPositionData.Y].Length && caretPositionData.Y == actualText.Count - 1)
            return;

        if (caretPositionData.X == actualText[caretPositionData.Y].Length)
        {
            actualText[caretPositionData.Y] += actualText[caretPositionData.Y + 1];
            actualText.RemoveAt(caretPositionData.Y + 1);
        }
        else //in other case remove just next character
            actualText[caretPositionData.Y] = actualText[caretPositionData.Y].Remove(caretPositionData.X, 1);
    }
}
