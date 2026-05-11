using Markdig;
using Markdig.Extensions.TaskLists;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Diagnostics;

namespace AvaloniaHotMarkdown.MarkdownParsing.Extensions;

/// <summary>
/// Handles uneccessary blocks before task lists 
/// </summary>
public class UnwrapTaskListExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline) {
        pipeline.DocumentProcessed += OnDocumentProcessed;
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
    }
    private void OnDocumentProcessed(MarkdownDocument document)
    {
        ProcessContainer(document);
    }
    private void ProcessContainer(ContainerBlock container)
    {
        for (int i = container.Count - 1; i >= 0; i--)
        {
            var block = container[i];
            if (block is ListBlock listBlock && IsTaskList(listBlock))
            {
                UnwrapList(container, listBlock, i);
            }
            else if (block is ContainerBlock subContainer)
            {
                ProcessContainer(subContainer);
            }
        }
    }

    private bool IsTaskList(ListBlock list)
    {
        foreach (var block in list)
        {
            if (block is ListItemBlock item && item.Count > 0 && item[0] is ParagraphBlock p && p.Inline != null)
            {
                foreach (var inline in p.Inline)
                {
                    if (inline is TaskList) return true;
                }
            }
        }
        return false;
    }

    private void UnwrapList(ContainerBlock parent, ListBlock list, int index)
    {
        parent.RemoveAt(index);
        int currentPosition = index;
        foreach (var block in list)
        {
            if (block is ListItemBlock item)
            {
                while (item.Count > 0)
                {
                    var inner = item[0];
                    item.RemoveAt(0);
                    parent.Insert(currentPosition++, inner);
                }
            }
        }
    }
}