using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling;

public interface IKeyInteractionHandler
{
    public Key MainKey { get; }

    public abstract void HandleCombination(KeyModifiers keyModifiers, ref List<string> actualText, ref TextCursor caretPositionData, ref TextCursor selectionPositionData);
}
