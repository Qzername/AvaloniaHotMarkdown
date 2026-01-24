namespace AvaloniaHotMarkdown;

public class MemoryBank
{
    List<Memory> memoryBank;

    public MemoryBank()
    {
        memoryBank = new List<Memory>();
    }

    public void Append(int indexPosition, string text) => AddToMemory(indexPosition, text, MemoryOperationType.Append);
    public void Shorten(int indexPosition, string text) => AddToMemory(indexPosition, text, MemoryOperationType.Shorten);

    void AddToMemory(int indexPositon, string text, MemoryOperationType operationType)
    {
        memoryBank.Add(new Memory
        {
            IndexPosition = indexPositon,
            Text = text,
            OperationType = operationType
        });
    }

    public Memory? Undo()
    {
        if(memoryBank.Count == 0)
            return null;

        var last = memoryBank[^1];
        memoryBank.RemoveAt(memoryBank.Count - 1);
        return last;
    }
}

public struct Memory
{
    public int IndexPosition;
    public string Text;
    public MemoryOperationType OperationType;
}

public enum MemoryOperationType
{
    Append,
    Shorten
}