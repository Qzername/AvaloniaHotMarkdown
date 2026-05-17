using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Markdig.Extensions.TaskLists;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Diagnostics;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal abstract class BlockHandler
{
    StandardMarkdownParser _parser;

    public BlockHandler(StandardMarkdownParser parser)
    {
        _parser = parser;
    }

    public abstract Control Handle(Block block, string markdown, LineInformation[] lineInformations);
    public virtual void UpdateTextEffects(Control control, LineInformation[] lineInformations)
    {
        if(control is not StackPanel stackPanel)
            return;

        var mainTree = stackPanel.Children;

        int temp = 0;

        //update caret
        for (int i = 0; i < lineInformations.Length; i++)
        {
            int? caretIndex = lineInformations[i].CaretIndex;

            if (mainTree.Count <= i)
                break;

            if (lineInformations[i].CaretIndex is not null && mainTree[i] is DockPanel dockPanel)
                foreach (RichTextPresenter presenter in dockPanel.Children)
                {
                    if (temp + presenter.Text.Length >= caretIndex)
                    {
                        presenter.CaretIndex = caretIndex.Value - temp;
                        presenter.ShowCaret();
                        break;
                    }

                    temp += presenter.Text.Length;
                }

            temp = 0;
        }

        //update selection
        for (int i = 0; i < lineInformations.Length; i++)
        {
            var selectionInformation = lineInformations[i].SelectionInformation;

            if (mainTree.Count <= i)
                break;

            if (selectionInformation is null)
                continue;

            int minSelectionStart = Math.Min(selectionInformation.Value.StartIndex, selectionInformation.Value.EndIndex);
            int maxSelectionStart = Math.Max(selectionInformation.Value.StartIndex, selectionInformation.Value.EndIndex);

            if (mainTree[i] is not DockPanel dockPanel)
                continue;

            foreach (RichTextPresenter presenter in dockPanel.Children)
            {
                if (temp + presenter.Text.Length >= minSelectionStart &&
                    temp <= maxSelectionStart)
                {
                    presenter.SelectionStart = minSelectionStart - temp;
                    presenter.SelectionEnd = maxSelectionStart - temp;
                    presenter.ShowCaret();
                }

                temp += presenter.Text.Length;
            }

            temp = 0;
        }
    }

    /// <summary>
    /// Parses the specified block and returns a corresponding control representation.
    /// 
    /// This is due to the fact that some blocks (like list blocks) have nested blocks and inlines, so this method is used to parse those nested elements.
    /// </summary>
    protected Control ParseBlock(Block block, string markdownText, LineInformation[] lineInformation) => _parser.ParseBlock(block, markdownText, lineInformation);

    /// <summary>
    /// Parses a collection of inline Markdown objects and returns a control that visually represents the formatted text
    /// according to Markdown styling rules.
    /// </summary>
    /// <param name="inlineObjects">An enumerable collection of MarkdownObject instances representing the inline elements to be parsed and rendered.</param>
    /// <param name="parseAsFullText">true to parse the content as full text, applying emphasis delimiters to the entire literal; otherwise, false to
    /// render the content without additional emphasis.</param>
    /// <param name="defaultXOffset">The initial horizontal offset, in pixels, to apply to the parsed content for positioning within the container.</param>
    /// <returns>A StackPanel control containing the formatted text representation of the parsed inline Markdown objects.</returns>
    protected Control ParseInline(IEnumerable<MarkdownObject> inlineObjects, bool parseAsFullText, int defaultXOffset = 0) => _parser.ParseInline(inlineObjects, parseAsFullText, defaultXOffset);
}
