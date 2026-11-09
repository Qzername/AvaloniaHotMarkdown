using Avalonia.Controls.Documents;
using Markdig.Extensions.TaskLists;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers.Implementations;

internal class TaskListInlineHandler : InlineHandler<TaskList>
{
    const string CheckedButton = "[x]";
    const string UncheckedButton = "[ ]";

    public override Run[] Parse(TaskList taskList)
    {
        return [new()
        {
            Text = taskList.Checked ? CheckedButton : UncheckedButton
        }];
    }
}