namespace AvaloniaHotMarkdown;

public class MemoryBank
{
    List<Memory> memoryBank;

    public MemoryBank()
    {
        memoryBank = new List<Memory>();
    }

    public void Append(TextCursor startingPosition, string text) => AddToMemory(startingPosition, text, MemoryOperationType.Append);
    public void Shorten(TextCursor startingPosition, string text) => AddToMemory(startingPosition, text, MemoryOperationType.Shorten);

    void AddToMemory(TextCursor position, string text, MemoryOperationType operationType)
    {
        memoryBank.Add(new Memory
        {
            StartingPosition = position,
            Text = text,
            OperationType = operationType
        });
    }

    public Memory Undo()
    {
        var last = memoryBank[^1];
        memoryBank.RemoveAt(memoryBank.Count - 1);
        return last;
    }
}

public struct Memory
{
    public TextCursor StartingPosition;
    public string Text;
    public MemoryOperationType OperationType;
}

public enum MemoryOperationType
{
    Append,
    Shorten
}