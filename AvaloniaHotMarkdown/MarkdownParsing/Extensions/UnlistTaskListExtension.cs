using Markdig;
using Markdig.Extensions.TaskLists;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace AvaloniaHotMarkdown.MarkdownParsing.Extensions;

/// <summary>
/// Makes so that checkboxes are no longer considered lists
/// </summary>
// this code is digusting but its working
public class UnlistTaskListExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline) {
        pipeline.DocumentProcessed += OnDocumentProcessed;
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
    }

    void OnDocumentProcessed(MarkdownDocument document)
    {
        var listBlocks = document.OfType<ListBlock>();

        foreach (var listBlock in listBlocks)
        {
            //1. split the list by task list items
            List<ParagraphBlock> checkboxBlocks = new List<ParagraphBlock>();
            List<List<ListItemBlock>> splitedLists = new();
            int currentIndex = 0;

            //true means block is a checkbox
            Queue<bool> blockQueue = new Queue<bool>();

            for(int i = 0; i < listBlock.Count; i++)
            {
                ListItemBlock listItemBlock = (ListItemBlock)listBlock[i];

                //is not a paragraph block or
                //paragraph block does not contain checkbox
                if (listItemBlock.Count == 0 || listItemBlock[0] is not ParagraphBlock paragraphBlock ||
                    paragraphBlock.Inline is null || paragraphBlock.Inline.FirstChild is not TaskList taskList)
                {
                    if (splitedLists.Count == 0)
                    {
                        splitedLists.Add(new List<ListItemBlock>());
                        blockQueue.Enqueue(false);
                    }

                    splitedLists[currentIndex].Add(listItemBlock);
                    continue;
                }

                blockQueue.Enqueue(true);

                if (i != listBlock.Count -1)
                {
                    splitedLists.Add(new List<ListItemBlock>());
                    blockQueue.Enqueue(false);
                }

                if(splitedLists.Count!=1)
                    currentIndex++;

                checkboxBlocks.Add(paragraphBlock);
            }

            //no checkboxes detected, skip
            if (splitedLists.Count == 1 && checkboxBlocks.Count == 0)
                continue;

            //2. remove all elements from old list
            int indexOfListBlock = document.IndexOf(listBlock);
            int currentLine = listBlock.Line;

            int baseLine = listBlock.Line;
            listBlock.Remove();

            //3. reconstruct new elements
            int currentListBlockIndex = 0;
            int currentParagraphBlockIndex = 0;

            while(blockQueue.Count != 0)
            {
                bool isCurrentParagraph = blockQueue.Dequeue();

                if(isCurrentParagraph)
                {
                    checkboxBlocks[currentParagraphBlockIndex].Remove();

                    document.Insert(indexOfListBlock, checkboxBlocks[currentParagraphBlockIndex]);

                    RecalculateLineInChildren(checkboxBlocks[currentParagraphBlockIndex], currentLine++);

                    currentParagraphBlockIndex++;
                }
                else
                {
                    ListBlock newList = new ListBlock(listBlock.Parser!);
                    RecalculateLineInChildren(newList, currentLine);

                    foreach (var listItemBlock in splitedLists[currentListBlockIndex])
                    {
                        listItemBlock.Remove();
                        newList.Add(listItemBlock);
                        RecalculateLineInChildren(listItemBlock, currentLine++);
                    }

                    document.Insert(indexOfListBlock, newList);

        
                    currentListBlockIndex++;
                }
                
                indexOfListBlock++;
            }

        }
    }

    void RecalculateLineInChildren(MarkdownObject node, int line)
    {
        node.Line = line;

        if (node is ContainerBlock container)
            foreach (var subNode in container)
                RecalculateLineInChildren(subNode, line);
        else if (node is LeafBlock leaf && leaf.Inline != null)
            RecalculateLineInChildren(leaf.Inline, line);
        else if (node is ContainerInline inline)
        {
            var current = inline.FirstChild;
            while (current != null)
            {
                RecalculateLineInChildren(current, line);
                current = current.NextSibling;
            }
        }
    }
}