using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Markdig.Extensions.TaskLists;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers.Implementations;

internal class TaskListInlineHandler : InlineHandler<TaskList>
{
    public override Inline[] Parse(TaskList taskList)
    {
        var checkbox = new CheckBox
        {
            IsChecked = taskList.Checked,
            Margin = new Thickness(0, 0, 4, 0),
            VerticalAlignment = VerticalAlignment.Center
        };

        var container = new InlineUIContainer
        {
            Child = checkbox
        };

        return [container];
    }
}
