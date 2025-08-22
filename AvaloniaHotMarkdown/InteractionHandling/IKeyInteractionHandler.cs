using Avalonia.Input;

namespace AvaloniaHotMarkdown.InteractionHandling;

public interface IKeyInteractionHandler
{
    public Key MainKey { get; }

    public abstract void HandleCombination(KeyModifiers keyModifiers, HotMarkdownEditor editor, ref List<string> actualText, ref MemoryBank memoryBank);
}
